// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitIoInfoSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitIoInfoSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    /// <summary>
    /// The view model for the section that shows all I/O's of a unit.
    /// </summary>
    public class UnitIoInfoSectionViewModel : InfoSectionViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitIoInfoSectionViewModel"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        public UnitIoInfoSectionViewModel(UnitViewModelBase unit)
            : base(unit)
        {
        }
    }
}