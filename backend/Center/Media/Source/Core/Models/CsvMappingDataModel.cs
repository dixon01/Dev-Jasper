// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CsvMappingDataModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines resource information.
    /// </summary>
    [XmlRoot("CsvDataModel")]
    public class CsvMappingDataModel : DataModelBase
    {
        private const string DefaultDelimiter = ";";

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        public string RawContent { get; set; }
    }
}