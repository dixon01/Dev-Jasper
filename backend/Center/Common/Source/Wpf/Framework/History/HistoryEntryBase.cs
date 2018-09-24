// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryEntryBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HistoryEntryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.History
{
    /// <summary>
    /// Defines a base implementation with common implementation for history entries.
    /// </summary>
    public abstract class HistoryEntryBase : IHistoryEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryEntryBase"/> class.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        /// <remarks>
        /// The <see cref="IsVisible"/> flag is set to <c>true</c>.
        /// </remarks>
        protected HistoryEntryBase(string displayText)
        {
            this.DisplayText = displayText;
            this.IsVisible = true;
        }

        /// <summary>
        /// Gets the text to be displayed for this entry on the UI.
        /// </summary>
        public string DisplayText { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this history entry is visible outside the
        /// <see cref="ChangeHistory"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this history entry is visible outside the <see cref="ChangeHistory"/>; otherwise,
        /// <c>false</c>.
        /// </value>
        public bool IsVisible { get; protected set; }

        /// <summary>
        /// Aggregates the two entries
        /// </summary>
        /// <param name="otherEntry">the other entry to be aggregated into this one</param>
        /// <returns>
        /// a value indicating whether or not the entry was aggregated
        /// </returns>
        public virtual bool Aggregate(IHistoryEntry otherEntry)
        {
            return false;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public abstract void Do();

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// Executes again the logic of this entry.
        /// </summary>
        public virtual void Redo()
        {
            this.Do();
        }
    }
}