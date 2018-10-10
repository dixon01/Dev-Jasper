// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncSemaphore.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncSemaphore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Async semaphore implementation.
    /// </summary>
    /// <remarks><see cref="http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266983.aspx"/></remarks>
    public class AsyncSemaphore
    {
        private static readonly Task Completed = Task.FromResult(true);

        private readonly Queue<TaskCompletionSource<bool>> waiters = new Queue<TaskCompletionSource<bool>>();

        private int currentCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSemaphore"/> class.
        /// </summary>
        /// <param name="initialCount">
        /// The initial count.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">The initial count is lower than 0.</exception>
        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException("initialCount");
            }

            this.currentCount = initialCount;
        }

        /// <summary>
        /// Waits asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public Task WaitAsync()
        {
            lock (this.waiters)
            {
                if (this.currentCount > 0)
                {
                    --this.currentCount;
                    return Completed;
                }

                var waiter = new TaskCompletionSource<bool>();
                this.waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        /// <summary>
        /// Releases the resource.
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (this.waiters)
            {
                if (this.waiters.Count > 0)
                {
                    toRelease = this.waiters.Dequeue();
                }
                else
                {
                    ++this.currentCount;
                }
            }

            if (toRelease == null)
            {
                return;
            }

            toRelease.SetResult(true);
        }
    }
}