// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout.Dictionary
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the DataModel of a dictionary column that can be serialized.
    /// </summary>
    [XmlRoot("Column")]
    [Serializable]
    public class ColumnDataModel
    {
        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [XmlAttribute("Index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table reference name
        /// </summary>
        [XmlElement("Table")]
        public TableDataModel Table { get; set; }
    }
}
