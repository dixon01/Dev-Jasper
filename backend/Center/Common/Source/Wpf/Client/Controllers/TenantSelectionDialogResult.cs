// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantSelectionDialogResult.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The tenant selection dialog result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Framework;

    /// <summary>
    /// The tenant selection dialog result.
    /// </summary>
    public class TenantSelectionDialogResult : DialogResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantSelectionDialogResult"/> class.
        /// </summary>
        /// <param name="selectedTenant">
        /// The selected tenant.
        /// </param>
        public TenantSelectionDialogResult(TenantReadableModel selectedTenant)
        {
            this.SelectedTenant = selectedTenant;
        }

        /// <summary>
        /// Gets the selected tenant.
        /// </summary>
        public TenantReadableModel SelectedTenant { get; private set; }
    }
}
