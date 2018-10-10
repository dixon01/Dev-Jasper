// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeHistory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IChangeHistory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.History
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines the interface of the component supervising the change history management.
    /// </summary>
    public interface IChangeHistory
    {
        /// <summary>
        /// Gets the redo stack
        /// </summary>
        ReadOnlyCollection<IHistoryEntry> RedoStack { get; }

        /// <summary>
        /// Gets the undo stack
        /// </summary>
        ReadOnlyCollection<IHistoryEntry> UndoStack { get; }

        /// <summary>
        /// Adds a new <see cref="IHistoryEntry"/> to the Undo stack.
        /// </summary>
        /// <param name="entry">the history entry</param>
        void Add(IHistoryEntry entry);

        /// <summary>
        /// Undoes the last <see cref="IHistoryEntry"/>.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redoes the last <see cref="IHistoryEntry"/>.
        /// </summary>
        void Redo();

        /// <summary>
        /// Adds a new save marker (<see cref="ChangeHistory.SaveHistoryEntry"/>).
        /// </summary>
        void AddSaveMarker();
    }
}