// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Time evaluator data view model
    /// </summary>
    [DisplayName(@"Time Evaluator")]
    public class TimeEvaluatorDataViewModel : DateTimeEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the time evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public TimeEvalDataViewModel Time { get; set; }
    }
}
