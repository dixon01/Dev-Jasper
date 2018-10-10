// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSet.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration that tells the Update Agent which application,  folder,
    /// and file needs to be added, deleted or replaced
    /// </summary>
    public class UpdateSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSet"/> class.
        /// </summary>
        public UpdateSet()
        {
            this.Folders = new List<UpdateFolder>();
        }

        /// <summary>
        /// Gets or sets the root folders for the update set.
        /// </summary>
        [XmlElement("Folder")]
        public List<UpdateFolder> Folders { get; set; }
    }
}
