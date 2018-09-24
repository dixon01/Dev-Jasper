// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaybackItemBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    /// <summary>
    /// Data view model of the playback item base
    /// </summary>
    [DisplayName(@"Playback Properties")]
    public class PlaybackItemBaseDataViewModel : AudioItemBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the volume of the playback item.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int Volume { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int Priority { get; set; }
    }
}
