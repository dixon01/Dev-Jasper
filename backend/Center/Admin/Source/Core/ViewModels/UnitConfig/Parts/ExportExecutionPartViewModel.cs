// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportExecutionPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportExecutionPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;

    /// <summary>
    /// The view model for the export execution part.
    /// </summary>
    public class ExportExecutionPartViewModel : SingleEditorPartViewModelBase<ExportExecutionEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportExecutionPartViewModel"/> class.
        /// </summary>
        public ExportExecutionPartViewModel()
            : base(new ExportExecutionEditorViewModel())
        {
        }
    }
}