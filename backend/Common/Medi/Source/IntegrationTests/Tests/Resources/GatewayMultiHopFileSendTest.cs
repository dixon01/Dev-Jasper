// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayMultiHopFileSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayMultiHopFileSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// The gateway multi hop file send test.
    /// </summary>
    public class GatewayMultiHopFileSendTest : FileSendTestBase
    {
        /// <summary>
        /// Creates the node factory used by this test.
        /// </summary>
        /// <returns>
        /// The <see cref="INodeFactory"/>.
        /// </returns>
        internal override INodeFactory CreateNodeFactory()
        {
            return new GatewayMultiHopNodeFactory();
        }
    }
}