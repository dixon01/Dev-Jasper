// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationReadOnlyDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AuthorizationReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.AccessControl
{
    /// <summary>
    /// Partial implementation of <see cref="AuthorizationReadOnlyDataViewModel"/>.
    /// </summary>
    public partial class AuthorizationReadOnlyDataViewModel
    {
        // ReSharper disable once RedundantAssignment
        partial void GetDisplayText(ref string displayText)
        {
            displayText = string.Format("{0}: {1}", this.DataScope, this.Permission);
        }
    }
}
