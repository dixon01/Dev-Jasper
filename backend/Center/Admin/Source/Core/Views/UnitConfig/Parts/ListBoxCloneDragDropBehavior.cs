// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListBoxCloneDragDropBehavior.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListBoxCloneDragDropBehavior type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views.UnitConfig.Parts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Telerik.Windows.DragDrop.Behaviors;

    /// <summary>
    /// An extension of <see cref="ListBoxDragDropBehavior"/> that supports cloning items on copy.
    /// </summary>
    public class ListBoxCloneDragDropBehavior : ListBoxDragDropBehavior
    {
        /// <summary>
        /// Copies the items that are dragged in the operation specified by the provided ListBoxDragDropState.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The copied items.
        /// </returns>
        protected override IEnumerable<object> CopyDraggedItems(DragDropState state)
        {
            return state.DraggedItems.OfType<ICloneable>().Select(i => i.Clone());
        }
    }
}
