// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Date evaluation data view model
    /// </summary>
    public class DateEvalDataViewModel : DateTimeEvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the begin.
        /// </summary>
        [ReadOnly(true)]
        public DateTime Begin { get; set; }
    }
}
