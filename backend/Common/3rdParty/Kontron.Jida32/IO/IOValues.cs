// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOValues.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOValues type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32.IO
{
    /// <summary>
    /// Wrapper for an array of 8 items from an I/O port.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items in this array.
    /// </typeparam>
    public class IOValues<T>
    {
        private readonly T[] values = new T[8];

        /// <summary>
        /// Gets the length, always 8.
        /// </summary>
        public int Length
        {
            get
            {
                return this.values.Length;
            }
        }

        /// <summary>
        /// Gets or sets the value at the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        public T this[int index]
        {
            get
            {
                return this.values[index];
            }

            set
            {
                this.values[index] = value;
            }
        }
    }
}