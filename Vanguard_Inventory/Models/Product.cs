using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Vanguard_Inventory.Models
{
    public class Product
    {
        [BsonId] // This is the primary key for MongoDB
        public ObjectId Id { get; set; }

        [BsonElement("sku")]
        public string SKU { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        // We embed the stock info directly for better performance
        public StockInfo Stock { get; set; }

        // Useful for the "Omnibox" global search
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class StockInfo
    {
        public int Current { get; set; }
        public int MinThreshold { get; set; }
        public int MaxCapacity { get; set; }
    }
}