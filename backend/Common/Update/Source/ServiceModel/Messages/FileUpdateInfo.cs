// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUpdateInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileUpdateInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Update state information of a file.
    /// </summary>
    [Serializable]
    public class FileUpdateInfo : FileSystemUpdateInfo
    {
        /// <summary>
        /// Gets or sets the MD5 hash of the file found in the directory.
        /// </summary>
        [XmlAttribute]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the expected MD5 hash of the resource that should have been
        /// copied to the given file name.
        /// </summary>
        [XmlAttribute]
        public string ExpectedHash { get; set; }
    }
}