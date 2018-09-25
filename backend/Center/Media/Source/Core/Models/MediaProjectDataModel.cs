// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaProjectDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// The media project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Models.Presentation;

    /// <summary>
    /// Defines the data model for a media project.
    /// </summary>
    [XmlRoot("MediaProject")]
    public class MediaProjectDataModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaProjectDataModel"/> class.
        /// </summary>
        public MediaProjectDataModel()
        {
            this.Authors = new List<string>();
            this.Resources = new List<ResourceInfo>();
            this.Replacements = new List<TextualReplacementDataModel>();
            this.CsvMappings = new List<CsvMappingDataModel>();
            this.Pools = new List<PoolConfigDataModel>();
        }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        /// <value>
        /// The authors.
        /// </value>
        public List<string> Authors { get; set; }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        /// <value>
        /// The authors.
        /// </value>
        public List<TextualReplacementDataModel> Replacements { get; set; }

        /// <summary>
        /// Gets or sets the csv mappings.
        /// </summary>
        public List<CsvMappingDataModel> CsvMappings { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        /// <value>
        /// The date created.
        /// </value>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date last modified.
        /// </summary>
        /// <value>
        /// The date last modified.
        /// </value>
        public DateTime DateLastModified { get; set; }

        /// <summary>
        /// Gets or sets the infomedia config.
        /// </summary>
        /// <value>
        /// The infomedia config.
        /// </value>
        public InfomediaConfigDataModel InfomediaConfig { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>
        /// The project id.
        /// </value>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public List<ResourceInfo> Resources { get; set; }

        /// <summary>
        /// Gets or sets the pools.
        /// </summary>
        /// <value>
        /// The pools.
        /// </value>
        public List<PoolConfigDataModel> Pools { get; set; }

        /// <summary>
        /// Gets or sets the project size in KBytes.
        /// </summary>
        public long ProjectSize { get; set; }
    }
}