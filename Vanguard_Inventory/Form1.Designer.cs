namespace Vanguard_Inventory
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.pnlNavIndicator = new System.Windows.Forms.Panel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnLedger = new System.Windows.Forms.Button();
            this.btnCatalog = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnMaximize = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.lblMongoStatus = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.pnlMainContent = new System.Windows.Forms.Panel();
            this.pnlLowerSection = new System.Windows.Forms.Panel();
            this.pnlCriticalAlerts = new System.Windows.Forms.Panel();
            this.lblCriticalTitle = new System.Windows.Forms.Label();
            this.pnlGraphContainer = new System.Windows.Forms.Panel();
            this.lblGraphTitle = new System.Windows.Forms.Label();
            this.flpStatCards = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlCardValue = new System.Windows.Forms.Panel();
            this.lblCardValueData = new System.Windows.Forms.Label();
            this.lblCardValueTitle = new System.Windows.Forms.Label();
            this.pnlCardAlert = new System.Windows.Forms.Panel();
            this.lblCardAlertData = new System.Windows.Forms.Label();
            this.lblCardAlertTitle = new System.Windows.Forms.Label();
            this.pnlCardStock = new System.Windows.Forms.Panel();
            this.lblCardStockData = new System.Windows.Forms.Label();
            this.lblCardStockTitle = new System.Windows.Forms.Label();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlTopBar.SuspendLayout();
            this.pnlMainContent.SuspendLayout();
            this.pnlLowerSection.SuspendLayout();
            this.pnlCriticalAlerts.SuspendLayout();
            this.pnlGraphContainer.SuspendLayout();
            this.flpStatCards.SuspendLayout();
            this.pnlCardValue.SuspendLayout();
            this.pnlCardAlert.SuspendLayout();
            this.pnlCardStock.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(38)))));
            this.pnlSidebar.Controls.Add(this.pnlNavIndicator);
            this.pnlSidebar.Controls.Add(this.btnSettings);
            this.pnlSidebar.Controls.Add(this.btnUsers);
            this.pnlSidebar.Controls.Add(this.btnLedger);
            this.pnlSidebar.Controls.Add(this.btnCatalog);
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Controls.Add(this.picLogo);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(1, 1);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(260, 866);
            this.pnlSidebar.TabIndex = 2;
            // 
            // pnlNavIndicator
            // 
            this.pnlNavIndicator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.pnlNavIndicator.Location = new System.Drawing.Point(0, 225);
            this.pnlNavIndicator.Name = "pnlNavIndicator";
            this.pnlNavIndicator.Size = new System.Drawing.Size(4, 62);
            this.pnlNavIndicator.TabIndex = 0;
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(0, 473);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnSettings.Size = new System.Drawing.Size(260, 62);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "⚙️ Settings";
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.UseVisualStyleBackColor = true;
            // 
            // btnUsers
            // 
            this.btnUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUsers.FlatAppearance.BorderSize = 0;
            this.btnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsers.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnUsers.ForeColor = System.Drawing.Color.White;
            this.btnUsers.Location = new System.Drawing.Point(0, 411);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnUsers.Size = new System.Drawing.Size(260, 62);
            this.btnUsers.TabIndex = 2;
            this.btnUsers.Text = "👥 Users";
            this.btnUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUsers.UseVisualStyleBackColor = true;
            // 
            // btnLedger
            // 
            this.btnLedger.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLedger.FlatAppearance.BorderSize = 0;
            this.btnLedger.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLedger.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnLedger.ForeColor = System.Drawing.Color.White;
            this.btnLedger.Location = new System.Drawing.Point(0, 349);
            this.btnLedger.Name = "btnLedger";
            this.btnLedger.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnLedger.Size = new System.Drawing.Size(260, 62);
            this.btnLedger.TabIndex = 3;
            this.btnLedger.Text = "📋 Ledger";
            this.btnLedger.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLedger.UseVisualStyleBackColor = true;
            // 
            // btnCatalog
            // 
            this.btnCatalog.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatalog.FlatAppearance.BorderSize = 0;
            this.btnCatalog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatalog.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnCatalog.ForeColor = System.Drawing.Color.White;
            this.btnCatalog.Location = new System.Drawing.Point(0, 287);
            this.btnCatalog.Name = "btnCatalog";
            this.btnCatalog.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnCatalog.Size = new System.Drawing.Size(260, 62);
            this.btnCatalog.TabIndex = 4;
            this.btnCatalog.Text = "📦 Catalog";
            this.btnCatalog.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatalog.UseVisualStyleBackColor = true;
            // 
            // btnDashboard
            // 
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnDashboard.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Location = new System.Drawing.Point(0, 225);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            this.btnDashboard.Size = new System.Drawing.Size(260, 62);
            this.btnDashboard.TabIndex = 5;
            this.btnDashboard.Text = "📊 Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.UseVisualStyleBackColor = true;
            // 
            // picLogo
            // 
            this.picLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
            this.picLogo.Location = new System.Drawing.Point(0, 0);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(260, 225);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 6;
            this.picLogo.TabStop = false;
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(46)))));
            this.pnlTopBar.Controls.Add(this.btnRefresh);
            this.pnlTopBar.Controls.Add(this.btnClose);
            this.pnlTopBar.Controls.Add(this.btnMaximize);
            this.pnlTopBar.Controls.Add(this.btnMinimize);
            this.pnlTopBar.Controls.Add(this.lblMongoStatus);
            this.pnlTopBar.Controls.Add(this.lblName);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(261, 1);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Size = new System.Drawing.Size(1063, 70);
            this.pnlTopBar.TabIndex = 1;
            this.pnlTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlTopBar_MouseDown);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(837, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(71, 33);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "⟳ Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(69)))), ((int)(((byte)(58)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1018, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 35);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "✕";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMaximize
            // 
            this.btnMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaximize.FlatAppearance.BorderSize = 0;
            this.btnMaximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaximize.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMaximize.ForeColor = System.Drawing.Color.White;
            this.btnMaximize.Location = new System.Drawing.Point(973, 0);
            this.btnMaximize.Name = "btnMaximize";
            this.btnMaximize.Size = new System.Drawing.Size(45, 35);
            this.btnMaximize.TabIndex = 3;
            this.btnMaximize.Text = "□";
            this.btnMaximize.UseVisualStyleBackColor = true;
            this.btnMaximize.Click += new System.EventHandler(this.btnMaximize_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.Location = new System.Drawing.Point(928, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(45, 35);
            this.btnMinimize.TabIndex = 2;
            this.btnMinimize.Text = "—";
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // lblMongoStatus
            // 
            this.lblMongoStatus.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMongoStatus.AutoSize = true;
            this.lblMongoStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMongoStatus.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.lblMongoStatus.Location = new System.Drawing.Point(593, 26);
            this.lblMongoStatus.Name = "lblMongoStatus";
            this.lblMongoStatus.Size = new System.Drawing.Size(113, 25);
            this.lblMongoStatus.TabIndex = 0;
            this.lblMongoStatus.Text = "● Connected";
            this.lblMongoStatus.Click += new System.EventHandler(this.lblMongo_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(383, 32);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "VANGUARD SMART INVENTORY";
            // 
            // pnlMainContent
            // 
            this.pnlMainContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(52)))));
            this.pnlMainContent.Controls.Add(this.pnlLowerSection);
            this.pnlMainContent.Controls.Add(this.flpStatCards);
            this.pnlMainContent.Controls.Add(this.lblPageTitle);
            this.pnlMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainContent.Location = new System.Drawing.Point(261, 71);
            this.pnlMainContent.Name = "pnlMainContent";
            this.pnlMainContent.Padding = new System.Windows.Forms.Padding(30);
            this.pnlMainContent.Size = new System.Drawing.Size(1063, 796);
            this.pnlMainContent.TabIndex = 0;
            // 
            // pnlLowerSection
            // 
            this.pnlLowerSection.Controls.Add(this.pnlCriticalAlerts);
            this.pnlLowerSection.Controls.Add(this.pnlGraphContainer);
            this.pnlLowerSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLowerSection.Location = new System.Drawing.Point(30, 250);
            this.pnlLowerSection.Name = "pnlLowerSection";
            this.pnlLowerSection.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.pnlLowerSection.Size = new System.Drawing.Size(1003, 516);
            this.pnlLowerSection.TabIndex = 2;
            // 
            // pnlCriticalAlerts
            // 
            this.pnlCriticalAlerts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(60)))));
            this.pnlCriticalAlerts.Controls.Add(this.lblCriticalTitle);
            this.pnlCriticalAlerts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCriticalAlerts.Location = new System.Drawing.Point(630, 20);
            this.pnlCriticalAlerts.Name = "pnlCriticalAlerts";
            this.pnlCriticalAlerts.Padding = new System.Windows.Forms.Padding(20);
            this.pnlCriticalAlerts.Size = new System.Drawing.Size(373, 496);
            this.pnlCriticalAlerts.TabIndex = 3;
            // 
            // lblCriticalTitle
            // 
            this.lblCriticalTitle.AutoSize = true;
            this.lblCriticalTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCriticalTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(69)))), ((int)(((byte)(58)))));
            this.lblCriticalTitle.Location = new System.Drawing.Point(15, 15);
            this.lblCriticalTitle.Name = "lblCriticalTitle";
            this.lblCriticalTitle.Size = new System.Drawing.Size(200, 32);
            this.lblCriticalTitle.TabIndex = 0;
            this.lblCriticalTitle.Text = "⚠️ Critical Risks";
            // 
            // pnlGraphContainer
            // 
            this.pnlGraphContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(60)))));
            this.pnlGraphContainer.Controls.Add(this.lblGraphTitle);
            this.pnlGraphContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlGraphContainer.Location = new System.Drawing.Point(0, 20);
            this.pnlGraphContainer.Name = "pnlGraphContainer";
            this.pnlGraphContainer.Size = new System.Drawing.Size(630, 496);
            this.pnlGraphContainer.TabIndex = 2;
            // 
            // lblGraphTitle
            // 
            this.lblGraphTitle.AutoSize = true;
            this.lblGraphTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblGraphTitle.ForeColor = System.Drawing.Color.White;
            this.lblGraphTitle.Location = new System.Drawing.Point(20, 15);
            this.lblGraphTitle.Name = "lblGraphTitle";
            this.lblGraphTitle.Size = new System.Drawing.Size(474, 32);
            this.lblGraphTitle.TabIndex = 0;
            this.lblGraphTitle.Text = "📈 Stock Depletion Trend (Last 30 Days)";
            // 
            // flpStatCards
            // 
            this.flpStatCards.Controls.Add(this.pnlCardValue);
            this.flpStatCards.Controls.Add(this.pnlCardAlert);
            this.flpStatCards.Controls.Add(this.pnlCardStock);
            this.flpStatCards.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpStatCards.Location = new System.Drawing.Point(30, 110);
            this.flpStatCards.Name = "flpStatCards";
            this.flpStatCards.Size = new System.Drawing.Size(1003, 140);
            this.flpStatCards.TabIndex = 1;
            // 
            // pnlCardValue
            // 
            this.pnlCardValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(60)))));
            this.pnlCardValue.Controls.Add(this.lblCardValueData);
            this.pnlCardValue.Controls.Add(this.lblCardValueTitle);
            this.pnlCardValue.Location = new System.Drawing.Point(0, 0);
            this.pnlCardValue.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.pnlCardValue.Name = "pnlCardValue";
            this.pnlCardValue.Size = new System.Drawing.Size(300, 130);
            this.pnlCardValue.TabIndex = 0;
            // 
            // lblCardValueData
            // 
            this.lblCardValueData.AutoSize = true;
            this.lblCardValueData.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCardValueData.ForeColor = System.Drawing.Color.White;
            this.lblCardValueData.Location = new System.Drawing.Point(3, 55);
            this.lblCardValueData.Name = "lblCardValueData";
            this.lblCardValueData.Size = new System.Drawing.Size(84, 65);
            this.lblCardValueData.TabIndex = 0;
            this.lblCardValueData.Text = "$0";
            // 
            // lblCardValueTitle
            // 
            this.lblCardValueTitle.AutoSize = true;
            this.lblCardValueTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCardValueTitle.ForeColor = System.Drawing.Color.DarkGray;
            this.lblCardValueTitle.Location = new System.Drawing.Point(15, 20);
            this.lblCardValueTitle.Name = "lblCardValueTitle";
            this.lblCardValueTitle.Size = new System.Drawing.Size(269, 28);
            this.lblCardValueTitle.TabIndex = 1;
            this.lblCardValueTitle.Text = "💰 TOTAL INVENTORY VALUE";
            // 
            // pnlCardAlert
            // 
            this.pnlCardAlert.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.pnlCardAlert.Controls.Add(this.lblCardAlertData);
            this.pnlCardAlert.Controls.Add(this.lblCardAlertTitle);
            this.pnlCardAlert.Location = new System.Drawing.Point(320, 0);
            this.pnlCardAlert.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.pnlCardAlert.Name = "pnlCardAlert";
            this.pnlCardAlert.Size = new System.Drawing.Size(300, 130);
            this.pnlCardAlert.TabIndex = 1;
            // 
            // lblCardAlertData
            // 
            this.lblCardAlertData.AutoSize = true;
            this.lblCardAlertData.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCardAlertData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(69)))), ((int)(((byte)(58)))));
            this.lblCardAlertData.Location = new System.Drawing.Point(3, 55);
            this.lblCardAlertData.Name = "lblCardAlertData";
            this.lblCardAlertData.Size = new System.Drawing.Size(194, 65);
            this.lblCardAlertData.TabIndex = 0;
            this.lblCardAlertData.Text = "0 Items";
            // 
            // lblCardAlertTitle
            // 
            this.lblCardAlertTitle.AutoSize = true;
            this.lblCardAlertTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCardAlertTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblCardAlertTitle.Location = new System.Drawing.Point(15, 20);
            this.lblCardAlertTitle.Name = "lblCardAlertTitle";
            this.lblCardAlertTitle.Size = new System.Drawing.Size(170, 28);
            this.lblCardAlertTitle.TabIndex = 1;
            this.lblCardAlertTitle.Text = "⚠️ ITEMS AT RISK";
            // 
            // pnlCardStock
            // 
            this.pnlCardStock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(60)))), ((int)(((byte)(50)))));
            this.pnlCardStock.Controls.Add(this.lblCardStockData);
            this.pnlCardStock.Controls.Add(this.lblCardStockTitle);
            this.pnlCardStock.Location = new System.Drawing.Point(640, 0);
            this.pnlCardStock.Margin = new System.Windows.Forms.Padding(0);
            this.pnlCardStock.Name = "pnlCardStock";
            this.pnlCardStock.Size = new System.Drawing.Size(300, 130);
            this.pnlCardStock.TabIndex = 2;
            // 
            // lblCardStockData
            // 
            this.lblCardStockData.AutoSize = true;
            this.lblCardStockData.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCardStockData.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblCardStockData.Location = new System.Drawing.Point(3, 55);
            this.lblCardStockData.Name = "lblCardStockData";
            this.lblCardStockData.Size = new System.Drawing.Size(56, 65);
            this.lblCardStockData.TabIndex = 0;
            this.lblCardStockData.Text = "0";
            // 
            // lblCardStockTitle
            // 
            this.lblCardStockTitle.AutoSize = true;
            this.lblCardStockTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCardStockTitle.ForeColor = System.Drawing.Color.LightGray;
            this.lblCardStockTitle.Location = new System.Drawing.Point(15, 20);
            this.lblCardStockTitle.Name = "lblCardStockTitle";
            this.lblCardStockTitle.Size = new System.Drawing.Size(246, 28);
            this.lblCardStockTitle.TabIndex = 1;
            this.lblCardStockTitle.Text = "📦 TOTAL ITEMS IN STOCK";
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.White;
            this.lblPageTitle.Location = new System.Drawing.Point(30, 30);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.lblPageTitle.Size = new System.Drawing.Size(273, 80);
            this.lblPageTitle.TabIndex = 1;
            this.lblPageTitle.Text = "Dashboard";
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1325, 868);
            this.Controls.Add(this.pnlMainContent);
            this.Controls.Add(this.pnlTopBar);
            this.Controls.Add(this.pnlSidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vanguard Smart Inventory";
            this.pnlSidebar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            this.pnlMainContent.ResumeLayout(false);
            this.pnlMainContent.PerformLayout();
            this.pnlLowerSection.ResumeLayout(false);
            this.pnlCriticalAlerts.ResumeLayout(false);
            this.pnlCriticalAlerts.PerformLayout();
            this.pnlGraphContainer.ResumeLayout(false);
            this.pnlGraphContainer.PerformLayout();
            this.flpStatCards.ResumeLayout(false);
            this.pnlCardValue.ResumeLayout(false);
            this.pnlCardValue.PerformLayout();
            this.pnlCardAlert.ResumeLayout(false);
            this.pnlCardAlert.PerformLayout();
            this.pnlCardStock.ResumeLayout(false);
            this.pnlCardStock.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // FIELD DECLARATIONS
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Panel pnlMainContent;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnLedger;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnCatalog;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblMongoStatus;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Panel pnlNavIndicator;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.FlowLayoutPanel flpStatCards;
        private System.Windows.Forms.Panel pnlLowerSection;
        private System.Windows.Forms.Panel pnlCriticalAlerts;
        private System.Windows.Forms.Label lblCriticalTitle;
        private System.Windows.Forms.Panel pnlGraphContainer;
        private System.Windows.Forms.Label lblGraphTitle;
        private System.Windows.Forms.Panel pnlCardValue;
        private System.Windows.Forms.Label lblCardValueTitle;
        private System.Windows.Forms.Label lblCardValueData;
        private System.Windows.Forms.Panel pnlCardAlert;
        private System.Windows.Forms.Label lblCardAlertTitle;
        private System.Windows.Forms.Label lblCardAlertData;
        private System.Windows.Forms.Panel pnlCardStock;
        private System.Windows.Forms.Label lblCardStockTitle;
        private System.Windows.Forms.Label lblCardStockData;
    }
}