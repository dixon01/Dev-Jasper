// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDragHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DefaultDragHandler.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// the default drag handler
    /// </summary>
    public class DefaultDragHandler : IDragSource
    {
        /// <summary>
        /// the start drag handler
        /// </summary>
        /// <param name="dragInfo">the drag info</param>
        public virtual void StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            }
            else if (itemCount > 1)
            {
                dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            }

            dragInfo.Effects = (dragInfo.Data != null) ?
                DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link :
                DragDropEffects.None;
        }

        /// <summary>
        /// Determines if we can start dragging
        /// </summary>
        /// <param name="dragInfo">the drag info</param>
        /// <returns>whether or not we can start dragging</returns>
        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        /// <summary>
        /// the dropped handler
        /// </summary>
        /// <param name="dropInfo">the drop info</param>
        public virtual void Dropped(IDropInfo dropInfo)
        {
        }

        /// <summary>
        /// the drag cancelled handler
        /// </summary>
        public virtual void DragCancelled()
        {
        }
    }
}