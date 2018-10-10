// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSectionConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageSectionConfigDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Section
{
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
    /// The data view model for the image section.
    /// </summary>
    public partial class ImageSectionConfigDataViewModel
    {
        /// <summary>
        /// Gets or sets the image resource shown in this section.
        /// </summary>
        public ResourceInfoDataViewModel Image
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
                        model => Path.GetFileName(model.Filename) == this.Filename.Value);
                return resource;
            }

            set
            {
                if (value == null)
                {
                    this.Filename.Value = string.Empty;
                    return;
                }

                this.Filename.Value = Path.GetFileName(value.Filename);
            }
        }

        partial void Initialize(ImageSectionConfigDataModel dataModel)
        {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void Initialize(ImageSectionConfigDataViewModel dataViewModel)
        {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void ExportNotGeneratedValues(ImageSectionConfig model, object exportParameters)
        {
            if (model.Filename != null)
            {
                model.Filename = Path.Combine(Settings.Default.ImageExportPath, model.Filename);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Filename")
            {
                this.RaisePropertyChanged(() => this.Image);
                this.Image.UpdateIsUsedVisible();
                return;
            }

            if (e.PropertyName == "Enabled")
            {
                this.Image.UpdateIsUsedVisible();
            }
        }
    }
}
