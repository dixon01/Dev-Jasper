// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatchEvalBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Match dynamic property data view model
    /// </summary>
    public class MatchEvalBaseDataViewModel : DynamicPropertyDataViewModel
    {
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        [ReadOnly(true)]
        public int Column { get; set; }
    }
}
