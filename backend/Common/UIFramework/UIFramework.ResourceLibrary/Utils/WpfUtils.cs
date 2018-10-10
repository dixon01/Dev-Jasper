// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WpfUtils.cs" company="">
//   
// </copyright>
// <summary>
//   The wpf utils.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.ResourceLibrary.Utils
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    ///     The wpf utils.
    /// </summary>
    public static class WpfUtils
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Searches the visual tree for a parent of the given type
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="obj"> The obj. </param>
        public static T FindParent<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parent = obj;
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }

        /// <summary>
        ///     This method is an alternative to WPF's <see cref="VisualTreeHelper.GetParent" /> method, which also supports content elements. Keep in mind that for content element, this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child"> The item to be processed. </param>
        /// <returns> The submitted item's parent, if available. Otherwise null. </returns>
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null)
            {
                return null;
            }

            // handle content elements separately
            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                {
                    return parent;
                }

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            // also try searching for parent in framework elements (such as DockPanel, etc)
            var frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null)
                {
                    return parent;
                }
            }

            // if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        /// <summary>
        ///     The invoke on ui.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="methodcall"> The methodcall. </param>
        public static void InvokeOnUI(this object source, Action methodcall)
        {
            InvokeOnUI(source, DispatcherPriority.Normal, methodcall);
        }

        /// <summary>
        ///     The invoke on ui.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="priorityForCall"> The priority for call. </param>
        /// <param name="methodcall"> The methodcall. </param>
        public static void InvokeOnUI(this object source, DispatcherPriority priorityForCall, Action methodcall)
        {
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
            {
                Application.Current.Dispatcher.Invoke(priorityForCall, methodcall);
            }
            else
            {
                methodcall();
            }
        }

        /// <summary>
        ///     Determines whether the current object is a parent of the specified child
        /// </summary>
        /// <param name="current"> The current. </param>
        /// <param name="child"> The child. </param>
        /// <returns> The is ancestor. </returns>
        public static bool IsAncestor(this DependencyObject current, DependencyObject child)
        {
            DependencyObject parent = child;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent == current)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T"> The type of the queried item. </typeparam>
        /// <param name="child"> A direct or indirect child of the queried item. </param>
        /// <returns> The first parent item that matches the submitted type parameter. If not matching item can be found, a null reference is being returned. </returns>
        public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = GetParentObject(child);

            // we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            // check if the parent matches the type we're looking for
            var parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return TryFindParent<T>(parentObject);
            }
        }

        public static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        #endregion
    }
}