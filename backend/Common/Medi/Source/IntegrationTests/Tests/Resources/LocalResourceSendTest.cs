// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalResourceSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalResourceSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// The local resource send test.
    /// </summary>
    public class LocalResourceSendTest : ResourceSendTestBase
    {
        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and 
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public override void Run()
        {
            this.Run(new LocalNodeFactory());
        }
    }
}