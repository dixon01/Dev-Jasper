// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioOutputFrameDropHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The drop handler to rearrange the audio elements and adjust the ListIndex.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The drop handler to rearrange the audio elements and adjust the ListIndex.
    /// </summary>
    public class AudioOutputFrameDropHandler : DefaultDropHandler
    {
        /// <summary>
        /// Moves the item to the drop position within the list and updates the ListIndex of all elements
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
                var historyEntry = new OrderAudioElementsHistoryEntry(
                    insertIndex,
                    destinationList,
                    dataList,
                    string.Format("Move element to index {0}", depthIndex));
                var applicationController = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                applicationController.ShellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
            else
            {
                var audioElement = dataList.First() as AudioElementDataViewModelBase;
                if (audioElement == null)
                {
                    return;
                }

                var state = ServiceLocator.Current.GetInstance<IMediaApplicationController>();

                var createParams = new CreateElementParameters
                                   {
                                       InsertIndex = insertIndex,
                                       Type = this.GetElementType(audioElement)
                                   };

                var editor = state.ShellController.Shell.Editors[PhysicalScreenType.Audio] as AudioEditorViewModel;
                if (editor != null)
                {
                    editor.CreateLayoutElementCommand.Execute(createParams);
                }
                else
                {
                    throw new Exception("Could not get AudioEditorViewModel");
                }
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

        private LayoutElementType GetElementType(AudioElementDataViewModelBase element)
        {
            if (element is AudioFileElementDataViewModel)
            {
                return LayoutElementType.AudioFile;
            }

            if (element is AudioPauseElementDataViewModel)
            {
                return LayoutElementType.AudioPause;
            }

            if (element is DynamicTtsElementDataViewModel)
            {
                return LayoutElementType.DynamicTts;
            }

            if (element is TextToSpeechElementDataViewModel)
            {
                return LayoutElementType.TextToSpeech;
            }

            return LayoutElementType.Unknown;
        }
    }
}
