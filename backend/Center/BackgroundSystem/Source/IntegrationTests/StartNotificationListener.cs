// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartNotificationListener.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StartNotificationListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests
{
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Utility.IntegrationTests;

    /// <summary>
    /// Starts the notification listener.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "NotificationListener")]
    public class StartNotificationListener : AsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the background system configuration.
        /// </summary>
        [Parameter(Mandatory = true)]
        public BackgroundSystemConfiguration BackgroundSystemConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the integration test context.
        /// </summary>
        [Parameter(Mandatory = true)]
        public IntegrationTestContext InTeContext { get; set; }

        /// <summary>
        /// Process the record.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            var listener = new NotificationListener(this.InTeContext, this.BackgroundSystemConfiguration);
            await listener.StartAsync().ConfigureAwait(false);
            this.WriteObject(listener);
        }
    }
}