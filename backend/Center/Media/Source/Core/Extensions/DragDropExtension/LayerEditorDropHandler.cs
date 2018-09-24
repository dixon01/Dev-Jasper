// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayerEditorDropHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.History;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The drop handler to rearrange the layout elements and adjust the z-Index.
    /// </summary>
    public class LayerEditorDropHandler : DefaultDropHandler
    {
        /// <summary>
        /// Moves the item to the drop position within the list and updates the z-Index of all elements
        /// in the destination list in descending order.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop info.
        /// </param>
        public override void Drop(IDropInfo dropInfo)
        {
            var insertIndex = dropInfo.InsertIndex;
            var destinationList = GetList(dropInfo.TargetCollection);
            var data = ExtractData(dropInfo.Data);
            var dataList = data as IList<object> ?? data.Cast<object>().ToList();
            if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
            {
                var depthIndex = destinationList.Count - insertIndex;
                var applicationController = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                var selectedElementsList = applicationController.ShellController.Shell.Editor.SelectedElements;
                var historyEntry = new OrderLayerHistoryEntry(
                    insertIndex,
                    destinationList,
                    dataList,
                    selectedElementsList,
                    string.Format("Change z-Index to {0}", depthIndex));
                applicationController.ShellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }

        /// <summary>
        /// The handle for the drag over event
        /// </summary>
        /// <param name="dropInfo">the drop info</param>
        public override void DragOver(IDropInfo dropInfo)
        {
            if (DefaultDropHandler.CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }
    }
}
