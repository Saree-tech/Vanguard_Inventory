using System;

namespace Vanguard_Inventory.Services
{
    public static class SessionManager
    {
        // This stores the currently logged-in user's info
        public static string Username { get; set; }
        public static string Role { get; set; } // Admin, Seller, or Viewer
        public static string FullName { get; set; }

        // Helper to check if the user is an Admin
        public static bool IsAdmin => Role == "Admin";

        // Call this when the user clicks Logout
        public static void Logout()
        {
            Username = null;
            Role = null;
            FullName = null;
        }
    }
}