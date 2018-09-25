// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedListPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NamedListPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;

    /// <summary>
    /// <see cref="PartViewModelBase"/> that contains one <see cref="NamedListEditorViewModel"/>.
    /// </summary>
    public class NamedListPartViewModel : SingleEditorPartViewModelBase<NamedListEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedListPartViewModel"/> class.
        /// </summary>
        public NamedListPartViewModel()
            : base(new NamedListEditorViewModel())
        {
        }
    }
}
