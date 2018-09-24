// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveLayoutElementsHistoryEntry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    using NLog;

    /// <summary>
    /// History entry that contains all information to undo / redo the movement of layout elements.
    /// </summary>
    public class MoveLayoutElementsHistoryEntry : HistoryEntryBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IEditorViewModel editor;

        private IEnumerable<GraphicalElementDataViewModelBase> elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveLayoutElementsHistoryEntry"/> class.
        /// </summary>
        /// <param name="elements">
        /// The selected elements to move.
        /// </param>
        /// <param name="editor">
        /// The editor view model.
        /// </param>
        /// <param name="moveX">
        /// The horizontal movement.
        /// </param>
        /// <param name="moveY">
        /// The vertical movement.
        /// </param>
        /// <param name="displayText">
        /// The text to be displayed for this Entry on the UI.
        /// </param>
        public MoveLayoutElementsHistoryEntry(
             IEnumerable<GraphicalElementDataViewModelBase> elements,
            IEditorViewModel editor,
            int moveX,
            int moveY,
            string displayText)
            : base(displayText)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }

            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            this.elements = elements;
            this.MoveX = moveX;
            this.MoveY = moveY;
            this.editor = editor;
        }

        /// <summary>
        /// Gets the <see cref="GraphicalElementDataViewModelBase"/> elements that have been moved.
        /// </summary>
        public ReadOnlyCollection<GraphicalElementDataViewModelBase> Elements
        {
            get
            {
                return new ReadOnlyCollection<GraphicalElementDataViewModelBase>(this.elements.ToArray());
            }
        }

        /// <summary>
        /// Gets the horizontal movement.
        /// </summary>
        public int MoveX { get; private set; }

        /// <summary>
        /// Gets the vertical movement.
        /// </summary>
        public int MoveY { get; private set; }

        /// <summary>
        /// Aggregates the two entries.
        /// </summary>
        /// <param name="otherEntry">the other entry to be aggregated into this one</param>
        /// <returns>a value indicating whether or not the entry was aggregated</returns>
        public override bool Aggregate(IHistoryEntry otherEntry)
        {
            var otherMoveEntry = otherEntry as MoveLayoutElementsHistoryEntry;

            if (otherMoveEntry != null && this.elements.Count() == otherMoveEntry.Elements.Count
                && this.elements.All(n => otherMoveEntry.Elements.Select(n1 => n1.ElementName).Contains(n.ElementName)))
            {
                this.elements = otherMoveEntry.Elements.ToList();
                this.MoveX += otherMoveEntry.MoveX;
                this.MoveY += otherMoveEntry.MoveY;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes the logic of this entry.
        /// </summary>
        public override void Do()
        {
            this.UpdateElements();
        }

        /// <summary>
        /// Executes the logic to undo the changes of this entry.
        /// </summary>
        public override void Undo()
        {
            this.UpdateElements(true);
        }

        private void UpdateElements(bool undo = false)
        {
            var layoutEditor = this.editor;
            if (layoutEditor == null)
            {
                Logger.Error("The LayoutEditor can't be null.");
                return;
            }

            var undoModifier = undo ? -1 : 1;
            foreach (var element in this.elements)
            {
                element.X.Value += undoModifier * this.MoveX;
                element.Y.Value += undoModifier * this.MoveY;
            }

            layoutEditor.SelectedElements.Clear();
            foreach (var element in this.elements)
            {
                element.MakeDirty();
                layoutEditor.SelectedElements.Add(element);
            }
        }
    }
}
