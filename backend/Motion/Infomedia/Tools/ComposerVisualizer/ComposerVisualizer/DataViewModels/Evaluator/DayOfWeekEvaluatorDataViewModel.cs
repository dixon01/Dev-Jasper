// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DayOfWeekEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// DayOfWeek Evaluator DataViewModel
    /// </summary>
    [DisplayName(@"Day Of Week Evaluator")]
    public class DayOfWeekEvaluatorDataViewModel : DateTimeEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the day of week evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public DayOfWeekEvalDataViewModel DayOfWeek { get; set; }
    }
}
