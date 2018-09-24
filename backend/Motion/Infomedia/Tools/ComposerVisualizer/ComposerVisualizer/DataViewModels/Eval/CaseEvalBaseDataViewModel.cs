// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseEvalBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CaseDynamicPropertyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// The case dynamic property data view model.
    /// </summary>
    public class CaseEvalBaseDataViewModel : DynamicPropertyDataViewModel
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [ReadOnly(true)]
        public string Value { get; set; }
    }
}