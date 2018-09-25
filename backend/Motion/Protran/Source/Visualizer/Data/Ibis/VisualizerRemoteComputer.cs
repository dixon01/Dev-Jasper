// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerRemoteComputer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerRemoteComputer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using Gorba.Motion.Protran.Ibis.Remote;

    /// <summary>
    /// <see cref="RemoteComputer"/> implementation for visualizer.
    /// The <see cref="RemoteComputer.Status"/> of this object always returns
    /// <see cref="RemoteComputerStatus.Active"/>.
    /// </summary>
    public class VisualizerRemoteComputer : RemoteComputer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerRemoteComputer"/> class.
        /// </summary>
        public VisualizerRemoteComputer()
        {
            this.Status = RemoteComputerStatus.Active;
        }
    }
}
