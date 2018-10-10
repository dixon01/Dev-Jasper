// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Format evaluation data view model
    /// </summary>
    public class FormatEvalDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatEvalDataViewModel"/> class.
        /// </summary>
        public FormatEvalDataViewModel()
        {
            this.Arguments = new List<EvalBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        [ReadOnly(true)]
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public List<EvalBaseDataViewModel> Arguments { get; set; }
    }
}
