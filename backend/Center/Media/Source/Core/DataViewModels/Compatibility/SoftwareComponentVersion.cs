// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwareComponentVersion.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The software component version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Compatibility
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The software component version.
    /// </summary>
    public class SoftwareComponentVersion
    {
        /// <summary>
        /// The version segments.
        /// </summary>
        public readonly List<int> VersionSegments;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwareComponentVersion"/> class.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If version string is empty an exception is thrown.
        /// </exception>
        public SoftwareComponentVersion(string version)
        {
            var segments = version.Split('.');
            if (segments.Length == 0 || string.IsNullOrEmpty(version))
            {
                throw new ArgumentException("Could not splitt version string.");
            }

            this.VersionSegments = new List<int>();
            foreach (var segment in segments)
            {
                var value = Convert.ToInt32(segment);
                if (value < 0)
                {
                    throw new ArgumentException("Negativer version not allowed");
                }

                this.VersionSegments.Add(value);
            }
        }

        /// <summary>
        /// The ==. Equality is checked only until one or both do not have another segment
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// True if all index positions are equal.
        /// </returns>
        public static bool operator ==(SoftwareComponentVersion a, SoftwareComponentVersion b)
        {
            // If both are null, or both are same instance
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if all segments match
            var segmentsA = a.VersionSegments;
            var segmentsB = b.VersionSegments;
            var maxIndex = Math.Min(segmentsA.Count, segmentsB.Count);

            for (var i = 0; i < maxIndex; i++)
            {
                if (segmentsA[i] != segmentsB[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// False if all index positions are equal.
        /// </returns>
        public static bool operator !=(SoftwareComponentVersion a, SoftwareComponentVersion b)
        {
            return !(a == b);
        }

        /// <summary>
        /// The &gt;=.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// True if all version segments of a are bigger or equal than b.
        /// </returns>
        public static bool operator >=(SoftwareComponentVersion a, SoftwareComponentVersion b)
        {
            // If both are null, or both are same instance
            if (ReferenceEquals(a, b))
            {
                return false;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if all segments match
            var segmentsA = a.VersionSegments;
            var segmentsB = b.VersionSegments;
            var maxIndex = Math.Min(segmentsA.Count, segmentsB.Count);

            for (var i = 0; i < maxIndex; i++)
            {
                if (segmentsA[i] < segmentsB[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The &lt;=.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// True if all version segments of b are bigger or equal than a.
        /// </returns>
        public static bool operator <=(SoftwareComponentVersion a, SoftwareComponentVersion b)
        {
            return b >= a;
        }

        /// <summary>
        /// The &gt;.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// True if at least one segment of a is bigger than b from high to low
        /// </returns>
        public static bool operator >(SoftwareComponentVersion a, SoftwareComponentVersion b)
        {
            // If both are null, or both are same instance
            if (ReferenceEquals(a, b))
            {
                return false;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if all segments match
            var segmentsA = a.VersionSegments;
            var segmentsB = b.VersionSegments;
            var maxIndex = Math.Min(segmentsA.Count, segmentsB.Count);

            for (var i = 0; i < maxIndex; i++)
            {
                if (segmentsA[i] < segmentsB[i])
                {
                    return false;
                }

                if (segmentsA[i] > segmentsB[i])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The &lt;.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// True if at least one segment of b is bigger than a from high to low
        /// </returns>
        public static bool operator <(SoftwareComponentVersion a, SoftwareComponentVersion b)
        {
            return b > a;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((SoftwareComponentVersion)obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.VersionSegments != null ? this.VersionSegments.GetHashCode() : 0;
        }

        /// <summary>
        /// The get version string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetVersionString()
        {
            return string.Join(".", this.VersionSegments);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool Equals(SoftwareComponentVersion other)
        {
            return object.Equals(this.VersionSegments, other.VersionSegments);
        }
    }
}