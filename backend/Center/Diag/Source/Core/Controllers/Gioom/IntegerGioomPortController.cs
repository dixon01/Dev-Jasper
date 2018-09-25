// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerGioomPortController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerGioomPortController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Gioom
{
    using Gorba.Center.Diag.Core.ViewModels.Gioom;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// Controller for GIOoM ports with values of type <see cref="IntegerValues"/>.
    /// </summary>
    public class IntegerGioomPortController : GioomPortControllerBase<IntegerValues, IntegerGioomPortViewModel>
    {
        /// <summary>
        /// Prepares the view model with the given possible values.
        /// </summary>
        /// <param name="values">
        /// The possible values of the port.
        /// </param>
        protected override void PrepareViewModel(IntegerValues values)
        {
            this.ViewModel.MinValue = values.MinValue;
            this.ViewModel.MaxValue = values.MaxValue;
        }
    }
}