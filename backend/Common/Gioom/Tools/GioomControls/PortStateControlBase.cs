// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortStateControlBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortStateControlBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// Base class for all port state controls.
    /// </summary>
    public partial class PortStateControlBase : UserControl
    {
        private IPort port;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortStateControlBase"/> class.
        /// </summary>
        public PortStateControlBase()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the port is local.
        /// A local port is always writable even if <see cref="ReadOnly"/> is set to true.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool IsLocal { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPort Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if (this.port == value)
                {
                    return;
                }

                this.ChangePort(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is read-only.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual bool ReadOnly { get; set; }

        /// <summary>
        /// Changes the currently used port to a new one.
        /// </summary>
        /// <param name="newPort">
        /// The new port.
        /// </param>
        protected virtual void ChangePort(IPort newPort)
        {
            if (this.port != null)
            {
                this.port.ValueChanged -= this.PortOnValueChanged;
            }

            this.port = newPort;
            if (this.port != null)
            {
                this.ReadOnly = !this.port.Info.CanWrite && !this.IsLocal;
                if (this.port.Info.CanRead)
                {
                    this.UpdateValue(this.port.Value);
                    this.port.ValueChanged += this.PortOnValueChanged;
                }
            }
        }

        /// <summary>
        /// Updates this control with the given value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        protected virtual void UpdateValue(IOValue value)
        {
        }

        private void PortOnValueChanged(object sender, EventArgs eventArgs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.PortOnValueChanged));
                return;
            }

            this.UpdateValue(this.port.Value);
        }
    }
}
