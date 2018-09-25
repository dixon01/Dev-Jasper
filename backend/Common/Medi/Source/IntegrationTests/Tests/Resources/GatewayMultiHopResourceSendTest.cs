// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayMultiHopResourceSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayMultiHopResourceSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// The gateway multi hop resource send test.
    /// </summary>
    public class GatewayMultiHopResourceSendTest : ResourceSendTestBase
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