// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviourConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BehaviourConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// General AbuDhabi protocol behavior configuration.
    /// </summary>
    [Serializable]
    public class BehaviourConfig
    {
        /// <summary>
        /// Gets or sets the usage of the connection status (fallback mode) to fill a generic cell.
        /// </summary>
        public GenericUsage ConnectionStatusUsedFor { get; set; }

        /// <summary>
        /// Gets or sets the usage of the cycle to fill a generic cell.
        /// </summary>
        public GenericUsage UsedForCycle { get; set; }
    }
}
