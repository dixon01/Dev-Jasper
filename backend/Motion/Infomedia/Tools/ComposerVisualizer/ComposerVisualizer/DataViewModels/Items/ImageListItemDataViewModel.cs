// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListItemDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Data view model of the image list item
    /// </summary>
    [DisplayName(@"ImageList Properties")]
    public class ImageListItemDataViewModel : DrawableItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListItemDataViewModel"/> class.
        /// </summary>
        public ImageListItemDataViewModel()
        {
            this.Overflow = new TextOverflow();
            this.Align = new HorizontalAlignment();
            this.Direction = new TextDirection();
            this.FallbackImage = string.Empty;
            this.Images = new EvaluatorBaseDataViewModel();
        }

        /// <summary>
        /// Gets or sets the overflow.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public TextOverflow Overflow { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public HorizontalAlignment Align { get; set; }

        /// <summary>
        /// Gets or sets the direction of the text.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public TextDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the horizontal image gap.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int HorizontalImageGap { get; set; }

        /// <summary>
        /// Gets or sets the vertical image gap.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int VerticalImageGap { get; set; }

        /// <summary>
        /// Gets or sets the image width.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the image height.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the fallback image.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public string FallbackImage { get; set; }

        /// <summary>
        /// Gets or sets the list of images.
        /// </summary>
        [Category("Content")]
        [DisplayName(@"Images*")]
        [ReadOnly(true)]
        [ExpandableObject]
        public EvaluatorBaseDataViewModel Images { get; set; }
    }
}
