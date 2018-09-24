// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigPart.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration parameters for a part of a category.
    /// </summary>
    public class UnitConfigPart : IEquatable<UnitConfigPart>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfigPart"/> class.
        /// </summary>
        public UnitConfigPart()
        {
            this.Values = new List<UnitConfigPartValue>();
        }

        /// <summary>
        /// Gets or sets the unique key of this part.
        /// </summary>
        [XmlAttribute]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration values.
        /// </summary>
        [XmlElement("Value")]
        public List<UnitConfigPartValue> Values { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(UnitConfigPart other)
        {
            if (other == null || other.Values.Count != this.Values.Count)
            {
                return false;
            }

            return this.Values.All(c => other.Values.Any(c.Equals));
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
            return this.Equals(obj as UnitConfigPart);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Values.Count ^ this.Key.GetHashCode();
        }
    }
}