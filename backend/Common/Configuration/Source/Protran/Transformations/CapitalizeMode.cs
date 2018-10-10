// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CapitalizeMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CapitalizeMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    /// <summary>
    /// Possible modes of capitalization for <see cref="Capitalize"/>.
    /// </summary>
    public enum CapitalizeMode
    {
        /// <summary>
        /// Make the first character uppercase and the rest of
        /// the string lowercase.
        /// </summary>
        UpperLower,

        /// <summary>
        /// Make the first character uppercase and don't change
        /// the rest of the string.
        /// </summary>
        UpperOnly,

        /// <summary>
        /// Don't change the first character but make the rest of
        /// the string lowercase.
        /// </summary>
        LowerOnly
    }
}
