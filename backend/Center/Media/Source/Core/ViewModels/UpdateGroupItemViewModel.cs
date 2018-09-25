// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupItemViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGroupItemViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Collections.Generic;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;

    /// <summary>
    /// View model for items that can be selected (e.g. in a list).
    /// </summary>
    public class UpdateGroupItemViewModel : SelectableItemViewModel<UpdateGroupReadableModel>
    {
        private bool hasCompatibilityIssue;

        private List<FeatureComponentRequirements.SoftwareConfig> componentVersions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGroupItemViewModel"/> class.
        /// </summary>
        /// <param name="updateGroupReadableModel">
        /// The update group readable model.
        /// </param>
        public UpdateGroupItemViewModel(UpdateGroupReadableModel updateGroupReadableModel)
            : base(updateGroupReadableModel)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether has compatibility issue.
        /// </summary>
        public bool HasCompatibilityIssue
        {
            get
            {
                return this.hasCompatibilityIssue;
            }

            set
            {
                this.SetProperty(ref this.hasCompatibilityIssue, value, () => this.HasCompatibilityIssue);
            }
        }

        /// <summary>
        /// Gets or sets the component versions.
        /// </summary>
        public List<FeatureComponentRequirements.SoftwareConfig> ComponentVersions
        {
            get
            {
                return this.componentVersions;
            }

            set
            {
                this.SetProperty(ref this.componentVersions, value, () => this.ComponentVersions);
            }
        }
    }
}