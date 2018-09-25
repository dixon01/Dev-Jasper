// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadDataPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoadDataPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Init
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The view model for the load data part.
    /// </summary>
    public class LoadDataPartViewModel : SingleEditorPartViewModelBase<LoadDataEditorViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadDataPartViewModel"/> class.
        /// </summary>
        public LoadDataPartViewModel()
            : base(new LoadDataEditorViewModel())
        {
        }
    }
}
