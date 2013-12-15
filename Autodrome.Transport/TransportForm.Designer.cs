namespace Autodrome.Transport
{
    partial class TransportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
           this.components = new System.ComponentModel.Container();
           this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
           this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
           this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
           this.startButton = new System.Windows.Forms.Button();
           this.baseLabel = new System.Windows.Forms.Label();
           this.label1 = new System.Windows.Forms.Label();
           this.label2 = new System.Windows.Forms.Label();
           this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
           this.label3 = new System.Windows.Forms.Label();
           this.dataGridView1 = new System.Windows.Forms.DataGridView();
           this.availabilityTimer = new System.Windows.Forms.Timer(this.components);
           this.gnssAddressBox = new System.Windows.Forms.ComboBox();
           this.gnssPortBox = new System.Windows.Forms.TextBox();
           this.applicationAddressBox = new System.Windows.Forms.ComboBox();
           this.basePortBox = new System.Windows.Forms.ComboBox();
           this.autodromAddressBox = new System.Windows.Forms.ComboBox();
           this.applicationPortBox = new System.Windows.Forms.TextBox();
           this.autodromPortBox = new System.Windows.Forms.TextBox();
           this.logView = new System.Windows.Forms.ListBox();
           this.updateTimer = new System.Windows.Forms.Timer(this.components);
           this.splitter1 = new System.Windows.Forms.Splitter();
           this.clearButton = new System.Windows.Forms.Button();
           this.panel1 = new System.Windows.Forms.Panel();
           this.columnAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.columnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.columnReading = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.columnWriting = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.columnReadBPS = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.columnWriteBPS = new System.Windows.Forms.DataGridViewTextBoxColumn();
           this.trayMenu.SuspendLayout();
           ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
           this.panel1.SuspendLayout();
           this.SuspendLayout();
           // 
           // trayIcon
           // 
           this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
           this.trayIcon.BalloonTipTitle = "Autodrome.Transport";
           this.trayIcon.ContextMenuStrip = this.trayMenu;
           this.trayIcon.Text = "Autodrome.Transport";
           this.trayIcon.Visible = true;
           this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
           // 
           // trayMenu
           // 
           this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitMenuItem});
           this.trayMenu.Name = "trayMenu";
           this.trayMenu.Size = new System.Drawing.Size(119, 26);
           // 
           // exitMenuItem
           // 
           this.exitMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
           this.exitMenuItem.Name = "exitMenuItem";
           this.exitMenuItem.Size = new System.Drawing.Size(118, 22);
           this.exitMenuItem.Text = "Выход";
           this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
           // 
           // startButton
           // 
           this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
           this.startButton.Location = new System.Drawing.Point(477, 3);
           this.startButton.Name = "startButton";
           this.startButton.Size = new System.Drawing.Size(75, 23);
           this.startButton.TabIndex = 8;
           this.startButton.Text = "Старт";
           this.startButton.UseVisualStyleBackColor = true;
           this.startButton.Click += new System.EventHandler(this.startButton_Click);
           // 
           // baseLabel
           // 
           this.baseLabel.AutoSize = true;
           this.baseLabel.Location = new System.Drawing.Point(3, 6);
           this.baseLabel.Name = "baseLabel";
           this.baseLabel.Size = new System.Drawing.Size(64, 13);
           this.baseLabel.TabIndex = 18;
           this.baseLabel.Text = "GNSS-база";
           // 
           // label1
           // 
           this.label1.AutoSize = true;
           this.label1.Location = new System.Drawing.Point(3, 54);
           this.label1.Name = "label1";
           this.label1.Size = new System.Drawing.Size(57, 13);
           this.label1.TabIndex = 20;
           this.label1.Text = "Автодром";
           // 
           // label2
           // 
           this.label2.AutoSize = true;
           this.label2.Location = new System.Drawing.Point(3, 75);
           this.label2.Name = "label2";
           this.label2.Size = new System.Drawing.Size(71, 13);
           this.label2.TabIndex = 23;
           this.label2.Text = "Приложения";
           // 
           // openFileDialog1
           // 
           this.openFileDialog1.FileName = "openFileDialog1";
           this.openFileDialog1.InitialDirectory = "v:\\autodrom\\logs";
           // 
           // label3
           // 
           this.label3.AutoSize = true;
           this.label3.Location = new System.Drawing.Point(3, 33);
           this.label3.Name = "label3";
           this.label3.Size = new System.Drawing.Size(83, 13);
           this.label3.TabIndex = 29;
           this.label3.Text = "GNSS-клиенты";
           // 
           // dataGridView1
           // 
           this.dataGridView1.AllowUserToAddRows = false;
           this.dataGridView1.AllowUserToDeleteRows = false;
           this.dataGridView1.AllowUserToResizeRows = false;
           this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
           this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnAddress,
            this.columnType,
            this.columnReading,
            this.columnWriting,
            this.columnReadBPS,
            this.columnWriteBPS});
           this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
           this.dataGridView1.Location = new System.Drawing.Point(0, 100);
           this.dataGridView1.Name = "dataGridView1";
           this.dataGridView1.ReadOnly = true;
           this.dataGridView1.RowHeadersVisible = false;
           this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
           this.dataGridView1.Size = new System.Drawing.Size(555, 160);
           this.dataGridView1.TabIndex = 32;
           this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
           // 
           // availabilityTimer
           // 
           this.availabilityTimer.Interval = 2000;
           this.availabilityTimer.Tick += new System.EventHandler(/*Logic.availabilityTimer_Tick*/this.availabilityTimer_Tick);
           // 
           // gnssAddressBox
           // 
           this.gnssAddressBox.FormattingEnabled = true;
           this.gnssAddressBox.Items.AddRange(new object[] {
            "127.0.0.1"});
           this.gnssAddressBox.Location = new System.Drawing.Point(89, 30);
           this.gnssAddressBox.Name = "gnssAddressBox";
           this.gnssAddressBox.Size = new System.Drawing.Size(119, 21);
           this.gnssAddressBox.TabIndex = 30;
           this.gnssAddressBox.Text = global::Autodrome.Transport.Properties.Settings.Default.GNSSClientAddress;
           this.gnssAddressBox.DropDown += new System.EventHandler(this.localIP_DropDown);
           // 
           // gnssPortBox
           // 
           this.gnssPortBox.Location = new System.Drawing.Point(214, 30);
           this.gnssPortBox.Name = "gnssPortBox";
           this.gnssPortBox.Size = new System.Drawing.Size(39, 20);
           this.gnssPortBox.TabIndex = 28;
           this.gnssPortBox.Text = global::Autodrome.Transport.Properties.Settings.Default.GNSSClientPort;
           // 
           // applicationAddressBox
           // 
           this.applicationAddressBox.FormattingEnabled = true;
           this.applicationAddressBox.Location = new System.Drawing.Point(89, 72);
           this.applicationAddressBox.Name = "applicationAddressBox";
           this.applicationAddressBox.Size = new System.Drawing.Size(119, 21);
           this.applicationAddressBox.TabIndex = 24;
           this.applicationAddressBox.Text = global::Autodrome.Transport.Properties.Settings.Default.ApplicationAddress;
           this.applicationAddressBox.DropDown += new System.EventHandler(this.localIP_DropDown);
           // 
           // basePortBox
           // 
           this.basePortBox.FormattingEnabled = true;
           this.basePortBox.Location = new System.Drawing.Point(89, 3);
           this.basePortBox.Name = "basePortBox";
           this.basePortBox.Size = new System.Drawing.Size(164, 21);
           this.basePortBox.TabIndex = 19;
           this.basePortBox.Text = global::Autodrome.Transport.Properties.Settings.Default.GNSSBasePort;
           this.basePortBox.DropDown += new System.EventHandler(this.serial_DropDown);
           // 
           // autodromAddressBox
           // 
           this.autodromAddressBox.FormattingEnabled = true;
           this.autodromAddressBox.Items.AddRange(new object[] {
            "127.0.0.1"});
           this.autodromAddressBox.Location = new System.Drawing.Point(89, 51);
           this.autodromAddressBox.Name = "autodromAddressBox";
           this.autodromAddressBox.Size = new System.Drawing.Size(119, 21);
           this.autodromAddressBox.TabIndex = 21;
           this.autodromAddressBox.Text = global::Autodrome.Transport.Properties.Settings.Default.AutodromeAddress;
           this.autodromAddressBox.DropDown += new System.EventHandler(this.localIP_DropDown);
           // 
           // applicationPortBox
           // 
           this.applicationPortBox.Location = new System.Drawing.Point(214, 72);
           this.applicationPortBox.Name = "applicationPortBox";
           this.applicationPortBox.Size = new System.Drawing.Size(39, 20);
           this.applicationPortBox.TabIndex = 22;
           this.applicationPortBox.Text = global::Autodrome.Transport.Properties.Settings.Default.ApplicationPort;
           // 
           // autodromPortBox
           // 
           this.autodromPortBox.Location = new System.Drawing.Point(214, 51);
           this.autodromPortBox.Name = "autodromPortBox";
           this.autodromPortBox.Size = new System.Drawing.Size(39, 20);
           this.autodromPortBox.TabIndex = 16;
           this.autodromPortBox.Text = global::Autodrome.Transport.Properties.Settings.Default.AutodromePort;
           // 
           // logView
           // 
           this.logView.Dock = System.Windows.Forms.DockStyle.Bottom;
           this.logView.FormattingEnabled = true;
           this.logView.Location = new System.Drawing.Point(0, 263);
           this.logView.Name = "logView";
           this.logView.Size = new System.Drawing.Size(555, 95);
           this.logView.TabIndex = 33;
           // 
           // updateTimer
           // 
           this.updateTimer.Enabled = true;
           this.updateTimer.Interval = 1000;
           this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
           // 
           // splitter1
           // 
           this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
           this.splitter1.Location = new System.Drawing.Point(0, 260);
           this.splitter1.Name = "splitter1";
           this.splitter1.Size = new System.Drawing.Size(555, 3);
           this.splitter1.TabIndex = 34;
           this.splitter1.TabStop = false;
           // 
           // clearButton
           // 
           this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
           this.clearButton.Location = new System.Drawing.Point(477, 74);
           this.clearButton.Name = "clearButton";
           this.clearButton.Size = new System.Drawing.Size(75, 23);
           this.clearButton.TabIndex = 35;
           this.clearButton.Text = "Очистить";
           this.clearButton.UseVisualStyleBackColor = true;
           this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
           // 
           // panel1
           // 
           this.panel1.Controls.Add(this.startButton);
           this.panel1.Controls.Add(this.autodromPortBox);
           this.panel1.Controls.Add(this.applicationPortBox);
           this.panel1.Controls.Add(this.baseLabel);
           this.panel1.Controls.Add(this.label1);
           this.panel1.Controls.Add(this.autodromAddressBox);
           this.panel1.Controls.Add(this.basePortBox);
           this.panel1.Controls.Add(this.label2);
           this.panel1.Controls.Add(this.applicationAddressBox);
           this.panel1.Controls.Add(this.gnssPortBox);
           this.panel1.Controls.Add(this.label3);
           this.panel1.Controls.Add(this.clearButton);
           this.panel1.Controls.Add(this.gnssAddressBox);
           this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
           this.panel1.Location = new System.Drawing.Point(0, 0);
           this.panel1.Name = "panel1";
           this.panel1.Size = new System.Drawing.Size(555, 100);
           this.panel1.TabIndex = 36;
           // 
           // columnAddress
           // 
           this.columnAddress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
           this.columnAddress.HeaderText = "Адрес";
           this.columnAddress.Name = "columnAddress";
           this.columnAddress.ReadOnly = true;
           this.columnAddress.Width = 63;
           // 
           // columnType
           // 
           this.columnType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
           this.columnType.HeaderText = "Тип";
           this.columnType.Name = "columnType";
           this.columnType.ReadOnly = true;
           // 
           // columnReading
           // 
           this.columnReading.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
           this.columnReading.HeaderText = "Прочинато";
           this.columnReading.Name = "columnReading";
           this.columnReading.ReadOnly = true;
           this.columnReading.Resizable = System.Windows.Forms.DataGridViewTriState.False;
           this.columnReading.Width = 86;
           // 
           // columnWriting
           // 
           this.columnWriting.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
           this.columnWriting.HeaderText = "Записано";
           this.columnWriting.Name = "columnWriting";
           this.columnWriting.ReadOnly = true;
           this.columnWriting.Resizable = System.Windows.Forms.DataGridViewTriState.False;
           this.columnWriting.Width = 81;
           // 
           // columnReadBPS
           // 
           this.columnReadBPS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
           this.columnReadBPS.HeaderText = "BPS-чтение";
           this.columnReadBPS.Name = "columnReadBPS";
           this.columnReadBPS.ReadOnly = true;
           this.columnReadBPS.Width = 90;
           // 
           // columnWriteBPS
           // 
           this.columnWriteBPS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
           this.columnWriteBPS.HeaderText = "BPS-запись";
           this.columnWriteBPS.Name = "columnWriteBPS";
           this.columnWriteBPS.ReadOnly = true;
           this.columnWriteBPS.Width = 92;
           // 
           // TransportForm
           // 
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.ClientSize = new System.Drawing.Size(555, 358);
           this.Controls.Add(this.dataGridView1);
           this.Controls.Add(this.panel1);
           this.Controls.Add(this.splitter1);
           this.Controls.Add(this.logView);
           this.MaximizeBox = false;
           this.MinimumSize = new System.Drawing.Size(360, 360);
           this.Name = "TransportForm";
           this.ShowIcon = false;
           this.Text = "TransportForm";
           this.Load += new System.EventHandler(this.TransportForm_Load);
           this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TransportForm_FormClosed);
           this.Resize += new System.EventHandler(this.TransportForm_Resize);
           this.trayMenu.ResumeLayout(false);
           ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
           this.panel1.ResumeLayout(false);
           this.panel1.PerformLayout();
           this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
       private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.TextBox autodromPortBox;
        private System.Windows.Forms.Label baseLabel;
        private System.Windows.Forms.ComboBox basePortBox;
        private System.Windows.Forms.ComboBox autodromAddressBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox applicationAddressBox;
        private System.Windows.Forms.Label label2;
       private System.Windows.Forms.TextBox applicationPortBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
       private System.Windows.Forms.ComboBox gnssAddressBox;
       private System.Windows.Forms.Label label3;
       private System.Windows.Forms.TextBox gnssPortBox;
       private System.Windows.Forms.DataGridView dataGridView1;
       private System.Windows.Forms.Timer availabilityTimer;
       private System.Windows.Forms.ListBox logView;
       private System.Windows.Forms.Timer updateTimer;
       private System.Windows.Forms.Splitter splitter1;
       private System.Windows.Forms.Button clearButton;
       private System.Windows.Forms.Panel panel1;
       private System.Windows.Forms.DataGridViewTextBoxColumn columnAddress;
       private System.Windows.Forms.DataGridViewTextBoxColumn columnType;
       private System.Windows.Forms.DataGridViewTextBoxColumn columnReading;
       private System.Windows.Forms.DataGridViewTextBoxColumn columnWriting;
       private System.Windows.Forms.DataGridViewTextBoxColumn columnReadBPS;
       private System.Windows.Forms.DataGridViewTextBoxColumn columnWriteBPS;
    }
}