// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IHistoryEntry.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.History
{
    /// <summary>
    /// The History Entry that can be redone and undone
    /// </summary>
    public interface IHistoryEntry
    {
        /// <summary>
        /// Gets the text to be displayed for this entry on the UI.
        /// </summary>
        string DisplayText { get; }

        /// <summary>
        /// Gets a value indicating whether this history entry is visible outside the <see cref="ChangeHistory"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this history entry is visible outside the <see cref="ChangeHistory"/>; otherwise,
        /// <c>false</c>.
        /// </value>
        bool IsVisible { get; }

        /// <summary>
        /// Aggregates the two entries
        /// </summary>
        /// <param name="otherEntry">the other entry to be aggregated into this one</param>
        /// <returns>a value indicating whether or not the entry was aggregated</returns>
        bool Aggregate(IHistoryEntry otherEntry);

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        void Do();

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        void Undo();

        /// <summary>
        /// Executes again the logic of this entry.
        /// </summary>
        void Redo();
    }
}