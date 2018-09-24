// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortStateWrapperControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortStateWrapperControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// The port state wrapper control.
    /// </summary>
    public partial class PortStateWrapperControl : UserControl
    {
        private IPort port;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortStateWrapperControl"/> class.
        /// </summary>
        public PortStateWrapperControl()
        {
            this.InitializeComponent();
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
                return this.flagPortStateControl1.IsLocal;
            }

            set
            {
                this.flagPortStateControl1.IsLocal = value;
                this.integerPortStateControl1.IsLocal = value;
                this.enumPortStateControl1.IsLocal = value;
                this.enumFlagPortStateControl1.IsLocal = value;
            }
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

                this.UpdateStatControlVisibility();
            }
        }

        private void UpdateStatControlVisibility()
        {
            this.flagPortStateControl1.Visible = false;
            this.integerPortStateControl1.Visible = false;
            this.enumPortStateControl1.Visible = false;
            this.enumFlagPortStateControl1.Visible = false;

            if (this.port.Info.ValidValues is FlagValues)
            {
                this.flagPortStateControl1.Port = this.port;
                this.flagPortStateControl1.Visible = true;
                return;
            }

            if (this.port.Info.ValidValues is IntegerValues)
            {
                this.integerPortStateControl1.Port = this.port;
                this.integerPortStateControl1.Visible = true;
                return;
            }

            if (this.port.Info.ValidValues is EnumFlagValues)
            {
                this.enumFlagPortStateControl1.Port = this.port;
                this.enumFlagPortStateControl1.Visible = true;
                return;
            }

            if (this.port.Info.ValidValues is EnumValues)
            {
                this.enumPortStateControl1.Port = this.port;
                this.enumPortStateControl1.Visible = true;
            }
        }
    }
}
