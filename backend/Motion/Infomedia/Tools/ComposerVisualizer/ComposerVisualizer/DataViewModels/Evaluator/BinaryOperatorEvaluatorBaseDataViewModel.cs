// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryOperatorEvaluatorBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// BinaryOperatorEvaluatorBase data view model
    /// </summary>
    public class BinaryOperatorEvaluatorBaseDataViewModel : EvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the handler left.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel LeftComparator { get; set; }

        /// <summary>
        /// Gets or sets the handler right.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel RightComparator { get; set; }
    }
}
