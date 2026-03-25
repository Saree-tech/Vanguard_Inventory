using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using Vanguard_Inventory.Models;
using Vanguard_Inventory.Services;
using System.Collections.Generic;

namespace Vanguard_Inventory.UserControls
{
    public partial class ucLedger : UserControl
    {
        private DataGridView dgvTransactions;
        private ComboBox cmbFilterType;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private Button btnFilter;
        private Button btnRefresh;
        private Label lblTotalRecords;
        private Label lblStats;
        private Panel pnlStats;

        private List<Ledger> allTransactions = new List<Ledger>();

        public ucLedger()
        {
            InitializeComponent();
            CreateControls();
            LoadTransactions();
        }

        private void CreateControls()
        {
            this.BackColor = Color.FromArgb(30, 30, 44);
            this.Padding = new Padding(0);

            // Top Stats Panel - Modern cards
            pnlStats = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(30, 30, 44),
                Padding = new Padding(20, 15, 20, 15)
            };

            // Filter Panel
            var pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(35, 35, 52),
                Padding = new Padding(20, 15, 20, 15)
            };

            // Filter Controls
            var lblType = new Label
            {
                Text = "📋 TRANSACTION TYPE",
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(15, 5),
                AutoSize = true
            };

            cmbFilterType = new ComboBox
            {
                Location = new Point(15, 28),
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(45, 45, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Cursor = Cursors.Hand
            };
            cmbFilterType.Items.AddRange(new string[] { "All", "💰 Sale", "📦 Restock", "⚠️ Damaged", "⚙️ Adjustment" });
            cmbFilterType.SelectedIndex = 0;

            // From Date
            var lblFrom = new Label
            {
                Text = "📅 FROM DATE",
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(180, 5),
                AutoSize = true
            };

            dtpFromDate = new DateTimePicker
            {
                Location = new Point(180, 28),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(45, 45, 60),
                ForeColor = Color.White,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-30)
            };

            // To Date
            var lblTo = new Label
            {
                Text = "📅 TO DATE",
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Location = new Point(355, 5),
                AutoSize = true
            };

            dtpToDate = new DateTimePicker
            {
                Location = new Point(355, 28),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(45, 45, 60),
                ForeColor = Color.White,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Filter Button
            btnFilter = new Button
            {
                Text = "🔍 APPLY FILTER",
                Location = new Point(530, 25),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFilter.FlatAppearance.BorderSize = 0;
            btnFilter.Click += BtnFilter_Click;
            btnFilter.MouseEnter += (s, e) => btnFilter.BackColor = Color.FromArgb(50, 150, 255);
            btnFilter.MouseLeave += (s, e) => btnFilter.BackColor = Color.FromArgb(0, 122, 255);

            // Refresh Button
            btnRefresh = new Button
            {
                Text = "⟳ REFRESH",
                Location = new Point(670, 25),
                Size = new Size(110, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;
            btnRefresh.MouseEnter += (s, e) => btnRefresh.BackColor = Color.FromArgb(100, 100, 120);
            btnRefresh.MouseLeave += (s, e) => btnRefresh.BackColor = Color.FromArgb(80, 80, 100);

            // Export Button
            var btnExport = new Button
            {
                Text = "📎 EXPORT",
                Location = new Point(790, 25),
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(50, 150, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += (s, e) => ExportToCSV();
            btnExport.MouseEnter += (s, e) => btnExport.BackColor = Color.FromArgb(70, 170, 120);
            btnExport.MouseLeave += (s, e) => btnExport.BackColor = Color.FromArgb(50, 150, 100);

            pnlFilter.Controls.Add(lblType);
            pnlFilter.Controls.Add(cmbFilterType);
            pnlFilter.Controls.Add(lblFrom);
            pnlFilter.Controls.Add(dtpFromDate);
            pnlFilter.Controls.Add(lblTo);
            pnlFilter.Controls.Add(dtpToDate);
            pnlFilter.Controls.Add(btnFilter);
            pnlFilter.Controls.Add(btnRefresh);
            pnlFilter.Controls.Add(btnExport);

            // Modern DataGridView
            dgvTransactions = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(50, 50, 70),
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                RowTemplate = { Height = 40 },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
            };

            // Modern styling
            dgvTransactions.EnableHeadersVisualStyles = false;
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 60);
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvTransactions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTransactions.ColumnHeadersHeight = 60;
            dgvTransactions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvTransactions.DefaultCellStyle.BackColor = Color.FromArgb(35, 35, 52);
            dgvTransactions.DefaultCellStyle.ForeColor = Color.White;
            dgvTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 255);
            dgvTransactions.DefaultCellStyle.Padding = new Padding(8, 5, 8, 5);

            // Add columns with FIXED widths
            dgvTransactions.Columns.Add("Timestamp", "🕐 DATE & TIME");
            dgvTransactions.Columns.Add("Type", "📌 TYPE");
            dgvTransactions.Columns.Add("Product", "📦 PRODUCT");
            dgvTransactions.Columns.Add("SKU", "🏷️ SKU");
            dgvTransactions.Columns.Add("Quantity", "🔢 QTY");
            dgvTransactions.Columns.Add("Value", "💰 VALUE");
            dgvTransactions.Columns.Add("User", "👤 USER");
            dgvTransactions.Columns.Add("Stock", "📊 STOCK");
            dgvTransactions.Columns.Add("Notes", "📝 NOTES");

            // Set FIXED column widths (not AutoSize)
            dgvTransactions.Columns["Timestamp"].Width = 140;
            dgvTransactions.Columns["Timestamp"].MinimumWidth = 140;
            dgvTransactions.Columns["Type"].Width = 100;
            dgvTransactions.Columns["Type"].MinimumWidth = 100;
            dgvTransactions.Columns["Product"].Width = 180;
            dgvTransactions.Columns["Product"].MinimumWidth = 150;
            dgvTransactions.Columns["SKU"].Width = 90;
            dgvTransactions.Columns["SKU"].MinimumWidth = 80;
            dgvTransactions.Columns["Quantity"].Width = 70;
            dgvTransactions.Columns["Quantity"].MinimumWidth = 70;
            dgvTransactions.Columns["Value"].Width = 100;
            dgvTransactions.Columns["Value"].MinimumWidth = 90;
            dgvTransactions.Columns["User"].Width = 130;
            dgvTransactions.Columns["User"].MinimumWidth = 120;
            dgvTransactions.Columns["Stock"].Width = 110;
            dgvTransactions.Columns["Stock"].MinimumWidth = 100;
            dgvTransactions.Columns["Notes"].Width = 180;
            dgvTransactions.Columns["Notes"].MinimumWidth = 150;

            // Make columns fill remaining space
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Bottom Status Panel
            var pnlStatus = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(35, 35, 52),
                Padding = new Padding(20, 10, 20, 10)
            };

            lblTotalRecords = new Label
            {
                Text = "📊 0 Records",
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 9F),
                Dock = DockStyle.Left,
                AutoSize = true
            };

            lblStats = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 9F),
                Dock = DockStyle.Right,
                AutoSize = true
            };

            pnlStatus.Controls.Add(lblTotalRecords);
            pnlStatus.Controls.Add(lblStats);

            // Add controls
            this.Controls.Add(dgvTransactions);
            this.Controls.Add(pnlStatus);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(pnlStats);

            CreateStatsCards();
        }

        private void CreateStatsCards()
        {
            pnlStats.Controls.Clear();

            // Total Sales Card
            var cardTotal = CreateStatCard("💰 TOTAL SALES", "$0", Color.FromArgb(0, 122, 255), 0);
            // Total Restocks Card
            var cardRestock = CreateStatCard("📦 TOTAL RESTOCKS", "0", Color.FromArgb(50, 150, 50), 1);
            // Total Transactions Card
            var cardTransactions = CreateStatCard("📋 TOTAL TRANSACTIONS", "0", Color.FromArgb(255, 150, 50), 2);

            pnlStats.Controls.Add(cardTotal);
            pnlStats.Controls.Add(cardRestock);
            pnlStats.Controls.Add(cardTransactions);
        }

        private Panel CreateStatCard(string title, string value, Color color, int index)
        {
            var card = new Panel
            {
                Width = 220,
                Height = 70,
                BackColor = Color.FromArgb(42, 42, 60),
                Location = new Point(20 + (index * 240), 15),
                Cursor = Cursors.Hand
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(150, 150, 170),
                Location = new Point(15, 12),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(15, 35),
                AutoSize = true
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            return card;
        }

        private void UpdateStats()
        {
            if (allTransactions.Any())
            {
                decimal totalSales = allTransactions.Where(t => t.Type == "Sale").Sum(t => t.TotalValue);
                int totalRestocks = allTransactions.Where(t => t.Type == "Restock").Sum(t => t.QuantityChange);

                foreach (Control ctrl in pnlStats.Controls)
                {
                    if (ctrl is Panel card)
                    {
                        foreach (Control child in card.Controls)
                        {
                            if (child is Label lbl && lbl.Font.Bold && lbl.Font.Size == 18)
                            {
                                if (card.Location.X == 20)
                                    lbl.Text = $"${totalSales:N2}";
                                else if (card.Location.X == 260)
                                    lbl.Text = totalRestocks.ToString();
                                else if (card.Location.X == 500)
                                    lbl.Text = allTransactions.Count.ToString();
                            }
                        }
                    }
                }
            }
        }

        private async void LoadTransactions()
        {
            try
            {
                dgvTransactions.Rows.Clear();

                // Show loading indicator
                dgvTransactions.Rows.Add();
                dgvTransactions.Rows[0].Cells[0].Value = "⏳ Loading transactions...";
                dgvTransactions.Rows[0].Height = 50;
                Application.DoEvents();

                string selectedType = cmbFilterType.SelectedItem?.ToString();
                if (selectedType == "All") selectedType = null;
                else if (!string.IsNullOrEmpty(selectedType))
                {
                    selectedType = selectedType.Split(' ').Last();
                }

                allTransactions = await LedgerService.GetTransactions(
                    selectedType,
                    dtpFromDate.Value.Date,
                    dtpToDate.Value.Date.AddDays(1).AddSeconds(-1)
                );

                dgvTransactions.Rows.Clear();

                if (allTransactions.Any())
                {
                    foreach (var t in allTransactions)
                    {
                        int rowIndex = dgvTransactions.Rows.Add();
                        var row = dgvTransactions.Rows[rowIndex];

                        row.Cells["Timestamp"].Value = t.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
                        row.Cells["Type"].Value = GetTypeIcon(t.Type) + " " + t.Type;
                        row.Cells["Product"].Value = t.ProductName.Length > 25 ? t.ProductName.Substring(0, 22) + "..." : t.ProductName;
                        row.Cells["SKU"].Value = t.ProductSKU;

                        string qtyText = t.QuantityChange > 0 ? $"+{t.QuantityChange}" : t.QuantityChange.ToString();
                        row.Cells["Quantity"].Value = qtyText;
                        row.Cells["Quantity"].Style.ForeColor = t.QuantityChange > 0 ? Color.LightGreen : Color.Tomato;
                        row.Cells["Quantity"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                        row.Cells["Value"].Value = $"${t.TotalValue:N2}";
                        row.Cells["User"].Value = t.UserName;
                        row.Cells["Stock"].Value = $"{t.PreviousStock} → {t.NewStock}";
                        row.Cells["Notes"].Value = t.Notes;

                        // Color code rows
                        if (t.Type == "Sale")
                            row.DefaultCellStyle.BackColor = Color.FromArgb(45, 35, 35);
                        else if (t.Type == "Restock")
                            row.DefaultCellStyle.BackColor = Color.FromArgb(35, 45, 35);
                    }

                    lblTotalRecords.Text = $"📊 {allTransactions.Count} Records";
                    UpdateStats();

                    decimal totalValue = allTransactions.Sum(t => t.TotalValue);
                    lblStats.Text = $"💰 Total Value: ${totalValue:N2}";
                }
                else
                {
                    dgvTransactions.Rows.Add();
                    dgvTransactions.Rows[0].Cells[0].Value = "✨ No transactions found for the selected criteria";
                    dgvTransactions.Rows[0].Height = 50;
                    lblTotalRecords.Text = "📊 0 Records";
                    lblStats.Text = "";

                    // Reset stats
                    foreach (Control ctrl in pnlStats.Controls)
                    {
                        if (ctrl is Panel card)
                        {
                            foreach (Control child in card.Controls)
                            {
                                if (child is Label lbl && lbl.Font.Bold && lbl.Font.Size == 18)
                                {
                                    if (card.Location.X == 20)
                                        lbl.Text = "$0";
                                    else if (card.Location.X == 260)
                                        lbl.Text = "0";
                                    else if (card.Location.X == 500)
                                        lbl.Text = "0";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetTypeIcon(string type)
        {
            switch (type)
            {
                case "Sale": return "💰";
                case "Restock": return "📦";
                case "Damaged": return "⚠️";
                case "Adjustment": return "⚙️";
                default: return "📋";
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            cmbFilterType.SelectedIndex = 0;
            dtpFromDate.Value = DateTime.Now.AddDays(-30);
            dtpToDate.Value = DateTime.Now;
            LoadTransactions();
        }

        private void ExportToCSV()
        {
            if (!allTransactions.Any())
            {
                MessageBox.Show("No data to export.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = $"Ledger_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                sfd.Title = "Export Ledger to CSV";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName))
                        {
                            sw.WriteLine("Date & Time,Type,Product,SKU,Quantity,Value,User,Previous Stock,New Stock,Notes");

                            foreach (var t in allTransactions)
                            {
                                sw.WriteLine($"\"{t.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm}\",{t.Type},{t.ProductName},{t.ProductSKU},{t.QuantityChange},${t.TotalValue:N2},{t.UserName},{t.PreviousStock},{t.NewStock},{t.Notes}");
                            }
                        }

                        MessageBox.Show($"Exported {allTransactions.Count} records successfully!", "Export Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting: {ex.Message}", "Export Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "ucLedger";
            this.Size = new System.Drawing.Size(1000, 600);
            this.ResumeLayout(false);
        }
    }
}