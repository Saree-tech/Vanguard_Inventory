using System;
using System.Windows.Forms;
using Vanguard_Inventory.Services;

namespace Vanguard_Inventory
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize database connection
            DatabaseService.Initialize();

            // Show login form
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Login successful, open main form
                    Application.Run(new Form1());
                }
                else
                {
                    // Login cancelled or failed
                    return;
                }
            }
        }
    }
}