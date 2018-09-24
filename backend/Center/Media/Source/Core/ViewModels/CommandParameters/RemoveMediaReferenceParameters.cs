// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveMediaReferenceParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The RemoveMediaReferenceParameters.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;

    /// <summary>
    /// Defines the set of parameters required to create a layout element
    /// </summary>
    public class RemoveMediaReferenceParameters
    {
        /// <summary>
        /// Gets or sets the pool
        /// </summary>
        public PoolConfigDataViewModel Pool { get; set; }

        /// <summary>
        /// Gets or sets the References
        /// </summary>
        public List<ResourceReferenceDataViewModel> References { get; set; }

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
            const string Format = "[RemoveMediaReferenceParameters Pool: {0}, References: {1}, modifiers: {2}]";
            return string.Format(Format, this.Pool, this.References, this.Modifiers);
        }
    }
}