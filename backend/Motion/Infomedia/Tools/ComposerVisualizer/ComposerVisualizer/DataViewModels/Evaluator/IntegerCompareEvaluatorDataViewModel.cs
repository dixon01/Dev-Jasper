// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerCompareEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Integer compare evaluator data view model
    /// </summary>
    [DisplayName(@"Interger Compare Evaluator")]
    public class IntegerCompareEvaluatorDataViewModel : ContainerEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the integer compare evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public IntegerCompareEvalDataViewModel IntegerCompare { get; set; }
    }
}
