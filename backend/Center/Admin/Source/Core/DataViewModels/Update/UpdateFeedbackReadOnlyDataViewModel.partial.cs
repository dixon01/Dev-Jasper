// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFeedbackReadOnlyDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateFeedbackReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Update
{
    /// <summary>
    /// Partial implementation of <see cref="UpdateFeedbackReadOnlyDataViewModel"/>.
    /// </summary>
    public partial class UpdateFeedbackReadOnlyDataViewModel
    {
        // ReSharper disable once RedundantAssignment
        partial void GetDisplayText(ref string displayText)
        {
            displayText = string.Format("{0} {1}", this.Timestamp, this.State);
        }
    }
}
