// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleFileSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleFileSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// Test that sends directly a file from one node to another.
    /// </summary>
    public class SimpleFileSendTest : FileSendTestBase
    {
        /// <summary>
        /// Creates the node factory used by this test.
        /// </summary>
        /// <returns>
        /// The <see cref="INodeFactory"/>.
        /// </returns>
        internal override INodeFactory CreateNodeFactory()
        {
            return new ClientServerNodeFactory();
        }
    }
}