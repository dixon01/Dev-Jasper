// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLock.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncLock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a lock that can be used in an async method.
    /// </summary>
    /// <example>
    /// using (await this.asyncLocker.LockAsync())
    /// {
    /// .. do stuff here
    /// }
    /// </example>
    public class AsyncLock
    {
        private readonly AsyncSemaphore semaphore;

        private readonly Task<Releaser> releaser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLock"/> class.
        /// </summary>
        public AsyncLock()
        {
            this.semaphore = new AsyncSemaphore(1);
            this.releaser = Task.FromResult(new Releaser(this));
        }

        /// <summary>
        /// Returns a locker.
        /// </summary>
        /// <returns>
        /// The lock.
        /// </returns>
        public Task<Releaser> LockAsync()
        {
            var wait = this.semaphore.WaitAsync();
            return wait.IsCompleted
                       ? this.releaser
                       : wait.ContinueWith(
                           (_, state) => new Releaser((AsyncLock)state),
                           this,
                           CancellationToken.None,
                           TaskContinuationOptions.ExecuteSynchronously,
                           TaskScheduler.Default);
        }

        /// <summary>
        /// The objects used to lock.
        /// </summary>
        public struct Releaser : IDisposable
        {
            private readonly AsyncLock toRelease;

            /// <summary>
            /// Initializes a new instance of the <see cref="Releaser"/> struct.
            /// </summary>
            /// <param name="toRelease">
            /// The to release.
            /// </param>
            internal Releaser(AsyncLock toRelease)
            {
                this.toRelease = toRelease;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                if (this.toRelease == null)
                {
                    return;
                }

                this.toRelease.semaphore.Release();
            }
        }
    }
}