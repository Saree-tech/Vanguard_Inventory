using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vanguard_Inventory.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; } // We store the hash, not the real password

        [BsonElement("full_name")]
        public string FullName { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } // Admin, Seller, or Viewer
    }
}