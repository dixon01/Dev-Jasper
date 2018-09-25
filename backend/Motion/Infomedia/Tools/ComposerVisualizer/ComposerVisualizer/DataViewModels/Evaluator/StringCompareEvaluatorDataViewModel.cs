// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringCompareEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// String compare evaluator data view model
    /// </summary>
    [DisplayName(@"String Compare Evaluator")]
    public class StringCompareEvaluatorDataViewModel : ContainerEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the string compare evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public StringCompareEvalDataViewModel StringCompare { get; set; }
    }
}
