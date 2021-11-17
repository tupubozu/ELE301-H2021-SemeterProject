namespace SemesterProject.Monitor.GUI
{
    partial class Monitor
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggInnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rapportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brukerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brukerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dørToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filToolStripMenuItem
            // 
            this.filToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loggInnToolStripMenuItem,
            this.rapportToolStripMenuItem});
            this.filToolStripMenuItem.Name = "filToolStripMenuItem";
            this.filToolStripMenuItem.Size = new System.Drawing.Size(31, 20);
            this.filToolStripMenuItem.Text = "Fil";
            // 
            // loggInnToolStripMenuItem
            // 
            this.loggInnToolStripMenuItem.Name = "loggInnToolStripMenuItem";
            this.loggInnToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loggInnToolStripMenuItem.Text = "Logg inn";
            this.loggInnToolStripMenuItem.Click += new System.EventHandler(this.loggInnToolStripMenuItem_Click);
            // 
            // rapportToolStripMenuItem
            // 
            this.rapportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.brukerToolStripMenuItem,
            this.brukerToolStripMenuItem1,
            this.dørToolStripMenuItem});
            this.rapportToolStripMenuItem.Name = "rapportToolStripMenuItem";
            this.rapportToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.rapportToolStripMenuItem.Text = "Rapport";
            // 
            // brukerToolStripMenuItem
            // 
            this.brukerToolStripMenuItem.Name = "brukerToolStripMenuItem";
            this.brukerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.brukerToolStripMenuItem.Text = "Adgangslogg";
            // 
            // brukerToolStripMenuItem1
            // 
            this.brukerToolStripMenuItem1.Name = "brukerToolStripMenuItem1";
            this.brukerToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.brukerToolStripMenuItem1.Text = "Bruker";
            // 
            // dørToolStripMenuItem
            // 
            this.dørToolStripMenuItem.Name = "dørToolStripMenuItem";
            this.dørToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.dørToolStripMenuItem.Text = "Dør";
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 24);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(800, 426);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Monitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Monitor";
            this.Text = "Monitor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem filToolStripMenuItem;
        private ToolStripMenuItem loggInnToolStripMenuItem;
        private ToolStripMenuItem rapportToolStripMenuItem;
        private ToolStripMenuItem brukerToolStripMenuItem;
        private ToolStripMenuItem brukerToolStripMenuItem1;
        private ToolStripMenuItem dørToolStripMenuItem;
        private ListView listView1;
        private System.Windows.Forms.Timer timer1;
    }
}