// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Assert.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Assert type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Utils
{
    /// <summary>
    /// Our implementation of assertions.
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Asserts that two objects are equal.
        /// </summary>
        /// <param name="expected">
        /// The expected.
        /// </param>
        /// <param name="actual">
        /// The actual.
        /// </param>
        /// <exception cref="IntegrationTestException">
        /// if the two objects are not the same.
        /// </exception>
        public static void AreEqual(object expected, object actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new IntegrationTestException(
                    string.Format("Expected {0}, but got {1}", expected, actual));
            }
        }

        /// <summary>
        /// Asserts that an object is not null.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <exception cref="IntegrationTestException">
        /// if the given object is null.
        /// </exception>
        public static void IsNotNull(object obj)
        {
            if (obj == null)
            {
                throw new IntegrationTestException("Expected non-null object");
            }
        }

        /// <summary>
        /// Asserts that an object is null.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <exception cref="IntegrationTestException">
        /// if the given object is not null.
        /// </exception>
        public static void IsNull(object obj)
        {
            if (obj != null)
            {
                throw new IntegrationTestException("Expected null object");
            }
        }
    }
}
