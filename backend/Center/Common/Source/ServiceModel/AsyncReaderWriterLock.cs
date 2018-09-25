// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncReaderWriterLock.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncReaderWriterLock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// An async reader-writer lock.
    /// </summary>
    public class AsyncReaderWriterLock
    {
        private readonly Task<Releaser> readerReleaser;
        private readonly Task<Releaser> writerReleaser;

        private readonly Queue<TaskCompletionSource<Releaser>> waitingWriters =
            new Queue<TaskCompletionSource<Releaser>>();

        private TaskCompletionSource<Releaser> waitingReader = new TaskCompletionSource<Releaser>();

        private int readersWaiting;

        /// <summary>
        /// The value of 0 means that no one has acquired the lock,
        /// a value of –1 means that a writer has acquired the lock,
        /// and a positive value means that one or more readers have acquired the lock,
        /// where the positive value indicates how many.
        /// </summary>
        private int status;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncReaderWriterLock"/> class.
        /// </summary>
        public AsyncReaderWriterLock()
        {
            this.readerReleaser = Task.FromResult(new Releaser(this, false));
            this.writerReleaser = Task.FromResult(new Releaser(this, true));
        }

        /// <summary>
        /// Acquires a reader lock, waiting for any writers to finish.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to await the lock.
        /// The returned releaser can then be used in a <c>using</c> statement to release the lock later.
        /// </returns>
        public Task<Releaser> ReaderLockAsync()
        {
            lock (this.waitingWriters)
            {
                if (this.status >= 0 && this.waitingWriters.Count == 0)
                {
                    ++this.status;
                    return this.readerReleaser;
                }

                ++this.readersWaiting;
                return this.waitingReader.Task.ContinueWith(t => t.Result);
            }
        }

        /// <summary>
        /// Acquires a writer lock, waiting for any readers or writers to finish.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to await the lock.
        /// The returned releaser can then be used in a <c>using</c> statement to release the lock later.
        /// </returns>
        public Task<Releaser> WriterLockAsync()
        {
            lock (this.waitingWriters)
            {
                if (this.status == 0)
                {
                    this.status = -1;
                    return this.writerReleaser;
                }

                var waiter = new TaskCompletionSource<Releaser>();
                this.waitingWriters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        private void ReaderRelease()
        {
            TaskCompletionSource<Releaser> toWake = null;

            lock (this.waitingWriters)
            {
                --this.status;
                if (this.status == 0 && this.waitingWriters.Count > 0)
                {
                    this.status = -1;
                    toWake = this.waitingWriters.Dequeue();
                }
            }

            if (toWake != null)
            {
                toWake.SetResult(new Releaser(this, true));
            }
        }

        private void WriterRelease()
        {
            TaskCompletionSource<Releaser> toWake = null;
            var toWakeIsWriter = false;

            lock (this.waitingWriters)
            {
                if (this.waitingWriters.Count > 0)
                {
                    toWake = this.waitingWriters.Dequeue();
                    toWakeIsWriter = true;
                }
                else if (this.readersWaiting > 0)
                {
                    toWake = this.waitingReader;
                    this.status = this.readersWaiting;
                    this.readersWaiting = 0;
                    this.waitingReader = new TaskCompletionSource<Releaser>();
                }
                else
                {
                    this.status = 0;
                }
            }

            if (toWake != null)
            {
                toWake.SetResult(new Releaser(this, toWakeIsWriter));
            }
        }

        /// <summary>
        /// The lock releaser. Do not create objects of type yourself, they are only created by the outer class.
        /// </summary>
        public struct Releaser : IDisposable
        {
            private readonly AsyncReaderWriterLock toRelease;
            private readonly bool writer;

            /// <summary>
            /// Initializes a new instance of the <see cref="Releaser"/> struct.
            /// </summary>
            /// <param name="toRelease">
            /// The lock to release.
            /// </param>
            /// <param name="writer">
            /// A flag indicating whether this releaser is used for a writer (true) or a reader (false).
            /// </param>
            internal Releaser(AsyncReaderWriterLock toRelease, bool writer)
            {
                this.toRelease = toRelease;
                this.writer = writer;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                if (this.toRelease == null)
                {
                    return;
                }

                if (this.writer)
                {
                    this.toRelease.WriterRelease();
                }
                else
                {
                    this.toRelease.ReaderRelease();
                }
            }
        }
    }
}
