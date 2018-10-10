// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Format evaluator data view model
    /// </summary>
    [DisplayName(@"Format Evaluator")]
    public class FormatEvaluatorDataViewModel : EvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatEvaluatorDataViewModel"/> class.
        /// </summary>
        public FormatEvaluatorDataViewModel()
        {
            this.FormatArguments = new List<EvaluatorBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the evaluation arguments.
        /// </summary>
        [Browsable(false)]
        public List<EvaluatorBaseDataViewModel> FormatArguments { get; set; }

        /// <summary>
        /// Gets or sets the evaluation arguments.
        /// </summary>
        [DisplayName(@"FormatArguments")]
        [Editor(typeof(ReadOnlyCollectionEditor), typeof(ReadOnlyCollectionEditor))]
        public ObservableCollection<EvaluatorBaseDataViewModel> Arguments
        {
            get
            {
                var list = new ObservableCollection<EvaluatorBaseDataViewModel>();

                foreach (var argument in this.FormatArguments)
                {
                    list.Add(argument);
                }

                return list;
            }

            set
            {
            }
        }
    }
}
