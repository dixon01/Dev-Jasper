// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckableTreePartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CheckableTreePartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;

    /// <summary>
    /// A part view model that contains a <see cref="CheckableTreeEditorViewModel"/>.
    /// </summary>
    public class CheckableTreePartViewModel : SingleEditorPartViewModelBase<CheckableTreeEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckableTreePartViewModel"/> class.
        /// </summary>
        public CheckableTreePartViewModel()
            : base(new CheckableTreeEditorViewModel())
        {
        }
    }
}
