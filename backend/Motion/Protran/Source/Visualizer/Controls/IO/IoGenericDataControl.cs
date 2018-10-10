// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoGenericDataControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IoGenericDataControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    using System;

    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Generic data control for IO Protocol.
    /// </summary>
    public partial class IoGenericDataControl : GenericDataControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IoGenericDataControl"/> class.
        /// </summary>
        public IoGenericDataControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public void Configure(Data.IO.IIOVisualizationService control)
        {
            control.XimpleCreated += this.ControllerOnXimpleCreated;
            this.genericDataTabControl1.Dictionary = control.Dictionary;
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
