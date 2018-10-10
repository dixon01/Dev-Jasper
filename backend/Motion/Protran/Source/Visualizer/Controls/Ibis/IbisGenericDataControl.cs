// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisGenericDataControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisGenericDataControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using System;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    /// <summary>
    /// Generic data control for IBIS protocol.
    /// </summary>
    public partial class IbisGenericDataControl : GenericDataControl, IIbisVisualizationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisGenericDataControl"/> class.
        /// </summary>
        public IbisGenericDataControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Configure the control with the given controller.
        /// </summary>
        /// <param name="controller">
        ///   The controller to which you can for example attach event handlers
        /// </param>
        public void Configure(IIbisVisualizationService controller)
        {
            controller.XimpleCreated += this.ControllerOnXimpleCreated;
            this.genericDataTabControl1.Dictionary = controller.Dictionary;
        }

        private void ControllerOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<XimpleEventArgs>(this.ControllerOnXimpleCreated), sender, e);
                return;
            }

            this.genericDataTabControl1.AddXimple(e.Ximple);
        }
    }
}
