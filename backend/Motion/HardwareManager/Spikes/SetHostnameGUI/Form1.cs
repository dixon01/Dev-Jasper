using System;
using System.Windows.Forms;

namespace SetHostnameGUI
{
    using System.Management;

    public partial class Form1 : Form
    {
        public Form1()
        {
            this.InitializeComponent();
        }

        private void SetHostname(string hostname)
        {
            // Invoke WMI to populate the machine name
            var query = string.Format("Win32_ComputerSystem.Name='{0}'", Environment.MachineName);
            using (var wmiObject = new ManagementObject(new ManagementPath(query)))
            {
                var inputArgs = wmiObject.GetMethodParameters("Rename");
                inputArgs["Name"] = hostname;

                // Set the name
                var outParams = wmiObject.InvokeMethod("Rename", inputArgs, null);
                if (outParams == null)
                {
                    return;
                }

                var ret = (uint)outParams.Properties["ReturnValue"].Value;
                MessageBox.Show(this, "Win32_ComputerSystem.Rename() returned " + ret);
            }
        }


        private void ButtonReloadClick(object sender, EventArgs e)
        {
            this.textBoxHostname.Text = Environment.MachineName;
        }

        private void ButtonSetHostnameClick(object sender, EventArgs e)
        {
            this.SetHostname(this.textBoxHostname.Text);
        }

        private void Form1OnLoad(object sender, EventArgs e)
        {
            this.textBoxHostname.Text = Environment.MachineName;
        }
    }
}
