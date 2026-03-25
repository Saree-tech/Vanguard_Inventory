using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Vanguard_Inventory.Models
{
    public class Product
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("sku")]
        public string SKU { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        // This is the embedded stock info
        [BsonElement("stock")]
        public StockInfo Stock { get; set; }

        // Useful for the "Omnibox" global search
        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
}