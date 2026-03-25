using System;
using System.Drawing;
using System.Windows.Forms;
using MongoDB.Driver;
using Vanguard_Inventory.Models;
using Vanguard_Inventory.Services;
using System.Security.Cryptography;
using System.Text;

namespace Vanguard_Inventory.UserControls
{
    public partial class ChangePasswordForm : Form
    {
        private IMongoCollection<User> usersCollection;
        private User currentUser;

        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnSave;
        private Button btnCancel;
        private Label lblError;

        public ChangePasswordForm(User user)
        {
            InitializeComponent();
            CreateControls();
            usersCollection = DatabaseService.GetCollection<User>("Users");
            currentUser = user;
            this.Text = "Change Password";
        }

        private void CreateControls()
        {
            this.BackColor = Color.FromArgb(45, 45, 60);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(450, 350);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.FromArgb(45, 45, 60)
            };

            // Title
            var lblTitle = new Label
            {
                Text = "🔑 CHANGE PASSWORD",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 0),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Current Password
            var lblCurrent = new Label
            {
                Text = "Current Password",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 60),
                AutoSize = true
            };

            txtCurrentPassword = new TextBox
            {
                Location = new Point(0, 90),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // New Password
            var lblNew = new Label
            {
                Text = "New Password",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 140),
                AutoSize = true
            };

            txtNewPassword = new TextBox
            {
                Location = new Point(0, 170),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Confirm Password
            var lblConfirm = new Label
            {
                Text = "Confirm New Password",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 220),
                AutoSize = true
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(0, 250),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Error Label
            lblError = new Label
            {
                ForeColor = Color.Tomato,
                Location = new Point(0, 295),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Buttons
            btnSave = new Button
            {
                Text = "Change Password",
                Location = new Point(200, 340),
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(310, 340),
                Size = new Size(90, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            pnlContent.Controls.Add(lblTitle);
            pnlContent.Controls.Add(lblCurrent);
            pnlContent.Controls.Add(txtCurrentPassword);
            pnlContent.Controls.Add(lblNew);
            pnlContent.Controls.Add(txtNewPassword);
            pnlContent.Controls.Add(lblConfirm);
            pnlContent.Controls.Add(txtConfirmPassword);
            pnlContent.Controls.Add(lblError);
            pnlContent.Controls.Add(btnSave);
            pnlContent.Controls.Add(btnCancel);

            this.Controls.Add(pnlContent);
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                ShowError("Current password is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                ShowError("New password is required");
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                ShowError("New passwords do not match");
                return;
            }

            if (txtNewPassword.Text.Length < 4)
            {
                ShowError("New password must be at least 4 characters");
                return;
            }

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Changing...";

                // Verify current password
                string currentPasswordHash = HashPassword(txtCurrentPassword.Text);
                if (currentPasswordHash != currentUser.PasswordHash)
                {
                    ShowError("Current password is incorrect");
                    return;
                }

                var filter = Builders<User>.Filter.Eq(u => u.Id, currentUser.Id);
                var update = Builders<User>.Update
                    .Set(u => u.PasswordHash, HashPassword(txtNewPassword.Text))
                    .Set(u => u.MustChangePassword, false);

                await usersCollection.UpdateOneAsync(filter, update);

                MessageBox.Show("Password changed successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Change Password";
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}