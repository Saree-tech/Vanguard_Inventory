using MongoDB.Bson;

namespace Vanguard_Inventory.Services
{
    public static class SessionManager
    {
        public static string Username { get; set; }
        public static string Role { get; set; }
        public static string FullName { get; set; }
        public static ObjectId UserId { get; set; }

        public static bool IsAdmin => Role == "Admin";
        public static bool IsSeller => Role == "Seller";
        public static bool IsViewer => Role == "Viewer";
        public static bool IsLoggedIn => !string.IsNullOrEmpty(Username);

        public static void Logout()
        {
            Username = null;
            Role = null;
            FullName = null;
            UserId = ObjectId.Empty;
        }
    }
}