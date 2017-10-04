namespace Sandboxberry
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.uxSourceUrl = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.uxSourcePassword = new System.Windows.Forms.TextBox();
            this.uxTargetPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.uxTargetUrl = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.uxOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.uxLaunchFileOpen = new System.Windows.Forms.Button();
            this.uxStartButton = new System.Windows.Forms.Button();
            this.uxProgressBox = new System.Windows.Forms.TextBox();
            this.uxWaitPicture = new System.Windows.Forms.PictureBox();
            this.uxMenuStrip = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTargetDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uxInstructionsFilename = new System.Windows.Forms.TextBox();
            this.uxTargetUsername = new System.Windows.Forms.TextBox();
            this.uxSourceUsername = new System.Windows.Forms.TextBox();
            this.uxTestLoginSource = new System.Windows.Forms.Button();
            this.uxTestLoginTarget = new System.Windows.Forms.Button();
            this.uxTestLoginSourceLabel = new System.Windows.Forms.Label();
            this.uxTestLoginTargetLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.uxWaitPicture)).BeginInit();
            this.uxMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Salesforce Source";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "LoginUrl";
            // 
            // uxSourceUrl
            // 
            this.uxSourceUrl.FormattingEnabled = true;
            this.uxSourceUrl.Items.AddRange(new object[] {
            "Sandbox - https://test.salesforce.com",
            "Live - https://login.salesforce.com"});
            this.uxSourceUrl.Location = new System.Drawing.Point(106, 56);
            this.uxSourceUrl.Margin = new System.Windows.Forms.Padding(2);
            this.uxSourceUrl.Name = "uxSourceUrl";
            this.uxSourceUrl.Size = new System.Drawing.Size(210, 21);
            this.uxSourceUrl.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 83);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Username";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 108);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password";
            // 
            // uxSourcePassword
            // 
            this.uxSourcePassword.Location = new System.Drawing.Point(106, 108);
            this.uxSourcePassword.Margin = new System.Windows.Forms.Padding(2);
            this.uxSourcePassword.Name = "uxSourcePassword";
            this.uxSourcePassword.Size = new System.Drawing.Size(210, 20);
            this.uxSourcePassword.TabIndex = 7;
            this.uxSourcePassword.UseSystemPasswordChar = true;
            // 
            // uxTargetPassword
            // 
            this.uxTargetPassword.Location = new System.Drawing.Point(408, 108);
            this.uxTargetPassword.Margin = new System.Windows.Forms.Padding(2);
            this.uxTargetPassword.Name = "uxTargetPassword";
            this.uxTargetPassword.Size = new System.Drawing.Size(230, 20);
            this.uxTargetPassword.TabIndex = 14;
            this.uxTargetPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(346, 108);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(346, 83);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Username";
            // 
            // uxTargetUrl
            // 
            this.uxTargetUrl.FormattingEnabled = true;
            this.uxTargetUrl.Items.AddRange(new object[] {
            "Sandbox - https://test.salesforce.com"});
            this.uxTargetUrl.Location = new System.Drawing.Point(408, 56);
            this.uxTargetUrl.Margin = new System.Windows.Forms.Padding(2);
            this.uxTargetUrl.Name = "uxTargetUrl";
            this.uxTargetUrl.Size = new System.Drawing.Size(230, 21);
            this.uxTargetUrl.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(348, 58);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "LoginUrl";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(329, 37);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Salesforce Target";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 175);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Instructions File";
            // 
            // uxOpenFileDialog
            // 
            this.uxOpenFileDialog.FileName = "openFileDialog1";
            // 
            // uxLaunchFileOpen
            // 
            this.uxLaunchFileOpen.Location = new System.Drawing.Point(612, 172);
            this.uxLaunchFileOpen.Margin = new System.Windows.Forms.Padding(2);
            this.uxLaunchFileOpen.Name = "uxLaunchFileOpen";
            this.uxLaunchFileOpen.Size = new System.Drawing.Size(24, 21);
            this.uxLaunchFileOpen.TabIndex = 17;
            this.uxLaunchFileOpen.Text = "...";
            this.uxLaunchFileOpen.UseVisualStyleBackColor = true;
            this.uxLaunchFileOpen.Click += new System.EventHandler(this.uxLaunchFileOpen_Click);
            // 
            // uxStartButton
            // 
            this.uxStartButton.Location = new System.Drawing.Point(14, 205);
            this.uxStartButton.Margin = new System.Windows.Forms.Padding(2);
            this.uxStartButton.Name = "uxStartButton";
            this.uxStartButton.Size = new System.Drawing.Size(96, 27);
            this.uxStartButton.TabIndex = 18;
            this.uxStartButton.Text = "Start";
            this.uxStartButton.UseVisualStyleBackColor = true;
            this.uxStartButton.Click += new System.EventHandler(this.uxStartButton_Click);
            // 
            // uxProgressBox
            // 
            this.uxProgressBox.Location = new System.Drawing.Point(14, 242);
            this.uxProgressBox.Margin = new System.Windows.Forms.Padding(2);
            this.uxProgressBox.Multiline = true;
            this.uxProgressBox.Name = "uxProgressBox";
            this.uxProgressBox.ReadOnly = true;
            this.uxProgressBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uxProgressBox.Size = new System.Drawing.Size(624, 183);
            this.uxProgressBox.TabIndex = 19;
            // 
            // uxWaitPicture
            // 
            this.uxWaitPicture.Image = ((System.Drawing.Image)(resources.GetObject("uxWaitPicture.Image")));
            this.uxWaitPicture.InitialImage = null;
            this.uxWaitPicture.Location = new System.Drawing.Point(124, 205);
            this.uxWaitPicture.Margin = new System.Windows.Forms.Padding(2);
            this.uxWaitPicture.Name = "uxWaitPicture";
            this.uxWaitPicture.Padding = new System.Windows.Forms.Padding(10, 5, 0, 0);
            this.uxWaitPicture.Size = new System.Drawing.Size(50, 26);
            this.uxWaitPicture.TabIndex = 20;
            this.uxWaitPicture.TabStop = false;
            // 
            // uxMenuStrip
            // 
            this.uxMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.uxMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.uxMenuStrip.Name = "uxMenuStrip";
            this.uxMenuStrip.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.uxMenuStrip.Size = new System.Drawing.Size(680, 24);
            this.uxMenuStrip.TabIndex = 21;
            this.uxMenuStrip.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearTargetDataToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 22);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // clearTargetDataToolStripMenuItem
            // 
            this.clearTargetDataToolStripMenuItem.Name = "clearTargetDataToolStripMenuItem";
            this.clearTargetDataToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.clearTargetDataToolStripMenuItem.Text = "Clear Target Data ...";
            this.clearTargetDataToolStripMenuItem.Click += new System.EventHandler(this.clearTargetDataToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // uxInstructionsFilename
            // 
            this.uxInstructionsFilename.Location = new System.Drawing.Point(106, 172);
            this.uxInstructionsFilename.Margin = new System.Windows.Forms.Padding(2);
            this.uxInstructionsFilename.Name = "uxInstructionsFilename";
            this.uxInstructionsFilename.Size = new System.Drawing.Size(505, 20);
            this.uxInstructionsFilename.TabIndex = 16;
            // 
            // uxTargetUsername
            // 
            this.uxTargetUsername.Location = new System.Drawing.Point(408, 83);
            this.uxTargetUsername.Margin = new System.Windows.Forms.Padding(2);
            this.uxTargetUsername.Name = "uxTargetUsername";
            this.uxTargetUsername.Size = new System.Drawing.Size(230, 20);
            this.uxTargetUsername.TabIndex = 13;
            // 
            // uxSourceUsername
            // 
            this.uxSourceUsername.Location = new System.Drawing.Point(106, 83);
            this.uxSourceUsername.Margin = new System.Windows.Forms.Padding(2);
            this.uxSourceUsername.Name = "uxSourceUsername";
            this.uxSourceUsername.Size = new System.Drawing.Size(210, 20);
            this.uxSourceUsername.TabIndex = 6;
            // 
            // uxTestLoginSource
            // 
            this.uxTestLoginSource.Location = new System.Drawing.Point(106, 132);
            this.uxTestLoginSource.Margin = new System.Windows.Forms.Padding(2);
            this.uxTestLoginSource.Name = "uxTestLoginSource";
            this.uxTestLoginSource.Size = new System.Drawing.Size(68, 22);
            this.uxTestLoginSource.TabIndex = 22;
            this.uxTestLoginSource.Text = "Test Login";
            this.uxTestLoginSource.UseVisualStyleBackColor = true;
            this.uxTestLoginSource.Click += new System.EventHandler(this.uxTestLoginSource_Click);
            // 
            // uxTestLoginTarget
            // 
            this.uxTestLoginTarget.Location = new System.Drawing.Point(408, 132);
            this.uxTestLoginTarget.Margin = new System.Windows.Forms.Padding(2);
            this.uxTestLoginTarget.Name = "uxTestLoginTarget";
            this.uxTestLoginTarget.Size = new System.Drawing.Size(68, 22);
            this.uxTestLoginTarget.TabIndex = 23;
            this.uxTestLoginTarget.Text = "Test Login";
            this.uxTestLoginTarget.UseVisualStyleBackColor = true;
            this.uxTestLoginTarget.Click += new System.EventHandler(this.uxTestLoginTarget_Click);
            // 
            // uxTestLoginSourceLabel
            // 
            this.uxTestLoginSourceLabel.AutoSize = true;
            this.uxTestLoginSourceLabel.Location = new System.Drawing.Point(185, 137);
            this.uxTestLoginSourceLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uxTestLoginSourceLabel.Name = "uxTestLoginSourceLabel";
            this.uxTestLoginSourceLabel.Size = new System.Drawing.Size(90, 13);
            this.uxTestLoginSourceLabel.TabIndex = 24;
            this.uxTestLoginSourceLabel.Text = "Test Login Result";
            this.uxTestLoginSourceLabel.Visible = false;
            // 
            // uxTestLoginTargetLabel
            // 
            this.uxTestLoginTargetLabel.AutoSize = true;
            this.uxTestLoginTargetLabel.Location = new System.Drawing.Point(488, 137);
            this.uxTestLoginTargetLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.uxTestLoginTargetLabel.Name = "uxTestLoginTargetLabel";
            this.uxTestLoginTargetLabel.Size = new System.Drawing.Size(90, 13);
            this.uxTestLoginTargetLabel.TabIndex = 25;
            this.uxTestLoginTargetLabel.Text = "Test Login Result";
            this.uxTestLoginTargetLabel.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(680, 439);
            this.Controls.Add(this.uxTestLoginTargetLabel);
            this.Controls.Add(this.uxTestLoginSourceLabel);
            this.Controls.Add(this.uxTestLoginTarget);
            this.Controls.Add(this.uxTestLoginSource);
            this.Controls.Add(this.uxWaitPicture);
            this.Controls.Add(this.uxProgressBox);
            this.Controls.Add(this.uxStartButton);
            this.Controls.Add(this.uxLaunchFileOpen);
            this.Controls.Add(this.uxInstructionsFilename);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.uxTargetPassword);
            this.Controls.Add(this.uxTargetUsername);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.uxTargetUrl);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.uxSourcePassword);
            this.Controls.Add(this.uxSourceUsername);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.uxSourceUrl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uxMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.uxMenuStrip;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "Sandboxberry";
            ((System.ComponentModel.ISupportInitialize)(this.uxWaitPicture)).EndInit();
            this.uxMenuStrip.ResumeLayout(false);
            this.uxMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox uxSourceUrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox uxSourceUsername;
        private System.Windows.Forms.TextBox uxSourcePassword;
        private System.Windows.Forms.TextBox uxTargetPassword;
        private System.Windows.Forms.TextBox uxTargetUsername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox uxTargetUrl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox uxInstructionsFilename;
        private System.Windows.Forms.OpenFileDialog uxOpenFileDialog;
        private System.Windows.Forms.Button uxLaunchFileOpen;
        private System.Windows.Forms.Button uxStartButton;
        private System.Windows.Forms.TextBox uxProgressBox;
        private System.Windows.Forms.PictureBox uxWaitPicture;
        private System.Windows.Forms.MenuStrip uxMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearTargetDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button uxTestLoginSource;
        private System.Windows.Forms.Button uxTestLoginTarget;
        private System.Windows.Forms.Label uxTestLoginSourceLabel;
        private System.Windows.Forms.Label uxTestLoginTargetLabel;
    }
}

