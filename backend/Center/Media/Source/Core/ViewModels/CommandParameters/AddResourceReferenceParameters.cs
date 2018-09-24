// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddResourceReferenceParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the set of parameters required to add a resource reference.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;

    /// <summary>
    /// Defines the set of parameters required to add a resource reference.
    /// </summary>
    public class AddResourceReferenceParameters
    {
        /// <summary>
        /// Gets or sets the pool
        /// </summary>
        public PoolConfigDataViewModel Pool { get; set; }

        /// <summary>
        /// Gets or sets the Media
        /// </summary>
        public ResourceInfoDataViewModel Media { get; set; }

        /// <summary>
        /// Gets or sets the modifiers
        /// </summary>
        public ModifiersState Modifiers { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string Format = "[AddResourceParameters Type: {0}, Bounds: {1}, modifiers: {2}]";
            return string.Format(Format, this.Pool, this.Media, this.Modifiers);
        }
    }
}
