// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerEvalBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Container evaluation base data view model
    /// </summary>
    public class ContainerEvalBaseDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Evaluation { get; set; }
    }
}
