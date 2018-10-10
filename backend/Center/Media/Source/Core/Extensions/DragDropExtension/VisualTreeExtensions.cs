// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualTreeExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The VisualTreeExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// the visual tree extensions
    /// </summary>
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// Gets the visual ancestor
        /// </summary>
        /// <typeparam name="T">the type of the ancestor</typeparam>
        /// <param name="d">the dependency object</param>
        /// <returns>the ancestor of type T</returns>
        public static T GetVisualAncestor<T>(this DependencyObject d) where T : class
        {
            var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());

            while (item != null)
            {
                var itemAsT = item as T;
                if (itemAsT != null)
                {
                    return itemAsT;
                }

                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        /// <summary>
        /// Gets the visual ancestor
        /// </summary>
        /// <param name="d">the dependency object</param>
        /// <param name="type">the ancestor type</param>
        /// <returns>the ancestor</returns>
        public static DependencyObject GetVisualAncestor(this DependencyObject d, Type type)
        {
            var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());

            while (item != null && type != null)
            {
                if (item.GetType() == type || item.GetType().IsSubclassOf(type))
                {
                    return item;
                }

                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }

        /// <summary>
        /// find the visual ancestor by type and go through the visual tree until the given items control will be found
        /// </summary>
        /// <param name="d">the dependency object</param>
        /// <param name="type">the searched type</param>
        /// <param name="itemsControl">the items control</param>
        /// <returns>a dependency object</returns>
        public static DependencyObject GetVisualAncestor(this DependencyObject d, Type type, ItemsControl itemsControl)
        {
            var item = VisualTreeHelper.GetParent(d.FindVisualTreeRoot());
            DependencyObject lastFoundItemByType = null;

            while (item != null && type != null)
            {
                if (item == itemsControl)
                {
                    return lastFoundItemByType;
                }

                if (item.GetType() == type || item.GetType().IsSubclassOf(type))
                {
                    lastFoundItemByType = item;
                }

                item = VisualTreeHelper.GetParent(item);
            }

            return lastFoundItemByType;
        }

        /// <summary>
        /// Gets the visual descendent of type T
        /// </summary>
        /// <typeparam name="T">the type of the descendent</typeparam>
        /// <param name="d">the dependency object</param>
        /// <returns>the visual descendent of type T</returns>
        public static T GetVisualDescendent<T>(this DependencyObject d) where T : DependencyObject
        {
            return d.GetVisualDescendants<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the visual descendants of type T
        /// </summary>
        /// <typeparam name="T">the type of the descendants</typeparam>
        /// <param name="d">the dependency object</param>
        /// <returns>the descendants</returns>
        public static IEnumerable<T> GetVisualDescendants<T>(this DependencyObject d) where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var n = 0; n < childCount; n++)
            {
                var child = VisualTreeHelper.GetChild(d, n);

                if (child is T)
                {
                    yield return (T)child;
                }

                foreach (var match in GetVisualDescendants<T>(child))
                {
                    yield return match;
                }
            }

            yield break;
        }

        /// <summary>
        /// finds the visual tree root
        /// </summary>
        /// <param name="d">the dependency object</param>
        /// <returns>another dependency object</returns>
        internal static DependencyObject FindVisualTreeRoot(this DependencyObject d)
        {
            var current = d;
            var result = d;

            while (current != null)
            {
                result = current;
                if (current is Visual || current is Visual3D)
                {
                    break;
                }
                
                // If we're in Logical Land then we must walk 
                // up the logical tree until we find a 
                // Visual/Visual3D to get us back to Visual Land.
                current = LogicalTreeHelper.GetParent(current);
            }

            return result;
        }
    }
}