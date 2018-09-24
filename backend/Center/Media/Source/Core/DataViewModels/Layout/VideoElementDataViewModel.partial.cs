// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Common.Configuration.Infomedia.Layout;

    using NLog;

    /// <summary>
    /// Defines the properties of an Video layout element.
    /// </summary>
    public partial class VideoElementDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string previewImageHash;

        /// <summary>
        /// Gets or sets the hash of the video file.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the hash of the preview image.
        /// </summary>
        public string PreviewImageHash
        {
            get
            {
                return this.previewImageHash;
            }

            set
            {
                this.SetProperty(ref this.previewImageHash, value, () => this.PreviewImageHash);
            }
        }

        /// <summary>
        /// The unset media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void UnsetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.VideoUri != null && this.VideoUri.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.VideoUri.Value);
                this.DecreaseMediaReferenceByHash(hash, commandRegistry);
            }

            this.ResourceManager.VideoElementManager.UnsetReferences(this);
        }

        /// <summary>
        /// The set media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void SetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.VideoUri != null && this.VideoUri.Value != string.Empty)
            {
                var hash = this.Shell.MediaApplicationState.CurrentProject.GetMediaHash(this.VideoUri.Value);
                this.IncreaseMediaReferenceByHash(hash, commandRegistry);
            }

            this.ResourceManager.VideoElementManager.SetReferences(this);
        }

        partial void Initialize(VideoElementDataViewModel dataViewModel)
        {
            this.PreviewImageHash = dataViewModel.PreviewImageHash;
            this.Hash = dataViewModel.Hash;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void Initialize(VideoElementDataModel dataModel)
        {
            if (dataModel != null)
            {
                this.PreviewImageHash = dataModel.PreviewImageHash;
                this.Hash = dataModel.Hash;
            }

            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void ExportNotGeneratedValues(VideoElement model, object exportParameters)
        {
            if (model.VideoUri != null)
            {
                if (!Uri.IsWellFormedUriString(model.VideoUri, UriKind.Absolute))
                {
                    model.VideoUri = Path.Combine(Settings.Default.VideoExportPath, model.VideoUri);
                }
            }

            if (string.IsNullOrWhiteSpace(model.FallbackImage))
            {
                Logger.Debug("No fallback image set.");
                return;
            }

            if (this.FallbackImage == null)
            {
                Logger.Error("Resource for fallback image '{0}' not found", model.FallbackImage);
                return;
            }

            model.FallbackImage = Path.Combine(Settings.Default.ImageExportPath, model.FallbackImage);
        }

        partial void ConvertNotGeneratedToDataModel(ref VideoElementDataModel dataModel)
        {
            dataModel.PreviewImageHash = this.PreviewImageHash;
            dataModel.Hash = this.Hash;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Shell.MediaApplicationState.CurrentProject == null)
            {
                return;
            }

            if (e.PropertyName == "VideoUri")
            {
                var resource =
                    this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                        model => Path.GetFileName(model.Filename) == this.VideoUri.Value);
                if (resource != null)
                {
                    resource.UpdateIsUsedVisible();
                }

                return;
            }

            if (e.PropertyName == "FallbackImage")
            {
                var resource =
                    this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                        model => Path.GetFileName(model.Filename) == this.FallbackImage.Value);
                if (resource != null)
                {
                    resource.UpdateIsUsedVisible();
                }

                return;
            }

            if (e.PropertyName == "Visible")
            {
                var videoResource =
                    this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                        model => Path.GetFileName(model.Filename) == this.FallbackImage.Value);
                if (videoResource != null)
                {
                    videoResource.UpdateIsUsedVisible();
                }

                var fallbackResource =
                   this.Shell.MediaApplicationState.CurrentProject.Resources.FirstOrDefault(
                       model => Path.GetFileName(model.Filename) == this.VideoUri.Value);
                if (fallbackResource != null)
                {
                    fallbackResource.UpdateIsUsedVisible();
                }
            }
        }
    }
}
