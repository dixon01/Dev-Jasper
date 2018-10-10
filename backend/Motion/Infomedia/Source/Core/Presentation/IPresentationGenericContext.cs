// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPresentationGenericContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPresentationGenericContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Infomedia.Core.Data;

    /// <summary>
    /// The generic presentation context that contains methods to access
    /// generic data as well as to register to cell changes.
    /// </summary>
    public interface IPresentationGenericContext
    {
        /// <summary>
        /// Gets the value of a generic cell.
        /// </summary>
        /// <param name="coord">
        ///   The generic coordinate.
        /// </param>
        /// <returns>
        /// The cell value or null if the cell is not found.
        /// </returns>
        string GetGenericCellValue(GenericCoordinate coord);

        /// <summary>
        /// Adds a handler for a given coordinate.
        /// The handler will be called when the cell at the given 
        /// coordinate changes its value.
        /// </summary>
        /// <param name="coord">
        ///   The generic coordinate.
        /// </param>
        /// <param name="action">
        ///   The action to be performed.
        /// </param>
        void AddCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action);

        /// <summary>
        /// Removes a handler previously registered with <see cref="AddCellChangeHandler"/>.
        /// </summary>
        /// <param name="coord">
        ///   The coordinate.
        /// </param>
        /// <param name="action">
        ///   The action.
        /// </param>
        void RemoveCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action);

        /// <summary>
        /// Adds a handler for a given table (in the given language).
        /// The handler will be called when any cell in the given table changes.
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <param name="handler">
        /// The handler to be called.
        /// </param>
        void AddTableChangeHandler(int language, int table, EventHandler handler);

        /// <summary>
        /// Removes a handler previously registered with <see cref="AddTableChangeHandler"/>.
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <param name="handler">
        /// The handler to be removed.
        /// </param>
        void RemoveTableChangeHandler(int language, int table, EventHandler handler);

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
    }
}