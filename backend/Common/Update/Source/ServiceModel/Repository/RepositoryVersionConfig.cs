// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryVersionConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RepositoryVersionConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Repository
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Common;

    /// <summary>
    /// Repository configuration for a specific range of update versions.
    /// </summary>
    public class RepositoryVersionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryVersionConfig"/> class.
        /// </summary>
        public RepositoryVersionConfig()
        {
            this.Compression = CompressionAlgorithm.None;
        }

        /// <summary>
        /// Gets or sets the version from which this update is valid.
        /// If this is null, this config is valid for all versions.
        /// </summary>
        [XmlIgnore]
        public Version ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ValidFrom"/> as an XML serializable string.
        /// </summary>
        [XmlAttribute("ValidFrom")]
        public string ValidFromString
        {
            get
            {
                return this.ValidFrom == null ? null : this.ValidFrom.ToString();
            }

            set
            {
                this.ValidFrom = value == null ? null : new Version(value);
            }
        }

        /// <summary>
        /// Gets or sets the compression algorithm to be used in all directories.
        /// Not all kinds of Update Clients and/or Providers might support all compression modes.
        /// </summary>
        public CompressionAlgorithm Compression { get; set; }

        /// <summary>
        /// Gets or sets the name resource directory.
        /// Inside the resource directory, all resources are stored with the
        /// extension ".rx".
        /// </summary>
        public string ResourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the commands directory.
        /// Inside the commands directory, there is a directory for each unit which is named
        /// after the unit name; the unit directory then contains all commands for the given
        /// unit with the extension <code>.guc</code>.
        /// </summary>
        public string CommandsDirectory { get; set; }

        /// <summary>
        /// Gets or sets the feedback directory.
        /// Inside the feedback directory, there is a directory for each unit which is named
        /// after the unit name; the unit directory then contains all feedback from the given
        /// unit with the extension <code>.guf</code> as well as all log files with the
        /// extension <code>.log</code>.
        /// </summary>
        public string FeedbackDirectory { get; set; }
        
        /// <summary>
        /// Gets or sets the uploads directory.
        /// Suggested structure: Inside the uploads directory, there is a subdirectory for the type of upload (Proof of play, Misc Upload, Whatever)
        /// Beneath the upload type directory, there is another subdirectory for the display, and inside that are the files to upload.
        /// Technically, all files and folders are upload, the client doesn't care about the folder structure. Only the server apps
        /// really care.
        /// </summary>
        public string UploadsDirectory { get; set; }
    }
}