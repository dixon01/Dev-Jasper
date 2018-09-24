// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Generic evaluation data view model
    /// </summary>
    public class GenericEvalDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        [ReadOnly(true)]
        public int Language { get; set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        [ReadOnly(true)]
        public int Table { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        [ReadOnly(true)]
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        [ReadOnly(true)]
        public int Row { get; set; }
    }
}
