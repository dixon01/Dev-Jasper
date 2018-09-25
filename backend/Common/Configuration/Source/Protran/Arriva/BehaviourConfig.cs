// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviourConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Arriva
{
    /// <summary>
    /// Arriva connections configuration.
    /// </summary>
    public class BehaviourConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourConfig"/> class.
        /// </summary>
        public BehaviourConfig()
        {
            this.MaxDepartures = 1;
            this.ConnectionsEnabled = false;
            this.ConnectionSource = ConnectionSource.Ftp;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Polling.
        /// </summary>
        public bool ConnectionsEnabled { get; set; }

        /// <summary>
        /// Gets or sets ConnectionSource.
        /// </summary>
        public ConnectionSource ConnectionSource { get; set; }

        /// <summary>
        /// Gets or sets MaxDepartures.
        /// </summary>
        public int MaxDepartures { get; set; }
    }
}
