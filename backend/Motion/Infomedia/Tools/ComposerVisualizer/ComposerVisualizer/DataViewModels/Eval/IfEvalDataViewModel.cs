// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IfEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// If evaluation data view model
    /// </summary>
    public class IfEvalDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Condition { get; set; }

        /// <summary>
        /// Gets or sets the then.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Then { get; set; }

        /// <summary>
        /// Gets or sets the else.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Else { get; set; }
    }
}
