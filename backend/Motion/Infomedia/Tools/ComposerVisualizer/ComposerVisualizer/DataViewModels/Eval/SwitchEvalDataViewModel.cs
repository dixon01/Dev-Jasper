// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Switch evaluation data view model
    /// </summary>
    public class SwitchEvalDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchEvalDataViewModel"/> class.
        /// </summary>
        public SwitchEvalDataViewModel()
        {
            this.Cases = new List<CaseEvalBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Value { get; set; }

        /// <summary>
        /// Gets or sets the cases.
        /// </summary>
        public List<CaseEvalBaseDataViewModel> Cases { get; set; }

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Default { get; set; }
    }
}
