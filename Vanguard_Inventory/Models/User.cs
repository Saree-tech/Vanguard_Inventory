using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Vanguard_Inventory.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; }

        [BsonElement("full_name")]
        public string FullName { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } // Admin, Seller, Viewer

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        [BsonElement("must_change_password")]
        public bool MustChangePassword { get; set; } = false;

        [BsonElement("password_reset_requested")]
        public bool PasswordResetRequested { get; set; } = false;
    }
}