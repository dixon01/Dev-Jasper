// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePartReadOnlyDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdatePartReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Update
{
    /// <summary>
    /// Partial implementation of <see cref="UpdatePartReadOnlyDataViewModel"/>.
    /// </summary>
    public partial class UpdatePartReadOnlyDataViewModel
    {
        // ReSharper disable once RedundantAssignment
        partial void GetDisplayText(ref string displayText)
        {
            displayText = string.Format("{0} ({1})", this.Type, this.Description);
        }
    }
}