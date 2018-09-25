// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortTreeLeaf.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortTreeLeaf type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.ClientGUI
{
    using System;

    using Gorba.Common.Gioom.Core;

    /// <summary>
    /// A leaf (terminal node) of the <see cref="PortsTreeModel"/>.
    /// </summary>
    public class PortTreeLeaf : PortTreeNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortTreeLeaf"/> class.
        /// </summary>
        /// <param name="portInfo">
        /// The port info.
        /// </param>
        public PortTreeLeaf(IPortInfo portInfo)
            : base(portInfo.Name)
        {
            this.Port = GioomClient.Instance.OpenPort(portInfo);
            this.Port.ValueChanged += this.PortOnValueChanged;
        }

        /// <summary>
        /// Event that is risen if <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets the port.
        /// </summary>
        public IPort Port { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value
        {
            get
            {
                return this.Port.Info.CanRead ? this.Port.Value.Name : "n/a";
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.Port.ValueChanged -= this.PortOnValueChanged;
            this.Port.Dispose();
        }

        private void PortOnValueChanged(object sender, EventArgs e)
        {
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}