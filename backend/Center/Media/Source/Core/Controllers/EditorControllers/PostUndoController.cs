// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostUndoController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.EditorControllers
{
    using System;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Defines the controller used for the layout editor of the Media application.
    /// </summary>
    public class PostUndoController : EditorControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostUndoController"/> class.
        /// </summary>
        /// <param name="shellController">
        ///     The application controller.
        /// </param>
        /// <param name="shell">
        ///     The shell view model.
        /// </param>
        /// <param name="commandRegistry">
        ///     The command registry.
        /// </param>
        public PostUndoController(
            IMediaShellController shellController, IMediaShell shell, ICommandRegistry commandRegistry)
            : base(shellController, shell, commandRegistry)
        {
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
               typeof(StaticTextElementDataViewModel), this.OnAfterUpdateStaticTextElement);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
               typeof(DynamicTextElementDataViewModel), this.OnAfterUpdateDynamicTextElement);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
              typeof(ImageElementDataViewModel), this.OnAfterUndoUpdateImageElement);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
               typeof(LiveStreamElementDataViewModel), this.OnAfterUndoUpdateLiveStreamElement);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
               typeof(VideoElementDataViewModel), this.OnAfterUndoUpdateVideoElement);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
                typeof(AnalogClockElementDataViewModel), this.OnAfterUndoUpdateAnalogElement);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
                typeof(ImageSectionConfigDataViewModel), this.OnAfterUndoUpdateImageSection);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
                typeof(VideoSectionConfigDataViewModel), this.OnAfterUndoUpdateVideoSection);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
                typeof(AudioFileElementDataViewModel), this.OnAfterUndoUpdateAudioElement);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
                typeof(PoolSectionConfigDataViewModel), this.OnAfterUndoUpdatePoolSection);
            UpdateViewModelHistoryEntry.RegisterPostUndoHook(
                typeof(TextualReplacementDataViewModel), this.OnAfterUpdateTextualReplacement);
        }

        private void OnAfterUndoUpdatePoolSection(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousSection = (PoolSectionConfigDataViewModel)previousElement;
            var currentSection = (PoolSectionConfigDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.PoolManager.UnsetReferences(previousSection);
            manager.PoolManager.SetReferences(currentSection);

            if (currentSection.Pool != previousSection.Pool)
            {
                if (currentSection.Pool != null)
                {
                    currentSection.Pool.ReferencesCount++;
                }

                if (previousSection.Pool != null)
                {
                    if (previousSection.Pool.ReferencesCount > 0)
                    {
                        previousSection.Pool.ReferencesCount--;
                    }
                }
            }
            else
            {
                if (currentSection.Pool != null)
                {
                    currentSection.Pool.ReferencesCount++;
                }
            }
        }

        private void OnAfterUndoUpdateVideoSection(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousSection = (VideoSectionConfigDataViewModel)previousElement;
            var currentSection = (VideoSectionConfigDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.VideoSectionManager.UnsetReferences(previousSection);
            manager.VideoSectionManager.SetReferences(currentSection);
            if (currentSection.VideoUri.Value != previousSection.VideoUri.Value)
            {
                this.UpdateMediaReference(currentSection.VideoUri.Value, previousSection.VideoUri.Value);
            }
        }

        private void OnAfterUndoUpdateImageSection(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousSection = (ImageSectionConfigDataViewModel)previousElement;
            var currentSection = (ImageSectionConfigDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.ImageSectionManager.UnsetReferences(previousSection);
            manager.ImageSectionManager.SetReferences(currentSection);
            if (currentSection.Filename.Value != previousSection.Filename.Value)
            {
                this.UpdateMediaReference(currentSection.Filename.Value, previousSection.Filename.Value);
            }
        }

        private void OnAfterUndoDeleteImageSection(ImageSectionConfigDataViewModel currentElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.ImageSectionManager.UnsetReferences(currentElement);
        }

        private void OnAfterUpdateStaticTextElement(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previous = (StaticTextElementDataViewModel)previousElement;
            var current = (StaticTextElementDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();

            manager.TextElementManager.UnsetReferences(previous);
            manager.TextElementManager.SetReferences(current);
            if (this.Shell.MediaApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.LED)
            {
                InteractionManager<UpdateLedDisplayPrompt>.Current.Raise(new UpdateLedDisplayPrompt());
            }
        }

                /// <summary>
        /// The update font reference.
        /// </summary>
        /// <param name="currentFaceName">
        /// The current face name.
        /// </param>
        /// <param name="previousFaceName">
        /// The previous face name.
        /// </param>
        private void UpdateFontReference(string currentFaceName, string previousFaceName)
        {
            if (currentFaceName == previousFaceName)
            {
                return;
            }

            // is a user font
            var currentUserFont = this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                r => r.Facename == currentFaceName);
            var previousUserFont = this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                r => r.Facename == previousFaceName);

            var selectionParameters = new SelectResourceParameters
                                      {
                                          CurrentSelectedResourceHash =
                                            currentUserFont != null ? currentUserFont.Hash : null,
                                          PreviousSelectedResourceHash =
                                            previousUserFont != null ? previousUserFont.Hash : null
                                      };

            this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource)
                .Execute(selectionParameters);
        }

        private void OnAfterUpdateDynamicTextElement(
            DataViewModelBase currentElement,
            DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previous = (DynamicTextElementDataViewModel)previousElement;
            var current = (DynamicTextElementDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();

            manager.TextElementManager.UnsetReferences(previous);
            manager.TextElementManager.SetReferences(current);
            if (current.FontFace.Value != previous.FontFace.Value)
            {
                this.UpdateFontReference(current.FontFace.Value, previous.FontFace.Value);
            }

            if (this.Shell.MediaApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.LED)
            {
                InteractionManager<UpdateLedDisplayPrompt>.Current.Raise(new UpdateLedDisplayPrompt());
            }
        }

        private void OnAfterUndoUpdateVideoElement(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousVideo = (VideoElementDataViewModel)previousElement;
            var currentVideo = (VideoElementDataViewModel)currentElement;
            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.VideoElementManager.UnsetReferences(previousVideo);
            manager.VideoElementManager.SetReferences(currentVideo);
            if (currentVideo.VideoUri.Value != previousVideo.VideoUri.Value)
            {
                this.UpdateMediaReference(currentVideo.VideoUri.Value, previousVideo.VideoUri.Value);
            }
        }

        private void OnAfterUndoUpdateLiveStreamElement(
            DataViewModelBase currentElement,
            DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousVideo = (VideoElementDataViewModel)previousElement;
            var currentVideo = (VideoElementDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.VideoElementManager.UnsetReferences(previousVideo);
            manager.VideoElementManager.SetReferences(currentVideo);

            if (currentVideo.VideoUri.Value != previousVideo.VideoUri.Value)
            {
                this.UpdateMediaReference(currentVideo.VideoUri.Value, previousVideo.VideoUri.Value);
            }

            if (currentVideo.FallbackImage.Value != previousVideo.FallbackImage.Value)
            {
                this.UpdateMediaReference(currentVideo.FallbackImage.Value, previousVideo.FallbackImage.Value);
            }
        }

        private void OnAfterUndoUpdateImageElement(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousImage = (ImageElementDataViewModel)previousElement;
            var currentImage = (ImageElementDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.ImageElementManager.UnsetReferences(previousImage);
            manager.ImageElementManager.SetReferences(currentImage);

            if (currentImage.Filename.Value != previousImage.Filename.Value)
            {
                this.UpdateMediaReference(currentImage.Filename.Value, previousImage.Filename.Value);
            }
        }

        private void OnAfterUndoUpdateAudioElement(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousAudioFile = (AudioFileElementDataViewModel)previousElement;
            var currentAudioFile = (AudioFileElementDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.AudioElementManager.UnsetReferences(previousAudioFile);
            manager.AudioElementManager.SetReferences(currentAudioFile);

            if (currentAudioFile.Filename.Value != previousAudioFile.Filename.Value)
            {
                this.UpdateMediaReference(currentAudioFile.Filename.Value, previousAudioFile.Filename.Value);
            }
        }

        private void OnAfterUndoUpdateAnalogElement(DataViewModelBase currentElement, DataViewModelBase previousElement)
        {
            if (currentElement == null)
            {
                throw new ArgumentNullException("currentElement");
            }

            if (previousElement == null)
            {
                throw new ArgumentNullException("previousElement");
            }

            var previousClock = (AnalogClockElementDataViewModel)previousElement;
            var currentClock = (AnalogClockElementDataViewModel)currentElement;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.AnalogClockElementManager.UnsetReferences(previousClock);
            manager.AnalogClockElementManager.SetReferences(currentClock);

            if (currentClock.HourFilename.Value != previousClock.HourFilename.Value)
            {
                this.UpdateMediaReference(currentClock.HourFilename.Value, previousClock.HourFilename.Value);
            }

            if (currentClock.MinuteFilename.Value != previousClock.MinuteFilename.Value)
            {
                this.UpdateMediaReference(currentClock.MinuteFilename.Value, previousClock.MinuteFilename.Value);
            }

            if (currentClock.SecondsFilename.Value != previousClock.SecondsFilename.Value)
            {
                this.UpdateMediaReference(currentClock.SecondsFilename.Value, previousClock.SecondsFilename.Value);
            }
        }

        private void OnAfterUpdateTextualReplacement(DataViewModelBase current, DataViewModelBase previous)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            if (previous == null)
            {
                throw new ArgumentNullException("previous");
            }

            var currentReplacement = (TextualReplacementDataViewModel)current;
            var previousReplacement = (TextualReplacementDataViewModel)previous;

            var manager = ServiceLocator.Current.GetInstance<IResourceManager>();
            manager.TextualReplacementElementManager.UnsetReferences(previousReplacement);
            if (currentReplacement.IsImageReplacement)
            {
                manager.TextualReplacementElementManager.SetReferences(currentReplacement);
            }

            if (!currentReplacement.IsImageReplacement && !previousReplacement.IsImageReplacement)
            {
                return;
            }

            if (currentReplacement.IsImageReplacement && !previousReplacement.IsImageReplacement)
            {
                if (currentReplacement.Image != null)
                {
                    this.UpdateMediaReference(currentReplacement.Image, null);
                }
            }

            if (!currentReplacement.IsImageReplacement && previousReplacement.IsImageReplacement)
            {
                if (previousReplacement.Image != null)
                {
                    this.UpdateMediaReference(null, previousReplacement.Image);
                }
            }

            if (currentReplacement.IsImageReplacement && previousReplacement.IsImageReplacement)
            {
                if (currentReplacement.Image != null &&
                    previousReplacement.Image != null &&
                    currentReplacement.Image.Hash != previousReplacement.Image.Hash)
                {
                    this.UpdateMediaReference(currentReplacement.Image, previousReplacement.Image);
                }
                else if (currentReplacement.Image != null)
                {
                    this.UpdateMediaReference(currentReplacement.Image, null);
                }
                else if (previousReplacement.Image != null)
                {
                    this.UpdateMediaReference(null, previousReplacement.Image);
                }
            }
        }
    }
}