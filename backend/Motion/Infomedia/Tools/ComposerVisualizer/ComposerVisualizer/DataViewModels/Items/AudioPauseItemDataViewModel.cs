// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioPauseItemDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Data view model of the audio pause item
    /// </summary>
    [DisplayName(@"Audio Pause Properties")]
    public class AudioPauseItemDataViewModel : PlaybackItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPauseItemDataViewModel"/> class.
        /// </summary>
        public AudioPauseItemDataViewModel()
        {
            this.Duration = new TimeSpan();
        }

        /// <summary>
        /// Gets or sets the duration of the pause.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public TimeSpan Duration { get; set; }
    }
}
