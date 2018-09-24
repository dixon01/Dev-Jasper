// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.ClientGUI
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly PortsTreeModel treeModel;

        private SimplePort posXPort;
        private SimplePort windowStatePort;
        private bool updatingWindowState;

        private PortListener searchListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.treeModel = new PortsTreeModel();
            this.treeViewPorts.Model = this.treeModel;

            this.SetWaiting(false);
        }

        /// <summary>
        /// The on load.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MessageDispatcher.Instance.Configure(
                new FileConfigurator("medi.config", Environment.MachineName, Guid.NewGuid().ToString()));

            this.textBoxNodeSearch.Text = Environment.MachineName;

            this.posXPort = new SimplePort("Window.X", true, false, new IntegerValues(-10000, 10000), this.Left);
            this.localPortsControl1.AddPort(this.posXPort);

            this.windowStatePort = new SimplePort(
                "WindowState", true, true, EnumValues.FromEnum<FormWindowState>(), (int)this.WindowState);
            this.windowStatePort.ValueChanged += this.WindowStatePortOnValueChanged;
            this.localPortsControl1.AddPort(this.windowStatePort);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LocationChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (this.posXPort != null && this.WindowState != FormWindowState.Minimized)
            {
                this.posXPort.IntegerValue = this.Left;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            this.UpdateWindowStatePort();
        }

        private void SetWaiting(bool value)
        {
            this.treeViewPorts.UseWaitCursor = value;

            this.buttonSearch.UseWaitCursor = value;
            this.buttonSearch.Enabled = !value;

            this.textBoxNodeSearch.UseWaitCursor = value;
            this.textBoxNodeSearch.Enabled = !value;
        }

        private void UpdateWindowStatePort()
        {
            if (this.windowStatePort == null)
            {
                return;
            }

            this.updatingWindowState = true;
            try
            {
                this.windowStatePort.IntegerValue = (int)this.WindowState;
            }
            finally
            {
                this.updatingWindowState = false;
            }
        }

        private void WindowStatePortOnValueChanged(object sender, EventArgs eventArgs)
        {
            if (this.updatingWindowState)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.WindowStatePortOnValueChanged));
                return;
            }

            this.WindowState = (FormWindowState)this.windowStatePort.IntegerValue;
        }

        private void TreeViewPortsOnSelectionChanged(object sender, EventArgs e)
        {
            var node = this.treeViewPorts.SelectedNode;
            if (node != null)
            {
                var leaf = node.Tag as PortTreeLeaf;
                if (leaf != null)
                {
                    this.portInfoControl1.Port = leaf.Port;
                    this.portInfoControl1.Visible = true;
                    return;
                }
            }

            this.portInfoControl1.Visible = false;
        }

        private void ButtonSearchOnClick(object sender, EventArgs e)
        {
            var unit = this.textBoxNodeSearch.Text;
            this.SetWaiting(true);
            GioomClient.Instance.BeginFindPorts(
                new MediAddress(unit, "*"), TimeSpan.FromSeconds(3), this.FoundPorts, null);
        }

        private void FoundPorts(IAsyncResult ar)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AsyncCallback(this.FoundPorts), ar);
                return;
            }

            var ports = GioomClient.Instance.EndFindPorts(ar);
            this.treeModel.LoadPorts(ports);
            this.SetWaiting(false);
        }

        private void ButtonSearchPortClick(object sender, EventArgs e)
        {
            if (this.searchListener != null)
            {
                this.searchListener.ValueChanged -= this.SearchListenerOnValueChanged;
                this.searchListener.Dispose();
                this.searchListener = null;
                this.portInfoSearch.Visible = false;
            }

            this.searchListener =
                new PortListener(
                    new MediAddress(this.textBoxSearchUnit.Text, this.textBoxSearchApp.Text),
                    this.textBoxSearchName.Text);
            this.searchListener.ValueChanged += this.SearchListenerOnValueChanged;
            this.searchListener.Start(TimeSpan.FromSeconds(1));
        }

        private void SearchListenerOnValueChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.SearchListenerOnValueChanged));
                return;
            }

            this.portInfoSearch.Port = this.searchListener.Port;
            this.portInfoSearch.Visible = true;
        }
    }
}
