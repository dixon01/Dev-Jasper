// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoTransformationControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The io transformation control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    using System;

    using Gorba.Motion.Protran.Visualizer.Data;
    using Gorba.Motion.Protran.Visualizer.Data.IO;

    /// <summary>
    /// Transformation control for IO Protocol.
    /// </summary>
    public partial class IoTransformationControl : TransformationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IoTransformationControl"/> class.
        /// </summary>
        public IoTransformationControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public void Configure(IIOVisualizationService control)
        {
            control.IoInputTransforming += this.OnIoInputTransforming;
            control.IoInputTransformed += this.OnIoTransformed;
        }

        private void OnIoInputTransforming(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.OnIoInputTransforming));
                return;
            }

            this.Clear();
        }

        private void OnIoTransformed(object sender, TransformationChainEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TransformationChainEventArgs>(this.OnIoTransformed), sender, e);
                return;
            }

            this.SetTransformations(e.ChainName, e.Transformations);
        }
    }
}
