using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TreeView
{
    [Guid("87a5b373-fdef-4810-90c5-38b587ab90b2")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("TreeView.DockableWindow1")]
    public partial class DockableWindow1 : UserControl, IDockableWindowDef
    {
        private IApplication m_application;

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Unregister(regKey);

        }

        #endregion
        #endregion

        public DockableWindow1()
        {
            InitializeComponent();
            this.treeView1.Hide();
        }

        private void GetFolders(string node_name, TreeNode node)
        {
            string folder_name = this.connect.GetPath("path/listRoot", "base");
            TreeNode new_node = node.Nodes.Add(folder_name);
        }

        #region IDockableWindowDef Members

        string IDockableWindowDef.Caption
        {
            get
            {
                //TODO: Replace with locale-based initial title bar caption
                return "Ellipsis Drive";
            }
        }

        int IDockableWindowDef.ChildHWND
        {
            get { return this.Handle.ToInt32(); }
        }

        string IDockableWindowDef.Name
        {
            get
            {
                //TODO: Replace with any non-localizable string
                return this.Name;
            }
        }

        void IDockableWindowDef.OnCreate(object hook)
        {
            m_application = hook as IApplication;
        }

        void IDockableWindowDef.OnDestroy()
        {
            //TODO: Release resources and call dispose of any ActiveX control initialized
        }

        object IDockableWindowDef.UserData
        {
            get { return null; }
        }

        #endregion
        private void SearchDirectory(DirectoryInfo directory, TreeNode node)
        {
            DirectoryInfo[] directories = default;
            FileInfo[] files = default;
            try
            {
                directories = directory.GetDirectories();
            }
            catch (Exception e)
            {
                // crash, burn 
            }
            if (directories != null)
            {
                foreach (var subdirectory in directories)
                {
                    TreeNode new_node = node.Nodes.Add(subdirectory.Name);
                    SearchDirectory(subdirectory, new_node);
                }
            }
            try
            {
                files = directory.GetFiles();
            }
            catch(Exception e)
            {
                // crash, burn
            }
            if (files != null)
                foreach (var file in files)
                    node.Nodes.Add(file.Name);
        }
        

        private bool once = true;
        /*
         * TODO: 
         * - Connect to personal drive instead of dummy directory
         */
        private void button1_Click(object sender, EventArgs e)
        {
            bool login = false ;
            if (this.connect != null && this.connect.GetStatus() == false)
                login = this.connect.LoginRequest();

            if (login == true)
            {
                // Hide instead of remove ....
                this.SuspendLayout();
                this.Controls.Remove(this.textBox1);
                this.Controls.Remove(this.textBox2);
                this.Controls.Remove(this.label1);
                this.Controls.Remove(this.label2);
                this.treeView1.Show();
                this.ResumeLayout(false);
                this.PerformLayout();


                if (once == true)
                {
                    // Prepare data
                    //var directory = new DirectoryInfo("C:/Users/ROCVA/Desktop");
                    TreeNode node = treeView1.Nodes.Add("myMaps");
                    TreeNode node1 = treeView1.Nodes.Add("shared");
                    TreeNode node2 = treeView1.Nodes.Add("favorites");
                    //SearchDirectory(directory, node);
                    GetFolders("myMaps", node);
                    //SearchDirectory(directory, node1);
                    //SearchDirectory(directory, node2);
                    once = false;
                }
            }
            else
            {
                this.textBox1.Text = "";
                this.textBox2.Text = "";
                this.connect.SetUsername("");
                this.connect.SetPassword("");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.connect != null)
                this.connect.SetUsername(this.textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (this.connect != null)
                this.connect.SetPassword(this.textBox2.Text);
        }
    }

}
