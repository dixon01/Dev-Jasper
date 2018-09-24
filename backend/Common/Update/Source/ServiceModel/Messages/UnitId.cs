// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitId.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Unique identification for a unit - currently its tenant ID and unit name.
    /// </summary>
    public class UnitId : IEquatable<UnitId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitId"/> class.
        /// </summary>
        public UnitId()
        {
            this.UnitName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitId"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        public UnitId(string unitName)
        {
            this.UnitName = unitName;
        }

        /// <summary>
        /// Gets or sets the unit name.
        /// </summary>
        [XmlAttribute("Name")]
        public string UnitName { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(UnitId other)
        {
            return other != null && this.UnitName.Equals(other.UnitName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        public override bool Equals(object obj)
        {
            var other = obj as UnitId;
            return this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.UnitName.GetHashCode();
        }
    }
}