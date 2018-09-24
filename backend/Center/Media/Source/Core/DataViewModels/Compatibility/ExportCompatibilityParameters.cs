// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportCompatibilityParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Compatibility
{
    using System.Collections.Generic;

    /// <summary>
    /// The export compatibility parameters.
    /// </summary>
    public class ExportCompatibilityParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCompatibilityParameters"/> class.
        /// </summary>
        public ExportCompatibilityParameters()
        {
            this.CurrentSoftwareConfigs = new List<FeatureComponentRequirements.SoftwareConfig>();
            this.CsvMappingCompatibilityRequired = false;
        }

        /// <summary>
        /// Gets or sets the currently used software configurations.
        /// </summary>
        public List<FeatureComponentRequirements.SoftwareConfig> CurrentSoftwareConfigs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether csv mapping compatibility required.
        /// </summary>
        public bool CsvMappingCompatibilityRequired { get; set; }
    }
}
