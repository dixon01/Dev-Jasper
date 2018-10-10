// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Ports;

    /// <summary>
    /// Main form of this application.
    /// </summary>
    public sealed partial class MainForm : Form
    {
        private readonly string defaultTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            this.defaultTitle = this.Text;

            this.mediAddressEditor1.Address = new MediAddress(Environment.MachineName, Guid.NewGuid().ToString());

            this.treeView1.Nodes.Add(TreeNodeFactory.CreateNode("Services", typeof(List<ServiceConfigBase>)));
            this.treeView1.Nodes.Add(TreeNodeFactory.CreateNode("Peers", typeof(List<PeerConfig>)));
        }

        /// <summary>
        /// Loads the Medi configuration from the given config file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void LoadConfig(string fileName)
        {
            var address = this.mediAddressEditor1.Address;
            var configurator = new FileConfigurator(fileName, address.Unit, address.Application);
            this.tabControl1.TabPages.Remove(this.configurationTabPage);
            this.LoadConfig(configurator);
        }

        private void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.splitContainer1.Panel2.Controls.Clear();

            if (e.Node == null)
            {
                return;
            }

            var control = e.Node.Tag as Control;
            if (control == null)
            {
                return;
            }

            control.Dock = DockStyle.Fill;

            this.splitContainer1.Panel2.Controls.Add(control);
        }

        private void Button1Click(object sender, EventArgs e)
        {
            var config = new MediConfig();
            foreach (TreeNode serviceNode in this.treeView1.Nodes[0].Nodes)
            {
                var cfg = this.CreateConfig<ServiceConfigBase>(serviceNode);
                if (cfg != null)
                {
                    config.Services.Add(cfg);
                }
            }

            foreach (TreeNode peerNode in this.treeView1.Nodes[1].Nodes)
            {
                var cfg = this.CreateConfig<PeerConfig>(peerNode);
                if (cfg != null)
                {
                    config.Peers.Add(cfg);
                }
            }

            var address = this.mediAddressEditor1.Address;
            var configurator = new ObjectConfigurator(config, address.Unit, address.Application);

            this.LoadConfig(configurator);
        }

        private void LoadConfig(IConfigurator configurator)
        {
            MessageDispatcher.Instance.Configure(configurator);
            this.Text = string.Format("{0} [{1}]", this.defaultTitle, MessageDispatcher.Instance.LocalAddress);
            this.tabControl1.SelectedTab = this.messageDispatcherTabPage;

            this.AddTabPage("Logging", new LoggingView());

            if (MessageDispatcher.Instance.GetService<IResourceService>() != null)
            {
                this.AddTabPage("Resources", new ResourcesView());
            }

            if (MessageDispatcher.Instance.GetService<IPortForwardingService>() != null)
            {
                this.AddTabPage("Port Forwarding", new PortForwardingView());
            }
        }

        private void AddTabPage(string name, Control control)
        {
            var page = new TabPage(name);
            control.Dock = DockStyle.Fill;
            page.Controls.Add(control);
            this.tabControl1.TabPages.Add(page);
        }

        private T CreateConfig<T>(TreeNode node) where T : class
        {
            var editor = node.Tag as PeerStackConfigEditor;
            if (editor == null)
            {
                var propertyGrid = node.Tag as PropertyGrid;
                if (propertyGrid == null)
                {
                    return null;
                }

                return propertyGrid.SelectedObject as T;
            }

            return editor.Config as T;
        }
    }
}
