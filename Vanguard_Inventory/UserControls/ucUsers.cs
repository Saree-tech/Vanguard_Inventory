using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver;
using Vanguard_Inventory.Models;
using Vanguard_Inventory.Services;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Drawing.Drawing2D;

namespace Vanguard_Inventory.UserControls
{
    public partial class ucUsers : UserControl
    {
        private IMongoCollection<User> usersCollection;
        private List<User> allUsers = new List<User>();

        // Controls
        private FlowLayoutPanel flpUserCards;
        private TextBox txtSearch;
        private Button btnAddUser;
        private Button btnRefresh;
        private Label lblTotalRecords;
        private Panel pnlStats;
        private TableLayoutPanel tlpStats;

        public ucUsers()
        {
            InitializeComponent();
            usersCollection = DatabaseService.GetCollection<User>("Users");
            CreateControls();
            LoadUsers();

            // Only Admins can access this screen
            if (!SessionManager.IsAdmin)
            {
                MessageBox.Show("Access Denied. Admin privileges required.", "Access Denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Visible = false;
            }
        }

        private void CreateControls()
        {
            this.BackColor = Color.FromArgb(30, 30, 44);
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);

            // Stats Panel - Fixed height and spacing
            pnlStats = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(30, 30, 44),
                Padding = new Padding(20, 12, 20, 12)
            };

            tlpStats = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStats.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            CreateStatCards();
            pnlStats.Controls.Add(tlpStats);

            // Search Panel - Fixed layout
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.FromArgb(35, 35, 52),
                Padding = new Padding(20, 12, 20, 12)
            };

            var tlpSearch = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };

            tlpSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tlpSearch.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpSearch.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Search box container
            var pnlSearchBox = new Panel
            {
                Height = 45,
                BackColor = Color.FromArgb(45, 45, 60),
                Dock = DockStyle.Fill
            };

            var lblSearchIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 14F),
                ForeColor = Color.Gray,
                Location = new Point(12, 12),
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                Location = new Point(45, 8),
                Width = pnlSearchBox.Width - 60,
                Height = 30,
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(45, 45, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Text = "Search users...",
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search users...") { txtSearch.Text = ""; txtSearch.ForeColor = Color.White; } };
            txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search users..."; txtSearch.ForeColor = Color.Gray; } };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            pnlSearchBox.Controls.Add(lblSearchIcon);
            pnlSearchBox.Controls.Add(txtSearch);

            // Add User Button
            btnAddUser = new Button
            {
                Text = "➕ Add User",
                Size = new Size(125, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None
            };
            btnAddUser.FlatAppearance.BorderSize = 0;
            btnAddUser.Click += BtnAddUser_Click;
            AddHoverEffect(btnAddUser, Color.FromArgb(0, 122, 255), Color.FromArgb(50, 150, 255));

            // Refresh Button
            btnRefresh = new Button
            {
                Text = "⟳",
                Size = new Size(45, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadUsers();

            // Add tooltip to refresh button
            var refreshTooltip = new ToolTip();
            refreshTooltip.SetToolTip(btnRefresh, "Refresh User List");

            tlpSearch.Controls.Add(pnlSearchBox, 0, 0);
            tlpSearch.Controls.Add(btnAddUser, 1, 0);
            tlpSearch.Controls.Add(btnRefresh, 2, 0);

            // Center align the buttons column
            tlpSearch.SetCellPosition(btnAddUser, new TableLayoutPanelCellPosition(1, 0));
            tlpSearch.SetCellPosition(btnRefresh, new TableLayoutPanelCellPosition(2, 0));

            pnlSearch.Controls.Add(tlpSearch);

            // User Cards FlowLayoutPanel
            flpUserCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 44),
                Padding = new Padding(20),
                WrapContents = true
            };

            // Bottom Status Panel
            var pnlStatus = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 42,
                BackColor = Color.FromArgb(35, 35, 52),
                Padding = new Padding(20, 10, 20, 10)
            };

            lblTotalRecords = new Label
            {
                Text = "👥 0 Users",
                ForeColor = Color.FromArgb(150, 150, 170),
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Left,
                AutoSize = true
            };

            pnlStatus.Controls.Add(lblTotalRecords);

            this.Controls.Add(flpUserCards);
            this.Controls.Add(pnlStatus);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlStats);
        }

        private void AddHoverEffect(Button btn, Color normalColor, Color hoverColor)
        {
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = hoverColor;
            };
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;
        }

        private void CreateStatCards()
        {
            var cards = new (string icon, string title, Color color)[]
            {
                ("👑", "ADMINS", Color.FromArgb(0, 122, 255)),
                ("🛒", "SELLERS", Color.FromArgb(50, 150, 50)),
                ("👁️", "VIEWERS", Color.FromArgb(255, 150, 50)),
                ("🟢", "ACTIVE", Color.FromArgb(100, 200, 100))
            };

            for (int i = 0; i < cards.Length; i++)
            {
                var card = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(42, 42, 60),
                    Margin = new Padding(6, 4, 6, 4)
                };

                // Add hover effect
                card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(52, 52, 70);
                card.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(42, 42, 60);

                var cardLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 2,
                    BackColor = Color.Transparent,
                    Padding = new Padding(10, 8, 8, 8)
                };

                cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
                cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));

                var lblIcon = new Label
                {
                    Text = cards[i].icon,
                    Font = new Font("Segoe UI", 28F),
                    ForeColor = cards[i].color,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(0)
                };

                var lblTitle = new Label
                {
                    Text = cards[i].title,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(150, 150, 170),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(0, 5, 0, 0)
                };

                var lblValue = new Label
                {
                    Text = "0",
                    Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                    ForeColor = cards[i].color,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(0)
                };

                cardLayout.Controls.Add(lblIcon, 0, 0);
                cardLayout.SetRowSpan(lblIcon, 2);
                cardLayout.Controls.Add(lblTitle, 1, 0);
                cardLayout.Controls.Add(lblValue, 1, 1);

                card.Controls.Add(cardLayout);
                tlpStats.Controls.Add(card, i, 0);

                if (cards[i].title == "ADMINS") lblAdminCount = lblValue;
                else if (cards[i].title == "SELLERS") lblSellerCount = lblValue;
                else if (cards[i].title == "VIEWERS") lblViewerCount = lblValue;
                else if (cards[i].title == "ACTIVE") lblActiveCount = lblValue;
            }
        }

        private Label lblAdminCount;
        private Label lblSellerCount;
        private Label lblViewerCount;
        private Label lblActiveCount;

        private async void LoadUsers()
        {
            try
            {
                flpUserCards.Controls.Clear();

                allUsers = await usersCollection.Find(_ => true).ToListAsync();
                allUsers = allUsers.OrderBy(u => u.Role).ThenBy(u => u.Username).ToList();

                foreach (var user in allUsers)
                {
                    var card = CreateUserCard(user);
                    flpUserCards.Controls.Add(card);
                }

                int adminCount = allUsers.Count(u => u.Role == "Admin");
                int sellerCount = allUsers.Count(u => u.Role == "Seller");
                int viewerCount = allUsers.Count(u => u.Role == "Viewer");
                int activeCount = allUsers.Count(u => u.IsActive);

                if (lblAdminCount != null) lblAdminCount.Text = adminCount.ToString();
                if (lblSellerCount != null) lblSellerCount.Text = sellerCount.ToString();
                if (lblViewerCount != null) lblViewerCount.Text = viewerCount.ToString();
                if (lblActiveCount != null) lblActiveCount.Text = activeCount.ToString();

                lblTotalRecords.Text = $"👥 {allUsers.Count} Users";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateUserCard(User user)
        {
            var card = new Panel
            {
                Width = 310,
                Height = 210,
                BackColor = Color.FromArgb(42, 42, 60),
                Margin = new Padding(12)
            };

            // Add hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(52, 52, 70);
            card.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(42, 42, 60);

            var cardLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                BackColor = Color.Transparent,
                Padding = new Padding(12)
            };

            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));

            string avatarIcon = user.Role == "Admin" ? "👑" : user.Role == "Seller" ? "🛒" : "👁️";
            Color avatarColor = user.Role == "Admin" ? Color.FromArgb(0, 122, 255) :
                                user.Role == "Seller" ? Color.FromArgb(50, 150, 50) :
                                Color.FromArgb(255, 150, 50);

            var lblAvatar = new Label
            {
                Text = avatarIcon,
                Font = new Font("Segoe UI", 42F),
                ForeColor = avatarColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };

            var statusDot = new Label
            {
                Text = user.IsActive ? "🟢" : "🔴",
                Font = new Font("Segoe UI", 12F),
                ForeColor = user.IsActive ? Color.LightGreen : Color.Tomato,
                Location = new Point(265, 8),
                AutoSize = true
            };

            var lblUsername = new Label
            {
                Text = user.Username,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblFullName = new Label
            {
                Text = user.FullName,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(180, 180, 200),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var roleBadge = new Label
            {
                Text = user.Role,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = avatarColor,
                BackColor = Color.FromArgb(35, 35, 52),
                Size = new Size(75, 28),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblCreated = new Label
            {
                Text = $"📅 {user.CreatedAt.ToLocalTime():yyyy-MM-dd}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(120, 120, 140),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var flowButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            bool isCurrentUser = (user.Username == SessionManager.Username);

            if (isCurrentUser)
            {
                var btnEditSelf = CreateIconButtonWithTooltip("✏️", "Edit Profile",
                    Color.FromArgb(0, 122, 255), Color.FromArgb(50, 150, 255));
                btnEditSelf.Click += (s, e) => EditUser(user);
                flowButtons.Controls.Add(btnEditSelf);

                var btnChangePwd = CreateIconButtonWithTooltip("🔑", "Change Password",
                    Color.FromArgb(150, 100, 50), Color.FromArgb(180, 130, 80));
                btnChangePwd.Click += (s, e) => ChangeOwnPassword(user);
                flowButtons.Controls.Add(btnChangePwd);
            }
            else
            {
                if (user.PasswordResetRequested)
                {
                    var btnResetRequest = CreateIconButtonWithTooltip("🔑", "Password Reset Requested",
                        Color.FromArgb(200, 120, 50), Color.FromArgb(220, 150, 80));
                    btnResetRequest.Click += (s, e) => ResetPassword(user);
                    flowButtons.Controls.Add(btnResetRequest);
                }

                var btnDelete = CreateIconButtonWithTooltip("🗑️", "Delete User",
                    Color.FromArgb(200, 50, 50), Color.FromArgb(220, 80, 80));
                btnDelete.Click += (s, e) => DeleteUser(user);
                flowButtons.Controls.Add(btnDelete);

                var btnToggle = CreateIconButtonWithTooltip(
                    user.IsActive ? "🔴" : "🟢",
                    user.IsActive ? "Deactivate User" : "Activate User",
                    user.IsActive ? Color.FromArgb(200, 50, 50) : Color.FromArgb(50, 150, 50),
                    user.IsActive ? Color.FromArgb(220, 80, 80) : Color.FromArgb(80, 180, 80));
                btnToggle.Click += (s, e) => ToggleUserStatus(user);
                flowButtons.Controls.Add(btnToggle);
            }

            var usernamePanel = new Panel { Dock = DockStyle.Fill };
            usernamePanel.Controls.Add(lblUsername);
            usernamePanel.Controls.Add(statusDot);
            lblUsername.Dock = DockStyle.Left;
            lblUsername.Width = 180;
            statusDot.Location = new Point(210, 8);

            cardLayout.Controls.Add(lblAvatar, 0, 0);
            cardLayout.SetRowSpan(lblAvatar, 4);
            cardLayout.Controls.Add(usernamePanel, 1, 0);
            cardLayout.Controls.Add(lblFullName, 1, 1);
            cardLayout.Controls.Add(roleBadge, 1, 2);
            cardLayout.Controls.Add(lblCreated, 1, 3);
            cardLayout.Controls.Add(flowButtons, 1, 4);

            card.Controls.Add(cardLayout);

            return card;
        }

        private Button CreateIconButtonWithTooltip(string icon, string tooltip, Color normalColor, Color hoverColor)
        {
            var btn = new Button
            {
                Text = icon,
                Size = new Size(44, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = normalColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13F),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 0)
            };
            btn.FlatAppearance.BorderSize = 0;

            var toolTip = new ToolTip();
            toolTip.SetToolTip(btn, tooltip);
            toolTip.BackColor = Color.FromArgb(35, 35, 52);
            toolTip.ForeColor = Color.White;
            toolTip.IsBalloon = false;

            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;

            return btn;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower().Trim();

            if (searchText == "search users..." || string.IsNullOrEmpty(searchText))
            {
                flpUserCards.Controls.Clear();
                foreach (var user in allUsers)
                {
                    flpUserCards.Controls.Add(CreateUserCard(user));
                }
                return;
            }

            var filtered = allUsers.Where(u =>
                u.Username.ToLower().Contains(searchText) ||
                u.FullName.ToLower().Contains(searchText)
            ).ToList();

            flpUserCards.Controls.Clear();
            foreach (var user in filtered)
            {
                flpUserCards.Controls.Add(CreateUserCard(user));
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            ShowUserDialog(null);
        }

        private void EditUser(User user)
        {
            if (user.Username == SessionManager.Username)
            {
                ShowOwnProfileDialog(user);
            }
        }

        private void ChangeOwnPassword(User user)
        {
            using (var changePasswordForm = new ChangePasswordForm(user))
            {
                if (changePasswordForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private async void DeleteUser(User user)
        {
            if (user.Username == SessionManager.Username)
            {
                MessageBox.Show("You cannot delete your own account.", "Cannot Delete",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Delete user '{user.Username}'?\nThis action cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await usersCollection.DeleteOneAsync(u => u.Id == user.Id);
                LoadUsers();
                MessageBox.Show($"User '{user.Username}' has been deleted.", "Deleted",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void ToggleUserStatus(User user)
        {
            if (user.Username == SessionManager.Username)
            {
                MessageBox.Show("You cannot deactivate your own account.", "Cannot Deactivate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newStatus = !user.IsActive;
            var action = newStatus ? "activate" : "deactivate";

            var result = MessageBox.Show($"Are you sure you want to {action} user '{user.Username}'?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
                var update = Builders<User>.Update.Set(u => u.IsActive, newStatus);
                await usersCollection.UpdateOneAsync(filter, update);
                LoadUsers();
            }
        }

        private async void ResetPassword(User user)
        {
            string newPassword = GenerateRandomPassword();
            string newPasswordHash = HashPassword(newPassword);

            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, newPasswordHash)
                .Set(u => u.MustChangePassword, true)
                .Set(u => u.PasswordResetRequested, false);

            await usersCollection.UpdateOneAsync(filter, update);

            MessageBox.Show($"Password for '{user.Username}' has been reset to:\n\n{newPassword}\n\nUser must change password on next login.",
                "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void ShowUserDialog(User userToEdit)
        {
            if (userToEdit == null)
            {
                using (var userForm = new UserForm(null))
                {
                    if (userForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadUsers();
                    }
                }
            }
        }

        private void ShowOwnProfileDialog(User user)
        {
            using (var profileForm = new ProfileForm(user))
            {
                if (profileForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                    SessionManager.FullName = user.FullName;
                }
            }
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
            this.Name = "ucUsers";
            this.Size = new System.Drawing.Size(1000, 600);
            this.ResumeLayout(false);
        }
    }
}