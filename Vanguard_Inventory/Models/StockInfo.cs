using MongoDB.Bson.Serialization.Attributes;

namespace Vanguard_Inventory.Models
{
    public class StockInfo
    {
        [BsonElement("current")]
        public int Current { get; set; }

        [BsonElement("min_threshold")]
        public int MinThreshold { get; set; }

        [BsonElement("max_capacity")]
        public int MaxCapacity { get; set; }
    }
}