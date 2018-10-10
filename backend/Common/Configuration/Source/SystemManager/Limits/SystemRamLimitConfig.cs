// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemRamLimitConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemRamLimitConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The system RAM limit config.
    /// </summary>
    [Serializable]
    public class SystemRamLimitConfig : LimitConfigBase
    {
        /// <summary>
        /// Gets or sets the minimum allowed available RAM in megabytes.
        /// </summary>
        [XmlElement("FreeRamMB")]
        public int? FreeRamMb { get; set; }

        /// <summary>
        /// Gets or sets the minimum allowed available RAM in percent of the total RAM in the system.
        /// </summary>
        public int? FreeRamPercentage { get; set; }

        /// <summary>
        /// Gets or sets the actions to perform if the limit is reached.
        /// One action at the time is executed and then it is verified if the limit is still reached.
        /// </summary>
        [XmlArrayItem("Relaunch", typeof(RelaunchLimitActionConfig))]
        [XmlArrayItem("Reboot", typeof(RebootLimitActionConfig))]
        public override List<LimitActionConfigBase> Actions { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="FreeRamMb"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeFreeRamMb()
        {
            return this.FreeRamMb.HasValue;
        }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="FreeRamPercentage"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeFreeRamPercentage()
        {
            return this.FreeRamPercentage.HasValue;
        }
    }
}