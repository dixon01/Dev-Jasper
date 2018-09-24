// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandItemDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Data view model of the analog clock handle item
    /// </summary>
    public class AnalogClockHandItemDataViewModel : ImageItemDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogClockHandItemDataViewModel"/> class.
        /// </summary>
        public AnalogClockHandItemDataViewModel()
        {
            this.Mode = new AnalogClockHandMode();
        }

        /// <summary>
        /// Gets or sets the mode of the clock hand.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public AnalogClockHandMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the center x.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int CenterX { get; set; }

        /// <summary>
        /// Gets or sets the center y.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        public int CenterY { get; set; }

        /// <summary>
        /// Converts the filename to a string.
        /// </summary>
        /// <returns>
        /// The actual filename from the path<see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return Path.GetFileName(this.Filename.Value.ToString());
        }
    }
}
