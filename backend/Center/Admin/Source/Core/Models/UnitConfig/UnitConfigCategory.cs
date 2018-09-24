// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigCategory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigCategory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration parameters for a category.
    /// </summary>
    public class UnitConfigCategory : IEquatable<UnitConfigCategory>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfigCategory"/> class.
        /// </summary>
        public UnitConfigCategory()
        {
            this.Parts = new List<UnitConfigPart>();
        }

        /// <summary>
        /// Gets or sets the unique key of this category.
        /// </summary>
        [XmlAttribute]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the list of parts.
        /// </summary>
        [XmlElement("Part")]
        public List<UnitConfigPart> Parts { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(UnitConfigCategory other)
        {
            if (other == null || other.Parts.Count != this.Parts.Count || !this.Key.Equals(other.Key))
            {
                return false;
            }

            return this.Parts.All(p => other.Parts.Any(p.Equals));
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
            return this.Equals(obj as UnitConfigCategory);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Parts.Count ^ this.Key.GetHashCode();
        }
    }
}