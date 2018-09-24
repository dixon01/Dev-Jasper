// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Date evaluator data view model
    /// </summary>
    [DisplayName(@"Date Evaluator")]
    public class DateEvaluatorDataViewModel : DateTimeEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the date evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public DateEvalDataViewModel Date { get; set; }
    }
}
