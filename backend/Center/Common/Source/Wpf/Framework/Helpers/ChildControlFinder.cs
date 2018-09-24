// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildControlFinder.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Helpers
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Helper class to find a child control within a visual tree.
    /// </summary>
    public static class ChildControlFinder
    {
        /// <summary>
        /// Finds the first child control within a given item by it's type in the visual tree.
        /// </summary>
        /// <param name="parent">
        /// The parent control.
        /// </param>
        /// <typeparam name="T">
        /// The type of the control to find.
        /// </typeparam>
        /// <returns>
        /// The first child item that matches T if found, null otherwise.
        /// </returns>
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child);

                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// Finds the all child controls within a given item by their type in the visual tree.
        /// </summary>
        /// <param name="parent">
        /// The parent control.
        /// </param>
        /// <typeparam name="T">
        /// The type of the control to find.
        /// </typeparam>
        /// <returns>
        /// The child items that matches T if found, empty list otherwise.
        /// </returns>
        public static List<T> FindChilds<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            var foundChild = new List<T>();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childType = child as T;
                if (childType == null)
                {
                    var children = FindChilds<T>(child);
                    foundChild.AddRange(children);
                }
                else
                {
                    foundChild.Add((T)child);
                }
            }

            return foundChild;
        }
    }
}
