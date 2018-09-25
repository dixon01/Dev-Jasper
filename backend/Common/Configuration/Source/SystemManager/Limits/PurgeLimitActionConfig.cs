// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurgeLimitActionConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PurgeLimitActionConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Limits
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Action performed if a given limit is reached: purge the given directory and/or files.
    /// </summary>
    [Serializable]
    public class PurgeLimitActionConfig : LimitActionConfigBase
    {
        /// <summary>
        /// Gets or sets the path to be purged.
        /// This can either be a directory (which would be emptied) or
        /// a file pattern for files to delete or
        /// a file name to delete.
        /// </summary>
        [XmlAttribute]
        public string Path { get; set; }
    }
}