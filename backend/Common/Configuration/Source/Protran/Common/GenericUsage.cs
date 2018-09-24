// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericUsage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericUsage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Common
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the possible values for the XML TAG
    /// called "UsedFor" in the file ibis.xml.
    /// </summary>
    [Serializable]
    public class GenericUsage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsage"/> class.
        /// </summary>
        public GenericUsage()
        {
            this.Language = "0";
            this.Row = "0";
            this.RowOffset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsage"/> class.
        /// </summary>
        /// <param name="table">
        /// The table name or number.
        /// </param>
        /// <param name="column">
        /// The column name or number.
        /// </param>
        public GenericUsage(string table, string column)
            : this(table, column, "0")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsage"/> class.
        /// </summary>
        /// <param name="table">
        /// The table name or number.
        /// </param>
        /// <param name="column">
        /// The column name or number.
        /// </param>
        /// <param name="row">
        /// The row number.
        /// </param>
        public GenericUsage(string table, string column, string row)
            : this(table, column, row, "0")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsage"/> class.
        /// </summary>
        /// <param name="table">
        /// The table name or number.
        /// </param>
        /// <param name="column">
        /// The column name or number.
        /// </param>
        /// <param name="row">
        /// The row number.
        /// </param>
        /// <param name="language">
        /// The language name or number.
        /// </param>
        public GenericUsage(string table, string column, string row, string language)
        {
            this.Table = table;
            this.Column = column;
            this.Row = row;
            this.Language = language;
        }

        /// <summary>
        /// Gets or sets the language that this telegram updates.
        /// This can either be a name that is looked up in the
        /// dictionary or a number which is then used directly
        /// to address the language.
        /// </summary>
        [XmlAttribute("Language")]
        [DefaultValue("0")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the table that this telegram updates.
        /// This can either be a name that is looked up in the
        /// dictionary or a number which is then used directly
        /// to address the table.
        /// </summary>
        [XmlAttribute("Table")]
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the column that this telegram updates.
        /// This can either be a name that is looked up in the
        /// dictionary or a number which is then used directly
        /// to address the column.
        /// </summary>
        [XmlAttribute("Column")]
        public string Column { get; set; }

        /// <summary>
        /// Gets or sets the row that this telegram updates.
        /// This can either be a number which is then used directly
        /// to address the column or for some telegrams
        /// this can also be the format string {0}
        /// to be replaced by the index of the item.
        /// Default value is 0.
        /// </summary>
        [XmlAttribute("Row")]
        public string Row { get; set; }

        /// <summary>
        /// Gets or sets the offset added to the index for rows.
        /// </summary>
        /// <seealso cref="Row"/>
        [XmlAttribute("RowOffset")]
        [DefaultValue(0)]
        public int RowOffset { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[{0};{1};{2};{3}]", this.Language, this.Table, this.Column, this.Row);
        }
    }
}