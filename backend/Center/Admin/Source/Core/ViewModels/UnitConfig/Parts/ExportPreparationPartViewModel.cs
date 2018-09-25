// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportPreparationPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ExportPreparationPartViewModel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;

    /// <summary>
    /// The ExportPreparationPartViewModel.
    /// </summary>
    public class ExportPreparationPartViewModel : SingleEditorPartViewModelBase<ExportPreparationEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportPreparationPartViewModel"/> class.
        /// </summary>
        public ExportPreparationPartViewModel()
            : base(new ExportPreparationEditorViewModel())
        {
        }
    }
}