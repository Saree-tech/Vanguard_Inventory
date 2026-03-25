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
    public partial class ProfileForm : Form
    {
        private IMongoCollection<User> usersCollection;
        private User currentUser;

        private TextBox txtFullName;
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnSave;
        private Button btnCancel;
        private Label lblError;

        public ProfileForm(User user)
        {
            InitializeComponent();
            CreateControls();
            usersCollection = DatabaseService.GetCollection<User>("Users");
            currentUser = user;
            LoadUserData();
            this.Text = "My Profile";
        }

        private void CreateControls()
        {
            this.BackColor = Color.FromArgb(45, 45, 60);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(450, 450);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.FromArgb(45, 45, 60)
            };

            // Avatar
            var lblAvatar = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 48F),
                ForeColor = Color.FromArgb(0, 122, 255),
                Location = new Point(160, 15),
                AutoSize = true
            };

            // Username (readonly)
            var lblUsername = new Label
            {
                Text = "Username",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 90),
                AutoSize = true
            };

            var txtUsername = new TextBox
            {
                Location = new Point(0, 120),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true
            };
            txtUsername.Text = currentUser.Username;

            // Full Name
            var lblFullName = new Label
            {
                Text = "Full Name",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 170),
                AutoSize = true
            };

            txtFullName = new TextBox
            {
                Location = new Point(0, 200),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Current Password
            var lblCurrentPassword = new Label
            {
                Text = "Current Password",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 250),
                AutoSize = true
            };

            txtCurrentPassword = new TextBox
            {
                Location = new Point(0, 280),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // New Password
            var lblNewPassword = new Label
            {
                Text = "New Password (leave blank to keep current)",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 330),
                AutoSize = true
            };

            txtNewPassword = new TextBox
            {
                Location = new Point(0, 360),
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
                Location = new Point(0, 405),
                AutoSize = true
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(0, 435),
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
                Location = new Point(0, 480),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Buttons
            btnSave = new Button
            {
                Text = "Update Profile",
                Location = new Point(200, 520),
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
                Location = new Point(310, 520),
                Size = new Size(90, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            pnlContent.Controls.Add(lblAvatar);
            pnlContent.Controls.Add(lblUsername);
            pnlContent.Controls.Add(txtUsername);
            pnlContent.Controls.Add(lblFullName);
            pnlContent.Controls.Add(txtFullName);
            pnlContent.Controls.Add(lblCurrentPassword);
            pnlContent.Controls.Add(txtCurrentPassword);
            pnlContent.Controls.Add(lblNewPassword);
            pnlContent.Controls.Add(txtNewPassword);
            pnlContent.Controls.Add(lblConfirm);
            pnlContent.Controls.Add(txtConfirmPassword);
            pnlContent.Controls.Add(lblError);
            pnlContent.Controls.Add(btnSave);
            pnlContent.Controls.Add(btnCancel);

            this.Controls.Add(pnlContent);
        }

        private void LoadUserData()
        {
            txtFullName.Text = currentUser.FullName;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowError("Full Name is required");
                return;
            }

            // If changing password
            if (!string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
                {
                    ShowError("Current password is required to change password");
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

                // Verify current password
                string currentPasswordHash = HashPassword(txtCurrentPassword.Text);
                if (currentPasswordHash != currentUser.PasswordHash)
                {
                    ShowError("Current password is incorrect");
                    return;
                }
            }

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Saving...";

                var filter = Builders<User>.Filter.Eq(u => u.Id, currentUser.Id);
                var updateBuilder = Builders<User>.Update
                    .Set(u => u.FullName, txtFullName.Text.Trim());

                if (!string.IsNullOrWhiteSpace(txtNewPassword.Text))
                {
                    updateBuilder = updateBuilder
                        .Set(u => u.PasswordHash, HashPassword(txtNewPassword.Text))
                        .Set(u => u.MustChangePassword, false);
                }

                await usersCollection.UpdateOneAsync(filter, updateBuilder);

                // Update session
                SessionManager.FullName = txtFullName.Text.Trim();

                MessageBox.Show("Profile updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error updating profile: {ex.Message}");
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Update Profile";
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

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    this.ResumeLayout(false);
        //}
    }
}