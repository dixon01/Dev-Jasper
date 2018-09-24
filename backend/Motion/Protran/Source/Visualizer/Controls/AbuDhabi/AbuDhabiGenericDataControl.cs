// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiGenericDataControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AbuDhabiGenericDataControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    using System;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.AbuDhabi;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Generic data control for Abu Dhabi protocol.
    /// </summary>
    public partial class AbuDhabiGenericDataControl : GenericDataControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiGenericDataControl"/> class.
        /// </summary>
        public AbuDhabiGenericDataControl()
        {
            this.InitializeComponent();

            try
            {
                var protocol = ServiceLocator.Current.GetInstance<AbuDhabiProtocol>();
                this.genericDataTabControl1.Dictionary = protocol.Dictionary;
                protocol.XimpleCreated += this.ProtocolOnXimpleCreated;
            }
            catch
            {
                // ignore, this only happens in design mode
            }
        }

        private void ProtocolOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<XimpleEventArgs>(this.ProtocolOnXimpleCreated), sender, e);
                return;
            }

            this.genericDataTabControl1.AddXimple(e.Ximple);
        }
    }
}
