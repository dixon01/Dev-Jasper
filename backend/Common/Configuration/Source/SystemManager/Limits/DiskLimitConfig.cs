// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskLimitConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DiskLimitConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The disk limit config.
    /// </summary>
    [Serializable]
    public class DiskLimitConfig : LimitConfigBase
    {
        /// <summary>
        /// Gets or sets the path to observe.
        /// This should always be the root of a drive (e.g. "A:\")
        /// </summary>
        [XmlAttribute]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the minimum required free space on the disk in megabytes.
        /// </summary>
        [XmlElement("FreeSpaceMB")]
        public int? FreeSpaceMb { get; set; }

        /// <summary>
        /// Gets or sets the minimum required free space on the disk in percent of the disk size.
        /// </summary>
        public int? FreeSpacePercentage { get; set; }

        /// <summary>
        /// Gets or sets the actions to perform if the limit is reached.
        /// One action at the time is executed and then it is verified if the limit is still reached.
        /// </summary>
        [XmlArrayItem("Relaunch", typeof(RelaunchLimitActionConfig))]
        [XmlArrayItem("Reboot", typeof(RebootLimitActionConfig))]
        [XmlArrayItem("Purge", typeof(PurgeLimitActionConfig))]
        public override List<LimitActionConfigBase> Actions { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="FreeSpaceMb"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeFreeSpaceMb()
        {
            return this.FreeSpaceMb.HasValue;
        }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="FreeSpacePercentage"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeFreeSpacePercentage()
        {
            return this.FreeSpacePercentage.HasValue;
        }
    }
}