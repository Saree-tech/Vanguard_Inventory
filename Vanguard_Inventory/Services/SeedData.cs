using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Vanguard_Inventory.Services
{
    public static class SeedData
    {
        public static async void Seed()
        {
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var db = client.GetDatabase("Vanguard_Inventory_DB");

                // Seed Users
                var usersCollection = db.GetCollection<BsonDocument>("Users");
                if (await usersCollection.CountDocumentsAsync(new BsonDocument()) == 0)
                {
                    // Create sample users (password is "admin123" and "seller123" - you'll need to hash these)
                    var users = new List<BsonDocument>
                    {
                        new BsonDocument
                        {
                            { "username", "admin" },
                            { "password_hash", HashPassword("admin123") }, // You'll implement hash
                            { "full_name", "System Administrator" },
                            { "role", "Admin" },
                            { "created_at", DateTime.UtcNow },
                            { "is_active", true }
                        },
                        new BsonDocument
                        {
                            { "username", "seller" },
                            { "password_hash", HashPassword("seller123") },
                            { "full_name", "Sales Staff" },
                            { "role", "Seller" },
                            { "created_at", DateTime.UtcNow },
                            { "is_active", true }
                        },
                        new BsonDocument
                        {
                            { "username", "viewer" },
                            { "password_hash", HashPassword("viewer123") },
                            { "full_name", "Inventory Viewer" },
                            { "role", "Viewer" },
                            { "created_at", DateTime.UtcNow },
                            { "is_active", true }
                        }
                    };

                    await usersCollection.InsertManyAsync(users);
                    Console.WriteLine("Users seeded!");
                }

                // Seed Products
                var productsCollection = db.GetCollection<BsonDocument>("Products");
                if (await productsCollection.CountDocumentsAsync(new BsonDocument()) == 0)
                {
                    var products = new List<BsonDocument>
                    {
                        new BsonDocument
                        {
                            { "sku", "VG-001" },
                            { "name", "Industrial Sensor X1" },
                            { "description", "High-precision industrial temperature sensor" },
                            { "price", 89.99m },
                            { "stock", new BsonDocument
                                {
                                    { "current", 145 },
                                    { "min_threshold", 30 },
                                    { "max_capacity", 500 }
                                }
                            },
                            { "category", "Electronics" },
                            { "tags", new BsonArray { "Sensor", "Industrial", "Bestseller" } }
                        },
                        new BsonDocument
                        {
                            { "sku", "VG-002" },
                            { "name", "Smart Controller Pro" },
                            { "description", "IoT-enabled smart controller" },
                            { "price", 199.99m },
                            { "stock", new BsonDocument
                                {
                                    { "current", 12 },
                                    { "min_threshold", 25 },
                                    { "max_capacity", 200 }
                                }
                            },
                            { "category", "Controllers" },
                            { "tags", new BsonArray { "IoT", "Smart", "Critical" } }
                        },
                        new BsonDocument
                        {
                            { "sku", "VG-003" },
                            { "name", "Power Supply Unit" },
                            { "description", "Industrial grade power supply" },
                            { "price", 45.50m },
                            { "stock", new BsonDocument
                                {
                                    { "current", 78 },
                                    { "min_threshold", 15 },
                                    { "max_capacity", 300 }
                                }
                            },
                            { "category", "Power" },
                            { "tags", new BsonArray { "Power", "Essential" } }
                        }
                    };

                    await productsCollection.InsertManyAsync(products);
                    Console.WriteLine("Products seeded!");
                }

                Console.WriteLine("Database seeding completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }

        // Simple hash for demo - use BCrypt in production
        private static string HashPassword(string password)
        {
            // For demo only - use proper hashing in production
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}