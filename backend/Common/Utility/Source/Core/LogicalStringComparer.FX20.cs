// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicalStringComparer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogicalStringComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// String comparer that does logical string comparison.
    /// Digits in the strings are considered as numerical content rather than text. This test is not case-sensitive.
    /// </summary>
    public partial class LogicalStringComparer : StringComparer
    {
        /// <summary>
        /// When overridden in a derived class, compares two strings and
        /// returns an indication of their relative sort order.
        /// </summary>
        /// <returns>
        /// Less than zero:
        ///                 <paramref name="x"/> is less than <paramref name="y"/>.
        ///                     -or-
        ///                 <paramref name="x"/> is null.
        /// Zero:
        ///                 <paramref name="x"/> is equal to <paramref name="y"/>.
        /// Greater than zero:
        ///                 <paramref name="x"/> is greater than <paramref name="y"/>.
        ///                     -or-
        ///                 <paramref name="y"/> is null.
        /// </returns>
        /// <param name="x">
        /// A string to compare to <paramref name="y"/>.
        /// </param>
        /// <param name="y">
        /// A string to compare to <paramref name="x"/>.
        /// </param>
        public override int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }

        /// <summary>
        /// When overridden in a derived class, indicates whether two strings are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="x"/> and <paramref name="y"/> refer to the same object,
        /// or <paramref name="x"/> and <paramref name="y"/> are equal; otherwise, false.
        /// </returns>
        /// <param name="x">
        /// A string to compare to <paramref name="y"/>.
        /// </param>
        /// <param name="y">
        /// A string to compare to <paramref name="x"/>.
        /// </param>
        public override bool Equals(string x, string y)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(x, y);
        }

        /// <summary>
        /// When overridden in a derived class, gets the hash code for the specified string.
        /// </summary>
        /// <returns>
        /// A 32-bit signed hash code calculated from the value of the <paramref name="obj"/> parameter.
        /// </returns>
        /// <param name="obj">
        /// A string.
        /// </param>
        public override int GetHashCode(string obj)
        {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj);
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int StrCmpLogicalW(string x, string y);
    }
}
