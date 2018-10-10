// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextItemDataViewModel.cs" company="Gorba AG">
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
    /// Data view model of the text item
    /// </summary>
    [DisplayName(@"Text Properties")]
    public class TextItemDataViewModel : DrawableItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextItemDataViewModel"/> class.
        /// </summary>
        public TextItemDataViewModel()
        {
            this.Align = new HorizontalAlignment();
            this.VAlign = new VerticalAlignment();
            this.Overflow = new TextOverflow();
            this.Font = new Font();
            this.Text = new EvaluatorBaseDataViewModel();
        }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int Rotation { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public HorizontalAlignment Align { get; set; }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public VerticalAlignment VAlign { get; set; }

        /// <summary>
        /// Gets or sets the overflow.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public TextOverflow Overflow { get; set; }

        /// <summary>
        /// Gets or sets the scroll speed for the text.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int ScrollSpeed { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public Font Font { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [Category("Content")]
        [DisplayName(@"Text*")]
        [ReadOnly(true)]
        [ExpandableObject]
        public EvaluatorBaseDataViewModel Text { get; set; }
    }
}
