// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediIntegrationTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediIntegrationTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests
{
    using System;

    /// <summary>
    /// Integration test base class for MessageDispatcher tests
    /// </summary>
    public abstract class MediIntegrationTest : IIntegrationTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediIntegrationTest"/> class.
        /// </summary>
        protected MediIntegrationTest()
        {
            this.Enabled = true;
            this.Timeout = TimeSpan.FromMinutes(2);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this test is enabled and should be executed.
        /// </summary>
        public bool Enabled { get; protected set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public TimeSpan Timeout { get; protected set; }

        /// <summary>
        /// Prepares the test, this is called before <see cref="IIntegrationTest.Run"/>.
        /// </summary>
        public virtual void Setup()
        {
        }

        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Cleans up all test resources.
        /// </summary>
        public virtual void Teardown()
        {
        }
    }
}
