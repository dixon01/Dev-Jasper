// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVdv301VisualizationControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IVdv301VisualizationControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    /// <summary>
    /// Interface to be implemented by special controls that are used in
    /// VDV 301 Protran Visualizer.
    /// </summary>
    public interface IVdv301VisualizationControl : IVisualizationControl
    {
        /// <summary>
        /// Configure the control with the given service.
        /// </summary>
        /// <param name="service">
        /// The service to which you can for example attach event handlers
        /// </param>
        void Configure(IVdv301VisualizationService service);
    }
}
