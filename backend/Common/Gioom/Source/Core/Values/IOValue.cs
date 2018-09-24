// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOValue.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Values
{
    using System;

    /// <summary>
    /// A value of a port. This object can only be instantiated through
    /// <see cref="IPort.CreateValue"/>.
    /// </summary>
    public class IOValue : IEquatable<IOValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOValue"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        internal IOValue(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets a string representation for the value.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the raw value as an integer.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IOValue other)
        {
            return other != null && other.Name == this.Name && other.Value == this.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal
        /// to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the
        /// current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with
        /// the current <see cref="T:System.Object"/>. </param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as IOValue);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() * 37 ^ this.Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Name;
        }
    }
}