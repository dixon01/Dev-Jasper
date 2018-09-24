// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.Controller
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.ViewModel;
    using Gorba.Center.Common.ServiceModel;

    /// <summary>
    /// The application controller.
    /// </summary>
    public class ApplicationController : IDisposable
    {
        private CancellationTokenSource cancellationTokenSource;

        private NotificationSubscriber notificationSubscriber;

        /// <summary>
        /// Runs the controller.
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public async Task RunAsync()
        {
            var cts = this.cancellationTokenSource;
            if (cts != null)
            {
                cts.Cancel();
            }

            this.cancellationTokenSource = new CancellationTokenSource();
            if (this.notificationSubscriber != null)
            {
                this.notificationSubscriber.Dispose();
            }

            var shell = DependencyResolver.Current.Get<Shell>();
            shell.Status = "Creating subscriptions";
            this.notificationSubscriber = new NotificationSubscriber();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await this.notificationSubscriber.CreateSubscriptions(this.cancellationTokenSource.Token);
            stopwatch.Stop();
            shell.Status = string.Format("Subscriptions created in {0} ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // dispose unmanaged resources here
            }

            if (this.notificationSubscriber == null)
            {
                return;
            }

            this.notificationSubscriber.Dispose();
        }
    }
}