// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoItemDataViewModel.cs" company="Gorba AG">
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
    /// Data view model of the video item
    /// </summary>
    [DisplayName(@"Video Properties")]
    public class VideoItemDataViewModel : DrawableItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoItemDataViewModel"/> class.
        /// </summary>
        public VideoItemDataViewModel()
        {
            this.Scaling = new ElementScaling();
            this.Replay = true;
            this.VideoUri = new EvaluatorBaseDataViewModel();
        }

        /// <summary>
        /// Gets or sets the video uri.
        /// </summary>
        [Category("Content")]
        [DisplayName(@"Video Uri*")]
        [ReadOnly(true)]
        [ExpandableObject]
        public EvaluatorBaseDataViewModel VideoUri { get; set; }

        /// <summary>
        /// Gets or sets the scaling.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public ElementScaling Scaling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to replay.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public bool Replay { get; set; }
    }
}
