// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectResourceParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    /// <summary>
    /// Defines the set of parameters required to update the counters of a selected resource.
    /// </summary>
    public class SelectResourceParameters
    {
        /// <summary>
        /// Gets or sets the hash of the previously selected resource.
        /// </summary>
        public string PreviousSelectedResourceHash { get; set; }

        /// <summary>
        /// Gets or sets the hash of the current selected resource.
        /// </summary>
        public string CurrentSelectedResourceHash { get; set; }
    }
}
