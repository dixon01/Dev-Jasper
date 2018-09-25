// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionEvalBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Collection Evaluation Base data view model
    /// </summary>
    public class CollectionEvalBaseDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionEvalBaseDataViewModel"/> class.
        /// </summary>
        public CollectionEvalBaseDataViewModel()
        {
            this.Conditions = new List<EvalBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        public List<EvalBaseDataViewModel> Conditions { get; set; }
    }
}
