// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryOperatorEvalBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Binary operator evaluation base data view model
    /// </summary>
    public class BinaryOperatorEvalBaseDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Left { get; set; }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Right { get; set; }
    }
}
