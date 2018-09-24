// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Generic evaluator data view model
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Generic evaluator data view model
    /// </summary>
    [DisplayName(@"Generic Evaluator")]
    public class GenericEvaluatorDataViewModel : EvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the evaluation.
        /// </summary>
        [ReadOnly(true)]
        [ExpandableObject]
        public GenericEvalDataViewModel Generic { get; set; }
    }
}
