// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="Table.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Protocols.Ximple.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    ///     Data table for <see cref="Dictionary" />
    /// </summary>
    [Serializable]
    public class Table : IDictionaryItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        public Table()
        {
            this.Index = 0;
            this.Name = string.Empty;
            this.MultiLanguage = true;
            this.MultiRow = true;
            this.Columns = new List<Column>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the description of the table.
        /// </summary>
        [XmlElement]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the columns in the table.
        /// </summary>
        [XmlElement("Column")]
        public List<Column> Columns { get; set; }

        /// <summary>
        ///     Gets or sets the table index.
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this table supports multiple languages.
        /// </summary>
        [XmlAttribute]
        public bool MultiLanguage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this table can contain multiple rows.
        /// </summary>
        [XmlAttribute]
        public bool MultiRow { get; set; }

        /// <summary>
        ///     Gets or sets the table name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The find column or null if missing.</summary>
        /// <param name="columnNameOrId">The column Name Or Id.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
        /// <exception cref="InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>.-or-The
        ///     source sequence is empty.</exception>
        /// <returns>The <see cref="Column"/>The Column or null.</returns>
        public Column FirstColumn(string columnNameOrId)
        {
            return this.Columns.FirstOrDefault(m => m.Index.ToString() == columnNameOrId || m.Name == columnNameOrId);
        }

        /// <summary>The first or default column.</summary>
        /// <param name="columnNameOrId">The column name or id.</param>
        /// <returns>The <see cref="Column"/>.</returns>
        public Column FirstOrDefaultColumn(string columnNameOrId)
        {
            return this.Columns.FirstOrDefault(m => m.Index.ToString() == columnNameOrId || m.Name == columnNameOrId);
        }

        /// <summary>Gets a generic view column from this table for a given column name or number.
        ///     This checks first if a column exists with the given string in the name
        ///     and then searches for a column with the given number (assuming columnName
        ///     is a number).</summary>
        /// <param name="columnName">The column name. Can be part of the complete name.</param>
        /// <returns>The column. Returns null if the column can't be found.</returns>
        public Column GetColumnForNameOrNumber(string columnName)
        {
            return Dictionary.GetForNameOrNumber(this.Columns, columnName);
        }

        #endregion
    }
}