// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Switch evaluator data view model
    /// </summary>
    [DisplayName(@"Switch Evaluator")]
    public class SwitchEvaluatorDataViewModel : EvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchEvaluatorDataViewModel"/> class.
        /// </summary>
        public SwitchEvaluatorDataViewModel()
        {
            this.Cases = new List<EvaluatorBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the handler value.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel SwitchValue { get; set; }

        /// <summary>
        /// Gets or sets the handlers cases.
        /// </summary>
        [Browsable(false)]
        public List<EvaluatorBaseDataViewModel> Cases { get; set; }

        /// <summary>
        /// Gets or sets the handler default.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel Default { get; set; }

        /// <summary>
        /// Gets or sets the handlers cases.
        /// </summary>
        [DisplayName(@"Cases")]
        [Editor(typeof(ReadOnlyCollectionEditor), typeof(ReadOnlyCollectionEditor))]
        public ObservableCollection<EvaluatorBaseDataViewModel> AllCases
        {
            get
            {
                var list = new ObservableCollection<EvaluatorBaseDataViewModel>();

                foreach (var switchCase in this.Cases)
                {
                    list.Add(switchCase);
                }

                return list;
            }

            set
            {
            }
        }
    }
}
