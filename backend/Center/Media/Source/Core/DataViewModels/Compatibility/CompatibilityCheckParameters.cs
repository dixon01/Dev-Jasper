// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompatibilityCheckParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Parameters for compatibility check
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Compatibility
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.Models;

    /// <summary>
    /// The compatibility check parameters.
    /// </summary>
    public class CompatibilityCheckParameters
    {
        /// <summary>
        /// Gets or sets the media application state.
        /// </summary>
        public IMediaApplicationState MediaApplicationState { get; set; }

        /// <summary>
        /// Gets or sets the software configuration to test against.
        /// </summary>
        public List<FeatureComponentRequirements.SoftwareConfig> SoftwareConfigs { get; set; }

        /// <summary>
        /// Gets or sets the update group name.
        /// </summary>
        public string UpdateGroupName { get; set; }
    }
}