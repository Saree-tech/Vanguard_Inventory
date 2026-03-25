using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using MongoDB.Driver;
using Vanguard_Inventory.Models;
using Vanguard_Inventory.Services;

namespace Vanguard_Inventory
{
    public partial class AddProductForm : Form
    {
        private IMongoCollection<Product> productsCollection;
        private Product editingProduct = null;

        public AddProductForm()
        {
            InitializeComponent();
            productsCollection = DatabaseService.GetCollection<Product>("Products");
            this.Text = "Add New Product";
        }

        public AddProductForm(Product productToEdit)
        {
            InitializeComponent();
            productsCollection = DatabaseService.GetCollection<Product>("Products");
            editingProduct = productToEdit;
            this.Text = "Edit Product";
            LoadProductData();
        }

        private void LoadProductData()
        {
            if (editingProduct != null)
            {
                txtSKU.Text = editingProduct.SKU;
                txtName.Text = editingProduct.Name;
                txtDescription.Text = editingProduct.Description;
                txtPrice.Text = editingProduct.Price.ToString();
                txtCategory.Text = editingProduct.Category;
                txtCurrentStock.Text = editingProduct.Stock.Current.ToString();
                txtMinThreshold.Text = editingProduct.Stock.MinThreshold.ToString();
                txtMaxCapacity.Text = editingProduct.Stock.MaxCapacity.ToString();
                txtTags.Text = editingProduct.Tags != null ? string.Join(", ", editingProduct.Tags) : "";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSKU.Text))
            {
                ShowError("SKU is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowError("Product Name is required");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                ShowError("Invalid Price");
                return;
            }

            if (!int.TryParse(txtCurrentStock.Text, out int currentStock))
            {
                ShowError("Invalid Current Stock");
                return;
            }

            if (!int.TryParse(txtMinThreshold.Text, out int minThreshold))
            {
                ShowError("Invalid Min Threshold");
                return;
            }

            if (!int.TryParse(txtMaxCapacity.Text, out int maxCapacity))
            {
                ShowError("Invalid Max Capacity");
                return;
            }

            var product = new Product
            {
                SKU = txtSKU.Text.Trim().ToUpper(),
                Name = txtName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Price = price,
                Category = txtCategory.Text.Trim(),
                Stock = new StockInfo
                {
                    Current = currentStock,
                    MinThreshold = minThreshold,
                    MaxCapacity = maxCapacity
                },
                Tags = txtTags.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim()).ToList()
            };

            if (editingProduct != null)
            {
                product.Id = editingProduct.Id;
            }

            SaveProduct(product);
        }

        private async void SaveProduct(Product product)
        {
            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Saving...";

                if (editingProduct != null)
                {
                    var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
                    var update = Builders<Product>.Update
                        .Set(p => p.SKU, product.SKU)
                        .Set(p => p.Name, product.Name)
                        .Set(p => p.Description, product.Description)
                        .Set(p => p.Price, product.Price)
                        .Set(p => p.Category, product.Category)
                        .Set(p => p.Stock, product.Stock)
                        .Set(p => p.Tags, product.Tags);

                    await productsCollection.UpdateOneAsync(filter, update);
                    MessageBox.Show("Product updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var existing = await productsCollection.Find(p => p.SKU == product.SKU).FirstOrDefaultAsync();
                    if (existing != null)
                    {
                        MessageBox.Show("SKU already exists! Please use a unique SKU.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    await productsCollection.InsertOneAsync(product);
                    MessageBox.Show("Product added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Save Product";
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
        private void btnMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        [System.Runtime.InteropServices.DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void pnlTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }
    }
}