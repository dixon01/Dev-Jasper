// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRemoteManagementTableProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRemoteManagementTableProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Remote
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a table management provider from a different host.
    /// Supports reloading the cached management information and asynchronous methods.
    /// </summary>
    public interface IRemoteManagementTableProvider : IRemoteManagementProvider, IManagementTableProvider
    {
        /// <summary>
        /// Asynchronously begins to fetch the rows from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the rows where fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetRows"/>.
        /// </returns>
        IAsyncResult BeginGetRows(AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to fetch the rows from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetRows"/>.
        /// </param>
        /// <returns>
        /// The list of all rows.
        /// </returns>
        IEnumerable<List<ManagementProperty>> EndGetRows(IAsyncResult result);

        /// <summary>
        /// Asynchronously begins to fetch the row with the given index from the remote node.
        /// </summary>
        /// <param name="index">
        /// The index from zero on which to find the row.
        /// </param>
        /// <param name="callback">
        /// The callback called when the row was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetRow"/>.
        /// </returns>
        IAsyncResult BeginGetRow(int index, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to fetch a row from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetRow"/>.
        /// </param>
        /// <returns>
        /// The row with the index given to <see cref="BeginGetRow"/>
        /// or null if no row with the given index exists.
        /// </returns>
        List<ManagementProperty> EndGetRow(IAsyncResult result);

        /// <summary>
        /// Asynchronously begins to fetch the cell with the given row index and column name from the remote node.
        /// </summary>
        /// <param name="rowIndex">
        /// The index from zero on which to find the row.
        /// </param>
        /// <param name="columnName">
        /// The name of the column to return the value in the row.
        /// </param>
        /// <param name="callback">
        /// The callback called when the cell was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetCell"/>.
        /// </returns>
        IAsyncResult BeginGetCell(int rowIndex, string columnName, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to fetch a cell from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetCell"/>.
        /// </param>
        /// <returns>
        /// The cell with the row index and column name given to <see cref="BeginGetCell"/>
        /// or null if the cell was not found.
        /// </returns>
        ManagementProperty EndGetCell(IAsyncResult result);
    }
}