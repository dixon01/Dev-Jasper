// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentEnvironmentInfo.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CurrentEnvironmentInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core.PresentationPlayDataTracking
{
    /// <summary>
    /// The current environment information
    /// </summary>
    public class CurrentEnvironmentInfo
    {
        /// <summary>
        /// Gets or sets the current latitude
        /// </summary>
        public string Latitude { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current layout name.
        /// </summary>
        public string LayoutName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current longitude
        /// </summary>
        public string Longitude { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current resource id
        /// </summary>
        public string ResourceId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current resource name
        /// </summary>
        public string ResourceName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current route
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current vehicle id.
        /// </summary>
        public string VehicleId { get; set; } = string.Empty;
    }
}