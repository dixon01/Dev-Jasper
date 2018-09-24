// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFlagGioomPortViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumFlagGioomPortViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// View model that represents a GIOoM port with values of type <see cref="EnumFlagValues"/>.
    /// </summary>
    public class EnumFlagGioomPortViewModel : GioomPortViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFlagGioomPortViewModel"/> class.
        /// </summary>
        public EnumFlagGioomPortViewModel()
        {
            this.PossibleValues = new ObservableItemCollection<SelectableIOValueViewModel>();
        }

        /// <summary>
        /// Gets the list of possible values that can be selected.
        /// </summary>
        public ObservableItemCollection<SelectableIOValueViewModel> PossibleValues { get; private set; }
    }
}
