// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Version.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Version type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Defines a version for change tracking.
    /// </summary>
    public class Version : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Version"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Version(int value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the version.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Creates a new version that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new version that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public Version Clone()
        {
            return (Version)((ICloneable)this).Clone();
        }

        /// <summary>
        /// Creates a new <see cref="Version"/> incrementing the value of this instance.
        /// </summary>
        /// <returns>A new <see cref="Version"/> with the incremented value.</returns>
        public Version Increment()
        {
            return new Version(this.Value + 1);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}