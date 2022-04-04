
namespace Ellipsis
{
    partial class ExampleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExampleForm));
            this.usernameInput = new System.Windows.Forms.TextBox();
            this.passwordInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.loginButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.openInBrowserButton = new System.Windows.Forms.Button();
            this.searchInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.driveContainer = new System.Windows.Forms.TreeView();
            this.driveIcons = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // usernameInput
            // 
            this.usernameInput.Location = new System.Drawing.Point(638, 107);
            this.usernameInput.Name = "usernameInput";
            this.usernameInput.Size = new System.Drawing.Size(100, 26);
            this.usernameInput.TabIndex = 0;
            // 
            // passwordInput
            // 
            this.passwordInput.Location = new System.Drawing.Point(638, 161);
            this.passwordInput.Name = "passwordInput";
            this.passwordInput.Size = new System.Drawing.Size(100, 26);
            this.passwordInput.TabIndex = 0;
            this.passwordInput.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(638, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(638, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(638, 193);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(100, 32);
            this.loginButton.TabIndex = 3;
            this.loginButton.Text = "login";
            this.loginButton.UseVisualStyleBackColor = true;
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(17, 47);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(90, 32);
            this.refreshButton.TabIndex = 4;
            this.refreshButton.Text = "refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            // 
            // openInBrowserButton
            // 
            this.openInBrowserButton.Location = new System.Drawing.Point(115, 47);
            this.openInBrowserButton.Name = "openInBrowserButton";
            this.openInBrowserButton.Size = new System.Drawing.Size(131, 32);
            this.openInBrowserButton.TabIndex = 5;
            this.openInBrowserButton.Text = "open in browser";
            this.openInBrowserButton.UseVisualStyleBackColor = true;
            // 
            // searchInput
            // 
            this.searchInput.Location = new System.Drawing.Point(17, 107);
            this.searchInput.Name = "searchInput";
            this.searchInput.Size = new System.Drawing.Size(229, 26);
            this.searchInput.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "search drive";
            // 
            // driveContainer
            // 
            this.driveContainer.Location = new System.Drawing.Point(17, 149);
            this.driveContainer.Name = "driveContainer";
            this.driveContainer.Size = new System.Drawing.Size(229, 246);
            this.driveContainer.TabIndex = 8;
            // 
            // driveIcons
            // 
            this.driveIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("driveIcons.ImageStream")));
            this.driveIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.driveIcons.Images.SetKeyName(0, "folder-removebg-preview.png");
            this.driveIcons.Images.SetKeyName(1, "map-removebg-preview.png");
            this.driveIcons.Images.SetKeyName(2, "shape-removebg-preview.png");
            // 
            // ExampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.driveContainer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.searchInput);
            this.Controls.Add(this.openInBrowserButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.passwordInput);
            this.Controls.Add(this.usernameInput);
            this.Name = "ExampleForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ExampleForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox usernameInput;
        private System.Windows.Forms.TextBox passwordInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button openInBrowserButton;
        private System.Windows.Forms.TextBox searchInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TreeView driveContainer;
        private System.Windows.Forms.ImageList driveIcons;
    }
}