// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301TransformationControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301TransformationControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using Gorba.Motion.Protran.Visualizer.Data;

    /// <summary>
    /// The VDV 301 control that shows a single transformation.
    /// </summary>
    public partial class Vdv301TransformationControl : TransformationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301TransformationControl"/> class.
        /// </summary>
        public Vdv301TransformationControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates this control with the given event args.
        /// This is used to fill the control when shown from the log view.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public override void Populate(TransformationChainEventArgs e)
        {
            this.Clear();
            if (e != null)
            {
                base.Populate(e);
            }
        }
    }
}
