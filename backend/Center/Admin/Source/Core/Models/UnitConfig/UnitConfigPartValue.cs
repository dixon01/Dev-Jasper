// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigPartValue.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigPartValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration parameter for a part of a category.
    /// </summary>
    public class UnitConfigPartValue : IEquatable<UnitConfigPartValue>
    {
        /// <summary>
        /// Gets or sets the unique key of this parameter.
        /// </summary>
        [XmlAttribute]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value as a string.
        /// </summary>
        [XmlAttribute]
        public string Value { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(UnitConfigPartValue other)
        {
            return other != null && this.Key == other.Key && this.Value == other.Value;
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
            return this.Equals(obj as UnitConfigPartValue);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return (this.Key == null ? 317 : this.Key.GetHashCode())
                   ^ (this.Value == null ? 541 : this.Value.GetHashCode());
        }
    }
}