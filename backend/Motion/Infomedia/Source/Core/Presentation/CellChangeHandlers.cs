// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CellChangeHandlers.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CellChangeHandlers type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Helper class that keeps a list of all cell and table change handlers
    /// for an <see cref="IPresentationGenericContext"/>.
    /// </summary>
    internal class CellChangeHandlers
    {
        private static readonly Logger Logger = LogHelper.GetLogger<CellChangeHandlers>();

        private readonly Dictionary<GenericCoordinate, Action<XimpleCell>> cellChangeHandlers =
            new Dictionary<GenericCoordinate, Action<XimpleCell>>();

        private readonly Dictionary<GenericCoordinate, EventHandler> tableChangeHandlers =
            new Dictionary<GenericCoordinate, EventHandler>();

        private readonly IPresentationGenericContext owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellChangeHandlers"/> class.
        /// </summary>
        /// <param name="owner">
        /// The owner of this object.
        /// </param>
        public CellChangeHandlers(IPresentationGenericContext owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Notifies all relevant handlers about a change in a cell.
        /// </summary>
        /// <param name="newValues">
        /// The new values.
        /// </param>
        public void NotifyCellChange(IEnumerable<XimpleCell> newValues)
        {
            var notifiedTables = new List<GenericCoordinate>();
            foreach (var cell in newValues)
            {
                var coord = new GenericCoordinate(
                    cell.LanguageNumber, cell.TableNumber, cell.ColumnNumber, cell.RowNumber);

                try
                {
                    Action<XimpleCell> action;
                    if (this.cellChangeHandlers.TryGetValue(coord, out action) && action != null)
                    {
                        action(cell);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't notify cell update");
                }

                // notify about table changes
                coord.Column = 0;
                coord.Row = 0;
                if (notifiedTables.Contains(coord))
                {
                    continue;
                }

                notifiedTables.Add(coord);

                try
                {
                    EventHandler handler;
                    if (this.tableChangeHandlers.TryGetValue(coord, out handler) && handler != null)
                    {
                        handler(this.owner, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't notify table update");
                }
            }
        }

        /// <summary>
        /// Adds a handler for a given coordinate.
        /// The handler will be called when the cell at the given coordinate changes its value.
        /// </summary>
        /// <param name="coord">
        ///   The generic coordinate.
        /// </param>
        /// <param name="action">
        ///   The action to be performed.
        /// </param>
        public void AddChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            Action<XimpleCell> handler;
            this.cellChangeHandlers.TryGetValue(coord, out handler);
            this.cellChangeHandlers[coord] = (Action<XimpleCell>)Delegate.Combine(handler, action);
        }

        /// <summary>
        /// Removes a handler previously registered with <see cref="AddChangeHandler"/>.
        /// </summary>
        /// <param name="coord">
        ///   The coordinate.
        /// </param>
        /// <param name="action">
        ///   The action.
        /// </param>
        public void RemoveChangeHandler(GenericCoordinate coord, Action<XimpleCell> action)
        {
            Action<XimpleCell> handler;
            this.cellChangeHandlers.TryGetValue(coord, out handler);
            this.cellChangeHandlers[coord] = (Action<XimpleCell>)Delegate.Remove(handler, action);
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
            var coord = new GenericCoordinate(language, table, 0, 0);
            EventHandler existing;
            this.tableChangeHandlers.TryGetValue(coord, out existing);
            this.tableChangeHandlers[coord] = (EventHandler)Delegate.Combine(existing, handler);
        }

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
        public void RemoveTableChangeHandler(int language, int table, EventHandler handler)
        {
            var coord = new GenericCoordinate(language, table, 0, 0);
            EventHandler existing;
            this.tableChangeHandlers.TryGetValue(coord, out existing);
            this.tableChangeHandlers[coord] = (EventHandler)Delegate.Remove(existing, handler);
        }

        /// <summary>
        /// Returns an enumerator that iterates through all subscribed cell changes for a given table (and language).
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerable<KeyValuePair<GenericCoordinate, Action<XimpleCell>>> GetCellChangeHandlers(
            int language, int table)
        {
            foreach (var pair in this.cellChangeHandlers)
            {
                if (pair.Key.Language != language || pair.Key.Table != table || pair.Value == null)
                {
                    continue;
                }

                yield return pair;
            }
        }

        /// <summary>
        /// Returns the subscribed handlers for a given table (and language).
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <returns>
        /// An <see cref="EventHandler"/> or null if none is found.
        /// </returns>
        public EventHandler GetTableChangeHandler(int language, int table)
        {
            EventHandler handler;
            this.tableChangeHandlers.TryGetValue(new GenericCoordinate(language, table, 0, 0), out handler);
            return handler;
        }
    }
}