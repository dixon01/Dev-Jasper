// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// And evaluator data view model
    /// </summary>
    [DisplayName(@"AND Evaluator")]
    public class AndEvaluatorDataViewModel : CollectionEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndEvaluatorDataViewModel"/> class.
        /// </summary>
        public AndEvaluatorDataViewModel()
        {
            this.AndConditions = new List<EvaluatorBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the evaluation conditions.
        /// </summary>
        [DisplayName(@"AndConditions")]
        [Editor(typeof(ReadOnlyCollectionEditor), typeof(ReadOnlyCollectionEditor))]
        public ObservableCollection<EvaluatorBaseDataViewModel> Conditions
        {
            get
            {
                var list = new ObservableCollection<EvaluatorBaseDataViewModel>();

                foreach (var condition in this.AndConditions)
                {
                    list.Add(condition);
                }

                return list;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the evaluation conditions for AND evaluator.
        /// </summary>
        [Browsable(false)]
        public List<EvaluatorBaseDataViewModel> AndConditions { get; set; }
    }
}
