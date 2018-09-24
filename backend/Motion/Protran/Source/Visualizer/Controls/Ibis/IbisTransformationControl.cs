// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTransformationControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Transformation control for IBIS transformations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using System;

    using Gorba.Motion.Protran.Visualizer.Data;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    /// <summary>
    /// Transformation control for IBIS transformations.
    /// </summary>
    public partial class IbisTransformationControl : TransformationControl, IIbisVisualizationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTransformationControl"/> class.
        /// </summary>
        public IbisTransformationControl()
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
            controller.TelegramParsing += this.OnTelegramParsing;
            controller.TelegramTransformed += this.OnTelegramTransformed;
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
            this.OnTelegramParsing(this, EventArgs.Empty);
            base.Populate(e);
        }

        /// <summary>
        /// Converts the given transformation input to a string.
        /// </summary>
        /// <param name="value">
        /// The transformation input.
        /// </param>
        /// <returns>
        /// a string representing the provided value.
        /// </returns>
        protected override string InputToString(object value)
        {
            var s = value as string;
            if (s != null)
            {
                return string.Format("\"{0}\"", TelegramFormatter.ToTelegramString(s));
            }

            return base.InputToString(value);
        }

        private void OnTelegramParsing(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(this.OnTelegramParsing));
                return;
            }

            this.Clear();
        }

        private void OnTelegramTransformed(object sender, TransformationChainEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TransformationChainEventArgs>(this.OnTelegramTransformed), sender, e);
                return;
            }

            this.SetTransformations(e.ChainName, e.Transformations);
        }
    }
}
