﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDragSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IDragSource.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Windows;

    /// <summary>
    /// Interface implemented by Drag Handlers.
    /// </summary>
    public interface IDragSource
    {
        /// <summary>
        /// Queries whether a drag can be started.
        /// </summary>
        /// <param name="dragInfo">
        /// Information about the drag.
        /// </param>
        /// <remarks>
        /// To allow a drag to be started, the <see cref="DragInfo.Effects"/> property on <paramref name="dragInfo"/> 
        /// should be set to a value other than <see cref="DragDropEffects.None"/>. 
        /// </remarks>
        void StartDrag(IDragInfo dragInfo);

        /// <summary>
        /// With this action it's possible to check if the drag and drop operation is allowed to start
        /// e.g. check for a UIElement inside a list view item, that should not start a drag and drop operation
        /// </summary>
        /// <param name="dragInfo">the drag info</param>
        /// <returns>a value indicating whether or not we can start dragging</returns>
        bool CanStartDrag(IDragInfo dragInfo);

        /// <summary>
        /// Notifies the drag handler that a drop has occurred.
        /// </summary>
        /// <param name="dropInfo">
        ///   Information about the drop.
        /// </param>
        void Dropped(IDropInfo dropInfo);

        /// <summary>
        /// Notifies the drag handler that a drag has been aborted.
        /// </summary>
        void DragCancelled();
    }
}