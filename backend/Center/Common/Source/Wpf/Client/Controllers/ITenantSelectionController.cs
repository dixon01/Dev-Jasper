// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITenantSelectionController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TenantSelectionController interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The TenantSelectionController interface.
    /// </summary>
    public interface ITenantSelectionController : IDialogController
    {
        /// <summary>
        /// Gets the tenant selection window.
        /// </summary>
        TenantSelectionViewModel TenantSelection { get; }
    }
}