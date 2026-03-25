using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vanguard_Inventory.Models;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Vanguard_Inventory.Services
{
    public static class AnalyticsService
    {
        private static IMongoCollection<Analytics> analyticsCollection;
        private static IMongoCollection<Product> productsCollection;
        private static IMongoCollection<Ledger> ledgerCollection;

        public static void Initialize()
        {
            analyticsCollection = DatabaseService.GetCollection<Analytics>("Analytics");
            productsCollection = DatabaseService.GetCollection<Product>("Products");
            ledgerCollection = DatabaseService.GetCollection<Ledger>("Ledger");
        }

        public static async Task CalculateAndUpdateAnalytics()
        {
            try
            {
                var products = await productsCollection.Find(_ => true).ToListAsync();
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

                foreach (var product in products)
                {
                    // Get sales from last 30 days
                    var sales = await ledgerCollection.Find(l =>
                        l.ProductId == product.Id &&
                        l.Type == "Sale" &&
                        l.Timestamp >= thirtyDaysAgo).ToListAsync();

                    int totalSales = sales.Sum(s => Math.Abs(s.QuantityChange));
                    double avgDailySales = totalSales / 30.0;

                    int daysUntilOut = 0;
                    string restockPriority = "Low";
                    string stockTrend = "Stable 📊";

                    if (avgDailySales > 0)
                    {
                        daysUntilOut = (int)(product.Stock.Current / avgDailySales);
                    }
                    else
                    {
                        daysUntilOut = 999; // No sales, stock will last long
                    }

                    // Determine restock priority
                    if (daysUntilOut <= 3)
                        restockPriority = "Critical 🔴";
                    else if (daysUntilOut <= 7)
                        restockPriority = "High 🟠";
                    else if (daysUntilOut <= 14)
                        restockPriority = "Medium 🟡";
                    else
                        restockPriority = "Low 🟢";

                    // Determine stock trend (compare last 7 days with previous 7 days)
                    var last7Days = await ledgerCollection.Find(l =>
                        l.ProductId == product.Id &&
                        l.Type == "Sale" &&
                        l.Timestamp >= DateTime.UtcNow.AddDays(-7)).ToListAsync();

                    var previous7Days = await ledgerCollection.Find(l =>
                        l.ProductId == product.Id &&
                        l.Type == "Sale" &&
                        l.Timestamp >= DateTime.UtcNow.AddDays(-14) &&
                        l.Timestamp < DateTime.UtcNow.AddDays(-7)).ToListAsync();

                    int lastWeekSales = last7Days.Sum(s => Math.Abs(s.QuantityChange));
                    int previousWeekSales = previous7Days.Sum(s => Math.Abs(s.QuantityChange));

                    if (lastWeekSales > previousWeekSales * 1.2)
                        stockTrend = "Increasing 📈";
                    else if (lastWeekSales < previousWeekSales * 0.8)
                        stockTrend = "Decreasing 📉";
                    else
                        stockTrend = "Stable 📊";

                    // Update or insert analytics
                    var filter = Builders<Analytics>.Filter.Eq(a => a.ProductId, product.Id);
                    var existing = await analyticsCollection.Find(filter).FirstOrDefaultAsync();

                    var analytics = new Analytics
                    {
                        ProductId = product.Id,
                        ProductSKU = product.SKU,
                        ProductName = product.Name,
                        AvgDailySales = Math.Round(avgDailySales, 2),
                        DaysUntilOut = Math.Max(0, daysUntilOut),
                        RestockPriority = restockPriority,
                        LastCalculated = DateTime.UtcNow,
                        TotalSalesLast30Days = totalSales,
                        StockTrend = stockTrend
                    };

                    if (existing == null)
                    {
                        await analyticsCollection.InsertOneAsync(analytics);
                    }
                    else
                    {
                        var update = Builders<Analytics>.Update
                            .Set(a => a.AvgDailySales, analytics.AvgDailySales)
                            .Set(a => a.DaysUntilOut, analytics.DaysUntilOut)
                            .Set(a => a.RestockPriority, analytics.RestockPriority)
                            .Set(a => a.LastCalculated, analytics.LastCalculated)
                            .Set(a => a.TotalSalesLast30Days, analytics.TotalSalesLast30Days)
                            .Set(a => a.StockTrend, analytics.StockTrend);

                        await analyticsCollection.UpdateOneAsync(filter, update);
                    }
                }

                Console.WriteLine("Analytics updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating analytics: {ex.Message}");
            }
        }

        public static async Task<List<Analytics>> GetTopMovingItems(int count = 5)
        {
            try
            {
                var analytics = await analyticsCollection.Find(_ => true)
                    .SortByDescending(a => a.TotalSalesLast30Days)
                    .Limit(count)
                    .ToListAsync();
                return analytics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting top moving items: {ex.Message}");
                return new List<Analytics>();
            }
        }

        public static async Task<List<Analytics>> GetCriticalItems()
        {
            try
            {
                var critical = await analyticsCollection.Find(a =>
                    a.RestockPriority == "Critical 🔴" || a.RestockPriority == "High 🟠")
                    .ToListAsync();
                return critical;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting critical items: {ex.Message}");
                return new List<Analytics>();
            }
        }

        public static async Task<Analytics> GetProductAnalytics(ObjectId productId)
        {
            try
            {
                return await analyticsCollection.Find(a => a.ProductId == productId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product analytics: {ex.Message}");
                return null;
            }
        }

        public static async Task<List<Analytics>> GetAllAnalytics()
        {
            try
            {
                return await analyticsCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all analytics: {ex.Message}");
                return new List<Analytics>();
            }
        }
    }
}