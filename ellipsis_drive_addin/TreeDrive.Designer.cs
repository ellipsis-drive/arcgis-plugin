﻿using Ellipsis.Api;
using Ellipsis.Drive;

namespace ellipsis_drive_addin
{
    partial class TreeDrive
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
            this.username_box = new System.Windows.Forms.TextBox();
            this.password_box = new System.Windows.Forms.TextBox();
            this.username_label = new System.Windows.Forms.Label();
            this.password_label = new System.Windows.Forms.Label();
            this.login_button = new System.Windows.Forms.Button();
            this.tree_drive = new System.Windows.Forms.TreeView();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.browserButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // username_box
            // 
            this.username_box.Location = new System.Drawing.Point(30, 101);
            this.username_box.Name = "username_box";
            this.username_box.Size = new System.Drawing.Size(217, 22);
            this.username_box.TabIndex = 0;
            this.username_box.TextChanged += new System.EventHandler(this.username_box_TextChanged);
            // 
            // password_box
            // 
            this.password_box.Location = new System.Drawing.Point(30, 163);
            this.password_box.Name = "password_box";
            this.password_box.PasswordChar = '*';
            this.password_box.Size = new System.Drawing.Size(217, 22);
            this.password_box.TabIndex = 1;
            this.password_box.TextChanged += new System.EventHandler(this.password_box_TextChanged);
            // 
            // username_label
            // 
            this.username_label.AutoSize = true;
            this.username_label.Location = new System.Drawing.Point(27, 81);
            this.username_label.Name = "username_label";
            this.username_label.Size = new System.Drawing.Size(73, 17);
            this.username_label.TabIndex = 2;
            this.username_label.Text = "Username";
            // 
            // password_label
            // 
            this.password_label.AutoSize = true;
            this.password_label.Location = new System.Drawing.Point(27, 143);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(69, 17);
            this.password_label.TabIndex = 3;
            this.password_label.Text = "Password";
            // 
            // login_button
            // 
            this.login_button.Location = new System.Drawing.Point(202, 257);
            this.login_button.Name = "login_button";
            this.login_button.Size = new System.Drawing.Size(75, 23);
            this.login_button.TabIndex = 4;
            this.login_button.Text = "Login";
            this.login_button.UseVisualStyleBackColor = true;
            this.login_button.Click += new System.EventHandler(this.login_button_Click);
            // 
            // tree_drive
            // 
            this.tree_drive.Location = new System.Drawing.Point(4, 46);
            this.tree_drive.Name = "tree_drive";
            this.tree_drive.Size = new System.Drawing.Size(273, 205);
            this.tree_drive.TabIndex = 5;
            this.tree_drive.Visible = false;
            this.tree_drive.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(4, 18);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(217, 22);
            this.searchBox.TabIndex = 6;
            this.searchBox.Tag = "Search...";
            this.searchBox.Text = "Search...";
            this.searchBox.Visible = false;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            this.searchBox.GotFocus += new System.EventHandler(this.searchBox_GotFocus);
            this.searchBox.LostFocus += new System.EventHandler(this.searchBox_LostFocus);
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(4, 46);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(126, 23);
            this.searchButton.TabIndex = 7;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Visible = false;
            // 
            // browserButton
            // 
            this.browserButton.Enabled = false;
            this.browserButton.Location = new System.Drawing.Point(4, 256);
            this.browserButton.Name = "browserButton";
            this.browserButton.Size = new System.Drawing.Size(165, 23);
            this.browserButton.TabIndex = 8;
            this.browserButton.Text = "Open in browser";
            this.browserButton.UseVisualStyleBackColor = true;
            this.browserButton.Visible = false;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(227, 17);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(30, 23);
            this.stopButton.TabIndex = 9;
            this.stopButton.Text = "x";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Visible = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // TreeDrive
            // 
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.browserButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.login_button);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.username_label);
            this.Controls.Add(this.password_box);
            this.Controls.Add(this.username_box);
            this.Controls.Add(this.tree_drive);
            this.Name = "TreeDrive";
            this.Size = new System.Drawing.Size(300, 300);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox username_box;
        private System.Windows.Forms.TextBox password_box;
        private System.Windows.Forms.Label username_label;
        private System.Windows.Forms.Label password_label;
        private System.Windows.Forms.Button login_button;
        private System.Windows.Forms.TreeView tree_drive;
        private Connect connect;
        private DriveView drive;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button browserButton;
        private System.Windows.Forms.Button stopButton;
    }
}
