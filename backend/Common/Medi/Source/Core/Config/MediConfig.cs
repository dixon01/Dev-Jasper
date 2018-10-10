// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Event Dispatcher configuration.
    /// This class is XML Serializable
    /// </summary>
    public class MediConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediConfig"/> class.
        /// </summary>
        public MediConfig()
        {
            this.InterceptLocalLogs = true;
            this.Peers = new List<PeerConfig>();
            this.Services = new List<ServiceConfigBase>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether Medi should intercept local log messages,
        /// so it can forward them to remote peers when requested.
        /// </summary>
        [DefaultValue(true)]
        public bool InterceptLocalLogs { get; set; }

        /// <summary>
        /// Gets or sets the list of peer configurations.
        /// </summary>
        public List<PeerConfig> Peers { get; set; }

        /// <summary>
        /// Gets or sets the list of service configurations.
        /// </summary>
        public List<ServiceConfigBase> Services { get; set; }
    }
}
