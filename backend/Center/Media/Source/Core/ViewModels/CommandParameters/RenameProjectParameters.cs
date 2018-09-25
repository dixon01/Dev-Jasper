// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenameProjectParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The create project parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    /// <summary>
    /// The create project parameters.
    /// </summary>
    public class RenameProjectParameters
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string NewName { get; set; }
    }
}
