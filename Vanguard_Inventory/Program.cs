using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanguard_Inventory.Services; // IMPORTANT: This tells the app where our Service is

namespace Vanguard_Inventory
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Turn on the Database connection here
            DatabaseService.Initialize();

            Application.Run(new Form1());
        }
    }
}