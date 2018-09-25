// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Math.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Definition of math utility methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Definition of math utility methods.
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// Evaluates the Fibonacci value for the given number.
        /// </summary>
        /// <param name="n">The seed for the Fibonacci evaluation.</param>
        /// <returns>The evaluated number according to the Fibonacci sequence.</returns>
        public static int Fibonacci(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException("n", "The Fibonacci's sequence is defined only for non negative integers.");
            }

            int a = 0;
            int b = 1;

            // In N steps compute Fibonacci sequence iteratively.
            for (int i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = temp + b;
            }

            return a;
        }
    }
}
