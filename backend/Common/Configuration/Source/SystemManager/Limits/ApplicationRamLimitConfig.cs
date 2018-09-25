// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationRamLimitConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationRamLimitConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The application RAM limit config.
    /// </summary>
    [Serializable]
    public class ApplicationRamLimitConfig : LimitConfigBase
    {
        /// <summary>
        /// Gets or sets the maximum allowed RAM size of the application in megabytes.
        /// </summary>
        [XmlElement("MaxRamMB")]
        public int MaxRamMb { get; set; }

        /// <summary>
        /// Gets or sets the actions to perform if the limit is reached.
        /// One action at the time is executed and then it is verified if the limit is still reached.
        /// </summary>
        [XmlArrayItem("Relaunch", typeof(RelaunchLimitActionConfig))]
        [XmlArrayItem("Reboot", typeof(RebootLimitActionConfig))]
        public override List<LimitActionConfigBase> Actions { get; set; }
    }
}