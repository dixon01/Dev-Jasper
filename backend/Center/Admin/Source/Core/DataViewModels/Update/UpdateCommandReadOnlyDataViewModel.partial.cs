// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandReadOnlyDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommandReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Update
{
    /// <summary>
    /// Partial implementation of <see cref="UpdateCommandReadOnlyDataViewModel"/>.
    /// </summary>
    public partial class UpdateCommandReadOnlyDataViewModel
    {
        // ReSharper disable once RedundantAssignment
        partial void GetDisplayText(ref string displayText)
        {
            displayText = string.Format("{0}: {1}", this.Unit, this.UpdateIndex);
        }
    }
}