// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageItemDataViewModel.cs" company="Gorba AG">
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
    /// Data view model of the image item
    /// </summary>
    [DisplayName(@"Image Properties")]
    public class ImageItemDataViewModel : DrawableItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageItemDataViewModel"/> class.
        /// </summary>
        public ImageItemDataViewModel()
        {
            this.Scaling = new ElementScaling();
            this.Blink = false;
            this.Filename = new EvaluatorBaseDataViewModel();
        }

        /// <summary>
        /// Gets or sets the filename of the image item.
        /// </summary>
        [Category("Content")]
        [DisplayName(@"Filename*")]
        [ReadOnly(true)]
        [ExpandableObject]
        public EvaluatorBaseDataViewModel Filename { get; set; }

        /// <summary>
        /// Gets or sets the scaling.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public ElementScaling Scaling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether blink is on or not.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public bool Blink { get; set; }
    }
}
