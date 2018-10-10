// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadTreeViewExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RadTreeViewExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Extensions
{
    using System.Windows.Controls;

    using Telerik.Windows.Controls;

    /// <summary>
    /// The rad tree view extensions.
    /// </summary>
    public static class RadTreeViewExtensions
    {
        /// <summary>
        /// The container from item.
        /// </summary>
        /// <param name="radTreeView">
        /// The rad tree view.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="RadTreeViewItem"/>.
        /// </returns>
        public static RadTreeViewItem ContainerFromItem(this RadTreeView radTreeView, object item)
        {
            var containerThatMightContainItem =
                (RadTreeViewItem)radTreeView.ItemContainerGenerator.ContainerFromItem(item);
            if (containerThatMightContainItem != null)
            {
                return containerThatMightContainItem;
            }

            return ContainerFromItem(radTreeView.ItemContainerGenerator, radTreeView.Items, item);
        }

        /// <summary>
        /// The item from container.
        /// </summary>
        /// <param name="radTreeView">
        /// The rad tree view.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ItemFromContainer(this RadTreeView radTreeView, RadTreeViewItem container)
        {
            var itemThatMightBelongToContainer =
                (RadTreeViewItem)radTreeView.ItemContainerGenerator.ItemFromContainer(container);
            if (itemThatMightBelongToContainer != null)
            {
                return itemThatMightBelongToContainer;
            }

            return ItemFromContainer(radTreeView.ItemContainerGenerator, radTreeView.Items, container);
        }

        private static RadTreeViewItem ContainerFromItem(
            ItemContainerGenerator parentItemContainerGenerator,
            ItemCollection itemCollection,
            object item)
        {
            foreach (var curChildItem in itemCollection)
            {
                var parentContainer = (RadTreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
                if (parentContainer == null)
                {
                    continue;
                }

                var containerThatMightContainItem =
                    (RadTreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                if (containerThatMightContainItem != null)
                {
                    return containerThatMightContainItem;
                }

                var recursionResult = ContainerFromItem(
                    parentContainer.ItemContainerGenerator,
                    parentContainer.Items,
                    item);
                if (recursionResult != null)
                {
                    return recursionResult;
                }
            }

            return null;
        }

        private static object ItemFromContainer(
            ItemContainerGenerator parentItemContainerGenerator,
            ItemCollection itemCollection,
            RadTreeViewItem container)
        {
            foreach (var curChildItem in itemCollection)
            {
                var parentContainer =
                    (RadTreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
                var itemThatMightBelongToContainer =
                    (RadTreeViewItem)parentContainer.ItemContainerGenerator.ItemFromContainer(container);
                if (itemThatMightBelongToContainer != null)
                {
                    return itemThatMightBelongToContainer;
                }

                var recursionResult =
                    ItemFromContainer(parentContainer.ItemContainerGenerator, parentContainer.Items, container) as
                    RadTreeViewItem;
                if (recursionResult != null)
                {
                    return recursionResult;
                }
            }

            return null;
        }
    }
}