// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingEvaluatorDataViewModel.cs" company="Gorba AG">
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
    /// Csv mapping evaluator data view model
    /// </summary>
    [DisplayName(@"CSV Mapping Evaluator")]
    public class CsvMappingEvaluatorDataViewModel : EvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvMappingEvaluatorDataViewModel"/> class.
        /// </summary>
        public CsvMappingEvaluatorDataViewModel()
        {
            this.Matches = new List<EvaluatorBaseDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the handler default value.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvaluatorBaseDataViewModel DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the handlers matches.
        /// </summary>
        [Browsable(false)]
        public List<EvaluatorBaseDataViewModel> Matches { get; set; }

        /// <summary>
        /// Gets or sets the csv matches.
        /// </summary>
        [DisplayName(@"Matches")]
        [Editor(typeof(ReadOnlyCollectionEditor), typeof(ReadOnlyCollectionEditor))]
        public ObservableCollection<EvaluatorBaseDataViewModel> CsvMatches
        {
            get
            {
                var list = new ObservableCollection<EvaluatorBaseDataViewModel>();

                foreach (var match in this.Matches)
                {
                    list.Add(match);
                }

                return list;
            }

            set
            {
            }
        }
    }
}
