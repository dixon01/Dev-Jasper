// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberInputEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NumberInputEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The number input event arguments.
    /// </summary>
    public class NumberInputEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberInputEventArgs"/> class.
        /// </summary>
        /// <param name="n1">
        /// The first number.
        /// </param>
        /// <param name="n2">
        /// The second number.
        /// </param>
        public NumberInputEventArgs(int n1, int n2)
        {
            this.Input1 = n1;
            this.Input2 = n2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberInputEventArgs"/> class.
        /// </summary>
        /// <param name="n1">
        /// The first number.
        /// </param>
        public NumberInputEventArgs(int n1)
        {
            this.Input1 = n1;
            this.Input2 = -1;
        }

        /// <summary>
        /// Gets or sets the input 1.
        /// </summary>
        public int Input1 { get; set; }

        /// <summary>
        /// Gets or sets the input 2.
        /// </summary>
        public int Input2 { get; set; }
    }
}
