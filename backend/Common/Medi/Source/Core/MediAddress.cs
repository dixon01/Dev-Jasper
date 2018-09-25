// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediAddress.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediAddress type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Address of a node in the MEDI network.
    /// </summary>
    [Serializable]
    public class MediAddress
    {
        /// <summary>
        /// Broadcast address for MEDI events.
        /// </summary>
        [XmlIgnore]
        public static readonly MediAddress Broadcast;

        /// <summary>
        /// Empty address for messages that are not routed using addresses.
        /// </summary>
        [XmlIgnore]
        internal static readonly MediAddress Empty;

        /// <summary>
        /// Wildcard string.
        /// </summary>
        [XmlIgnore]
        internal static readonly string Wildcard;

        /// <summary>
        /// Initializes static members of the <see cref="MediAddress"/> class.
        /// </summary>
        static MediAddress()
        {
            Wildcard = "*";
            Broadcast = new MediAddress(Wildcard, Wildcard);
            Empty = new MediAddress(string.Empty, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediAddress"/> class.
        /// Unit and Application are set to a wildcard.
        /// </summary>
        public MediAddress()
            : this(Wildcard, Wildcard)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediAddress"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit name.
        /// </param>
        /// <param name="application">
        /// The application name.
        /// </param>
        public MediAddress(string unit, string application)
        {
            this.Unit = unit;
            this.Application = application;
        }

        /// <summary>
        /// Gets or sets the unit name.
        /// </summary>
        [XmlAttribute("Unit")]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        [XmlAttribute("App")]
        public string Application { get; set; }

        /// <summary>
        /// Checks whether this address is a wildcard or the same as the given address.
        /// </summary>
        /// <param name="address">
        /// The address to compare to.
        /// </param>
        /// <returns>
        /// true if the address matches.
        /// </returns>
        public bool Matches(MediAddress address)
        {
            return IsWildcardOrEqual(this.Unit, address.Unit) &&
                   IsWildcardOrEqual(this.Application, address.Application);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Unit + ":" + this.Application;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.
        /// </param>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as MediAddress;
            if (other == null)
            {
                return false;
            }

            return string.Equals(this.Unit, other.Unit) && string.Equals(this.Application, other.Application);
        }

        /// <summary>
        /// Get the hash code for this object.
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            return this.Unit.GetHashCode() ^ this.Application.GetHashCode();
        }

        private static bool IsWildcardOrEqual(string name, string match)
        {
            return name.Equals(Wildcard) || name.Equals(match);
        }
    }
}