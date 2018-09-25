// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleCell.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XimpleCell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Ximple version 2.0 cell.
    /// <remarks>
    /// If used as key in a dictionary, don't change the object after adding it to the dictionary. The hash code is
    /// calculated from all properties, because there is no immutable field.
    /// </remarks>  
    /// </summary>
    [XmlRoot("Cell")]
    public class XimpleCell : ICloneable, IEquatable<XimpleCell>
    {
        private string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleCell"/> class.
        /// </summary>
        public XimpleCell()
        {
            this.Value = string.Empty;
        }

        /// <summary>Initializes a new instance of the <see cref="XimpleCell"/> class.</summary>
        /// <param name="value">The value.</param>
        /// <param name="tableNumber">The table number.</param>
        /// <param name="rowNumber">The row number.</param>
        /// <param name="columnNumber">The column number.</param>
        /// <param name="languageNumber">The language number.</param>
        public XimpleCell(string value, int tableNumber, int rowNumber, int columnNumber, int languageNumber = 0)
        {
            this.value = value;
            this.LanguageNumber = languageNumber;
            this.TableNumber = tableNumber;
            this.ColumnNumber = columnNumber;
            this.RowNumber = rowNumber;
        }

        /// <summary>
        /// Gets or sets the language number of this cell.
        /// </summary>
        [XmlAttribute("Language")]
        public int LanguageNumber { get; set; }

        /// <summary>
        /// Gets or sets the table number of this cell.
        /// </summary>
        [XmlAttribute("Table")]
        public int TableNumber { get; set; }

        /// <summary>
        /// Gets or sets the column number of this cell.
        /// </summary>
        [XmlAttribute("Column")]
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets the row number of this cell.
        /// </summary>
        [XmlAttribute("Row")]
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets or sets the value of this cell.
        /// </summary>
        [XmlText]
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns>New instance of <see cref="XimpleCell"/>.</returns>
        public XimpleCell Clone()
        {
            return new XimpleCell
                {
                    RowNumber = this.RowNumber,
                    ColumnNumber = this.ColumnNumber,
                    LanguageNumber = this.LanguageNumber,
                    TableNumber = this.TableNumber,
                    Value = this.Value
                };
        }

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

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            return this.Equals(obj as XimpleCell);
        }

        /// <summary>
        /// Indicates whether the current objects content is equal to another objects content.
        /// </summary>
        /// <param name="other">The XimpleCell to compare with this object.</param>
        /// <returns>True if the content of the current object is equal to the other object.</returns>
        public bool Equals(XimpleCell other)
        {
            return this.RowNumber == other.RowNumber && this.ColumnNumber == other.ColumnNumber
                   && this.TableNumber == other.TableNumber && this.LanguageNumber == other.LanguageNumber
                   && string.Equals(this.Value, other.Value);
        }

        /// <summary>
        /// Override the object.GetHashCode(). Serves as a hash function for a XimpleCell type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current instance of <see cref="XimpleCell"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.LanguageNumber.GetHashCode() ^ this.TableNumber.GetHashCode() ^ this.ColumnNumber.GetHashCode()
                   ^ this.RowNumber.GetHashCode() ^ this.Value.GetHashCode();
        }

        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns>New instance of <see cref="XimpleCell"/>.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                "Language={0}, Table={1}, Col={2}, Row={3}, Value={4}",
                this.LanguageNumber,
                this.TableNumber,
                this.ColumnNumber,
                this.RowNumber,
                this.Value);
        }
    }
}
