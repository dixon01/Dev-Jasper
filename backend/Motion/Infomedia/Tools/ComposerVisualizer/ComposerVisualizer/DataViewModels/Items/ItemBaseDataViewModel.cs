// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The item base data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    /// <summary>
    /// The item base data view model.
    /// </summary>
    public class ItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemBaseDataViewModel"/> class.
        /// </summary>
        public ItemBaseDataViewModel()
        {
            this.ImagePath = string.Empty;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Category("Item ID")]
        [ReadOnly(true)]
        [Browsable(false)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        [Browsable(false)]
        public string ImagePath { get; set; }
    }
}