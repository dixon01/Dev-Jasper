// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawableItemBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    /// <summary>
    /// Data view model of the draw able item base
    /// </summary>
    public class DrawableItemBaseDataViewModel : GraphicalItemBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the z index.
        /// </summary>
        [Category("Location")]
        [ReadOnly(true)]
        public int ZIndex { get; set; }
    }
}
