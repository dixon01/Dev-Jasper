// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixedMultiHopResourceSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MixedMultiHopResourceSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// The mixed multi hop resource send test.
    /// </summary>
    public class MixedMultiHopResourceSendTest : ResourceSendTestBase
    {
        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and 
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public override void Run()
        {
            this.Run(new MixedMultiHopNodeFactory());
        }
    }
}