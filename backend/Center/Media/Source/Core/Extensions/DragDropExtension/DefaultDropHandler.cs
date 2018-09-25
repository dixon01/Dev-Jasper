// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDropHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DefaultDropHandler.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The default drop handler
    /// </summary>
    public class DefaultDropHandler : IDropTarget
    {
        /// <summary>
        /// Checks if the target can accept the current drag load
        /// </summary>
        /// <param name="dropInfo">the drop info</param>
        /// <returns>the value indicating whether or not we can accept the data</returns>
        public static bool CanAcceptData(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return false;
            }

            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                return GetList(dropInfo.TargetCollection) != null;
            }
            
            if (dropInfo.DragInfo.SourceCollection is ItemCollection)
            {
                return false;
            }
            
            if (dropInfo.TargetCollection == null)
            {
                return false;
            }
            
            if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
            {
                return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
            }
            
            return false;
        }

        /// <summary>
        /// The method responsible for extracting the data
        /// </summary>
        /// <param name="data">the data</param>
        /// <returns>the result data</returns>
        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }

            return Enumerable.Repeat(data, 1);
        }

        /// <summary>
        /// Responsible for converting the enumerable to a list
        /// </summary>
        /// <param name="enumerable">the enumerable</param>
        /// <returns>a list</returns>
        public static IList GetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return ((ICollectionView)enumerable).SourceCollection as IList;
            }

            return enumerable as IList;
        }

        /// <summary>
        /// The handle for the drag over event
        /// </summary>
        /// <param name="dropInfo">the drop info</param>
        public virtual void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        /// <summary>
        /// the drop event handler
        /// </summary>
        /// <param name="dropInfo">the drop info</param>
        public virtual void Drop(IDropInfo dropInfo)
        {
            var insertIndex = dropInfo.InsertIndex;
            var destinationList = GetList(dropInfo.TargetCollection);
            var data = ExtractData(dropInfo.Data);

            if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
            {
                var sourceList = GetList(dropInfo.DragInfo.SourceCollection);

                foreach (var o in data)
                {
                    var index = sourceList.IndexOf(o);

                    if (index != -1)
                    {
                        sourceList.RemoveAt(index);

                        if (sourceList == destinationList && index < insertIndex)
                        {
                            --insertIndex;
                        }
                    }
                }
            }

            foreach (var o in data)
            {
                destinationList.Insert(insertIndex++, o);
            }
        }

        /// <summary>
        /// Checks if the target item is a child of the sourceItem
        /// </summary>
        /// <param name="targetItem">the target item</param>
        /// <param name="sourceItem">the source item</param>
        /// <returns>a value whether or not the target item is a child of the source item</returns>
        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (parent == sourceItem)
                {
                    return true;
                }

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        /// <summary>
        /// Tests if the types are compatible
        /// </summary>
        /// <param name="target">the target enumerable</param>
        /// <param name="data">the data</param>
        /// <returns>a value indicating whether the types are compatible</returns>
        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) => (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
            var enumerableTypes = (from i in enumerableInterfaces select i.GetGenericArguments().Single()).ToList();

            if (enumerableTypes.Any())
            {
                var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
            }
            
            return target is IList;
        }
    }
}