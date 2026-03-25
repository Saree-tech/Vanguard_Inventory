using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver;
using Vanguard_Inventory.Models;
using Vanguard_Inventory.Services;
using System.Collections.Generic;

namespace Vanguard_Inventory.UserControls
{
    public partial class ucCatalog : UserControl
    {
        private IMongoCollection<Product> productsCollection;
        private List<Product> allProducts = new List<Product>();

        // Controls
        private FlowLayoutPanel flpProducts;
        private TextBox txtSearch;
        private Button btnAddProduct;

        public ucCatalog()
        {
            InitializeComponent();
            CreateControls();
            productsCollection = DatabaseService.GetCollection<Product>("Products");

            btnAddProduct.Visible = SessionManager.IsAdmin;

            LoadProducts();
        }

        private void CreateControls()
        {
            // Search Panel
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(35, 35, 52),
                Padding = new Padding(15)
            };

            // Search TextBox
            txtSearch = new TextBox
            {
                Location = new Point(15, 15),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(45, 45, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.Text = "Search by SKU, Name, or Tag...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Add Product Button
            btnAddProduct = new Button
            {
                Text = "+ Add New Product",
                Location = new Point(430, 12),
                Size = new Size(140, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnAddProduct.FlatAppearance.BorderSize = 0;
            btnAddProduct.Click += BtnAddProduct_Click;

            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnAddProduct);

            // Products FlowLayoutPanel
            flpProducts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 44),
                Padding = new Padding(15),
                WrapContents = true
            };

            this.Controls.Add(flpProducts);
            this.Controls.Add(pnlSearch);
            this.BackColor = Color.FromArgb(30, 30, 44);
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Search by SKU, Name, or Tag...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.White;
            }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search by SKU, Name, or Tag...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private async void LoadProducts()
        {
            try
            {
                flpProducts.Controls.Clear();

                var loadingLabel = new Label
                {
                    Text = "Loading products...",
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 12F),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                flpProducts.Controls.Add(loadingLabel);
                Application.DoEvents();

                allProducts = await productsCollection.Find(_ => true).ToListAsync();
                flpProducts.Controls.Clear();

                if (allProducts.Any())
                {
                    DisplayProducts(allProducts);
                }
                else
                {
                    var emptyLabel = new Label
                    {
                        Text = "No products found.\nClick 'Add New Product' to get started.",
                        ForeColor = Color.Gray,
                        Font = new Font("Segoe UI", 12F),
                        AutoSize = true,
                        Location = new Point(20, 20)
                    };
                    flpProducts.Controls.Add(emptyLabel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayProducts(List<Product> products)
        {
            flpProducts.Controls.Clear();

            foreach (var product in products.OrderBy(p => p.Name))
            {
                var card = CreateProductCard(product);
                flpProducts.Controls.Add(card);
            }
        }

        private Panel CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Width = 340,
                Height = 240,
                BackColor = Color.FromArgb(42, 42, 60),
                Margin = new Padding(10),
                Padding = new Padding(12)
            };

            // Add hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(52, 52, 70);
            card.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(42, 42, 60);

            // Product Name
            var lblName = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 12),
                AutoSize = true
            };

            // SKU
            var lblSKU = new Label
            {
                Text = $"SKU: {product.SKU}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(150, 150, 170),
                Location = new Point(12, 42),
                AutoSize = true
            };

            // Price
            var lblPrice = new Label
            {
                Text = $"${product.Price:N2}",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 255),
                Location = new Point(12, 72),
                AutoSize = true
            };

            // Stock Progress Bar
            int stockPercent = (product.Stock.Current * 100) / product.Stock.MaxCapacity;
            var pbStock = new ProgressBar
            {
                Location = new Point(12, 105),
                Size = new Size(200, 10),
                Minimum = 0,
                Maximum = 100,
                Value = Math.Min(stockPercent, 100),
                Style = ProgressBarStyle.Continuous,
                ForeColor = stockPercent < 30 ? Color.FromArgb(255, 69, 58) : Color.FromArgb(0, 122, 255)
            };

            // Stock Label
            var lblStock = new Label
            {
                Text = $"Stock: {product.Stock.Current} / {product.Stock.MaxCapacity}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(150, 150, 170),
                Location = new Point(12, 120),
                AutoSize = true
            };

            // Status Badge
            string statusText;
            Color statusColor;

            if (product.Stock.Current <= 0)
            {
                statusText = "OUT OF STOCK";
                statusColor = Color.FromArgb(200, 50, 50);
            }
            else if (product.Stock.Current <= product.Stock.MinThreshold)
            {
                statusText = "LOW STOCK";
                statusColor = Color.FromArgb(255, 150, 50);
            }
            else
            {
                statusText = "IN STOCK";
                statusColor = Color.FromArgb(50, 200, 50);
            }

            var lblStatus = new Label
            {
                Text = statusText,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = statusColor,
                Location = new Point(240, 12),
                Size = new Size(85, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(lblName);
            card.Controls.Add(lblSKU);
            card.Controls.Add(lblPrice);
            card.Controls.Add(pbStock);
            card.Controls.Add(lblStock);
            card.Controls.Add(lblStatus);

            // Action Buttons based on Role
            int buttonY = 180;
            int buttonWidth = 70;
            int spacing = 8;

            if (SessionManager.IsAdmin)
            {
                // Admin: Sell, Restock, Edit, Delete - All buttons in a row
                var btnSell = new Button
                {
                    Text = "➖ Sell",
                    Location = new Point(12, buttonY),
                    Size = new Size(buttonWidth, 32),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(255, 100, 50),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnSell.FlatAppearance.BorderSize = 0;
                btnSell.Click += (s, e) => SellProduct(product);

                var btnRestock = new Button
                {
                    Text = "➕ Restock",
                    Location = new Point(12 + buttonWidth + spacing, buttonY),
                    Size = new Size(buttonWidth, 32),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(50, 150, 50),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnRestock.FlatAppearance.BorderSize = 0;
                btnRestock.Click += (s, e) => RestockProduct(product);

                var btnEdit = new Button
                {
                    Text = "✏️ Edit",
                    Location = new Point(12 + (buttonWidth + spacing) * 2, buttonY),
                    Size = new Size(buttonWidth, 32),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(0, 122, 255),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnEdit.FlatAppearance.BorderSize = 0;
                btnEdit.Click += (s, e) => EditProduct(product);

                var btnDelete = new Button
                {
                    Text = "🗑️ Delete",
                    Location = new Point(12 + (buttonWidth + spacing) * 3, buttonY),
                    Size = new Size(buttonWidth, 32),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(200, 50, 50),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnDelete.FlatAppearance.BorderSize = 0;
                btnDelete.Click += (s, e) => DeleteProduct(product);

                card.Controls.Add(btnSell);
                card.Controls.Add(btnRestock);
                card.Controls.Add(btnEdit);
                card.Controls.Add(btnDelete);
            }
            else if (SessionManager.IsSeller)
            {
                // Seller: Sell and Restock only
                var btnSell = new Button
                {
                    Text = "➖ Sell",
                    Location = new Point(85, buttonY),
                    Size = new Size(75, 32),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(255, 100, 50),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnSell.FlatAppearance.BorderSize = 0;
                btnSell.Click += (s, e) => SellProduct(product);

                var btnRestock = new Button
                {
                    Text = "➕ Restock",
                    Location = new Point(170, buttonY),
                    Size = new Size(80, 32),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(50, 150, 50),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnRestock.FlatAppearance.BorderSize = 0;
                btnRestock.Click += (s, e) => RestockProduct(product);

                card.Controls.Add(btnSell);
                card.Controls.Add(btnRestock);
            }
            else if (SessionManager.IsViewer)
            {
                // Viewer: View only
                var btnView = new Button
                {
                    Text = "👁️ View Details",
                    Location = new Point(110, buttonY),
                    Size = new Size(120, 32),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(100, 100, 150),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                btnView.FlatAppearance.BorderSize = 0;
                btnView.Click += (s, e) => ViewProductDetails(product);
                card.Controls.Add(btnView);
            }

            return card;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower().Trim();

            if (searchText == "search by sku, name, or tag..." || string.IsNullOrEmpty(searchText))
            {
                DisplayProducts(allProducts);
                return;
            }

            var filtered = allProducts.Where(p =>
                p.SKU.ToLower().Contains(searchText) ||
                p.Name.ToLower().Contains(searchText) ||
                (p.Tags != null && p.Tags.Any(t => t.ToLower().Contains(searchText)))
            ).ToList();

            DisplayProducts(filtered);
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddProductForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    (this.FindForm() as Form1)?.RefreshDashboard();
                }
            }
        }

        private void EditProduct(Product product)
        {
            using (var editForm = new AddProductForm(product))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    (this.FindForm() as Form1)?.RefreshDashboard();
                }
            }
        }

        private async void DeleteProduct(Product product)
        {
            var result = MessageBox.Show($"Delete {product.Name}?\nThis action cannot be undone.\n\nThis will also delete all transaction records for this product.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await productsCollection.DeleteOneAsync(p => p.Id == product.Id);
                LoadProducts();
                (this.FindForm() as Form1)?.RefreshDashboard();

                MessageBox.Show($"{product.Name} has been deleted.", "Deleted",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void SellProduct(Product product)
        {
            string quantityStr = ShowInputDialog("Enter quantity to sell:", "Sell Product", "1");

            if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
            {
                if (quantity > product.Stock.Current)
                {
                    MessageBox.Show($"Not enough stock! Available: {product.Stock.Current}",
                        "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int previousStock = product.Stock.Current;
                int newStock = previousStock - quantity;

                // Update stock
                var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
                var update = Builders<Product>.Update.Set(p => p.Stock.Current, newStock);
                await productsCollection.UpdateOneAsync(filter, update);

                // Record transaction in Ledger
                await LedgerService.RecordTransaction(
                    product.Id,
                    product.SKU,
                    product.Name,
                    "Sale",
                    -quantity,
                    previousStock,
                    newStock,
                    product.Price,
                    $"Sold {quantity} units by {SessionManager.FullName}"
                );

                LoadProducts();
                (this.FindForm() as Form1)?.RefreshDashboard();

                MessageBox.Show($"✅ Sold {quantity} units of {product.Name}!\n\nTotal: ${product.Price * quantity:N2}\nStock remaining: {newStock}",
                    "Sale Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void RestockProduct(Product product)
        {
            string quantityStr = ShowInputDialog("Enter quantity to restock:", "Restock Product", "10");

            if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
            {
                int previousStock = product.Stock.Current;
                int newStock = previousStock + quantity;
                int maxCap = product.Stock.MaxCapacity;

                if (newStock > maxCap)
                {
                    var confirm = MessageBox.Show($"This will exceed max capacity ({maxCap}). Current: {previousStock}, Adding: {quantity}\n\nContinue?",
                        "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirm != DialogResult.Yes) return;
                }

                // Update stock
                var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
                var update = Builders<Product>.Update.Set(p => p.Stock.Current, newStock);
                await productsCollection.UpdateOneAsync(filter, update);

                // Record transaction in Ledger
                await LedgerService.RecordTransaction(
                    product.Id,
                    product.SKU,
                    product.Name,
                    "Restock",
                    +quantity,
                    previousStock,
                    newStock,
                    product.Price,
                    $"Restocked {quantity} units by {SessionManager.FullName}"
                );

                LoadProducts();
                (this.FindForm() as Form1)?.RefreshDashboard();

                MessageBox.Show($"✅ Restocked {quantity} units of {product.Name}!\n\nStock: {previousStock} → {newStock}",
                    "Restock Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ViewProductDetails(Product product)
        {
            MessageBox.Show($"Product Details:\n\n" +
                $"Name: {product.Name}\n" +
                $"SKU: {product.SKU}\n" +
                $"Price: ${product.Price:N2}\n" +
                $"Stock: {product.Stock.Current} / {product.Stock.MaxCapacity}\n" +
                $"Min Threshold: {product.Stock.MinThreshold}\n" +
                $"Category: {product.Category}\n" +
                $"Tags: {(product.Tags != null ? string.Join(", ", product.Tags) : "None")}",
                product.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string ShowInputDialog(string text, string caption, string defaultValue)
        {
            Form inputForm = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(45, 45, 60)
            };

            Label lblText = new Label()
            {
                Text = text,
                Left = 20,
                Top = 20,
                Width = 350,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            TextBox txtInput = new TextBox()
            {
                Left = 20,
                Top = 55,
                Width = 350,
                Text = defaultValue,
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };

            Button btnOk = new Button()
            {
                Text = "OK",
                Left = 250,
                Width = 100,
                Top = 95,
                Height = 30,
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 0;

            Button btnCancel = new Button()
            {
                Text = "Cancel",
                Left = 140,
                Width = 100,
                Top = 95,
                Height = 30,
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            inputForm.Controls.Add(lblText);
            inputForm.Controls.Add(txtInput);
            inputForm.Controls.Add(btnOk);
            inputForm.Controls.Add(btnCancel);

            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            return inputForm.ShowDialog() == DialogResult.OK ? txtInput.Text : "";
        }

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    this.Name = "ucCatalog";
        //    this.ResumeLayout(false);
        //}
    }
}