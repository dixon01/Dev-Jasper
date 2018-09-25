// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITableController.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Data
{
    using System;

    /// <summary>
    /// The interface for a table controller.
    /// </summary>
    public interface ITableController
    {
        /// <summary>
        /// Event that is fired whenever new data arrived from Protran.
        /// </summary>
        event EventHandler<TableEventArgs> DataReceived;

        #region Public Methods

        /// <summary>
        /// Deletes all tables.
        /// </summary>
        void ClearTables();

        /// <summary>
        /// Gets the generic table for a given language and table index.
        /// </summary>
        /// <param name="languageIndex">
        /// The language index.
        /// </param>
        /// <param name="tableIndex">
        /// The table index.
        /// </param>
        /// <returns>
        /// The generic table or null if it doesn't exist.
        /// </returns>
        GenericTable GetTable(int languageIndex, int tableIndex);

        /// <summary>
        /// Gets a specific cell value of a specific table.
        /// </summary>
        /// <param name="languageIndex">
        /// The language index.
        /// </param>
        /// <param name="tableIndex">
        /// The table index.
        /// </param>
        /// <param name="columnIndex">
        /// The column index.
        /// </param>
        /// <param name="rowIndex">
        /// The row index.
        /// </param>
        /// <returns>
        /// The cell value.
        /// </returns>
        string GetCellValue(int languageIndex, int tableIndex, int columnIndex, int rowIndex);

        #endregion
    }
}