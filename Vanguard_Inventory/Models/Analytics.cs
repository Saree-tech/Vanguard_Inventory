using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Vanguard_Inventory.Models
{
    public class Analytics
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("product_id")]
        public ObjectId ProductId { get; set; }

        [BsonElement("product_sku")]
        public string ProductSKU { get; set; }

        [BsonElement("product_name")]
        public string ProductName { get; set; }

        [BsonElement("avg_daily_sales")]
        public double AvgDailySales { get; set; }

        [BsonElement("days_until_out")]
        public int DaysUntilOut { get; set; }

        [BsonElement("restock_priority")]
        public string RestockPriority { get; set; }

        [BsonElement("last_calculated")]
        public DateTime LastCalculated { get; set; }

        [BsonElement("total_sales_last_30_days")]
        public int TotalSalesLast30Days { get; set; }

        [BsonElement("stock_trend")]
        public string StockTrend { get; set; }
    }
}