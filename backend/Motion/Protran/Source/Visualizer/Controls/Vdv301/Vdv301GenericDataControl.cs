// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301GenericDataControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The vdv 301 generic data control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using System;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    /// <summary>
    /// The VDV 301 generic data control.
    /// </summary>
    public partial class Vdv301GenericDataControl : GenericDataControl, IVdv301VisualizationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301GenericDataControl"/> class.
        /// </summary>
        public Vdv301GenericDataControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Configure the control with the given service.
        /// </summary>
        /// <param name="service">
        /// The service to which you can for example attach event handlers
        /// </param>
        public void Configure(IVdv301VisualizationService service)
        {
            service.XimpleCreated += this.ServiceOnXimpleCreated;
            this.genericDataTabControl1.Dictionary = service.Dictionary;
        }

        private void ServiceOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<XimpleEventArgs>(this.ServiceOnXimpleCreated), sender, e);
                return;
            }

            this.genericDataTabControl1.AddXimple(e.Ximple);
        }
    }
}
