// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CpuLimitConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CpuLimitConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The CPU limit config.
    /// </summary>
    [Serializable]
    public class CpuLimitConfig : LimitConfigBase
    {
        /// <summary>
        /// Gets or sets the maximum allowed CPU usage percentage of the application.
        /// </summary>
        public int MaxCpuPercentage { get; set; }

        /// <summary>
        /// Gets or sets the actions to perform if the limit is reached.
        /// One action at the time is executed and then it is verified if the limit is still reached.
        /// </summary>
        [XmlArrayItem("Relaunch", typeof(RelaunchLimitActionConfig))]
        [XmlArrayItem("Reboot", typeof(RebootLimitActionConfig))]
        public override List<LimitActionConfigBase> Actions { get; set; }
    }
}