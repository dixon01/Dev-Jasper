// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicPropertyDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Dynamic property data view model
    /// </summary>
    public class DynamicPropertyDataViewModel
    {
        /// <summary>
        /// Gets or sets the evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel Evaluation { get; set; }
    }
}
