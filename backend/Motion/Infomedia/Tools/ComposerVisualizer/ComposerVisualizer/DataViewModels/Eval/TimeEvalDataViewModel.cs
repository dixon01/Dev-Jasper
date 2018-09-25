// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Rime evaluation data view model
    /// </summary>
    public class TimeEvalDataViewModel : DateTimeEvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the begin.
        /// </summary>
        [ReadOnly(true)]
        public TimeSpan Begin { get; set; }
    }
}
