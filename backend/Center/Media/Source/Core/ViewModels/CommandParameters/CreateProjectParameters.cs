// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateProjectParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The create project parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using Gorba.Center.Media.Core.DataViewModels.Project;

    /// <summary>
    /// The create project parameters.
    /// </summary>
    public class CreateProjectParameters : CreatePhysicalScreenParameters
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the importing project.
        /// </summary>
        public MediaProjectDataViewModel ImportingProject { get; set; }
    }
}
