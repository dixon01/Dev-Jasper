// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageElementDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Defines the properties of an image layout element.
    /// </summary>
    public partial class ImageElementDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        /// <summary>
        /// The unset media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void UnsetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.Image != null && this.Image.Hash != string.Empty)
            {
                this.DecreaseMediaReferenceByHash(this.Image.Hash, commandRegistry);
            }

            this.ResourceManager.ImageElementManager.UnsetReferences(this);
        }

        /// <summary>
        /// The set media reference.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public override void SetMediaReference(ICommandRegistry commandRegistry)
        {
            if (this.Image != null && this.Image.Hash != string.Empty)
            {
                this.IncreaseMediaReferenceByHash(this.Image.Hash, commandRegistry);
            }

            this.ResourceManager.ImageElementManager.SetReferences(this);
        }

        /// <summary>
        /// Gets the component to be rendered.
        /// </summary>
        /// <returns>
        /// The <see cref="ComponentBase"/>.
        /// </returns>
        public override ComponentBase GetComponent()
        {
            if (this.Image == null)
            {
                return null;
            }

            return new MediaImageComponent
                            {
                                Filename = this.ResourceManager.GetResourcePath(this.Image.Hash),
                                ImageResource = this.Image,
                                Height = this.Height.Value,
                                Width = this.Width.Value,
                                Visible = this.Visible.Value,
                                X = this.X.Value,
                                Y = this.Y.Value,
                                ZIndex = this.ZIndex.Value,
                            };
        }

        partial void ExportNotGeneratedValues(ImageElement model, object exportParameters)
        {
            if (string.IsNullOrWhiteSpace(model.Filename))
            {
                Logger.Debug("No filename set.");
                return;
            }

            if (this.Image == null)
            {
                Logger.Error("Resource for filename '{0}' not found", model.Filename);
                return;
            }

            if (this.Image.Type == ResourceType.Image)
            {
                model.Filename = Path.Combine(Settings.Default.ImageExportPath, model.Filename);
            }
            else if (this.Image.Type == ResourceType.Symbol)
            {
                model.Filename = Path.Combine(Settings.Default.SymbolExportPath, model.Filename);
            }
            else
            {
                throw new Exception("Image type is not image or symbol.");
            }
        }

        partial void Initialize(ImageElementDataViewModel dataViewModel)
        {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void Initialize(Models.Layout.ImageElementDataModel dataModel)
        {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Filename")
            {
                this.RaisePropertyChanged(() => this.Image);

                if (this.Image != null && this.Image.IsLedImage)
                {
                    var dimension = this.Image.Dimension.Split('x');
                    int width;
                    int.TryParse(dimension[0], out width);
                    int height;
                    int.TryParse(dimension[1], out height);

                    if (dimension[0] != null && dimension[1] != null)
                    {
                        this.Width.Value = int.Parse(dimension[0]);
                        this.Height.Value = int.Parse(dimension[1]);
                    }
                }

                return;
            }

            if (e.PropertyName == "Visible")
            {
                if (this.Image != null)
                {
                    this.Image.UpdateIsUsedVisible();
                }
            }
        }

        private class MediaImageComponent : ImageComponent
        {
            private IBitmap bitmap;

            /// <summary>
            /// Gets or sets the image resource.
            /// </summary>
            public ResourceInfoDataViewModel ImageResource { get; set; }

            /// <summary>
            /// The get bitmap.
            /// </summary>
            /// <returns>
            /// The <see cref="IBitmap"/>.
            /// </returns>
            public override IBitmap GetBitmap()
            {
                if (this.bitmap != null)
                {
                    return this.bitmap;
                }

                if (this.ImageResource == null)
                {
                    return null;
                }

                var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
                var resourceFileName = resourceManager.GetResourcePath(this.ImageResource.Hash);

                try
                {
                    if (this.ImageResource.Filename.EndsWith(".egf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new EgfBitmap(resourceFileName);
                    }

                    if (this.ImageResource.Filename.EndsWith(".egl", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new EglBitmap(resourceFileName);
                    }

                    if (this.ImageResource.Filename.EndsWith(".egr", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new EgrBitmap(resourceFileName);
                    }

                    this.bitmap = BitmapFactory.CreateBitmap(resourceFileName);
                    return this.bitmap;
                }
                catch (Exception exception)
                {
                    Logger.ErrorException(
                        string.Format(
                            "Could not get bitmap for resource '{0}' filename '{1}'",
                            this.ImageResource.Filename,
                            resourceFileName),
                        exception);

                    var prompt = new ConnectionExceptionPrompt(
                      exception,
                      MediaStrings.ImageElement_CouldNotCreateImage,
                      MediaStrings.ImageElement_CouldNotCreateImageTitle);
                    InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);

                    return new SimpleBitmap(1, 1);
                }
            }
        }
    }
}
