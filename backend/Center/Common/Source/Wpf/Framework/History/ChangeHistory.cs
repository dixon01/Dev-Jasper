// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeHistory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ChangeHistory.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.History
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Startup;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The Change History, able to undo and redo HistoryEntries
    /// </summary>
    public class ChangeHistory : IChangeHistory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Lazy<IApplicationState> lazyApplicationState =
            new Lazy<IApplicationState>(GetApplicationState);

        private readonly Stack<IHistoryEntry> undoStack;

        private readonly Stack<IHistoryEntry> redoStack;

        private SaveHistoryEntry lastSaveHistoryEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeHistory"/> class.
        /// </summary>
        public ChangeHistory()
        {
            this.undoStack = new Stack<IHistoryEntry>();
            this.redoStack = new Stack<IHistoryEntry>();
            this.AddSaveMarker();
        }

        /// <summary>
        /// The Change event
        /// </summary>
        public event Action HistoryChanged;

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>
        /// The state of the application.
        /// </value>
        public IApplicationState ApplicationState
        {
            get
            {
                return this.lazyApplicationState.Value;
            }
        }

        /// <summary>
        /// Gets the undo stack
        /// </summary>
        public ReadOnlyCollection<IHistoryEntry> UndoStack
        {
            get
            {
                return new ReadOnlyCollection<IHistoryEntry>(this.undoStack.Where(e => e.IsVisible).ToArray());
            }
        }

        /// <summary>
        /// Gets the redo stack
        /// </summary>
        public ReadOnlyCollection<IHistoryEntry> RedoStack
        {
            get
            {
                return new ReadOnlyCollection<IHistoryEntry>(this.redoStack.Where(e => e.IsVisible).ToArray());
            }
        }

        /// <summary>
        /// Adds a new save marker (<see cref="SaveHistoryEntry"/>).
        /// </summary>
        public void AddSaveMarker()
        {
            this.Add(new SaveHistoryEntry());
        }

        /// <summary>
        /// Adds a new <see cref="IHistoryEntry"/> to the Undo stack.
        /// </summary>
        /// <param name="entry">the history entry</param>
        public void Add(IHistoryEntry entry)
        {
            var existing = this.undoStack.FirstOrDefault();
            if (existing != null && existing.GetType() == entry.GetType() && existing.Aggregate(entry))
            {
                Logger.Trace("Aggregated history entry '{0}' with '{1}' in Undo stack.", existing, entry);
                this.OnHistoryChanged();
                return;
            }

            // was not aggregated in previous save mark.
            var saveMark = entry as SaveHistoryEntry;
            if (saveMark != null)
            {
                this.lastSaveHistoryEntry = saveMark;
            }

            this.undoStack.Push(entry);
            this.redoStack.Clear();
            Logger.Trace("Added new history entry {0} to the Undo stack.", entry);
            this.OnHistoryChanged();
        }

        /// <summary>
        /// Undoes the last <see cref="IHistoryEntry"/>.
        /// </summary>
        public void Undo()
        {
            if (this.undoStack.Count == 0)
            {
                Logger.Info("Undo stack is empty.");
                return;
            }

            var entry = this.undoStack.Pop();
            var isSaveHistoryEntry = entry is SaveHistoryEntry;

            // undo of SaveHistoryEntry must not require a user input
            if (isSaveHistoryEntry)
            {
                Logger.Info("Undo history entry '{0}'", entry.DisplayText);
                entry.Undo();

                if (this.undoStack.Count == 0)
                {
                    Logger.Info("Undo had only a save history entry.");
                    return;
                }

                // allow clean application on redo
                this.redoStack.Push(entry);

                entry = this.undoStack.Pop();
                isSaveHistoryEntry = entry is SaveHistoryEntry;
                if (isSaveHistoryEntry)
                {
                    throw new Exception("Duplicated SaveHistoryEntry. Should not happen.");
                }
            }

            Logger.Info("Undo history entry '{0}'", entry.DisplayText);
            entry.Undo();
            this.redoStack.Push(entry);
            Logger.Debug("New undo stack count: {0}", this.undoStack.Count);

            this.DecideIsProjectDirty();

            this.OnHistoryChanged();
        }

        /// <summary>
        /// Redoes the last <see cref="IHistoryEntry"/>.
        /// </summary>
        public void Redo()
        {
            if (this.redoStack.Count == 0)
            {
                Logger.Info("Redo stack is empty.");
                return;
            }

            var entry = this.redoStack.Pop();
            var isSaveHistoryEntry = entry is SaveHistoryEntry;

            if (isSaveHistoryEntry)
            {
                throw new Exception("Top of redo stack is save mark. Should not happen.");
            }

            Logger.Info("Redo history entry '{0}'", entry.DisplayText);
            entry.Redo();
            this.undoStack.Push(entry);
            Logger.Debug("New redo stack count: {0}", this.redoStack.Count);

            if (this.redoStack.Count > 0)
            {
                var historyEntry = this.redoStack.Peek();
                if (historyEntry is SaveHistoryEntry)
                {
                    entry = this.redoStack.Pop();
                    this.undoStack.Push(entry);
                }
            }

            this.DecideIsProjectDirty();

            this.OnHistoryChanged();
        }

        private static IApplicationState GetApplicationState()
        {
            return ServiceLocator.Current.GetInstance<IApplicationState>();
        }

        private void DecideIsProjectDirty()
        {
            if (this.undoStack.Count <= 0)
            {
                this.ApplicationState.ClearDirty();
                return;
            }

            var historyEntry = this.undoStack.Peek();
            if (historyEntry is SaveHistoryEntry && historyEntry == this.lastSaveHistoryEntry)
            {
                this.ApplicationState.ClearDirty();
            }
            else
            {
                this.ApplicationState.MakeDirty();
            }
        }

        private void OnHistoryChanged()
        {
            var handler = this.HistoryChanged;
            if (handler != null)
            {
                handler();
            }
        }

        /// <summary>
        /// Special history entry to mark a Save operation.
        /// </summary>
        private sealed class SaveHistoryEntry : HistoryEntryBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SaveHistoryEntry"/> class.
            /// </summary>
            public SaveHistoryEntry()
                : base("SaveHistoryEntry")
            {
                this.IsVisible = false;
            }

            /// <summary>
            /// Aggregates the two entries
            /// </summary>
            /// <param name="otherEntry">the other entry to be aggregated into this one</param>
            /// <returns>
            /// a value indicating whether or not the entry was aggregated
            /// </returns>
            public override bool Aggregate(IHistoryEntry otherEntry)
            {
                var otherSaveEntry = otherEntry as SaveHistoryEntry;

                if (otherSaveEntry != null)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Executes the logic of this entry.
            /// </summary>
            public override void Do()
            {
                // This entry is only a marker
            }

            /// <summary>
            /// Executes the logic to undo the changes of this entry.
            /// </summary>
            public override void Undo()
            {
                // this entry is only a marker
            }
        }
    }
}