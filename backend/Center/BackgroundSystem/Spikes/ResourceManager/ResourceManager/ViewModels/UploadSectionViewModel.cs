// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UploadSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels
{
    using System.Windows.Input;

    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Controllers;
    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Utility;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// Defines the section to upload a resource.
    /// </summary>
    public class UploadSectionViewModel : ViewModelBase
    {
        private bool? lastUploadSucceeded;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadSectionViewModel"/> class.
        /// </summary>
        public UploadSectionViewModel()
        {
            this.UploadCommand = CreateUploadCommand();
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the last operation was successful or not.
        /// </summary>
        public bool? LastUploadSucceeded
        {
            get
            {
                return this.lastUploadSucceeded;
            }

            set
            {
                if (value.Equals(this.lastUploadSucceeded))
                {
                    return;
                }

                this.lastUploadSucceeded = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the upload command.
        /// </summary>
        public ICommand UploadCommand { get; private set; }

        private static ICommand CreateUploadCommand()
        {
            var controller = DependencyResolver.Current.Get<ApplicationController>();
            return new AsyncCommand(() => controller.Upload());
        }
    }
}