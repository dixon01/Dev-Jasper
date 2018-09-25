// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Csv mapping evaluation data view model
    /// </summary>
    public class CsvMappingEvalDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [ReadOnly(true)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the output format.
        /// </summary>
        [ReadOnly(true)]
        public string OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public EvalBaseDataViewModel DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the matches.
        /// </summary>
        public List<MatchEvalBaseDataViewModel> Matches { get; set; }
    }
}
