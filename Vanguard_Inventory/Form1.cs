using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using Vanguard_Inventory.Services;
using Vanguard_Inventory.Models;
using Vanguard_Inventory.UserControls;
using System.Linq;

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

        // Product collection for database operations
        private IMongoCollection<Product> productsCollection;

        // --- Win32 API for dragging the borderless form ---
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public Form1()
        {
            InitializeComponent();

            // Initialize database connection
            DatabaseService.Initialize();
            productsCollection = DatabaseService.GetCollection<Product>("Products");

            SetupAnimationTimer();
            InitializeNavigation();

            // Apply enhanced styling to dashboard cards
            StyleDashboardCards();

            // Load real dashboard data
            LoadDashboardData();

            // Initial check
            CheckDatabaseConnection();

            // Add logout button
            AddLogoutButton();

            // WIRE UP BUTTON CLICK EVENTS - THIS IS THE CRITICAL FIX
            btnDashboard.Click += Navigation_Click;
            btnCatalog.Click += Navigation_Click;
            btnLedger.Click += Navigation_Click;
            btnUsers.Click += Navigation_Click;
            btnSettings.Click += Navigation_Click;
        }

        // NEW: Style dashboard cards with modern look
        private void StyleDashboardCards()
        {
            // Style the main stat cards with hover effects
            StyleCard(pnlCardValue, Color.FromArgb(42, 42, 60));
            StyleCard(pnlCardAlert, Color.FromArgb(55, 40, 40));
            StyleCard(pnlCardStock, Color.FromArgb(42, 60, 50));

            // Style the graph container
            pnlGraphContainer.BackColor = Color.FromArgb(42, 42, 60);
            pnlGraphContainer.Padding = new Padding(20);

            // Style the critical alerts panel
            pnlCriticalAlerts.BackColor = Color.FromArgb(42, 42, 60);
            pnlCriticalAlerts.Padding = new Padding(20);
        }

        private void StyleCard(Panel card, Color bgColor)
        {
            card.BackColor = bgColor;
            card.Padding = new Padding(20);

            // Add hover effect
            card.MouseEnter += (s, e) => {
                card.BackColor = ControlPaint.Light(bgColor, 0.1f);
                card.Cursor = Cursors.Hand;
            };
            card.MouseLeave += (s, e) => {
                card.BackColor = bgColor;
            };
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

        // Method to load real data from MongoDB
        private async void LoadDashboardData()
        {
            try
            {
                // Show loading state
                lblCardValueData.Text = "Loading...";
                lblCardAlertData.Text = "Loading...";
                lblCardStockData.Text = "Loading...";

                // Get all products from MongoDB
                var products = await productsCollection.Find(_ => true).ToListAsync();

                if (products != null && products.Any())
                {
                    // Calculate Total Inventory Value (price * current stock)
                    decimal totalValue = products.Sum(p => p.Price * p.Stock.Current);
                    lblCardValueData.Text = $"${totalValue:N2}";

                    // Add subtitle for total value
                    AddCardSubtitle(pnlCardValue, "Total Value");

                    // Calculate Items at Risk (current stock <= min threshold)
                    int itemsAtRisk = products.Count(p => p.Stock.Current <= p.Stock.MinThreshold);
                    lblCardAlertData.Text = $"{itemsAtRisk} Items";
                    AddCardSubtitle(pnlCardAlert, "Items Below Threshold");

                    // Calculate Total Items in Stock (sum of all current stock)
                    int totalItems = products.Sum(p => p.Stock.Current);
                    lblCardStockData.Text = totalItems.ToString("N0");
                    AddCardSubtitle(pnlCardStock, "Total Quantity");

                    // Update the critical risks panel with enhanced design
                    UpdateCriticalRisksList(products.Where(p => p.Stock.Current <= p.Stock.MinThreshold).ToList());

                    // Update the graph with real data
                    UpdateGraphWithData(products);
                }
                else
                {
                    ShowEmptyDashboardState();
                }
            }
            catch (Exception ex)
            {
                ShowErrorDashboardState(ex);
            }
        }

        // NEW: Add subtitle to stat cards
        private void AddCardSubtitle(Panel card, string subtitle)
        {
            // Remove existing subtitle if any
            foreach (Control ctrl in card.Controls)
            {
                if (ctrl is Label label && label.Name == "subtitle")
                {
                    card.Controls.Remove(label);
                    break;
                }
            }

            var subtitleLabel = new Label
            {
                Name = "subtitle",
                Text = subtitle,
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Location = new Point(20, 95),
                AutoSize = true
            };
            card.Controls.Add(subtitleLabel);
        }

        // NEW: Update graph with real data visualization
        private void UpdateGraphWithData(System.Collections.Generic.List<Product> products)
        {
            // Clear existing controls except the title
            for (int i = pnlGraphContainer.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlGraphContainer.Controls[i] != lblGraphTitle)
                {
                    pnlGraphContainer.Controls.RemoveAt(i);
                }
            }

            // Create a panel for graph visualization
            var graphPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(pnlGraphContainer.Width - 40, pnlGraphContainer.Height - 100),
                BackColor = Color.FromArgb(35, 35, 52)
            };

            // Add stock trend bars for top products
            int yPos = 20;
            var topProducts = products.OrderByDescending(p => p.Stock.Current).Take(6).ToList();

            foreach (var product in topProducts)
            {
                // Product label
                var productLabel = new Label
                {
                    Text = product.Name.Length > 20 ? product.Name.Substring(0, 20) + "..." : product.Name,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F),
                    Location = new Point(10, yPos),
                    AutoSize = true
                };
                graphPanel.Controls.Add(productLabel);

                // Stock percentage bar
                int stockPercent = (product.Stock.Current * 100) / product.Stock.MaxCapacity;
                var barBg = new Panel
                {
                    Location = new Point(200, yPos + 3),
                    Size = new Size(200, 12),
                    BackColor = Color.FromArgb(55, 55, 75)
                };

                var barFill = new Panel
                {
                    Location = new Point(0, 0),
                    Size = new Size(stockPercent * 2, 12),
                    BackColor = stockPercent < 30 ? Color.FromArgb(255, 69, 58) : Color.FromArgb(0, 122, 255)
                };
                barBg.Controls.Add(barFill);
                graphPanel.Controls.Add(barBg);

                // Percentage label
                var percentLabel = new Label
                {
                    Text = $"{stockPercent}%",
                    ForeColor = Color.FromArgb(150, 150, 170),
                    Font = new Font("Segoe UI", 8F),
                    Location = new Point(410, yPos + 2),
                    AutoSize = true
                };
                graphPanel.Controls.Add(percentLabel);

                yPos += 28;
            }

            // Add summary info
            var summaryLabel = new Label
            {
                Text = $"📊 Total Products: {products.Count} | Total Value: ${products.Sum(p => p.Price * p.Stock.Current):N2}",
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 9F),
                Location = new Point(10, yPos + 10),
                AutoSize = true
            };
            graphPanel.Controls.Add(summaryLabel);

            pnlGraphContainer.Controls.Add(graphPanel);
        }

        // Enhanced method to update the critical risks panel
        private void UpdateCriticalRisksList(System.Collections.Generic.List<Product> criticalProducts)
        {
            // Clear existing items but keep the title label
            for (int i = pnlCriticalAlerts.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlCriticalAlerts.Controls[i] != lblCriticalTitle)
                {
                    pnlCriticalAlerts.Controls.RemoveAt(i);
                }
            }

            if (!criticalProducts.Any())
            {
                var safeLabel = new Label
                {
                    Text = "✓ All items are above threshold",
                    ForeColor = Color.LightGreen,
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(20, 60),
                    AutoSize = true
                };
                pnlCriticalAlerts.Controls.Add(safeLabel);
                return;
            }

            int yPos = 60;
            foreach (var product in criticalProducts.OrderBy(p => p.Stock.Current))
            {
                var riskPanel = CreateEnhancedRiskPanel(product, yPos);
                pnlCriticalAlerts.Controls.Add(riskPanel);
                yPos += 85;
            }
        }

        // Enhanced risk panel with better visual design
        private Panel CreateEnhancedRiskPanel(Product product, int yPosition)
        {
            var panel = new Panel
            {
                Location = new Point(15, yPosition),
                Size = new Size(340, 80),
                BackColor = Color.FromArgb(55, 40, 40),
                Padding = new Padding(12)
            };

            // Product name and SKU
            var nameLabel = new Label
            {
                Text = product.Name,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(10, 8),
                AutoSize = true
            };

            var skuLabel = new Label
            {
                Text = product.SKU,
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 8F),
                Location = new Point(10, 32),
                AutoSize = true
            };

            // Stock info with warning icon
            var stockLabel = new Label
            {
                Text = $"⚠️ Current: {product.Stock.Current} | Min Required: {product.Stock.MinThreshold}",
                ForeColor = Color.FromArgb(255, 150, 150),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(10, 52),
                AutoSize = true
            };

            // Urgency badge
            int shortage = product.Stock.MinThreshold - product.Stock.Current;
            int urgencyPercent = (shortage * 100) / product.Stock.MinThreshold;
            var urgencyBadge = new Label
            {
                Text = urgencyPercent > 50 ? "CRITICAL" : "WARNING",
                ForeColor = Color.White,
                BackColor = urgencyPercent > 50 ? Color.FromArgb(200, 50, 50) : Color.FromArgb(200, 120, 50),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(250, 8),
                Size = new Size(70, 22),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panel.Controls.Add(nameLabel);
            panel.Controls.Add(skuLabel);
            panel.Controls.Add(stockLabel);
            panel.Controls.Add(urgencyBadge);

            return panel;
        }

        private void ShowEmptyDashboardState()
        {
            lblCardValueData.Text = "$0";
            lblCardAlertData.Text = "0 Items";
            lblCardStockData.Text = "0";

            var emptyLabel = new Label
            {
                Text = "📦 No products found.\nClick 'Catalog' to add products.",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 11F),
                Location = new Point(20, 60),
                AutoSize = true
            };
            pnlCriticalAlerts.Controls.Add(emptyLabel);

            // Clear graph container except title
            for (int i = pnlGraphContainer.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlGraphContainer.Controls[i] != lblGraphTitle)
                {
                    pnlGraphContainer.Controls.RemoveAt(i);
                }
            }

            var graphEmptyLabel = new Label
            {
                Text = "Add products to see stock trends",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 60),
                AutoSize = true
            };
            pnlGraphContainer.Controls.Add(graphEmptyLabel);
        }

        private void ShowErrorDashboardState(Exception ex)
        {
            MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Database Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblCardValueData.Text = "Error";
            lblCardAlertData.Text = "Error";
            lblCardStockData.Text = "Error";
        }

        // --- MongoDB Connection Logic ---

        private async void CheckDatabaseConnection()
        {
            await RunMongoPing(false);
        }

        // Add this method to Form1.cs
        public void RefreshDashboard()
        {
            // Refresh dashboard data
            LoadDashboardData();
        }

        private async Task<bool> RunMongoPing(bool showManualFeedback)
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(connectionString);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);

                var client = new MongoClient(settings);
                database = client.GetDatabase("VanguardDB");

                await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");

                UpdateConnectionStatus("● MongoDB: Connected", Color.LimeGreen);

                if (showManualFeedback)
                {
                    MessageBox.Show("Ping Successful! MongoDB is reachable.", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus("● MongoDB: Disconnected", Color.Tomato);

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

        // Method to load catalog
        private void LoadCatalog()
        {
            // Clear main content panel
            pnlMainContent.Controls.Clear();

            // Create new catalog user control
            var catalog = new ucCatalog();
            catalog.Dock = DockStyle.Fill;

            // Add to main content
            pnlMainContent.Controls.Add(catalog);

            // Optional: Show success message
            Console.WriteLine("Catalog loaded successfully");
        }

        private void LoadUsersView()
        {
            pnlMainContent.Controls.Clear();
            var users = new ucUsers();
            users.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(users);
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
                    case "btnDashboard":
                        // Restore dashboard view
                        LoadDashboardView();
                        break;
                    case "btnCatalog":
                        LoadCatalog();
                        break;
                    case "btnLedger":
                        LoadLedger();
                        break;
                    case "btnUsers":
                        LoadUsersView();
                        break;
                    case "btnSettings":
                        MessageBox.Show("Settings coming soon!", "Coming Soon",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
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

        // Add this method for the refresh button click event
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
        }
        // Add this method to Form1.cs
        // Method to restore dashboard view
        private void LoadDashboardView()
        {
            // Clear any user controls from main content
            pnlMainContent.Controls.Clear();

            // Re-add the dashboard controls that were removed
            // The dashboard controls are: pnlLowerSection, flpStatCards, lblPageTitle
            // They were originally in pnlMainContent, so we need to re-add them

            // Since the dashboard is built into the form, we need to restore the original layout
            // Re-add the controls in the correct order

            // First, add the page title
            pnlMainContent.Controls.Add(lblPageTitle);

            // Then add the stat cards
            pnlMainContent.Controls.Add(flpStatCards);

            // Then add the lower section (graph and critical alerts)
            pnlMainContent.Controls.Add(pnlLowerSection);

            // Make sure everything is visible
            lblPageTitle.Visible = true;
            flpStatCards.Visible = true;
            pnlLowerSection.Visible = true;
            pnlGraphContainer.Visible = true;
            pnlCriticalAlerts.Visible = true;

            // Refresh dashboard data
            LoadDashboardData();

            // Ensure proper z-order
            lblPageTitle.BringToFront();
            flpStatCards.BringToFront();
            pnlLowerSection.BringToFront();
        }

        // --- Manual Check Trigger ---
        private async void lblMongo_Click(object sender, EventArgs e)
        {
            lblMongoStatus.Text = "● MongoDB: Pinging...";
            lblMongoStatus.ForeColor = Color.Orange;

            await RunMongoPing(true);
        }

        private void LoadLedger()
        {
            pnlMainContent.Controls.Clear();
            var ledger = new ucLedger();
            ledger.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(ledger);
        }

        // Add this method to Form1.cs
        private void AddLogoutButton()
        {
            var btnLogout = new Button
            {
                Text = "🚪 Logout",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
                Size = new Size(90, 35),
                Location = new Point(btnRefresh.Location.X - 100, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                SessionManager.Logout();
                Application.Restart();
            };
            pnlTopBar.Controls.Add(btnLogout);
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