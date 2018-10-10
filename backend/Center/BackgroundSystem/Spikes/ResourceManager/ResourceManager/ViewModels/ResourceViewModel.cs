// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels
{
    using System.Windows.Input;

    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Controllers;
    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Utility;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Defines the view model for a resource.
    /// </summary>
    public class ResourceViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceViewModel"/> class.
        /// </summary>
        public ResourceViewModel()
        {
            this.DownloadCommand = this.CreateDownloadCommand();
        }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the mime type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is possible to download this resource (to discriminate local and
        /// remote resources).
        /// </summary>
        public bool CanDownload { get; set; }

        /// <summary>
        /// Gets the command to download this resource.
        /// </summary>
        public ICommand DownloadCommand { get; private set; }

        private ICommand CreateDownloadCommand()
        {
            var controller = DependencyResolver.Current.Get<ApplicationController>();
            return new AsyncCommand(() => controller.Download(this.Hash));
        }
    }
}