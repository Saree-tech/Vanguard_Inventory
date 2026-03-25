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
    public partial class UserForm : Form
    {
        private IMongoCollection<User> usersCollection;
        private User editingUser;

        private TextBox txtUsername;
        private TextBox txtFullName;
        private ComboBox cmbRole;
        private CheckBox chkActive;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnSave;
        private Button btnCancel;
        private Label lblError;
        private CheckBox chkMustChangePassword;

        public UserForm(User user = null)
        {
            InitializeComponent();
            CreateControls();
            usersCollection = DatabaseService.GetCollection<User>("Users");
            editingUser = user;
            this.Text = user == null ? "Add New User" : "Edit User";

            if (user != null)
            {
                LoadUserData();
            }
        }

        private void CreateControls()
        {
            this.BackColor = Color.FromArgb(45, 45, 60);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(450, 600);
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
                Text = editingUser == null ? "➕ ADD NEW USER" : "✏️ EDIT USER",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 0),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Username
            var lblUsername = new Label
            {
                Text = "Username",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 60),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(0, 90),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Full Name
            var lblFullName = new Label
            {
                Text = "Full Name",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 135),
                AutoSize = true
            };

            txtFullName = new TextBox
            {
                Location = new Point(0, 165),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Role
            var lblRole = new Label
            {
                Text = "Role",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 210),
                AutoSize = true
            };

            cmbRole = new ComboBox
            {
                Location = new Point(0, 240),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new string[] { "Seller", "Viewer" });
            cmbRole.SelectedIndex = 0;

            // If editing Admin, show Admin role
            if (editingUser != null && editingUser.Role == "Admin")
            {
                cmbRole.Items.Insert(0, "Admin");
                cmbRole.SelectedIndex = 0;
            }
            else
            {
                // Admin role only selectable when creating new user? Actually only Admins can create Admins
                if (SessionManager.IsAdmin)
                {
                    cmbRole.Items.Insert(0, "Admin");
                }
            }

            // Active Status
            chkActive = new CheckBox
            {
                Text = "Account Active",
                Location = new Point(0, 285),
                Size = new Size(150, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Checked = true
            };

            // Must Change Password
            chkMustChangePassword = new CheckBox
            {
                Text = "Must change password on next login",
                Location = new Point(0, 320),
                Size = new Size(250, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Checked = editingUser == null
            };

            // Password (only for new users or password reset)
            var lblPassword = new Label
            {
                Text = "Password (leave blank to keep current)",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 360),
                AutoSize = true,
                Visible = editingUser != null
            };

            txtPassword = new TextBox
            {
                Location = new Point(0, 390),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                Visible = editingUser != null
            };

            // Confirm Password
            var lblConfirm = new Label
            {
                Text = "Confirm Password",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 430),
                AutoSize = true,
                Visible = editingUser != null
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(0, 460),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                Visible = editingUser != null
            };

            // For new user - show password fields
            if (editingUser == null)
            {
                lblPassword.Visible = true;
                lblPassword.Text = "Password";
                txtPassword.Visible = true;
                lblConfirm.Visible = true;
                txtConfirmPassword.Visible = true;

                // Adjust positions for new user
                lblPassword.Location = new Point(0, 360);
                txtPassword.Location = new Point(0, 390);
                lblConfirm.Location = new Point(0, 430);
                txtConfirmPassword.Location = new Point(0, 460);
            }

            // Error Label
            lblError = new Label
            {
                ForeColor = Color.Tomato,
                Location = new Point(0, 510),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Buttons
            btnSave = new Button
            {
                Text = "Save User",
                Location = new Point(200, 550),
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
                Location = new Point(310, 550),
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
            pnlContent.Controls.Add(lblUsername);
            pnlContent.Controls.Add(txtUsername);
            pnlContent.Controls.Add(lblFullName);
            pnlContent.Controls.Add(txtFullName);
            pnlContent.Controls.Add(lblRole);
            pnlContent.Controls.Add(cmbRole);
            pnlContent.Controls.Add(chkActive);
            pnlContent.Controls.Add(chkMustChangePassword);
            pnlContent.Controls.Add(lblPassword);
            pnlContent.Controls.Add(txtPassword);
            pnlContent.Controls.Add(lblConfirm);
            pnlContent.Controls.Add(txtConfirmPassword);
            pnlContent.Controls.Add(lblError);
            pnlContent.Controls.Add(btnSave);
            pnlContent.Controls.Add(btnCancel);

            this.Controls.Add(pnlContent);
        }

        private void LoadUserData()
        {
            if (editingUser != null)
            {
                txtUsername.Text = editingUser.Username;
                txtUsername.Enabled = false; // Cannot change username
                txtFullName.Text = editingUser.FullName;
                chkActive.Checked = editingUser.IsActive;
                chkMustChangePassword.Checked = editingUser.MustChangePassword;

                // Set role
                int roleIndex = cmbRole.Items.IndexOf(editingUser.Role);
                if (roleIndex >= 0) cmbRole.SelectedIndex = roleIndex;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Username is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowError("Full Name is required");
                return;
            }

            string selectedRole = cmbRole.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedRole))
            {
                ShowError("Role is required");
                return;
            }

            // Password validation for new user or when password is provided
            if (editingUser == null)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowError("Password is required for new user");
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    ShowError("Passwords do not match");
                    return;
                }

                if (txtPassword.Text.Length < 4)
                {
                    ShowError("Password must be at least 4 characters");
                    return;
                }
            }
            else if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    ShowError("Passwords do not match");
                    return;
                }

                if (txtPassword.Text.Length < 4)
                {
                    ShowError("Password must be at least 4 characters");
                    return;
                }
            }

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Saving...";

                if (editingUser == null)
                {
                    // Check if username exists
                    var existing = await usersCollection.Find(u => u.Username == txtUsername.Text.Trim()).FirstOrDefaultAsync();
                    if (existing != null)
                    {
                        ShowError("Username already exists");
                        return;
                    }

                    var newUser = new User
                    {
                        Username = txtUsername.Text.Trim(),
                        FullName = txtFullName.Text.Trim(),
                        Role = selectedRole,
                        IsActive = chkActive.Checked,
                        MustChangePassword = chkMustChangePassword.Checked,
                        CreatedAt = DateTime.UtcNow,
                        PasswordHash = HashPassword(txtPassword.Text)
                    };

                    await usersCollection.InsertOneAsync(newUser);
                    MessageBox.Show("User added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var filter = Builders<User>.Filter.Eq(u => u.Id, editingUser.Id);
                    var updateBuilder = Builders<User>.Update
                        .Set(u => u.FullName, txtFullName.Text.Trim())
                        .Set(u => u.Role, selectedRole)
                        .Set(u => u.IsActive, chkActive.Checked)
                        .Set(u => u.MustChangePassword, chkMustChangePassword.Checked);

                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        updateBuilder = updateBuilder.Set(u => u.PasswordHash, HashPassword(txtPassword.Text));
                    }

                    await usersCollection.UpdateOneAsync(filter, updateBuilder);
                    MessageBox.Show("User updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Error saving user: {ex.Message}");
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Save User";
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
    }
}