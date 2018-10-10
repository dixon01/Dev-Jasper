// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeViewHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TreeViewHelper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The TreeViewHelper.
    /// </summary>
    public static class TreeViewHelper
    {
        /// <summary>
        /// IsMouseDirectlyOverItem:  A DependencyProperty that will be true only on the
        /// TreeViewItem that the mouse is directly over.  I.e., this won't be set on that
        /// parent item.
        /// This is the only public member, and is read-only.
        /// The property key (since this is a read-only DP)
        /// </summary>
        public static readonly DependencyPropertyKey IsMouseDirectlyOverItemKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "IsMouseDirectlyOverItem",
                typeof(bool),
                typeof(TreeViewHelper),
                new FrameworkPropertyMetadata(null, CalculateIsMouseDirectlyOverItem));

        /// <summary>
        /// The DP itself
        /// </summary>
        public static readonly DependencyProperty IsMouseDirectlyOverItemProperty =
            IsMouseDirectlyOverItemKey.DependencyProperty;

        /// <summary>
        /// UpdateOverItem:  A private RoutedEvent used to find the nearest encapsulating
        /// TreeViewItem to the mouse's current position.
        /// </summary>
        private static readonly RoutedEvent UpdateOverItemEvent = EventManager.RegisterRoutedEvent(
            "UpdateOverItem", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewHelper));

        /// <summary>
        /// The TreeViewItem that the mouse is currently directly over (or null).
        /// </summary>
        private static TreeViewItem currentItem;

        /// <summary>        
        /// Initializes static members of the <see cref="TreeViewHelper"/> class.
        /// </summary>
        static TreeViewHelper()
        {
            // Get all Mouse enter/leave events for TreeViewItem.
            EventManager.RegisterClassHandler(
                typeof(TreeViewItem),
                TreeViewItem.MouseEnterEvent,
                new MouseEventHandler(OnMouseTransition),
                true);
            EventManager.RegisterClassHandler(
                typeof(TreeViewItem),
                TreeViewItem.MouseLeaveEvent,
                new MouseEventHandler(OnMouseTransition),
                true);

            // Listen for the UpdateOverItemEvent on all TreeViewItem's.
            EventManager.RegisterClassHandler(
                typeof(TreeViewItem),
                UpdateOverItemEvent,
                new RoutedEventHandler(OnUpdateOverItem));
        }

        /// <summary>
        /// A strongly-typed getter for the property.
        /// </summary>
        /// <param name="obj">the object</param>
        /// <returns>a value indicating whether or not the mouse is directly over</returns>
        public static bool GetIsMouseDirectlyOverItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDirectlyOverItemProperty);
        }

        /// <summary>
        /// A coercion method for the property
        /// </summary>
        /// <param name="item">the item</param>
        /// <param name="value">the value</param>
        /// <returns>a value indicating whether or not the mouse is directly over</returns>
        private static object CalculateIsMouseDirectlyOverItem(DependencyObject item, object value)
        {
            // This method is called when the IsMouseDirectlyOver property is being calculated
            // for a TreeViewItem. 
            if (item == currentItem)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// OnUpdateOverItem:  This method is a listener for the UpdateOverItemEvent.  When it is received,
        /// it means that the sender is the closest TreeViewItem to the mouse (closest in the sense of the
        /// tree, not geographically).
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="args">the arguments</param>
        private static void OnUpdateOverItem(object sender, RoutedEventArgs args)
        {
            // Mark this object as the tree view item over which the mouse
            // is currently positioned.
            currentItem = sender as TreeViewItem;

            // Tell that item to re-calculate the IsMouseDirectlyOverItem property
            currentItem.InvalidateProperty(IsMouseDirectlyOverItemProperty);

            // Prevent this event from notifying other tree view items higher in the tree.
            args.Handled = true;
        }

        /// <summary>
        /// OnMouseTransition:  This method is a listener for both the MouseEnter event and
        /// the MouseLeave event on TreeViewItems.  It updates the _currentItem, and updates
        /// the IsMouseDirectlyOverItem property on the previous TreeViewItem and the new
        /// TreeViewItem.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="args">the arguments</param>
        private static void OnMouseTransition(object sender, MouseEventArgs args)
        {
            lock (IsMouseDirectlyOverItemProperty)
            {
                if (currentItem != null)
                {
                    // Tell the item that previously had the mouse that it no longer does.
                    DependencyObject oldItem = currentItem;
                    currentItem = null;
                    oldItem.InvalidateProperty(IsMouseDirectlyOverItemProperty);
                }

                // Get the element that is currently under the mouse.
                var currentPosition = Mouse.DirectlyOver;

                // See if the mouse is still over something (any element, not just a tree view item).
                if (currentPosition != null)
                {
                    // Yes, the mouse is over something.
                    // Raise an event from that point.  If a TreeViewItem is anywhere above this point
                    // in the tree, it will receive this event and update _currentItem.
                    var newItemArgs = new RoutedEventArgs(UpdateOverItemEvent);
                    currentPosition.RaiseEvent(newItemArgs);
                }
            }
        }
    }
}