// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayUnitEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayUnitEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.MainUnit
{
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// The editor view model for the load data part.
    /// </summary>
    public class DisplayUnitEditorViewModel : DataErrorViewModelBase
    {
        private bool isStaticContentSelected;

        private bool isDynamicContentSelected;

        private string staticContentFileName;

        private string dynamicContentUrl;

        private string staticContentHash;

        private ImageSource currentStaticImageSource;

        private bool previewImageIsValid;

        /// <summary>
        /// Gets or sets a value indicating whether create empty configuration is selected.
        /// </summary>
        public bool IsStaticContentSelected
        {
            get
            {
                return this.isStaticContentSelected;
            }

            set
            {
                if (this.SetProperty(ref this.isStaticContentSelected, value, () => this.IsStaticContentSelected)
                    && value)
                {
                    this.IsDynamicContentSelected = false;
                    this.MakeDirty();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether copy existing configuration is selected.
        /// </summary>
        public bool IsDynamicContentSelected
        {
            get
            {
                return this.isDynamicContentSelected;
            }

            set
            {
                if (this.SetProperty(ref this.isDynamicContentSelected, value, () => this.IsDynamicContentSelected)
                    && value)
                {
                    this.IsStaticContentSelected = false;
                    this.MakeDirty();
                }
            }
        }

        /// <summary>
        /// Gets or sets the file name of the file to be used as static content.
        /// </summary>
        public string StaticContentFileName
        {
            get
            {
                return this.staticContentFileName;
            }

            set
            {
                if (this.SetProperty(ref this.staticContentFileName, value, () => this.StaticContentFileName))
                {
                    this.MakeDirty();
                }
            }
        }

        /// <summary>
        /// Gets or sets the URL of the dynamic content source.
        /// </summary>
        public string DynamicContentUrl
        {
            get
            {
                return this.dynamicContentUrl;
            }

            set
            {
                if (this.SetProperty(ref this.dynamicContentUrl, value, () => this.DynamicContentUrl))
                {
                    this.MakeDirty();
                }
            }
        }

        /// <summary>
        /// Gets or sets the choose import file command.
        /// </summary>
        public ICommand ChooseStaticContentFileCommand { get; set; }

        /// <summary>
        /// Gets or sets the preview dynamic content in browser command.
        /// </summary>
        public ICommand PreviewDynamicContentInBrowserCommand { get; set; }

        /// <summary>
        /// Gets or sets the static content hash.
        /// </summary>
        public string StaticContentHash
        {
            get
            {
                return this.staticContentHash;
            }

            set
            {
                this.SetProperty(ref this.staticContentHash, value, () => this.StaticContentHash);
            }
        }

        /// <summary>
        /// Gets or sets the current static image source.
        /// </summary>
        public ImageSource CurrentStaticImageSource
        {
            get
            {
                return this.currentStaticImageSource;
            }

            set
            {
                this.SetProperty(ref this.currentStaticImageSource, value, () => this.CurrentStaticImageSource);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether preview image is valid.
        /// </summary>
        public bool PreviewImageIsValid
        {
            get
            {
                return this.previewImageIsValid;
            }

            set
            {
                this.SetProperty(ref this.previewImageIsValid, value, () => this.PreviewImageIsValid);
            }
        }
    }
}