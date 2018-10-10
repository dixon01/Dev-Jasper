// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalItemBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Data view model of the graphical item base
    /// </summary>
    public class GraphicalItemBaseDataViewModel : ScreenItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicalItemBaseDataViewModel"/> class.
        /// </summary>
        public GraphicalItemBaseDataViewModel()
        {
            this.Visible = new EvaluatorBaseDataViewModel();
            this.Visible.Value = true;
        }

        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        [Category("Location")]
        [ReadOnly(true)]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        [Category("Location")]
        [ReadOnly(true)]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [Category("Location")]
        [ReadOnly(true)]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [Category("Location")]
        [ReadOnly(true)]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is visible.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Visible*")]
        public EvaluatorBaseDataViewModel Visible { get; set; }
    }
}
