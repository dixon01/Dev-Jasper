// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayMultiHopRoutingTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayMultiHopRoutingTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Gateway
{
    using Gorba.Common.Medi.IntegrationTests.Tests.Resources;

    /// <summary>
    /// Test that sends messages in an environment with multiple nodes and a gateway.
    /// </summary>
    public class GatewayMultiHopRoutingTest : GatewayRoutingTestBase
    {
        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public override void Run()
        {
            this.Run(new GatewayMultiHopNodeFactory());
        }
    }
}