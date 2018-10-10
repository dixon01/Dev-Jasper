// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIntegrationTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIntegrationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests
{
    using System;

    /// <summary>
    /// Interface to be implemented by all integration tests.
    /// </summary>
    public interface IIntegrationTest
    {
        /// <summary>
        /// Gets a value indicating whether this test is enabled and should be executed.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Gets the timeout after which this test is considered as failed.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Prepares the test, this is called before <see cref="Run"/>.
        /// </summary>
        void Setup();

        /// <summary>
        /// Runs the tests, this is called after <see cref="Setup"/> and
        /// before <see cref="Teardown"/>.
        /// </summary>
        void Run();

        /// <summary>
        /// Cleans up all test resources.
        /// </summary>
        void Teardown();
    }
}