// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortInfoControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortInfoControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core;

    /// <summary>
    /// The port info control.
    /// </summary>
    public partial class PortInfoControl : UserControl
    {
        private IPort port;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortInfoControl"/> class.
        /// </summary>
        public PortInfoControl()
        {
            this.InitializeComponent();
        }

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

                this.port = value;

                this.textBoxName.Text = this.port.Info.Name;
                this.textBoxAddress.Text = this.port.Info.Address.ToString();
                this.textBoxAccess.Text = this.GetAccessString();
                this.textBoxValues.Text = this.port.Info.ValidValues.ToString();

                this.portStateWrapperControl1.Port = this.port;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the port is local.
        /// A local port is always writable even if it is read-only (<see cref="IPortInfo.CanWrite"/>).
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool IsLocal
        {
            get
            {
                return this.portStateWrapperControl1.IsLocal;
            }

            set
            {
                this.portStateWrapperControl1.IsLocal = value;
            }
        }

        private string GetAccessString()
        {
            var info = this.port.Info;
            if (info.CanRead && info.CanWrite)
            {
                return "Read / Write";
            }

            if (info.CanRead)
            {
                return "Read only";
            }

            return info.CanWrite ? "Write only" : "None";
        }
    }
}
