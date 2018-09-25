// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IfEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// If evaluator data view model
    /// </summary>
    [DisplayName(@"If Evaluator")]
    public class IfEvaluatorDataViewModel : EvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the handler condition.
        /// </summary>
        [PropertyOrder(1)]
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel Condition { get; set; }

        /// <summary>
        /// Gets or sets the handler then.
        /// </summary>
        [PropertyOrder(2)]
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel Then { get; set; }

        /// <summary>
        /// Gets or sets the handler else.
        /// </summary>
        [PropertyOrder(3)]
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel Else { get; set; }
    }
}
