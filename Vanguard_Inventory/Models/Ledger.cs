using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Vanguard_Inventory.Models
{
    public class Ledger
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("product_id")]
        public ObjectId ProductId { get; set; }

        [BsonElement("product_sku")]
        public string ProductSKU { get; set; }

        [BsonElement("product_name")]
        public string ProductName { get; set; }

        [BsonElement("user_id")]
        public ObjectId UserId { get; set; }

        [BsonElement("user_name")]
        public string UserName { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } // Sale, Restock, Damaged, Adjustment

        [BsonElement("quantity_change")]
        public int QuantityChange { get; set; } // Positive for restock, negative for sales

        [BsonElement("previous_stock")]
        public int PreviousStock { get; set; }

        [BsonElement("new_stock")]
        public int NewStock { get; set; }

        [BsonElement("total_value")]
        public decimal TotalValue { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }
    }
}