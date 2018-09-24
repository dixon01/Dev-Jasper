// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayRoutingTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayRoutingTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Gateway
{
    using Gorba.Common.Medi.IntegrationTests.Tests.Resources;

    /// <summary>
    /// Test that sends messages in an environment with a gateway.
    /// </summary>
    public class GatewayRoutingTest : GatewayRoutingTestBase
    {
        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public override void Run()
        {
            this.Run(new GatewayNodeFactory());
        }
    }
}
