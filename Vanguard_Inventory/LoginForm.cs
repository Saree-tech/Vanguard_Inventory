using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vanguard_Inventory.Models;
using Vanguard_Inventory.Services;

namespace Vanguard_Inventory
{
    public partial class LoginForm : Form
    {
        private IMongoCollection<User> usersCollection;
        private Timer shakeTimer;
        private int shakeCount = 0;
        private Point originalLocation;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public LoginForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            DatabaseService.Initialize();
            usersCollection = DatabaseService.GetCollection<User>("Users");

            StyleLoginButton();
        }

        private void StyleLoginButton()
        {
            btnLogin.Paint += (sender, e) =>
            {
                Rectangle rect = btnLogin.ClientRectangle;
                using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                    Color.FromArgb(0, 122, 255), Color.FromArgb(100, 0, 200), 90f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                TextRenderer.DrawText(e.Graphics, btnLogin.Text, btnLogin.Font, rect,
                    Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(50, 150, 255);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.Transparent;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                ShowError("Please enter username");
                StartShakeAnimation();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Please enter password");
                StartShakeAnimation();
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "LOGGING IN...";
            lblError.Visible = false;

            try
            {
                var user = await usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();

                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    if (!user.IsActive)
                    {
                        ShowError("Account is disabled. Contact administrator.");
                        return;
                    }

                    SessionManager.Username = user.Username;
                    SessionManager.Role = user.Role;
                    SessionManager.FullName = user.FullName;
                    SessionManager.UserId = user.Id;

                    // Check if user must change password
                    if (user.MustChangePassword)
                    {
                        // Show change password dialog
                        ShowChangePasswordDialog(user);
                        return;
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Invalid username or password");
                    StartShakeAnimation();
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Database error: {ex.Message}");
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "LOGIN";
            }
        }

        private void ShowChangePasswordDialog(User user)
        {
            var changePasswordForm = new Form
            {
                Text = "Change Password",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(45, 45, 60)
            };

            var lblNewPassword = new Label
            {
                Text = "New Password:",
                Location = new Point(30, 50),
                Size = new Size(120, 25),
                ForeColor = Color.White
            };

            var txtNewPassword = new TextBox
            {
                Location = new Point(30, 80),
                Size = new Size(320, 30),
                UseSystemPasswordChar = true,
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblConfirmPassword = new Label
            {
                Text = "Confirm Password:",
                Location = new Point(30, 120),
                Size = new Size(120, 25),
                ForeColor = Color.White
            };

            var txtConfirmPassword = new TextBox
            {
                Location = new Point(30, 150),
                Size = new Size(320, 30),
                UseSystemPasswordChar = true,
                BackColor = Color.FromArgb(35, 35, 52),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnConfirm = new Button
            {
                Text = "Change Password",
                Location = new Point(30, 200),
                Size = new Size(320, 40),
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var lblError = new Label
            {
                Location = new Point(30, 250),
                Size = new Size(320, 25),
                ForeColor = Color.Tomato,
                Visible = false
            };

            btnConfirm.Click += async (s, ev) =>
            {
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                if (string.IsNullOrEmpty(newPassword))
                {
                    lblError.Text = "Please enter a password";
                    lblError.Visible = true;
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    lblError.Text = "Passwords do not match";
                    lblError.Visible = true;
                    return;
                }

                if (newPassword.Length < 4)
                {
                    lblError.Text = "Password must be at least 4 characters";
                    lblError.Visible = true;
                    return;
                }

                // Update password
                var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                var update = Builders<User>.Update
                    .Set(u => u.PasswordHash, HashPassword(newPassword))
                    .Set(u => u.MustChangePassword, false);

                await usersCollection.UpdateOneAsync(filter, update);

                MessageBox.Show("Password changed successfully! Please login again.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                changePasswordForm.Close();
            };

            changePasswordForm.Controls.Add(lblNewPassword);
            changePasswordForm.Controls.Add(txtNewPassword);
            changePasswordForm.Controls.Add(lblConfirmPassword);
            changePasswordForm.Controls.Add(txtConfirmPassword);
            changePasswordForm.Controls.Add(btnConfirm);
            changePasswordForm.Controls.Add(lblError);

            changePasswordForm.ShowDialog();
        }

        private async void lblForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string username = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter your username first.", "Forgot Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUsername.Focus();
                return;
            }

            var user = await usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();

            if (user == null)
            {
                MessageBox.Show("Username not found.", "Forgot Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mark that user requested password reset
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update.Set(u => u.PasswordResetRequested, true);
            await usersCollection.UpdateOneAsync(filter, update);

            // In a real app, you would send email/SMS notification to admin
            // For now, show a message
            MessageBox.Show($"Password reset request sent to administrator.\n\nUser: {username}\n\nAn admin will review and reset your password.",
                "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Clear password field
            txtPassword.Clear();
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            string hashedInput = HashPassword(password);
            return hashedInput == storedHash;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        private void StartShakeAnimation()
        {
            originalLocation = this.Location;
            shakeCount = 0;
            shakeTimer = new Timer();
            shakeTimer.Interval = 50;
            shakeTimer.Tick += ShakeTimer_Tick;
            shakeTimer.Start();
        }

        private void ShakeTimer_Tick(object sender, EventArgs e)
        {
            if (shakeCount >= 10)
            {
                this.Location = originalLocation;
                shakeTimer.Stop();
                shakeTimer.Dispose();
            }
            else
            {
                int offset = (shakeCount % 2 == 0) ? -10 : 10;
                this.Location = new Point(originalLocation.X + offset, originalLocation.Y);
                shakeCount++;
            }
        }

        private void pnlTopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Exit();
        private void btnMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void pnlLoginBox_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}