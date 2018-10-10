// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CellCheck.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Ximple
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gorba.Common.Protocols.Ximple;

    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// The cell check for Ximple version 1.
    /// </summary>
    public class CellCheck : CellCheckBase
    {
        private readonly int language;
        private readonly int table;
        private readonly int column;
        private readonly int row;
        private readonly string value;
        private readonly Predicate<string> check;
        private readonly bool isUtf8HexValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellCheck"/> class.
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
        /// <param name="check">
        /// The check.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="isUtf8HexValue">
        /// The is utf 8 hex value.
        /// </param>
        public CellCheck(
            int language, int table, int column, int row, Predicate<string> check, string value, bool isUtf8HexValue)
        {
            this.language = language;
            this.table = table;
            this.column = column;
            this.row = row;
            this.check = check;
            this.value = value;
            this.isUtf8HexValue = isUtf8HexValue;
        }

        /// <summary>
        /// The verify.
        /// </summary>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Verify(Ximple ximple)
        {
            foreach (var cell in ximple.Cells)
            {
                string valueToCheck;
                if (!this.isUtf8HexValue)
                {
                    valueToCheck = cell.Value;
                }
                else
                {
                    byte[] utf8BufferValue = Encoding.UTF8.GetBytes(cell.Value);
                    valueToCheck = BufferUtils.FromByteArrayToHexString(utf8BufferValue);
                }

                if (cell.LanguageNumber == this.language && cell.TableNumber == this.table
                    && cell.ColumnNumber == this.column && cell.RowNumber == this.row
                    && this.check(valueToCheck))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ToString just for debug purposes.
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}={4}", this.language, this.table, this.column, this.row, this.value);
        }
    }
}
