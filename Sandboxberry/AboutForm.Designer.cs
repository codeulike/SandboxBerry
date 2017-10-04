namespace Sandboxberry
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.uxAboutText = new System.Windows.Forms.TextBox();
            this.uxOkButton = new System.Windows.Forms.Button();
            this.uxHomeLink = new System.Windows.Forms.LinkLabel();
            this.uxIconsLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // uxAboutText
            // 
            this.uxAboutText.Location = new System.Drawing.Point(12, 12);
            this.uxAboutText.Multiline = true;
            this.uxAboutText.Name = "uxAboutText";
            this.uxAboutText.ReadOnly = true;
            this.uxAboutText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uxAboutText.Size = new System.Drawing.Size(368, 185);
            this.uxAboutText.TabIndex = 0;
            this.uxAboutText.TextChanged += new System.EventHandler(this.uxAboutText_TextChanged);
            // 
            // uxOkButton
            // 
            this.uxOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.uxOkButton.Location = new System.Drawing.Point(297, 239);
            this.uxOkButton.Name = "uxOkButton";
            this.uxOkButton.Size = new System.Drawing.Size(82, 23);
            this.uxOkButton.TabIndex = 1;
            this.uxOkButton.Text = "OK";
            this.uxOkButton.UseVisualStyleBackColor = true;
            this.uxOkButton.Click += new System.EventHandler(this.uxOkButton_Click);
            // 
            // uxHomeLink
            // 
            this.uxHomeLink.AutoSize = true;
            this.uxHomeLink.Location = new System.Drawing.Point(13, 212);
            this.uxHomeLink.Name = "uxHomeLink";
            this.uxHomeLink.Size = new System.Drawing.Size(128, 13);
            this.uxHomeLink.TabIndex = 2;
            this.uxHomeLink.TabStop = true;
            this.uxHomeLink.Text = "SandboxBerry Homepage";
            this.uxHomeLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxHomeLink_LinkClicked);
            // 
            // uxIconsLink
            // 
            this.uxIconsLink.AutoSize = true;
            this.uxIconsLink.Location = new System.Drawing.Point(302, 212);
            this.uxIconsLink.Name = "uxIconsLink";
            this.uxIconsLink.Size = new System.Drawing.Size(77, 13);
            this.uxIconsLink.TabIndex = 3;
            this.uxIconsLink.TabStop = true;
            this.uxIconsLink.Text = "Icons by Icon8";
            this.uxIconsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uxIconsLink_LinkClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 274);
            this.Controls.Add(this.uxIconsLink);
            this.Controls.Add(this.uxHomeLink);
            this.Controls.Add(this.uxOkButton);
            this.Controls.Add(this.uxAboutText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About SandboxBerry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox uxAboutText;
        private System.Windows.Forms.Button uxOkButton;
        private System.Windows.Forms.LinkLabel uxHomeLink;
        private System.Windows.Forms.LinkLabel uxIconsLink;
    }
}