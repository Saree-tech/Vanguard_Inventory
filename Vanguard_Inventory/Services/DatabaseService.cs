using MongoDB.Driver;
using System;
using System.Windows.Forms;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to MongoDB: " + ex.Message);
            }
        }

        // This helper lets us grab any collection (Products, Users, etc.) easily
        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}