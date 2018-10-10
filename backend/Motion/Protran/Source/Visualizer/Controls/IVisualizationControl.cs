// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVisualizationControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IVisualizationControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    /// <summary>
    /// Interface to be implemented by special controls that are used in 
    /// Protran Visualizer.
    /// </summary>
    public interface IVisualizationControl
    {
        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="sideTab">
        /// The side tab.
        /// </param>
        void SetSideTab(SideTab sideTab);
    }
}
