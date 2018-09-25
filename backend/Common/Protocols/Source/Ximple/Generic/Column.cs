// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Column.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Generic
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Data column for <see cref="Dictionary"/>
    /// </summary>
    [Serializable]
    public class Column : IDictionaryItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Column" /> class.
        /// </summary>
        public Column()
        {
            this.Index = 0;
            this.Name = string.Empty;
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the column.
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }
    }
}
