// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioDragHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The drag handler to create audio elements at a predefined position.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Windows;
    using System.Windows.Controls;

    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The drag handler to create audio elements at a predefined position.
    /// </summary>
    public class AudioDragHandler : IDragSource
    {
        /// <summary>
        /// the start drag handler
        /// </summary>
        /// <param name="dragInfo">the drag info</param>
        public virtual void StartDrag(IDragInfo dragInfo)
        {
            var source = dragInfo.VisualSource as Button;
            if (source == null || source.Tag == null)
            {
                return;
            }

            var shell = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.Shell;
            var type = (LayoutElementType)source.Tag;
            switch (type)
            {
                case LayoutElementType.AudioFile:
                    dragInfo.Data = new AudioFileElementDataViewModel(shell);
                    break;
                case LayoutElementType.AudioPause:
                    dragInfo.Data = new AudioPauseElementDataViewModel(shell);
                    break;
                case LayoutElementType.TextToSpeech:
                    dragInfo.Data = new TextToSpeechElementDataViewModel(shell);
                    break;
                case LayoutElementType.DynamicTts:
                    dragInfo.Data = new DynamicTtsElementDataViewModel(shell);
                    break;
            }

            dragInfo.Effects = (dragInfo.Data != null) ?
                DragDropEffects.Copy | DragDropEffects.Move :
                DragDropEffects.None;
        }

        /// <summary>
        /// Determines if we can start dragging
        /// </summary>
        /// <param name="dragInfo">the drag info</param>
        /// <returns>whether or not we can start dragging</returns>
        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        /// <summary>
        /// the dropped handler
        /// </summary>
        /// <param name="dropInfo">the drop info</param>
        public virtual void Dropped(IDropInfo dropInfo)
        {
        }

        /// <summary>
        /// the drag cancelled handler
        /// </summary>
        public virtual void DragCancelled()
        {
        }
    }
}
