// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableCollectionExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ObservableCollectionExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The ObservableCollectionExtensions.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Returns the element at this position with the highest Z index
        /// </summary>
        /// <param name="list">the list</param>
        /// <param name="p">the point</param>
        /// <returns>the element at that position or null</returns>
        public static DrawableElementDataViewModelBase GetElementAt(
            this ObservableCollection<GraphicalElementDataViewModelBase> list, Point p)
        {
            return
                list.Where(
                    element =>
                    element.X.Value    <= p.X
                    && element.Y.Value <= p.Y
                    && (element.X.Value + element.Width.Value)  >= p.X
                    && (element.Y.Value + element.Height.Value) >= p.Y
                    && element.Visible.Value)
                    .OfType<DrawableElementDataViewModelBase>()
                    .OrderByDescending(e => e.ZIndex.Value)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Returns the element at this position with the highest Z index
        /// </summary>
        /// <param name="list">the list</param>
        /// <param name="p">the point</param>
        /// <returns>the element at that position or null</returns>
        public static DrawableElementDataViewModelBase GetElementAt(
            this IEnumerable<GraphicalElementDataViewModelBase> list, Point p)
        {
            return
                list.Where(
                    element =>
                    element.X.Value    <= p.X
                    && element.Y.Value <= p.Y
                    && (element.X.Value + element.Width.Value)  >= p.X
                    && (element.Y.Value + element.Height.Value) >= p.Y
                    && element.Visible.Value)
                    .OfType<DrawableElementDataViewModelBase>()
                    .OrderByDescending(e => e.ZIndex.Value)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Returns the elements at this position
        /// </summary>
        /// <param name="list">the list</param>
        /// <param name="p">the point</param>
        /// <returns>the elements at that position</returns>
        public static IEnumerable<GraphicalElementDataViewModelBase> GetElementsAt(
            this ObservableCollection<GraphicalElementDataViewModelBase> list, Point p)
        {
            return
                list.Where(
                    element =>
                    element.X.Value    <= p.X
                    && element.Y.Value <= p.Y
                    && (element.X.Value + element.Width.Value)  >= p.X
                    && (element.Y.Value + element.Height.Value) >= p.Y
                    && element.Visible.Value);
        }

        /// <summary>
        /// Returns the elements inside the given bounding rectangle
        /// </summary>
        /// <param name="list">the list</param>
        /// <param name="rect">the rectangle</param>
        /// <returns>the elements inside the given bounding rectangle</returns>
        public static IEnumerable<GraphicalElementDataViewModelBase> GetElementsIn(
            this ObservableCollection<GraphicalElementDataViewModelBase> list, Rect rect)
        {
            return
                list.Where(
                    element =>
                    element.X.Value    >= rect.Left
                    && element.Y.Value >= rect.Top
                    && (element.Width.Value + element.X.Value)  <= rect.Right
                    && (element.Y.Value + element.Height.Value) <= rect.Bottom
                    && element.Visible.Value);
        }
    }
}