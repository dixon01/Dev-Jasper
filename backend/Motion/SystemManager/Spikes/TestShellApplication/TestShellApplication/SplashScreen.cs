// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreen.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestShellApplication
{
    using System.Windows.Forms;

    using Microsoft.Win32;

    /// <summary>
    /// The splash screen.
    /// </summary>
    public partial class SplashScreen : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreen"/> class.
        /// </summary>
        public SplashScreen()
        {
            this.InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.Focus();
        }

        /// <summary>
        /// The on key up.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

            if (e.KeyCode == Keys.F1)
            {
                Application.Exit();
            }

            if (e.KeyCode == Keys.F2)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\WinLogon", true);
                if (key != null)
                {
                    key.SetValue("Shell", string.Empty, RegistryValueKind.String);
                    MessageBox.Show((string)key.GetValue("Shell"));
                }
            }

            if (e.KeyCode == Keys.S)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\WinLogon", false);
                if (key != null)
                {
                    MessageBox.Show((string)key.GetValue("Shell"));
                }
            }
        }
    }
}
