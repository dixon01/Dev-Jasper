// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValuesBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValuesBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core.Values
{
    using System;

    /// <summary>
    /// Base class for all classes that tell the range of possible values for a port.
    /// You can't create your own subclasses; please use the four available subclasses.
    /// </summary>
    public abstract class ValuesBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValuesBase"/> class.
        /// Constructor to prevent subclasses from different assemblies.
        /// </summary>
        internal ValuesBase()
        {
        }

        /// <summary>
        /// Creates a valid I/O value for the given <see cref="value"/>.
        /// </summary>
        /// <param name="value">
        /// The integer value to be converted into an <see cref="IOValue"/>.
        /// </param>
        /// <returns>
        /// The resulting <see cref="IOValue"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <see cref="value"/> is outside the allowed range of I/O values.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <see cref="value"/> is not a I/O valid value.
        /// </exception>
        internal abstract IOValue CreateValue(int value);
    }
}