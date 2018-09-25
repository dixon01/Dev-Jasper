// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shell.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Shell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels
{
    /// <summary>
    /// Defines the shell.
    /// </summary>
    public class Shell : ViewModelBase
    {
        private string localDirectory;

        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            this.LocalResources = new ResourceSectionViewModel();
            this.RemoteResources = new ResourceSectionViewModel();
            this.ConfigurationSection = new ConfigurationSectionViewModel();
            this.UploadSection = new UploadSectionViewModel();
            this.Status = new StatusViewModel();
        }

        /// <summary>
        /// Gets the configuration section.
        /// </summary>
        public ConfigurationSectionViewModel ConfigurationSection { get; private set; }

        /// <summary>
        /// Gets or sets the local directory.
        /// </summary>
        public string LocalDirectory
        {
            get
            {
                return this.localDirectory;
            }

            set
            {
                if (value == this.localDirectory)
                {
                    return;
                }

                this.localDirectory = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the section with local resources.
        /// </summary>
        public ResourceSectionViewModel LocalResources { get; private set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                if (value == this.title)
                {
                    return;
                }

                this.title = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the section with remote resources.
        /// </summary>
        public ResourceSectionViewModel RemoteResources { get; private set; }

        /// <summary>
        /// Gets the upload section.
        /// </summary>
        public UploadSectionViewModel UploadSection { get; private set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public StatusViewModel Status { get; private set; }
    }
}