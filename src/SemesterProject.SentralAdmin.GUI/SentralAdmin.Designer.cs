namespace SemesterProject.SentralAdmin.GUI
{
    partial class SentralAdmin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loggInnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oppfriskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbReaderSone = new System.Windows.Forms.ComboBox();
            this.labelReaderPlacement = new System.Windows.Forms.Label();
            this.labelReaderSone = new System.Windows.Forms.Label();
            this.txtPlacement = new System.Windows.Forms.TextBox();
            this.txtReaderID = new System.Windows.Forms.TextBox();
            this.btnRemoveReader = new System.Windows.Forms.Button();
            this.btnAddReader = new System.Windows.Forms.Button();
            this.listReaders = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelStopTime = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.StopTimePicker = new System.Windows.Forms.DateTimePicker();
            this.StartTimePicker = new System.Windows.Forms.DateTimePicker();
            this.labelSoneSelect = new System.Windows.Forms.Label();
            this.btnRemoveSone = new System.Windows.Forms.Button();
            this.btnAddSone = new System.Windows.Forms.Button();
            this.cbSoneSelector = new System.Windows.Forms.ComboBox();
            this.listSones = new System.Windows.Forms.ListBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelLastName = new System.Windows.Forms.Label();
            this.labelFirstName = new System.Windows.Forms.Label();
            this.btnRemoveUser = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.listUsers = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loggInnToolStripMenuItem,
            this.oppfriskToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loggInnToolStripMenuItem
            // 
            this.loggInnToolStripMenuItem.Name = "loggInnToolStripMenuItem";
            this.loggInnToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.loggInnToolStripMenuItem.Text = "Logg inn";
            this.loggInnToolStripMenuItem.Click += new System.EventHandler(this.loggInnToolStripMenuItem_Click);
            // 
            // oppfriskToolStripMenuItem
            // 
            this.oppfriskToolStripMenuItem.Name = "oppfriskToolStripMenuItem";
            this.oppfriskToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.oppfriskToolStripMenuItem.Text = "Oppfrisk";
            this.oppfriskToolStripMenuItem.Click += new System.EventHandler(this.oppfriskToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbReaderSone);
            this.groupBox1.Controls.Add(this.labelReaderPlacement);
            this.groupBox1.Controls.Add(this.labelReaderSone);
            this.groupBox1.Controls.Add(this.txtPlacement);
            this.groupBox1.Controls.Add(this.txtReaderID);
            this.groupBox1.Controls.Add(this.btnRemoveReader);
            this.groupBox1.Controls.Add(this.btnAddReader);
            this.groupBox1.Controls.Add(this.listReaders);
            this.groupBox1.Location = new System.Drawing.Point(12, 316);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(810, 233);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Kortlesere";
            // 
            // cbReaderSone
            // 
            this.cbReaderSone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbReaderSone.FormattingEnabled = true;
            this.cbReaderSone.Location = new System.Drawing.Point(369, 28);
            this.cbReaderSone.Name = "cbReaderSone";
            this.cbReaderSone.Size = new System.Drawing.Size(435, 23);
            this.cbReaderSone.TabIndex = 27;
            this.cbReaderSone.Leave += new System.EventHandler(this.cbReaderSone_Leave);
            // 
            // labelReaderPlacement
            // 
            this.labelReaderPlacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReaderPlacement.Location = new System.Drawing.Point(282, 60);
            this.labelReaderPlacement.Name = "labelReaderPlacement";
            this.labelReaderPlacement.Size = new System.Drawing.Size(60, 15);
            this.labelReaderPlacement.TabIndex = 26;
            this.labelReaderPlacement.Text = "Plassering";
            // 
            // labelReaderSone
            // 
            this.labelReaderSone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReaderSone.AutoSize = true;
            this.labelReaderSone.Location = new System.Drawing.Point(282, 31);
            this.labelReaderSone.Name = "labelReaderSone";
            this.labelReaderSone.Size = new System.Drawing.Size(33, 15);
            this.labelReaderSone.TabIndex = 25;
            this.labelReaderSone.Text = "Sone";
            // 
            // txtPlacement
            // 
            this.txtPlacement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPlacement.Location = new System.Drawing.Point(369, 57);
            this.txtPlacement.Name = "txtPlacement";
            this.txtPlacement.Size = new System.Drawing.Size(435, 23);
            this.txtPlacement.TabIndex = 24;
            this.txtPlacement.Leave += new System.EventHandler(this.txtPlacement_Leave);
            // 
            // txtReaderID
            // 
            this.txtReaderID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReaderID.Location = new System.Drawing.Point(6, 204);
            this.txtReaderID.Name = "txtReaderID";
            this.txtReaderID.Size = new System.Drawing.Size(117, 23);
            this.txtReaderID.TabIndex = 22;
            // 
            // btnRemoveReader
            // 
            this.btnRemoveReader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveReader.Location = new System.Drawing.Point(205, 203);
            this.btnRemoveReader.Name = "btnRemoveReader";
            this.btnRemoveReader.Size = new System.Drawing.Size(71, 23);
            this.btnRemoveReader.TabIndex = 21;
            this.btnRemoveReader.Text = "Fjern";
            this.btnRemoveReader.UseVisualStyleBackColor = true;
            this.btnRemoveReader.Click += new System.EventHandler(this.btnRemoveReader_Click);
            // 
            // btnAddReader
            // 
            this.btnAddReader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddReader.Location = new System.Drawing.Point(129, 203);
            this.btnAddReader.Name = "btnAddReader";
            this.btnAddReader.Size = new System.Drawing.Size(70, 23);
            this.btnAddReader.TabIndex = 20;
            this.btnAddReader.Text = "Legg til";
            this.btnAddReader.UseVisualStyleBackColor = true;
            this.btnAddReader.Click += new System.EventHandler(this.btnAddReader_Click);
            // 
            // listReaders
            // 
            this.listReaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listReaders.FormattingEnabled = true;
            this.listReaders.ItemHeight = 15;
            this.listReaders.Location = new System.Drawing.Point(6, 28);
            this.listReaders.Name = "listReaders";
            this.listReaders.Size = new System.Drawing.Size(270, 169);
            this.listReaders.TabIndex = 19;
            this.listReaders.SelectedIndexChanged += new System.EventHandler(this.listReaders_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelStopTime);
            this.groupBox2.Controls.Add(this.labelStartTime);
            this.groupBox2.Controls.Add(this.StopTimePicker);
            this.groupBox2.Controls.Add(this.StartTimePicker);
            this.groupBox2.Controls.Add(this.labelSoneSelect);
            this.groupBox2.Controls.Add(this.btnRemoveSone);
            this.groupBox2.Controls.Add(this.btnAddSone);
            this.groupBox2.Controls.Add(this.cbSoneSelector);
            this.groupBox2.Controls.Add(this.listSones);
            this.groupBox2.Controls.Add(this.txtUserID);
            this.groupBox2.Controls.Add(this.labelEmail);
            this.groupBox2.Controls.Add(this.labelLastName);
            this.groupBox2.Controls.Add(this.labelFirstName);
            this.groupBox2.Controls.Add(this.btnRemoveUser);
            this.groupBox2.Controls.Add(this.btnAddUser);
            this.groupBox2.Controls.Add(this.txtEmail);
            this.groupBox2.Controls.Add(this.txtLastName);
            this.groupBox2.Controls.Add(this.txtFirstName);
            this.groupBox2.Controls.Add(this.listUsers);
            this.groupBox2.Location = new System.Drawing.Point(12, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(810, 283);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Brukere";
            // 
            // labelStopTime
            // 
            this.labelStopTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStopTime.AutoSize = true;
            this.labelStopTime.Location = new System.Drawing.Point(282, 257);
            this.labelStopTime.Name = "labelStopTime";
            this.labelStopTime.Size = new System.Drawing.Size(62, 15);
            this.labelStopTime.TabIndex = 31;
            this.labelStopTime.Text = "Stoppdato";
            // 
            // labelStartTime
            // 
            this.labelStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(282, 228);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(55, 15);
            this.labelStartTime.TabIndex = 30;
            this.labelStartTime.Text = "Startdato";
            // 
            // StopTimePicker
            // 
            this.StopTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StopTimePicker.Location = new System.Drawing.Point(369, 251);
            this.StopTimePicker.Name = "StopTimePicker";
            this.StopTimePicker.Size = new System.Drawing.Size(187, 23);
            this.StopTimePicker.TabIndex = 29;
            // 
            // StartTimePicker
            // 
            this.StartTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartTimePicker.Location = new System.Drawing.Point(369, 222);
            this.StartTimePicker.Name = "StartTimePicker";
            this.StartTimePicker.Size = new System.Drawing.Size(187, 23);
            this.StartTimePicker.TabIndex = 28;
            // 
            // labelSoneSelect
            // 
            this.labelSoneSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSoneSelect.AutoSize = true;
            this.labelSoneSelect.Location = new System.Drawing.Point(282, 112);
            this.labelSoneSelect.Name = "labelSoneSelect";
            this.labelSoneSelect.Size = new System.Drawing.Size(83, 15);
            this.labelSoneSelect.TabIndex = 25;
            this.labelSoneSelect.Text = "Adgangssoner";
            // 
            // btnRemoveSone
            // 
            this.btnRemoveSone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveSone.Location = new System.Drawing.Point(481, 167);
            this.btnRemoveSone.Name = "btnRemoveSone";
            this.btnRemoveSone.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveSone.TabIndex = 24;
            this.btnRemoveSone.Text = "Fjern";
            this.btnRemoveSone.UseVisualStyleBackColor = true;
            this.btnRemoveSone.Click += new System.EventHandler(this.btnRemoveSone_Click);
            // 
            // btnAddSone
            // 
            this.btnAddSone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSone.Location = new System.Drawing.Point(481, 138);
            this.btnAddSone.Name = "btnAddSone";
            this.btnAddSone.Size = new System.Drawing.Size(75, 23);
            this.btnAddSone.TabIndex = 23;
            this.btnAddSone.Text = "Legg til";
            this.btnAddSone.UseVisualStyleBackColor = true;
            this.btnAddSone.Click += new System.EventHandler(this.btnAddSone_Click);
            // 
            // cbSoneSelector
            // 
            this.cbSoneSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSoneSelector.FormattingEnabled = true;
            this.cbSoneSelector.Location = new System.Drawing.Point(369, 109);
            this.cbSoneSelector.Name = "cbSoneSelector";
            this.cbSoneSelector.Size = new System.Drawing.Size(187, 23);
            this.cbSoneSelector.TabIndex = 22;
            // 
            // listSones
            // 
            this.listSones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listSones.FormattingEnabled = true;
            this.listSones.ItemHeight = 15;
            this.listSones.Location = new System.Drawing.Point(562, 109);
            this.listSones.Name = "listSones";
            this.listSones.Size = new System.Drawing.Size(242, 154);
            this.listSones.TabIndex = 21;
            // 
            // txtUserID
            // 
            this.txtUserID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserID.Location = new System.Drawing.Point(6, 254);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(117, 23);
            this.txtUserID.TabIndex = 20;
            // 
            // labelEmail
            // 
            this.labelEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(282, 83);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(41, 15);
            this.labelEmail.TabIndex = 19;
            this.labelEmail.Text = "E-post";
            // 
            // labelLastName
            // 
            this.labelLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLastName.AutoSize = true;
            this.labelLastName.Location = new System.Drawing.Point(282, 54);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(62, 15);
            this.labelLastName.TabIndex = 18;
            this.labelLastName.Text = "Etternamn";
            // 
            // labelFirstName
            // 
            this.labelFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFirstName.AutoSize = true;
            this.labelFirstName.Location = new System.Drawing.Point(282, 25);
            this.labelFirstName.Name = "labelFirstName";
            this.labelFirstName.Size = new System.Drawing.Size(55, 15);
            this.labelFirstName.TabIndex = 17;
            this.labelFirstName.Text = "Fornamn";
            // 
            // btnRemoveUser
            // 
            this.btnRemoveUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveUser.Location = new System.Drawing.Point(205, 253);
            this.btnRemoveUser.Name = "btnRemoveUser";
            this.btnRemoveUser.Size = new System.Drawing.Size(71, 23);
            this.btnRemoveUser.TabIndex = 16;
            this.btnRemoveUser.Text = "Fjern";
            this.btnRemoveUser.UseVisualStyleBackColor = true;
            this.btnRemoveUser.Click += new System.EventHandler(this.btnRemoveUser_Click);
            // 
            // btnAddUser
            // 
            this.btnAddUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddUser.Location = new System.Drawing.Point(129, 253);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(70, 23);
            this.btnAddUser.TabIndex = 15;
            this.btnAddUser.Text = "Legg til";
            this.btnAddUser.UseVisualStyleBackColor = true;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // txtEmail
            // 
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmail.Location = new System.Drawing.Point(369, 80);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(435, 23);
            this.txtEmail.TabIndex = 14;
            this.txtEmail.Leave += new System.EventHandler(this.txtEmail_Leave);
            // 
            // txtLastName
            // 
            this.txtLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLastName.Location = new System.Drawing.Point(369, 51);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(435, 23);
            this.txtLastName.TabIndex = 13;
            this.txtLastName.Leave += new System.EventHandler(this.txtLastName_Leave);
            // 
            // txtFirstName
            // 
            this.txtFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFirstName.Location = new System.Drawing.Point(369, 22);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(435, 23);
            this.txtFirstName.TabIndex = 12;
            this.txtFirstName.Leave += new System.EventHandler(this.txtFirstName_Leave);
            // 
            // listUsers
            // 
            this.listUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listUsers.FormattingEnabled = true;
            this.listUsers.ItemHeight = 15;
            this.listUsers.Location = new System.Drawing.Point(6, 22);
            this.listUsers.Name = "listUsers";
            this.listUsers.Size = new System.Drawing.Size(270, 214);
            this.listUsers.TabIndex = 11;
            this.listUsers.SelectedValueChanged += new System.EventHandler(this.listUsers_SelectedValueChanged);
            // 
            // SentralAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 561);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(850, 600);
            this.Name = "SentralAdmin";
            this.Text = "SentralAdmin";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem loggInnToolStripMenuItem;
        private ToolStripMenuItem oppfriskToolStripMenuItem;
        private GroupBox groupBox1;
        private Label labelReaderPlacement;
        private Label labelReaderSone;
        private TextBox txtPlacement;
        private TextBox txtReaderID;
        private Button btnRemoveReader;
        private Button btnAddReader;
        private ListBox listReaders;
        private GroupBox groupBox2;
        private TextBox txtUserID;
        private Label labelEmail;
        private Label labelLastName;
        private Label labelFirstName;
        private Button btnRemoveUser;
        private Button btnAddUser;
        private TextBox txtEmail;
        private TextBox txtLastName;
        private TextBox txtFirstName;
        private ListBox listUsers;
        private Label labelSoneSelect;
        private Button btnRemoveSone;
        private Button btnAddSone;
        private ComboBox cbSoneSelector;
        private ListBox listSones;
        private ComboBox cbReaderSone;
        private Label labelStopTime;
        private Label labelStartTime;
        private DateTimePicker StopTimePicker;
        private DateTimePicker StartTimePicker;
    }
}