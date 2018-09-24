// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleReconnectResourceSendTest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleReconnectResourceSendTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Resources
{
    using System;

    /// <summary>
    /// The simple reconnect resource send test.
    /// </summary>
    public class SimpleReconnectResourceSendTest : ResourceSendTestBase
    {
        private const int DisconnectAfterBytes = 100000;

        /// <summary>
        /// Runs the tests, this is called after <see cref="IIntegrationTest.Setup"/> and
        /// before <see cref="IIntegrationTest.Teardown"/>.
        /// </summary>
        public override void Run()
        {
            this.MinimumFileSize = DisconnectAfterBytes * 11 / 10; // +10%
            this.ExpectedTransferDelay = TimeSpan.FromSeconds(10);
            this.Run(new ReconnectNodeFactory(new ClientServerNodeFactory(), DisconnectAfterBytes));
        }
    }
}
