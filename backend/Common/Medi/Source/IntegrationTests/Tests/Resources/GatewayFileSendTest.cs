// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GatewayFileSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GatewayFileSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// Test that sends a file from one node to another with a gateway connection in-between.
    /// </summary>
    public class GatewayFileSendTest : FileSendTestBase
    {
        /// <summary>
        /// Creates the node factory used by this test.
        /// </summary>
        /// <returns>
        /// The <see cref="INodeFactory"/>.
        /// </returns>
        internal override INodeFactory CreateNodeFactory()
        {
            return new GatewayNodeFactory();
        }
    }
}