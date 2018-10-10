// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiHopResourceSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiHopResourceSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    /// <summary>
    /// The multi hop resource send test.
    /// </summary>
    public class MultiHopResourceSendTest : ResourceSendTestBase
    {
        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and 
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public override void Run()
        {
            this.Run(new MultiHopNodeFactory());
        }
    }
}
