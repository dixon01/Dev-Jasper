// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericCoordinate.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericCoordinate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Common
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Generic coordinate used in layout definitions.
    /// </summary>
    [Serializable]
    public class GenericCoordinate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCoordinate"/> class.
        /// </summary>
        public GenericCoordinate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCoordinate"/> class.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="row">
        /// The row.
        /// </param>
        public GenericCoordinate(int language, int table, int column, int row)
        {
            this.Language = language;
            this.Table = table;
            this.Column = column;
            this.Row = row;
        }

        /// <summary>
        /// Gets or sets the language index of the generic data layer.
        /// </summary>
        [XmlAttribute("Lang")]
        public int Language { get; set; }

        /// <summary>
        /// Gets or sets the table index of the generic data layer.
        /// </summary>
        [XmlAttribute("Table")]
        public int Table { get; set; }

        /// <summary>
        /// Gets or sets the column index of the generic data layer.
        /// </summary>
        [XmlAttribute("Column")]
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the row index of the generic data layer.
        /// </summary>
        [XmlAttribute("Row")]
        public int Row { get; set; }

        /// <summary>
        /// Override object.Equals. Indicates whether the current object is equal to another object.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with this object.
        /// </param>
        /// <returns>
        /// true if the current object is equal to the object parameter; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return this.Equals(obj as GenericCoordinate);
        }

        /// <summary>
        /// Indicates whether the current objects content is equal to another objects content.
        /// </summary>
        /// <param name="other">The XimpleCell to compare with this object.</param>
        /// <returns>True if the content of the current object is equal to the other object.</returns>
        public bool Equals(GenericCoordinate other)
        {
            return this.Row == other.Row && this.Column == other.Column && this.Table == other.Table
                   && this.Language == other.Language;
        }

        /// <summary>
        /// Override the object.GetHashCode(). Serves as a hash function for a XimpleCell type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current instance of <see cref="GenericCoordinate"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Table.GetHashCode() ^ this.Column.GetHashCode() ^ this.Row.GetHashCode()
                   ^ this.Language.GetHashCode();
        }
    }
}