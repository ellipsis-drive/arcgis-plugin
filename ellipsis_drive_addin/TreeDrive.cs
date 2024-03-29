﻿//using ESRI.ArcGIS.ADF.CATIDs;
using System;
using System.Windows.Forms;
using Ellipsis.Drive;
using Ellipsis.Api;
//using Aspose.Svg;

namespace ellipsis_drive_addin
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class TreeDrive : UserControl
    {
        public TreeDrive(object hook)
        {
            InitializeComponent();
            connect = new Connect();
            //var svgDoc = SVGDocument.Open(imagePath);
            drive = new DriveView(tree_drive, connect, null, null, null, browserButton);
            this.Hook = hook;
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private TreeDrive m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new TreeDrive(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void password_box_TextChanged(object sender, EventArgs e)
        {
            if (this.connect != null)
                this.connect.SetPassword(this.password_box.Text);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void login_button_Click(object sender, EventArgs e)
        {
            bool login = false;
            if (connect != null && this.connect.GetStatus() == false)
                login = this.connect.LoginRequest();
            else if (connect != null && this.connect.GetStatus() == true)
                login = this.connect.LogoutRequest();
            else if (connect == null || (!connect.GetStatus() && !connect.LoginRequest()))
            {
                username_box.Text = "";
                password_box.Text = "";
                connect.SetUsername("");
                connect.SetPassword("");
            }
            if (login == false)
            {
                username_box.Text = "";
                password_box.Text = "";
                connect.SetUsername("");
                connect.SetPassword("");
                this.SuspendLayout();
                this.tree_drive.Hide();
                searchBox.Visible = false;
                searchButton.Visible = false;
                browserButton.Visible = false;
                browserButton.Enabled = false;
                this.username_box.Show();
                this.password_box.Show();
                this.username_label.Show();
                this.password_label.Show();
                this.stopButton.Hide();
                this.login_button.Text = "Login";
                this.ResumeLayout(false);
                this.PerformLayout();
            }
            if (login == true)
            {
                // Hide instead of remove ....
                this.SuspendLayout();
                this.username_box.Hide();
                this.password_box.Hide();
                this.username_label.Hide();
                this.password_label.Hide();
                this.login_button.Text = "Logout";
                this.tree_drive.Show();
                this.stopButton.Show();
                searchBox.Visible = true;
                browserButton.Visible = true;
                //searchButton.Visible = true;
                this.ResumeLayout(false);
                this.PerformLayout();

                if (drive != null)
                {
                    //If the drive view was already loaded before, it's nice to
                    //reset the nodes for the user when it's shown again.
                    drive.resetNodes();
                    return;
                }

                //TODO add textbox for searching, refreshbutton and 'open in browser'-button.
                drive = new DriveView(tree_drive, connect, null, null, null, null);

                //Quick example of how to listen for events.
                drive.onLayerClick += (block, layer) => { };
                drive.onTimestampClick += (block, timestamp, visualization, protocol) => { };
            }
        }

        private void username_box_TextChanged(object sender, EventArgs e)
        {
            if (this.connect != null)
                this.connect.SetUsername(this.username_box.Text);
        }

        private void searchBox_GotFocus(object sender, EventArgs e)
        {
            if (this.searchBox.Text == "Search...")
                this.searchBox.Text = "";
        }

        private void searchBox_LostFocus(object sender, EventArgs e)
        {
            if (this.searchBox.Text == "")
                this.searchBox.Text = "Search...";
        }
        // Strategy:
        // If multiple matches, unfold all matches
        // If one match, fold all previous matches and select match
        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text == "")
                this.stopSearch();
            else if (searchBox.Text != "Search...")
            {
                searchButton.Text = "Stop search";
                drive.alterSearchText(searchBox.Text);
            }
        }

        private void stopSearch()
        {
            drive.alterSearchText("");
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (searchBox.Text != "Search...")
                searchBox.Text = "";
            drive.alterSearchText("");
        }
    }
}
