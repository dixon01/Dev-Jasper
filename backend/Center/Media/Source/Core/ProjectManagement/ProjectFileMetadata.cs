// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectFileMetaData.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectFileMetaData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Models;

    /// <summary>
    /// The project file meta data.
    /// This class is not to be used outside this namespace, it is only public to support XML serialization.
    /// </summary>
    [XmlRoot("ProjectFileMetadata")]
    public class ProjectFileMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectFileMetadata"/> class.
        /// </summary>
        public ProjectFileMetadata()
        {
            this.Resources = new List<ResourceMetadata>();
        }

        /// <summary>
        /// Gets or sets the <see cref="MediaProject"/> object.
        /// </summary>
        /// <value>
        /// The <see cref="MediaProject"/> object.
        /// </value>
        public MediaProjectDataModel MediaProject { get; set; }

        /// <summary>
        /// Gets or sets the list of resources.
        /// </summary>
        public List<ResourceMetadata> Resources { get; set; }
    }
}
