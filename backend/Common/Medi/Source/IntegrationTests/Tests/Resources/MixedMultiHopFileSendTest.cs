// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixedMultiHopFileSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MixedMultiHopFileSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// Test that sends a file from one node to another over several hops.
    /// </summary>
    public class MixedMultiHopFileSendTest : FileSendTestBase
    {
        /// <summary>
        /// Creates the node factory used by this test.
        /// </summary>
        /// <returns>
        /// The <see cref="INodeFactory"/>.
        /// </returns>
        internal override INodeFactory CreateNodeFactory()
        {
            return new MixedMultiHopNodeFactory();
        }
    }
}