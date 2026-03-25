using MongoDB.Driver;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using Vanguard_Inventory.Models;
using System.Threading.Tasks;

namespace Vanguard_Inventory.Services
{
    public static class DatabaseService
    {
        private static IMongoDatabase _db;
        private const string ConnectionString = "mongodb://localhost:27017";
        private const string DbName = "Vanguard_Inventory_DB";

        public static void Initialize()
        {
            try
            {
                var client = new MongoClient(ConnectionString);
                _db = client.GetDatabase(DbName);

                // Initialize services
                LedgerService.Initialize();
                AnalyticsService.Initialize();  // ADDED: Initialize Analytics Service

                // Auto-seed data if needed
                Task.Run(async () => await SeedDataIfEmpty());
                Task.Run(async () => await SeedAdminUser());

                // Calculate initial analytics after seeding
                Task.Run(async () => await AnalyticsService.CalculateAndUpdateAnalytics());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to MongoDB: " + ex.Message);
            }
        }

        private static async Task SeedDataIfEmpty()
        {
            try
            {
                var productsCollection = _db.GetCollection<Product>("Products");
                var count = await productsCollection.CountDocumentsAsync(new MongoDB.Bson.BsonDocument());

                if (count == 0)
                {
                    // Seed sample products
                    var sampleProducts = new List<Product>
                    {
                        new Product
                        {
                            SKU = "VG-001",
                            Name = "Industrial Sensor X1",
                            Description = "High-precision industrial temperature sensor",
                            Price = 89.99m,
                            Category = "Electronics",
                            Stock = new StockInfo
                            {
                                Current = 145,
                                MinThreshold = 30,
                                MaxCapacity = 500
                            },
                            Tags = new List<string> { "Sensor", "Industrial", "Bestseller" }
                        },
                        new Product
                        {
                            SKU = "VG-002",
                            Name = "Smart Controller Pro",
                            Description = "IoT-enabled smart controller",
                            Price = 199.99m,
                            Category = "Controllers",
                            Stock = new StockInfo
                            {
                                Current = 12,
                                MinThreshold = 25,
                                MaxCapacity = 200
                            },
                            Tags = new List<string> { "IoT", "Smart", "Critical" }
                        },
                        new Product
                        {
                            SKU = "VG-003",
                            Name = "Power Supply Unit",
                            Description = "Industrial grade power supply",
                            Price = 45.50m,
                            Category = "Power",
                            Stock = new StockInfo
                            {
                                Current = 78,
                                MinThreshold = 15,
                                MaxCapacity = 300
                            },
                            Tags = new List<string> { "Power", "Essential" }
                        }
                    };

                    await productsCollection.InsertManyAsync(sampleProducts);
                    Console.WriteLine("Sample products seeded!");

                    // Seed some sample transactions for analytics
                    await SeedSampleTransactions();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding products: {ex.Message}");
            }
        }

        private static async Task SeedSampleTransactions()
        {
            try
            {
                var ledgerCollection = _db.GetCollection<Ledger>("Ledger");
                var count = await ledgerCollection.CountDocumentsAsync(new MongoDB.Bson.BsonDocument());

                if (count == 0)
                {
                    var productsCollection = _db.GetCollection<Product>("Products");
                    var products = await productsCollection.Find(_ => true).ToListAsync();
                    var usersCollection = _db.GetCollection<User>("Users");
                    var adminUser = await usersCollection.Find(u => u.Username == "admin").FirstOrDefaultAsync();

                    var random = new Random();
                    var transactions = new List<Ledger>();

                    // Generate sample sales for the last 30 days
                    for (int day = 0; day < 30; day++)
                    {
                        var date = DateTime.UtcNow.AddDays(-day);

                        foreach (var product in products)
                        {
                            // Random sales between 0 and 5 per day
                            int salesQty = random.Next(0, 6);

                            if (salesQty > 0 && salesQty <= product.Stock.Current)
                            {
                                var transaction = new Ledger
                                {
                                    ProductId = product.Id,
                                    ProductSKU = product.SKU,
                                    ProductName = product.Name,
                                    UserId = adminUser.Id,
                                    UserName = adminUser.FullName,
                                    Type = "Sale",
                                    QuantityChange = -salesQty,
                                    PreviousStock = product.Stock.Current,
                                    NewStock = product.Stock.Current - salesQty,
                                    TotalValue = product.Price * salesQty,
                                    Timestamp = date,
                                    Notes = "Sample transaction for analytics"
                                };
                                transactions.Add(transaction);

                                // Update product stock for this sample
                                product.Stock.Current -= salesQty;
                            }
                        }
                    }

                    if (transactions.Count > 0)
                    {
                        await ledgerCollection.InsertManyAsync(transactions);
                        Console.WriteLine($"Seeded {transactions.Count} sample transactions!");

                        // Update product stocks after sample sales
                        foreach (var product in products)
                        {
                            var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
                            var update = Builders<Product>.Update.Set(p => p.Stock.Current, product.Stock.Current);
                            await productsCollection.UpdateOneAsync(filter, update);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding transactions: {ex.Message}");
            }
        }

        private static async Task SeedAdminUser()
        {
            try
            {
                var usersCollection = _db.GetCollection<User>("Users");
                var adminExists = await usersCollection.Find(u => u.Username == "admin").AnyAsync();

                if (!adminExists)
                {
                    // Hash password "admin123"
                    string hashedPassword = HashPassword("admin123");

                    var adminUser = new User
                    {
                        Username = "admin",
                        PasswordHash = hashedPassword,
                        FullName = "System Administrator",
                        Role = "Admin",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        MustChangePassword = false  // Admin doesn't need to change password
                    };

                    await usersCollection.InsertOneAsync(adminUser);
                    Console.WriteLine("Admin user seeded! Username: admin, Password: admin123");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding admin: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}