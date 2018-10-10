// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalPortsControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalPortsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// The local ports control.
    /// </summary>
    public partial class LocalPortsControl : UserControl
    {
        private readonly BindingList<PortWrapper> model = new BindingList<PortWrapper>();
        private int portCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalPortsControl"/> class.
        /// </summary>
        public LocalPortsControl()
        {
            this.InitializeComponent();

            this.listBoxPorts.DataSource = this.model;
            this.listBoxPorts.DisplayMember = "Name";
        }

        /// <summary>
        /// Adds a new port to this control.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        public void AddPort(IPort port)
        {
            GioomClient.Instance.RegisterPort(port);
            this.model.Add(new PortWrapper(port));
        }

        private void AddEnumPort<T>(bool readOnly, T initialValue)
            where T : struct, IConvertible
        {
            this.AddPort(readOnly, EnumValues.FromEnum<T>(), initialValue.ToInt32(null));
        }

        private void AddPort(bool readOnly, ValuesBase values, int initialValue)
        {
            this.AddPort(new SimplePort("Port" + this.portCounter++, true, !readOnly, values, initialValue));
        }

        private void ToolStripMenuItemAddFlagReadOnlyOnClick(object sender, EventArgs e)
        {
            this.AddPort(true, new FlagValues(), 0);
        }

        private void ToolStripMenuItemAddFlagReadWriteOnClick(object sender, EventArgs e)
        {
            this.AddPort(false, new FlagValues(), 0);
        }

        private void ToolStripButtonRemoveOnClick(object sender, EventArgs e)
        {
            var port = this.listBoxPorts.SelectedItem as PortWrapper;
            if (port != null)
            {
                this.model.Remove(port);
                GioomClient.Instance.DeregisterPort(port.Port);
            }
        }

        private void ListBoxPortsOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var port = this.listBoxPorts.SelectedItem as PortWrapper;
            if (port == null)
            {
                this.portInfoControl.Visible = false;
                this.toolStripButtonRemove.Enabled = false;
                return;
            }

            this.portInfoControl.Port = port.Port;
            this.portInfoControl.Visible = true;
            this.toolStripButtonRemove.Enabled = true;
        }

        private void ToolStripMenuItemAddIntReadWriteOnClick(object sender, EventArgs e)
        {
            this.AddPort(false, new IntegerValues(0, 20), 15);
        }

        private void ToolStripMenuItemAddIntReadOnlyOnClick(object sender, EventArgs e)
        {
            this.AddPort(false, new IntegerValues(-100, 100), -5);
        }

        private void ToolStripMenuItemAddEnumReadWriteOnClick(object sender, EventArgs e)
        {
            this.AddEnumPort(false, Process.GetCurrentProcess().PriorityClass);
        }

        private void ToolStripMenuItemAddEnumReadOnlyOnClick(object sender, EventArgs e)
        {
            this.AddEnumPort(true, DateTime.Now.DayOfWeek);
        }

        private void ToolStripMenuItemAddEnumFlagsReadOnlyOnClick(object sender, EventArgs e)
        {
            this.AddEnumPort(true, FileAttributes.Normal);
        }

        private void ToolStripMenuItemAddEnumFlagsReadWriteOnClick(object sender, EventArgs e)
        {
            this.AddEnumPort(false, FontStyle.Bold);
        }

        private class PortWrapper
        {
            public PortWrapper(IPort port)
            {
                this.Port = port;
            }

            public IPort Port { get; private set; }

            // ReSharper disable UnusedMember.Local
            public string Name
            {
                get
                {
                    return this.Port.Info.Name;
                }
            }

            // ReSharper restore UnusedMember.Local
        }
    }
}
