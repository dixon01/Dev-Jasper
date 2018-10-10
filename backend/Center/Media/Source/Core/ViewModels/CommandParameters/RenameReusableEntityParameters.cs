// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenameReusableEntityParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The RenameReusableEntityParameters.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    /// <summary>
    /// Defines the set of parameters required to rename a reusable entity
    /// </summary>
    public class RenameReusableEntityParameters
    {
        /// <summary>
        /// Gets or sets the entity
        /// </summary>
        public IReusableEntity Entity { get; set; }

        /// <summary>
        /// Gets or sets the new name
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string Format = "[RenameLayoutParameters Layout: {0}, NewName: {1}]";
            return string.Format(Format, this.Entity, this.NewName);
        }
    }
}