// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DayOfWeekEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Day of week evaluation data view model
    /// </summary>
    public class DayOfWeekEvalDataViewModel : DateTimeEvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether monday.
        /// </summary>
        [ReadOnly(true)]
        public bool Monday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tuesday.
        /// </summary>
        [ReadOnly(true)]
        public bool Tuesday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether wednesday.
        /// </summary>
        [ReadOnly(true)]
        public bool Wednesday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether thursday.
        /// </summary>
        [ReadOnly(true)]
        public bool Thursday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether friday.
        /// </summary>
        [ReadOnly(true)]
        public bool Friday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether saturday.
        /// </summary>
        [ReadOnly(true)]
        public bool Saturday { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sunday.
        /// </summary>
        [ReadOnly(true)]
        public bool Sunday { get; set; }
    }
}
