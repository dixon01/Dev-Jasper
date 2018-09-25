// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiSimulationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiSimulationConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;

    /// <summary>
    /// ISI Simulation configuration object.
    /// </summary>
    [Serializable]
    public class IsiSimulationConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the simulation mode is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the file to read in simulation.
        /// </summary>
        public string SimulationFile { get; set; }
    }
}
