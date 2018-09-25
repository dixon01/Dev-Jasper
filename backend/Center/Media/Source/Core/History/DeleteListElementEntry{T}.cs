// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteListElementEntry{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DeleteListElementEntry.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.History
{
    using System;

    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The DeleteListElementEntry.
    /// </summary>
    /// <typeparam name="T">the type of the element</typeparam>
    public class DeleteListElementEntry<T> : ListElementHistoryEntryBase
    {
        private readonly T element;

        private readonly ExtendedObservableCollection<T> targetCollection;

        private readonly Action onAfterDelete;

        private readonly Action onAfterUndoDelete;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteListElementEntry{T}"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media Shell.
        /// </param>
        /// <param name="element">
        /// the element
        /// </param>
        /// <param name="targetCollection">
        /// target collection
        /// </param>
        /// <param name="onAfterDelete">
        /// the callback on after delete
        /// </param>
        /// <param name="onAfterUndoDelete">
        /// the callback on after undo delete
        /// </param>
        /// <param name="displayText">
        /// display text
        /// </param>
        public DeleteListElementEntry(
            IMediaShell mediaShell,
            T element,
            ExtendedObservableCollection<T> targetCollection,
            Action onAfterDelete,
            Action onAfterUndoDelete,
            string displayText)
            : base(mediaShell, displayText)
        {
            this.element = element;
            this.targetCollection = targetCollection;
            this.onAfterDelete = onAfterDelete;
            this.onAfterUndoDelete = onAfterUndoDelete;
        }

        /// <summary>
        /// the do method
        /// </summary>
        public override void Do()
        {
            this.targetCollection.Remove(this.element);

            var resource = this.element as ResourceInfoDataViewModel;
            if (resource != null && resource.Type == ResourceType.Font)
            {
                this.RemoveFromAvailableFonts(resource);
            }

            this.onAfterDelete();
        }

        /// <summary>
        /// the undo method
        /// </summary>
        public override void Undo()
        {
            this.targetCollection.Add(this.element);

            var resource = this.element as ResourceInfoDataViewModel;
            if (resource != null && resource.Type == ResourceType.Font)
            {
                this.AddToAvailableFonts(resource);
            }

            this.onAfterUndoDelete();
        }
    }
}