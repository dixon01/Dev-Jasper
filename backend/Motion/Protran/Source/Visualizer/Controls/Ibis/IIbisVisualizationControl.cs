// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIbisVisualizationControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIbisVisualizationControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Ibis
{
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;

    /// <summary>
    /// Interface to be implemented by special controls that are used in 
    /// Ibis Protran Visualizer.
    /// </summary>
    public interface IIbisVisualizationControl : IVisualizationControl
    {
        /// <summary>
        /// Configure the control with the given controller.
        /// </summary>
        /// <param name="controller">
        ///   The controller to which you can for example attach event handlers
        /// </param>
        void Configure(IIbisVisualizationService controller);
    }
}
