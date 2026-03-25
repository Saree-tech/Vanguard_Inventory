using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vanguard_Inventory.Models;

namespace Vanguard_Inventory.Services
{
    public static class LedgerService
    {
        private static IMongoCollection<Ledger> ledgerCollection;

        public static void Initialize()
        {
            ledgerCollection = DatabaseService.GetCollection<Ledger>("Ledger");
        }

        public static async Task RecordTransaction(
            MongoDB.Bson.ObjectId productId,
            string productSKU,
            string productName,
            string type,
            int quantityChange,
            int previousStock,
            int newStock,
            decimal unitPrice,
            string notes = "")
        {
            try
            {
                var ledgerEntry = new Ledger
                {
                    ProductId = productId,
                    ProductSKU = productSKU,
                    ProductName = productName,
                    UserId = SessionManager.UserId,
                    UserName = SessionManager.FullName,
                    Type = type,
                    QuantityChange = quantityChange,
                    PreviousStock = previousStock,
                    NewStock = newStock,
                    TotalValue = unitPrice * Math.Abs(quantityChange),
                    Timestamp = DateTime.UtcNow,
                    Notes = notes
                };

                await ledgerCollection.InsertOneAsync(ledgerEntry);
                Console.WriteLine($"Transaction recorded: {type} - {quantityChange} units of {productName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recording transaction: {ex.Message}");
            }
        }

        public static async Task<List<Ledger>> GetTransactions(
            string type = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string userName = null)
        {
            try
            {
                var filterBuilder = Builders<Ledger>.Filter;
                var filter = filterBuilder.Empty;

                if (!string.IsNullOrEmpty(type))
                {
                    filter &= filterBuilder.Eq(l => l.Type, type);
                }

                if (fromDate.HasValue)
                {
                    filter &= filterBuilder.Gte(l => l.Timestamp, fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    filter &= filterBuilder.Lte(l => l.Timestamp, toDate.Value);
                }

                if (!string.IsNullOrEmpty(userName) && SessionManager.IsAdmin)
                {
                    filter &= filterBuilder.Eq(l => l.UserName, userName);
                }
                else if (!SessionManager.IsAdmin)
                {
                    // Sellers can only see their own transactions
                    filter &= filterBuilder.Eq(l => l.UserId, SessionManager.UserId);
                }

                var sort = Builders<Ledger>.Sort.Descending(l => l.Timestamp);
                var transactions = await ledgerCollection.Find(filter).Sort(sort).ToListAsync();
                return transactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transactions: {ex.Message}");
                return new List<Ledger>();
            }
        }
    }
}