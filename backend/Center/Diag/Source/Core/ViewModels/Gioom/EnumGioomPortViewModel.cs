// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumGioomPortViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumGioomPortViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    using System.Collections.ObjectModel;

    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// View model that represents a GIOoM port with values of type <see cref="EnumValues"/>.
    /// </summary>
    public class EnumGioomPortViewModel : GioomPortViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumGioomPortViewModel"/> class.
        /// </summary>
        public EnumGioomPortViewModel()
        {
            this.PossibleValues = new ObservableCollection<IOValueViewModel>();
        }

        /// <summary>
        /// Gets the list of possible values settable to <see cref="PossibleValues"/>.
        /// </summary>
        public ObservableCollection<IOValueViewModel> PossibleValues { get; private set; }
    }
}