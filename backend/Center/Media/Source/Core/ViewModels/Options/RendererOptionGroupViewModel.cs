// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererOptionGroupViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.Options
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Model.Options;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;
    using Gorba.Center.Media.Core.Models.Options;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;

    /// <summary>
    /// The renderer option group view model.
    /// </summary>
    public class RendererOptionGroupViewModel : OptionGroupViewModelBase
    {
        private TextMode selectedTextMode;
        private VideoMode selectedVideoMode;
        private FontQualities selectedFontQuality;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererOptionGroupViewModel"/> class.
        /// </summary>
        public RendererOptionGroupViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererOptionGroupViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public RendererOptionGroupViewModel(RendererOptionGroup model)
        {
            if (model != null)
            {
                VideoMode mode;
                if (Enum.TryParse(model.VideoMode, out mode))
                {
                    this.SelectedVideoMode = mode;
                }

                TextMode textMode;
                if (Enum.TryParse(model.TextMode, out textMode))
                {
                    this.SelectedTextMode = textMode;
                }

                FontQualities quality;
                if (Enum.TryParse(model.FontQuality, out quality))
                {
                    this.SelectedFontQuality = quality;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected text mode.
        /// </summary>
        public TextMode SelectedTextMode
        {
            get
            {
                return this.selectedTextMode;
            }

            set
            {
                this.SetProperty(ref this.selectedTextMode, value, () => this.SelectedTextMode);
            }
        }

        /// <summary>
        /// Gets or sets the selected font quality.
        /// </summary>
        public FontQualities SelectedFontQuality
        {
            get
            {
                return this.selectedFontQuality;
            }

            set
            {
                this.SetProperty(ref this.selectedFontQuality, value, () => this.SelectedFontQuality);
            }
        }

        /// <summary>
        /// Gets or sets the selected video mode.
        /// </summary>
        public VideoMode SelectedVideoMode
        {
            get
            {
                return this.selectedVideoMode;
            }

            set
            {
                this.SetProperty(ref this.selectedVideoMode, value, () => this.SelectedVideoMode);
            }
        }

        /// <summary>
        /// The create model.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionGroupBase"/>.
        /// </returns>
        public override OptionGroupBase CreateModel()
        {
            return new RendererOptionGroup
                       {
                           FontQuality = this.SelectedFontQuality.ToString(),
                           TextMode = this.SelectedTextMode.ToString(),
                           VideoMode = this.SelectedVideoMode.ToString()
                       };
        }
    }
}
