// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleItemDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RectangleItemDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    /// <summary>
    /// The rectangle item data view model.
    /// </summary>
    [DisplayName(@"Rectangle Properties")]
    public class RectangleItemDataViewModel : DrawableItemBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public string Color { get; set; }
    }
}
