// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupReadOnlyDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGroupReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Update
{
    /// <summary>
    /// The update group read only data view model.
    /// </summary>
    public partial class UpdateGroupReadOnlyDataViewModel
    {
        private UpdatePartsTimelineViewModel updatePartsVisualization;

        /// <summary>
        /// Gets the view model for the update parts timeline.
        /// </summary>
        public UpdatePartsTimelineViewModel UpdatePartsTimeline
        {
            get
            {
                return this.updatePartsVisualization
                       ?? (this.updatePartsVisualization = new UpdatePartsTimelineViewModel(this.UpdateParts));
            }
        }
    }
}
