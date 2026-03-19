using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Vanguard_Inventory
{
    public partial class Form1 : Form
    {
        // Animation Variables
        private Timer animationTimer = new Timer();
        private int targetTop = 0;

        // MongoDB Variables
        private IMongoDatabase database;
        private string connectionString = "mongodb://localhost:27017";

        // --- Win32 API for dragging the borderless form ---
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public Form1()
        {
            InitializeComponent();
            SetupAnimationTimer();
            InitializeNavigation();

            // Initial check
            CheckDatabaseConnection();
        }

        private void SetupAnimationTimer()
        {
            animationTimer.Interval = 10;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void InitializeNavigation()
        {
            pnlNavIndicator.BringToFront();
            MoveIndicator(btnDashboard);
            lblPageTitle.Text = "Dashboard";
        }

        // --- MongoDB Connection Logic ---

        // This is the background check used for UI status
        private async void CheckDatabaseConnection()
        {
            await RunMongoPing(false); // Run silently without a popup
        }

        // This is the core logic that actually "pings" the server
        private async Task<bool> RunMongoPing(bool showManualFeedback)
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);

                var client = new MongoClient(settings);
                database = client.GetDatabase("VanguardDB");

                // EXPLICIT PING: This sends a command to the server and waits for a reply
                await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");

                UpdateConnectionStatus("• MongoDB: Connected", Color.LimeGreen);

                if (showManualFeedback)
                {
                    MessageBox.Show("Ping Successful! MongoDB is reachable.", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus("• MongoDB: Disconnected", Color.Tomato);

                if (showManualFeedback)
                {
                    MessageBox.Show($"Ping Failed!\n\nError: {ex.Message}", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        private void UpdateConnectionStatus(string text, Color color)
        {
            if (lblMongoStatus.InvokeRequired)
            {
                lblMongoStatus.Invoke(new Action(() => UpdateConnectionStatus(text, color)));
                return;
            }
            lblMongoStatus.Text = text;
            lblMongoStatus.ForeColor = color;
        }

        // --- Window Controls ---
        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Text = "❐";
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                btnMaximize.Text = "▢";
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        // --- Form Dragging ---
        private void pnlTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        // --- Navigation & Animation Logic ---
        private void Navigation_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                MoveIndicator(btn);
                lblPageTitle.Text = btn.Text;

                switch (btn.Name)
                {
                    case "btnDashboard": break;
                    case "btnCatalog": break;
                }
            }
        }

        private void MoveIndicator(Button btn)
        {
            ResetButtonColors();
            btn.BackColor = Color.FromArgb(230, 230, 230);
            btn.ForeColor = Color.FromArgb(45, 45, 45);

            targetTop = btn.Top;
            pnlNavIndicator.Height = btn.Height;
            pnlNavIndicator.BringToFront();
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            int distance = targetTop - pnlNavIndicator.Top;
            if (Math.Abs(distance) < 2)
            {
                pnlNavIndicator.Top = targetTop;
                animationTimer.Stop();
            }
            else
            {
                pnlNavIndicator.Top += distance / 2;
            }
        }

        private void ResetButtonColors()
        {
            foreach (Control ctrl in pnlSidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = Color.White;
                }
            }
        }

        // --- Manual Check Trigger ---
        private async void lblMongo_Click(object sender, EventArgs e)
        {
            // When you click the label, it performs a ping and shows a MessageBox
            lblMongoStatus.Text = "• MongoDB: Pinging...";
            lblMongoStatus.ForeColor = Color.Orange;

            await RunMongoPing(true);
        }

        private void LoadUserControl(UserControl userControl)
        {
            if (pnlMainContent.Controls.Count > 0)
                pnlMainContent.Controls.Clear();

            userControl.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(userControl);
        }
    }
}