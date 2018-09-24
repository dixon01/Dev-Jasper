// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoLogsControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The io logs control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    using Gorba.Motion.Protran.Visualizer.Data;

    /// <summary>
    /// Logs control for IO protocol.
    /// </summary>
    public partial class IoLogsControl : LogsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IoLogsControl"/> class.
        /// </summary>
        public IoLogsControl()
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
            control.IoInputTransformed += this.OnIoInputTransformed;

            control.XimpleCreated += (s, e) => this.AddXimpleEvent(e);
        }

        private void OnIoInputTransformed(object sender, TransformationChainEventArgs e)
        {
            this.AddEvent(EventType.Transform, e.ChainName, e);
        }
    }
}
