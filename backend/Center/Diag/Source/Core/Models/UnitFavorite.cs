// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitFavorite.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitFavorite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// An entry in the application state that represents a favorited unit.
    /// </summary>
    [DataContract(Name = "UnitFavorite")]
    public class UnitFavorite
    {
        /// <summary>
        /// Gets or sets the name of the unit. This can be null.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the (IP) address of the unit. This should never be null.
        /// </summary>
        [DataMember]
        public string Address { get; set; }
    }
}