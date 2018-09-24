// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTable.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Wrapper for the component <see cref="Table"/>.
    /// </summary>
    public class GenericTable
    {
        private readonly IDictionary<int, IDictionary<int, string>> rows =
            new Dictionary<int, IDictionary<int, string>>();

        private int minRowNumber;

        private int nextRowNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTable"/> class.
        /// </summary>
        /// <param name="tableIndex">
        /// The table index.
        /// </param>
        public GenericTable(int tableIndex)
        {
            this.Name = tableIndex.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Event that is fired whenever the contents of a cell of this table changes.
        /// </summary>
        public event EventHandler<GenericCellEventArgs> CellChanged;

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the total number of columns in this table.
        /// </summary>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Gets the total number of rows in this table.
        /// Since a table can contain positive and negative row numbers,
        /// this number might be greater than the maximum row number.
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.nextRowNumber - this.minRowNumber;
            }
        }

        /// <summary>
        /// Gets the name of the column by its index.
        /// </summary>
        /// <param name="index">The column identifier.</param>
        /// <returns>The name of the column.</returns>
        public string GetColumnName(int index)
        {
            return index.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the row number for the given row index.
        /// Since a table can contain positive and negative row numbers,
        /// The number (positive or negative) of a row does not have to be equal
        /// to the index (always zero or positive).
        /// </summary>
        /// <param name="index">
        /// The row index counting from zero.
        /// </param>
        /// <returns>
        /// The row number for the given row index. This can be negative.
        /// </returns>
        public int GetRowNumber(int index)
        {
            return index - this.minRowNumber;
        }

        /// <summary>
        /// Gets the value of a given cell.
        /// </summary>
        /// <param name="rowNumber">
        /// The row number of the cell (this can also be negative for tables with negative rows).
        /// </param>
        /// <param name="columnNumber">
        /// The column index of the cell.
        /// </param>
        /// <returns>
        /// The contents of the cell or null if it doesn't exist.
        /// </returns>
        public string GetCellValue(int rowNumber, int columnNumber)
        {
            if (columnNumber >= this.ColumnCount || rowNumber >= this.nextRowNumber || rowNumber < this.minRowNumber)
            {
                return null;
            }

            IDictionary<int, string> cells;
            if (!this.rows.TryGetValue(rowNumber, out cells))
            {
                return null;
            }

            string value;
            cells.TryGetValue(columnNumber, out value);
            return value;
        }

        /// <summary>
        /// Sets the value of a given cell.
        /// </summary>
        /// <param name="rowNumber">
        /// The row number of the cell (this can also be negative for tables with negative rows).
        /// If necessary the number of rows in this table will be increased to match the given row number.
        /// </param>
        /// <param name="columnNumber">
        /// The column index of the cell.
        /// If necessary the number of columns in this table will be increased to match the given column number.
        /// </param>
        /// <param name="value">
        /// The new contents of the cell.
        /// </param>
        /// <returns>
        /// True if the given cell was updated, otherwise false.
        /// </returns>
        public bool SetCellValue(int rowNumber, int columnNumber, string value)
        {
            this.minRowNumber = Math.Min(this.minRowNumber, rowNumber);
            this.nextRowNumber = Math.Max(this.nextRowNumber, rowNumber + 1);
            this.ColumnCount = Math.Max(this.ColumnCount, columnNumber + 1);

            IDictionary<int, string> cells;
            if (!this.rows.TryGetValue(rowNumber, out cells))
            {
                cells = new Dictionary<int, string>();
                this.rows.Add(rowNumber, cells);
            }
            else
            {
                string oldValue;
                if (cells.TryGetValue(columnNumber, out oldValue) && string.Equals(oldValue, value))
                {
                    return false;
                }
            }

            cells[columnNumber] = value;
            this.RaiseCellChanged(new GenericCellEventArgs(rowNumber, columnNumber, value));
            return true;
        }

        /// <summary>
        /// Raises the <see cref="CellChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCellChanged(GenericCellEventArgs e)
        {
            var handler = this.CellChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}