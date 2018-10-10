// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectMediaPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectMediaPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the view model of the dialog used to select media elements.
    /// </summary>
    public class SelectMediaPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        private readonly ExtendedObservableCollection<ResourceInfoDataViewModel> recentMediaResources;

        private IMediaShell shell;

        private ResourceInfoDataViewModel selectedResource;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectMediaPrompt"/> class.
        /// </summary>
        /// <param name="mediaElement">The media element. It can be an ImageElement or VideoElement.</param>
        /// <param name="shell">The shell.</param>
        /// <param name="commandRegistry">The command registry.</param>
        /// <param name="recentMediaResources">The list of recent media files.</param>
        public SelectMediaPrompt(
            DataViewModelBase mediaElement,
            IMediaShell shell,
            ICommandRegistry commandRegistry,
            ExtendedObservableCollection<ResourceInfoDataViewModel> recentMediaResources)
        {
            this.commandRegistry = commandRegistry;
            this.MediaElement = mediaElement;
            this.PlacementTarget = mediaElement as IPlacementTarget;
            this.Shell = shell;
            this.recentMediaResources = recentMediaResources;

            if (mediaElement is ImageElementDataViewModel)
            {
                var imageElement = (ImageElementDataViewModel)mediaElement;
                if (!string.IsNullOrEmpty(imageElement.Filename.Value))
                {
                    this.SelectedResource = imageElement.Image;
                    return;
                }
            }
            else if (mediaElement is VideoElementDataViewModel)
            {
                var videoElement = mediaElement as VideoElementDataViewModel;
                if (!string.IsNullOrEmpty(videoElement.VideoUri.Value))
                {
                    this.SelectedResource =
                        this.Shell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                            r => r.Hash == videoElement.Hash);
                    return;
                }
            }
            else if (mediaElement is TextualReplacementDataViewModel)
            {
                var image = mediaElement as TextualReplacementDataViewModel;
                if (!string.IsNullOrEmpty(image.Filename.Value))
                {
                    this.SelectedResource =
                        this.Shell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                            r => r.Hash == image.Image.Hash);
                    return;
                }
            }
            else if (mediaElement is AudioFileElementDataViewModel)
            {
                var audioElement = (AudioFileElementDataViewModel)mediaElement;
                if (!string.IsNullOrEmpty(audioElement.Filename.Value))
                {
                    this.SelectedResource =
                        this.Shell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                            a => a.Hash == audioElement.AudioFile.Hash);
                    return;
                }
            }
            else
            {
                throw new NotImplementedException("Can not create media popup for this type.");
            }

            this.SelectedResource = null;
        }

        /// <summary>
        /// Gets the image element.
        /// </summary>
        /// <value>
        /// The image element.
        /// </value>
        public DataViewModelBase MediaElement { get; private set; }

        /// <summary>
        /// Gets or sets the placement target.
        /// </summary>
        /// <value>
        /// The placement target.
        /// </value>
        public IPlacementTarget PlacementTarget { get; set; }

        /// <summary>
        /// Gets or sets the dynamic text element
        /// </summary>
        public IMediaShell Shell
        {
            get
            {
                return this.shell;
            }

            set
            {
                this.SetProperty(ref this.shell, value, () => this.Shell);
            }
        }

        /// <summary>
        /// Gets the command to select a resource.
        /// </summary>
        public ICommand SelectResourceCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource);
            }
        }

        /// <summary>
        /// Gets the command to add a resource.
        /// </summary>
        public ICommand AddResourceCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.AddResource);
            }
        }

        /// <summary>
        /// Gets the command to delete a resource.
        /// </summary>
        public ICommand DeleteResourceCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.DeleteResource);
            }
        }

        /// <summary>
        /// Gets or sets the selected resource.
        /// </summary>
        /// <value>
        /// The selected resource.
        /// </value>
        public ResourceInfoDataViewModel SelectedResource
        {
            get
            {
                return this.selectedResource;
            }

            set
            {
                this.SetProperty(ref this.selectedResource, value, () => this.SelectedResource);
            }
        }

        /// <summary>
        /// Gets the recent resources.
        /// </summary>
        public ExtendedObservableCollection<ResourceInfoDataViewModel> RecentMediaResources
        {
            get
            {
                return this.recentMediaResources;
            }
        }
    }
}