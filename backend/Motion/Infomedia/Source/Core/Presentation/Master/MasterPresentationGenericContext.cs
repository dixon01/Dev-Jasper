// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterPresentationGenericContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterPresentationGenericContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Master
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Infomedia.Core.Data;

    /// <summary>
    /// The master presentation generic context.
    /// </summary>
    internal class MasterPresentationGenericContext : IPresentationGenericContext
    {
        private readonly CellChangeHandlers cellChangeHandlers;

        private readonly ITableController tableController;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPresentationGenericContext"/> class.
        /// </summary>
        /// <param name="tableController">
        /// The table controller.
        /// </param>
        public MasterPresentationGenericContext(ITableController tableController)
        {
            this.cellChangeHandlers = new CellChangeHandlers(this);
            this.tableController = tableController;
        }

        /// <summary>
        /// Notifies all handlers of a cell change.
        /// </summary>
        /// <param name="newValues">
        /// The new values.
        /// </param>
        public void NotifyCellChange(IEnumerable<XimpleCell> newValues)
        {
            this.cellChangeHandlers.NotifyCellChange(newValues);
        }

        /// <summary>
        /// Gets the value of a generic cell.
        /// </summary>
        /// <param name="coord">
        ///   The generic coordinate.
        /// </param>
        /// <returns>
        /// The cell value or null if the cell is not found.
        /// </returns>
        public string GetGenericCellValue(GenericCoordinate coord)
        {
            return this.tableController.GetCellValue(coord.Language, coord.Table, coord.Column, coord.Row);
        }

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
        public void AddCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            this.cellChangeHandlers.AddChangeHandler(coord, action);
        }

        /// <summary>
        /// Removes a handler previously registered with <see cref="IPresentationGenericContext.AddCellChangeHandler"/>.
        /// </summary>
        /// <param name="coord">
        ///   The coordinate.
        /// </param>
        /// <param name="action">
        ///   The action.
        /// </param>
        public void RemoveCellChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            this.cellChangeHandlers.RemoveChangeHandler(coord, action);
        }

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
        public void AddTableChangeHandler(int language, int table, EventHandler handler)
        {
            this.cellChangeHandlers.AddTableChangeHandler(language, table, handler);
        }

        /// <summary>
        /// Removes a handler previously registered with <see cref="IPresentationGenericContext.AddTableChangeHandler"/>.
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
        public void RemoveTableChangeHandler(int language, int table, EventHandler handler)
        {
            this.cellChangeHandlers.RemoveTableChangeHandler(language, table, handler);
        }

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
        public GenericTable GetTable(int languageIndex, int tableIndex)
        {
            return this.tableController.GetTable(languageIndex, tableIndex);
        }
    }
}