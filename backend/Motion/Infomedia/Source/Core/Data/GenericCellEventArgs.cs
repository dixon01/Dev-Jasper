// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericCellEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericCellEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Data
{
    using System;

    /// <summary>
    /// Event arguments for generic cell changes.
    /// </summary>
    public class GenericCellEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCellEventArgs"/> class.
        /// </summary>
        /// <param name="rowNumber">
        /// The row number (can be negative).
        /// </param>
        /// <param name="columnNumber">
        /// The column number.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public GenericCellEventArgs(int rowNumber, int columnNumber, string value)
        {
            this.Value = value;
            this.ColumnNumber = columnNumber;
            this.RowNumber = rowNumber;
        }

        /// <summary>
        /// Gets the row number (can be negative).
        /// </summary>
        public int RowNumber { get; private set; }

        /// <summary>
        /// Gets the column number.
        /// </summary>
        public int ColumnNumber { get; private set; }

        /// <summary>
        /// Gets the cell value.
        /// </summary>
        public string Value { get; private set; }
    }
}