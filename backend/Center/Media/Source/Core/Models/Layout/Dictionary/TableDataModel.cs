// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout.Dictionary
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the DataModel for a dictionary Table that can be serialized.
    /// </summary>
    [XmlRoot("Table")]
    [Serializable]
    public class TableDataModel
    {
        /// <summary>
        /// Gets or sets the table index.
        /// </summary>
       [XmlAttribute("Index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this table supports multiple languages.
        /// </summary>
        [XmlAttribute("MultiLanguage")]
        public bool MultiLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this table can contain multiple rows.
        /// </summary>
        [XmlAttribute("MultiRow")]
        public bool MultiRow { get; set; }
    }
}
