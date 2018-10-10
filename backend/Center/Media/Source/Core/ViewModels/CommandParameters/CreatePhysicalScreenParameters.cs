// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreatePhysicalScreenParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The create physical screen parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The create physical screen parameters.
    /// </summary>
    public class CreatePhysicalScreenParameters
    {
        /// <summary>
        /// Gets or sets the type of the physical screen.
        /// </summary>
        public PhysicalScreenType Type { get; set; }

        /// <summary>
        /// Gets or sets the resolution of the physical screen.
        /// </summary>
        public ResolutionConfiguration Resolution { get; set; }

        /// <summary>
        /// Gets or sets the master layout of the physical screen.
        /// </summary>
        public MasterLayout MasterLayout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is monochrome.
        /// </summary>
        public bool IsMonochrome { get; set; }
    }
}
