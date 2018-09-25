// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Or evaluator data view model
    /// </summary>
    [DisplayName(@"OR Evaluator")]
    public class OrEvaluatorDataViewModel : CollectionEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrEvaluatorDataViewModel"/> class.
        /// </summary>
        public OrEvaluatorDataViewModel()
        {
            this.OrConditions = new List<EvaluatorBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the evaluation conditions.
        /// </summary>
        [Browsable(false)]
        public List<EvaluatorBaseDataViewModel> OrConditions { get; set; }

        /// <summary>
        /// Gets or sets the evaluation conditions.
        /// </summary>
        [DisplayName(@"OrConditions")]
        [Editor(typeof(ReadOnlyCollectionEditor), typeof(ReadOnlyCollectionEditor))]
        public ObservableCollection<EvaluatorBaseDataViewModel> Conditions
        {
            get
            {
                var list = new ObservableCollection<EvaluatorBaseDataViewModel>();

                foreach (var condition in this.OrConditions)
                {
                    list.Add(condition);
                }

                return list;
            }

            set
            {
            }
        }
    }
}
