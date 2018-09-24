// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigData.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Root object for a unit configuration XML structure.
    /// </summary>
    [XmlRoot("UnitConfiguration")]
    public class UnitConfigData : IEquatable<UnitConfigData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfigData"/> class.
        /// </summary>
        public UnitConfigData()
        {
            this.Categories = new List<UnitConfigCategory>();
        }

        /// <summary>
        /// Gets or sets the list of categories in this configuration.
        /// </summary>
        [XmlElement("Category")]
        public List<UnitConfigCategory> Categories { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(UnitConfigData other)
        {
            if (other == null || other.Categories.Count != this.Categories.Count)
            {
                return false;
            }

            return this.Categories.All(c => other.Categories.Any(c.Equals));
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnitConfigData);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Categories.Count;
        }
    }
}
