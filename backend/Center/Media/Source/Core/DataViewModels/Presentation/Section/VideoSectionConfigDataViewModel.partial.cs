// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoSectionConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoSectionConfigDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Section
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The data view model for the video section.
    /// </summary>
    public partial class VideoSectionConfigDataViewModel
    {
        /// <summary>
        /// Gets or sets the video resource shown in this section.
        /// </summary>
        public ResourceInfoDataViewModel Video
        {
            get
            {
                var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
                if (state.CurrentProject == null)
                {
                    return null;
                }

                var resource =
                    state.CurrentProject.Resources.FirstOrDefault(
                        model => Path.GetFileName(model.Filename) == this.VideoUri.Value);
                return resource;
            }

            set
            {
                if (value == null)
                {
                    this.VideoUri.Value = string.Empty;
                    return;
                }

                this.VideoUri.Value = Path.GetFileName(value.Filename);
            }
        }

        partial void Initialize(VideoSectionConfigDataModel dataModel)
        {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void Initialize(VideoSectionConfigDataViewModel dataViewModel)
        {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void ExportNotGeneratedValues(VideoSectionConfig model, object exportParameters)
        {
            if (model.VideoUri != null)
            {
                if (!Uri.IsWellFormedUriString(model.VideoUri, UriKind.Absolute))
                {
                    model.VideoUri = Path.Combine(Settings.Default.VideoExportPath, model.VideoUri);
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VideoUri")
            {
                this.RaisePropertyChanged(() => this.Video);
            }
        }
    }
}