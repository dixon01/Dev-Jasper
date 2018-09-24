// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsControlExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ItemsControlExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    /// <summary>
    /// the items control extensions
    /// </summary>
    public static class ItemsControlExtensions
    {
        /// <summary>
        /// Determines if we can select multiple items
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <returns>a boolean</returns>
        public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector)
            {
                // The CanSelectMultipleItems property is protected. Use reflection to
                // get its value anyway.
                return (bool)itemsControl.GetType()
                    .GetProperty("CanSelectMultipleItems", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(itemsControl, null);
            }

            if (itemsControl is ListBox)
            {
                return ((ListBox)itemsControl).SelectionMode != SelectionMode.Single;
            }
            
            return false;
        }

        /// <summary>
        /// Finds the item container
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <param name="child">the child item</param>
        /// <returns>the UI element</returns>
        public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
        {
            bool isItemContainer;
            var itemType = GetItemContainerType(itemsControl, out isItemContainer);

            if (itemType != null)
            {
                return isItemContainer
                    ? (UIElement)child.GetVisualAncestor(itemType, itemsControl)
                    : (UIElement)child.GetVisualAncestor(itemType);
            }

            return null;
        }

        /// <summary>
        /// Gets the item container at a point
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <param name="position">the point</param>
        /// <returns>the UI element</returns>
        public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
        {
            var inputElement = itemsControl.InputHitTest(position);
            var uiElement = inputElement as UIElement;

            if (uiElement != null)
            {
                return GetItemContainer(itemsControl, uiElement);
            }

            return null;
        }

        /// <summary>
        /// Gets the item container at a point
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <param name="position">the point</param>
        /// <param name="searchDirection">the search direction</param>
        /// <returns>the UI element</returns>
        public static UIElement GetItemContainerAt(
            this ItemsControl itemsControl, 
            Point position,
            Orientation searchDirection)
        {
            bool isItemContainer;
            var itemContainerType = GetItemContainerType(itemsControl, out isItemContainer);

            Geometry hitTestGeometry;

            if (typeof(TreeViewItem).IsAssignableFrom(itemContainerType))
            {
                hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
            }
            else
            {
                switch (searchDirection)
                {
                    case Orientation.Horizontal:
                        hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
                        break;
                    case Orientation.Vertical:
                        hitTestGeometry = new LineGeometry(new Point(position.X, 0), new Point(position.X, itemsControl.RenderSize.Height));
                        break;
                    default:
                        throw new ArgumentException("Invalid value for searchDirection");
                }
            }

            var hits = new List<DependencyObject>();

            VisualTreeHelper.HitTest(
                itemsControl,
                null,
                result =>
                {
                    var itemContainer = isItemContainer
                        ? result.VisualHit.GetVisualAncestor(itemContainerType, itemsControl)
                        : result.VisualHit.GetVisualAncestor(itemContainerType);
                    if (itemContainer != null && !hits.Contains(itemContainer) && ((UIElement)itemContainer).IsVisible)
                    {
                        hits.Add(itemContainer);
                    }

                    return HitTestResultBehavior.Continue;
                },
                new GeometryHitTestParameters(hitTestGeometry));

            return GetClosest(itemsControl, hits, position, searchDirection);
        }

        /// <summary>
        /// Gets the item container type
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <param name="isItemContainer">the is item container result</param>
        /// <returns>the type</returns>
        public static Type GetItemContainerType(this ItemsControl itemsControl, out bool isItemContainer)
        {
            // determines if the itemsControl is not a ListView, ListBox or TreeView
            isItemContainer = false;

            if (itemsControl is DataGrid)
            {
                return typeof(DataGridRow);
            }

            // There is no safe way to get the item container type for an ItemsControl. 
            // First hard-code the types for the common ItemsControls.
            // if (itemsControl.GetType().IsAssignableFrom(typeof(ListView)))
            if (itemsControl is ListView)
            {
                return typeof(ListViewItem);
            }

            // if (itemsControl.GetType().IsAssignableFrom(typeof(ListBox)))
            if (itemsControl is ListBox)
            {
                return typeof(ListBoxItem);
            }

            // else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
            if (itemsControl is TreeView)
            {
                return typeof(TreeViewItem);
            }

            // Otherwise look for the control's ItemsPresenter, get it's child panel and the first 
            // child of that *should* be an item container.
            //
            // If the control currently has no items, we're out of luck.
            if (itemsControl.Items.Count > 0)
            {
                var itemsPresenters = itemsControl.GetVisualDescendants<ItemsPresenter>();

                foreach (var itemsPresenter in itemsPresenters)
                {
                    var panel = VisualTreeHelper.GetChild(itemsPresenter, 0);
                    var itemContainer = VisualTreeHelper.GetChildrenCount(panel) > 0
                        ? VisualTreeHelper.GetChild(panel, 0)
                        : null;

                    // Ensure that this actually *is* an item container by checking it with
                    // ItemContainerGenerator.
                    if (itemContainer != null &&
                        itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1)
                    {
                        isItemContainer = true;
                        return itemContainer.GetType();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the items panel orientation
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <returns>the orientation</returns>
        public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
        {
            var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>();

            if (itemsPresenter != null)
            {
                var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
                var orientationProperty = itemsPanel.GetType().GetProperty("Orientation", typeof(Orientation));

                if (orientationProperty != null)
                {
                    return (Orientation)orientationProperty.GetValue(itemsPanel, null);
                }
            }

            // Make a guess!
            return Orientation.Vertical;
        }

        /// <summary>
        /// Gets the items panel flow direction
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <returns>the flow direction</returns>
        public static FlowDirection GetItemsPanelFlowDirection(this ItemsControl itemsControl)
        {
            var itemsPresenter = itemsControl.GetVisualDescendent<ItemsPresenter>();

            if (itemsPresenter != null)
            {
                var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
                var flowDirectionProperty = itemsPanel.GetType().GetProperty("FlowDirection", typeof(FlowDirection));

                if (flowDirectionProperty != null)
                {
                    return (FlowDirection)flowDirectionProperty.GetValue(itemsPanel, null);
                }
            }

            // Make a guess!
            return FlowDirection.LeftToRight;
        }

        /// <summary>
        /// Sets the selected item
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <param name="item">the selected item</param>
        public static void SetSelectedItem(this ItemsControl itemsControl, object item)
        {
            if (itemsControl is MultiSelector)
            {
                ((MultiSelector)itemsControl).SelectedItem = null;
                ((MultiSelector)itemsControl).SelectedItem = item;
            }
            else if (itemsControl is ListBox)
            {
                ((ListBox)itemsControl).SelectedItem = null;
                ((ListBox)itemsControl).SelectedItem = item;
            }
            else if (itemsControl is TreeView)
            {
                // TODO: Select the TreeViewItem
                // ((TreeView)itemsControl)
            }
            else if (itemsControl is Selector)
            {
                ((Selector)itemsControl).SelectedItem = null;
                ((Selector)itemsControl).SelectedItem = item;
            }
        }

        /// <summary>
        /// Gets the selected items
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <returns>the selected items</returns>
        public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
        {
            // if (itemsControl.GetType().IsAssignableFrom(typeof(MultiSelector)))
            if (itemsControl is MultiSelector)
            {
                return ((MultiSelector)itemsControl).SelectedItems;
            }
            
            if (itemsControl is ListBox)
            {
                var listBox = (ListBox)itemsControl;

                if (listBox.SelectionMode == SelectionMode.Single)
                {
                    return Enumerable.Repeat(listBox.SelectedItem, 1);
                }

                return listBox.SelectedItems;
            }
            
            // else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
            if (itemsControl is TreeView)
            {
                return Enumerable.Repeat(((TreeView)itemsControl).SelectedItem, 1);
            }
            
            // else if (itemsControl.GetType().IsAssignableFrom(typeof(Selector)))
            if (itemsControl is Selector)
            {
                return Enumerable.Repeat(((Selector)itemsControl).SelectedItem, 1);
            }
            
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Determines whether the item is selected
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <param name="item">the item</param>
        /// <returns>whether the item is selected or not</returns>
        public static bool GetItemSelected(this ItemsControl itemsControl, object item)
        {
            if (itemsControl is MultiSelector)
            {
                return ((MultiSelector)itemsControl).SelectedItems.Contains(item);
            }
            
            if (itemsControl is ListBox)
            {
                return ((ListBox)itemsControl).SelectedItems.Contains(item);
            }
            
            if (itemsControl is TreeView)
            {
                return ((TreeView)itemsControl).SelectedItem == item;
            }
            
            if (itemsControl is Selector)
            {
                return ((Selector)itemsControl).SelectedItem == item;
            }
            
            return false;
        }

        /// <summary>
        /// Sets whether the item is selected
        /// </summary>
        /// <param name="itemsControl">the items control</param>
        /// <param name="item">the item</param>
        /// <param name="value">the value</param>
        public static void SetItemSelected(this ItemsControl itemsControl, object item, bool value)
        {
            if (itemsControl is MultiSelector)
            {
                var multiSelector = (MultiSelector)itemsControl;

                if (value)
                {
                    if (multiSelector.CanSelectMultipleItems())
                    {
                        multiSelector.SelectedItems.Add(item);
                    }
                    else
                    {
                        multiSelector.SelectedItem = item;
                    }
                }
                else
                {
                    multiSelector.SelectedItems.Remove(item);
                }
            }
            else if (itemsControl is ListBox)
            {
                var listBox = (ListBox)itemsControl;

                if (value)
                {
                    if (listBox.SelectionMode != SelectionMode.Single)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                    else
                    {
                        listBox.SelectedItem = item;
                    }
                }
                else
                {
                    listBox.SelectedItems.Remove(item);
                }
            }
        }

        private static UIElement GetClosest(
            ItemsControl itemsControl, 
            IEnumerable<DependencyObject> items,
            Point position, 
            Orientation searchDirection)
        {
            // Console.WriteLine("GetClosest - {0}", itemsControl.ToString());
            UIElement closest = null;
            var closestDistance = double.MaxValue;

            foreach (var i in items)
            {
                var uiElement = i as UIElement;

                if (uiElement != null)
                {
                    var p = uiElement.TransformToAncestor(itemsControl).Transform(new Point(0, 0));
                    var distance = double.MaxValue;

                    if (itemsControl is TreeView)
                    {
                        var xdiff = position.X - p.X;
                        var ydiff = position.Y - p.Y;
                        var hyp = Math.Sqrt(Math.Pow(xdiff, 2d) + Math.Pow(ydiff, 2d));
                        distance = Math.Abs(hyp);
                    }
                    else
                    {
                        switch (searchDirection)
                        {
                            case Orientation.Horizontal:
                                distance = Math.Abs(position.X - p.X);
                                break;
                            case Orientation.Vertical:
                                distance = Math.Abs(position.Y - p.Y);
                                break;
                        }
                    }

                    if (distance < closestDistance)
                    {
                        closest = uiElement;
                        closestDistance = distance;
                    }
                }
            }

            return closest;
        }
    }
}