// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpServersDialog.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpServersDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Configuration.Update.Providers;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Dialog for managing FTP servers of the current project.
    /// </summary>
    public partial class FtpServersDialog : Form
    {
        private readonly IProjectManager projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServersDialog"/> class.
        /// </summary>
        public FtpServersDialog()
        {
            this.InitializeComponent();

            this.propertyGrid.PropertyValueChanged += this.PropertyGridOnPropertyValueChanged;

            try
            {
                this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.projectManager == null)
            {
                return;
            }

            this.listBox.Items.Clear();
            foreach (var ftpServer in this.projectManager.CurrentProject.FtpServers)
            {
                this.listBox.Items.Add(new FtpServerInfo(ftpServer));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Closing"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this.projectManager.Save();
        }

        private void PropertyGridOnPropertyValueChanged(object o, PropertyValueChangedEventArgs e)
        {
            this.listBox.Items[this.listBox.SelectedIndex] = this.listBox.Items[this.listBox.SelectedIndex];
        }

        private void AddToolStripButtonOnClick(object sender, EventArgs e)
        {
            var config = new FtpUpdateProviderConfig { RepositoryBasePath = "/Gorba/Update" };
            this.projectManager.CurrentProject.FtpServers.Add(config);
            this.listBox.Items.Add(new FtpServerInfo(config));
            this.listBox.SelectedIndex = this.listBox.Items.Count - 1;
        }

        private void RemoveToolStripButtonOnClick(object sender, EventArgs e)
        {
            var item = this.listBox.SelectedItem as FtpServerInfo;
            if (item == null)
            {
                return;
            }

            this.listBox.Items.Remove(item);
            this.projectManager.CurrentProject.FtpServers.Remove(item.Config);
        }

        private void ListBoxOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var item = this.listBox.SelectedItem as FtpServerInfo;
            if (item == null)
            {
                this.removeToolStripButton.Enabled = false;
                this.propertyGrid.SelectedObject = null;
                return;
            }

            this.removeToolStripButton.Enabled = true;
            this.propertyGrid.SelectedObject = item;
        }

        private class FtpServerInfo
        {
            public FtpServerInfo(FtpUpdateProviderConfig config)
            {
                this.Config = config;
            }

            // ReSharper disable UnusedMember.Local
            [Category("General")]
            [Description("The compression algorithm to be used when uploading files to the FTP server")]
            [DefaultValue(CompressionAlgorithm.None)]
            public CompressionAlgorithm Compression
            {
                get
                {
                    return this.Config.Compression;
                }

                set
                {
                    this.Config.Compression = value;
                }
            }

            [Category("Server")]
            [Description("The host name or IP address of the FTP server")]
            public string Host
            {
                get
                {
                    return this.Config.Host;
                }

                set
                {
                    this.Config.Host = value;
                }
            }

            [Category("Server")]
            [Description("The TCP port of the FTP server (usually 21)")]
            [DefaultValue(21)]
            public int Port
            {
                get
                {
                    return this.Config.Port;
                }

                set
                {
                    this.Config.Port = value;
                }
            }

            [Category("Server")]
            [Description("Login username for the FTP server.")]
            public string Username
            {
                get
                {
                    return this.Config.Username;
                }

                set
                {
                    this.Config.Username = value;
                }
            }

            [Category("Server")]
            [Description("Login password for the FTP server.")]
            [PasswordPropertyText(true)]
            public string Password
            {
                get
                {
                    return this.Config.Password;
                }

                set
                {
                    this.Config.Password = value;
                }
            }

            [Category("Server")]
            [Description("The path to the root of the repository on the FTP server.")]
            public string RepositoryBasePath
            {
                get
                {
                    return this.Config.RepositoryBasePath;
                }

                set
                {
                    this.Config.RepositoryBasePath = value;
                }
            }

            [Browsable(false)]
            public FtpUpdateProviderConfig Config { get; private set; }

            public override string ToString()
            {
                return string.Format("{0}@{1}", this.Config.Username, this.Config.Host);
            }

            // ReSharper restore UnusedMember.Local
        }
    }
}
