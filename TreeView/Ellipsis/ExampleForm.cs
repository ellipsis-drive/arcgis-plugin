using Ellipsis.Api;
using Ellipsis.Drive;
using System;
using System.Windows.Forms;

namespace Ellipsis
{
    public partial class ExampleForm : Form
    {
        private Connect connect = new Connect();
        private DriveView drive = null;

        public ExampleForm()
        {
            InitializeComponent();
        }

        private void ExampleForm_Load(object sender, EventArgs eventArgs)
        {
            loginButton.Click += (s, e) =>
            {
                if (!connect.GetStatus())
                {
                    connect.SetUsername(usernameInput.Text);
                    connect.SetPassword(passwordInput.Text);
                    if (!connect.LoginRequest()) return;
                }

                usernameInput.Hide();
                passwordInput.Hide();
                loginButton.Hide();

                if(drive != null)
                {
                    drive.resetNodes();
                    return;
                }

                //If we reach this, user is logged in and we can load the drive.
                drive = new DriveView(driveContainer, connect, driveIcons, searchInput, refreshButton, openInBrowserButton);
                drive.onLayerClick += (block, layer) => { };
                drive.onTimestampClick += (block, timestamp, visualization, protocol) => { };
            };
        }
    }
}
