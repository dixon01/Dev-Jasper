// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumGioomPortController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumGioomPortController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Gioom
{
    using System;

    using Gorba.Center.Diag.Core.ViewModels.Gioom;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// Controller for GIOoM ports with values of type <see cref="EnumValues"/>.
    /// </summary>
    public class EnumGioomPortController : GioomPortControllerBase<EnumValues, EnumGioomPortViewModel>
    {
        /// <summary>
        /// Prepares the view model with the given possible values.
        /// </summary>
        /// <param name="values">
        /// The possible values of the port.
        /// </param>
        protected override void PrepareViewModel(EnumValues values)
        {
            Array.ForEach(
                values.Values,
                v => this.ViewModel.PossibleValues.Add(new IOValueViewModel(v.Name, v.Value)));
        }
    }
}