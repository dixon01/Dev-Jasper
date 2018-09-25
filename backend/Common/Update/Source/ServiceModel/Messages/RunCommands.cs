// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunCommands.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RunCommands type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The list of files and commands to be copied (temporarily) and executed
    /// before or after the installation.
    /// </summary>
    public class RunCommands
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunCommands"/> class.
        /// </summary>
        public RunCommands()
        {
            this.Items = new List<FileSystemUpdate>();
        }

        /// <summary>
        /// Gets or sets the items that are to be copied to a temporary location and then executed.
        /// </summary>
        [XmlElement("File", typeof(FileUpdate))]
        [XmlElement("Folder", typeof(FolderUpdate))]
        [XmlElement("Executable", typeof(ExecutableFile))]
        [XmlElement("Run", typeof(RunApplication))]
        public List<FileSystemUpdate> Items { get; set; }
    }
}