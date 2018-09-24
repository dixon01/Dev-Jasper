// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadyGate.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadyGate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.Notifications;

    using NLog;

    /// <summary>
    /// Default implementation of the <see cref="IReadyGate"/>.
    /// </summary>
    public class ReadyGate : IReadyGate
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(10);

        private readonly ConcurrentQueue<TaskCompletionSource<bool>> waiters =
            new ConcurrentQueue<TaskCompletionSource<bool>>();

        private readonly Func<Notification, Task<Guid>> postNotificationAsync;

        private readonly AsyncLock asyncLocker = new AsyncLock();

        private volatile bool isReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadyGate"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="postNotificationAsync">
        /// The post notification async.
        /// </param>
        internal ReadyGate(string name, Func<Notification, Task<Guid>> postNotificationAsync)
        {
            this.Name = name;
            this.postNotificationAsync = postNotificationAsync;
        }

        /// <summary>
        /// Gets the name of this instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Waits that the service is ready.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task WaitReadyAsync()
        {
            if (this.isReady)
            {
                return;
            }

            await this.PingPongAsync();
        }

        /// <summary>
        /// Sends a <see cref="PingNotification"/> and waits the <see cref="PongNotification"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task PingPongAsync()
        {
            var pingPong  = new TaskCompletionSource<bool>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var pingNotification = new PingNotification();
            this.waiters.Enqueue(pingPong);
            var id = await this.postNotificationAsync(pingNotification);
            Logger.Trace("Ping ({0} on {1})?", id, this.Name);
            var timeout = Task.Delay(PingTimeout);
            await Task.WhenAny(timeout, pingPong.Task);
            if (timeout.IsCompleted)
            {
                Logger.Trace("Ping timeout ({0})", this.Name);

                // ReSharper disable once CSharpWarnings::CS4014
                Task.Run(async () => await this.PingPongAsync()).ContinueWith(this.OnTaskCompleted);
            }

            await pingPong.Task;
            Logger.Trace("Pong ({0}, {1} ms)!", this.Name, stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
        }

        /// <summary>
        /// Forwards a <see cref="PongNotification"/>.
        /// </summary>
        /// <param name="notification">
        /// The notification to be forwarded.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public Task PongAsync(PongNotification notification)
        {
            this.SetIsReadyValue(true);
            TaskCompletionSource<bool> waiter;
            while (this.waiters.TryDequeue(out waiter))
            {
                waiter.TrySetResult(true);
            }

            return Task.FromResult(0);
        }

        private void OnTaskCompleted(Task task)
        {
            if (task.Exception == null)
            {
                return;
            }

            var exception = task.Exception.Flatten();
            Logger.Debug(exception, "Error on task");
        }

        private void SetIsReadyValue(bool value)
        {
            if (this.isReady == value)
            {
                return;
            }

            lock (this.asyncLocker)
            {
                if (this.isReady == value)
                {
                    return;
                }

                this.isReady = value;
            }
        }
    }
}
