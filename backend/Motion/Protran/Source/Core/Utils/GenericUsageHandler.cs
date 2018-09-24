// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericUsageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericUsageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Utils
{
    using System;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    using Dictionary = Gorba.Common.Protocols.Ximple.Generic.Dictionary;

    /// <summary>
    /// Handler that takes a <see cref="GenericUsage"/> and a <see cref="Dictionary"/>
    /// and then creates <see cref="XimpleCell"/>s for given values.
    /// </summary>
    public class GenericUsageHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<GenericUsageHandler>();

        private readonly GenericUsage usage;

        private readonly int languageNumber;
        private readonly int tableNumber;
        private readonly int columnNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericUsageHandler"/> class.
        /// </summary>
        /// <param name="usage">
        /// The generic usage.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary with the table information.
        /// </param>
        public GenericUsageHandler(GenericUsage usage, Dictionary dictionary)
        {
            if (usage == null)
            {
                return;
            }

            var table = dictionary.GetTableForNameOrNumber(usage.Table);
            if (table == null)
            {
                return;
            }

            var column = table.GetColumnForNameOrNumber(usage.Column);
            if (column == null)
            {
                return;
            }

            var language = dictionary.GetLanguageForNameOrNumber(usage.Language);
            if (language != null)
            {
                this.languageNumber = language.Index;
            }
            else
            {
                ParserUtil.TryParse(usage.Language, out this.languageNumber);
            }

            this.tableNumber = table.Index;
            this.columnNumber = column.Index;
            this.usage = usage;
        }

        /// <summary>
        /// Adds a <see cref="XimpleCell"/> to the given <see cref="Ximple"/> object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple object to which the cell should be added.
        /// </param>
        /// <param name="value">
        /// The value of the cell.
        /// </param>
        /// <returns>
        /// The cell that was added or null if no cell was added
        /// </returns>
        public XimpleCell AddCell(Ximple ximple, string value)
        {
            return this.AddCell(ximple, value, 0);
        }

        /// <summary>
        /// Adds a <see cref="XimpleCell"/> to the given <see cref="Ximple"/> object.
        /// </summary>
        /// <param name="ximple">
        /// The ximple object to which the cell should be added.
        /// </param>
        /// <param name="value">
        /// The value of the cell.
        /// </param>
        /// <param name="rowIndex">
        /// The row index for indexed usages.
        /// </param>
        /// <returns>
        /// The cell that was added or null if no cell was added
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <see cref="rowIndex"/> is less than zero.
        /// </exception>
        public XimpleCell AddCell(Ximple ximple, string value, int rowIndex)
        {
            if (this.usage == null)
            {
                return null;
            }

            var rowName = string.Format(this.usage.Row, rowIndex + this.usage.RowOffset);
            int row;
            if (!ParserUtil.TryParse(rowName, out row))
            {
                Logger.Warn("Couldn't parse row number: {0} for {1}", rowName, this.usage);
                return null;
            }

            var cell = new XimpleCell
                           {
                               LanguageNumber = this.languageNumber,
                               TableNumber = this.tableNumber,
                               ColumnNumber = this.columnNumber,
                               RowNumber = row,
                               Value = value
                           };
            ximple.Cells.Add(cell);
            return cell;
        }
    }
}
