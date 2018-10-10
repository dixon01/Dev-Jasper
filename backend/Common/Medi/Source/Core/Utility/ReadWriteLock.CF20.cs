// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadWriteLock.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadWriteLock type.
//   Based on ReaderWriterLockAlt (https://readerwriterlockalt.codeplex.com):
// Copyright (c) 2008 Jean-Paul Mikkers
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
// Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Utility
{
    using System;
    using System.Threading;

    /// <summary>
    /// Read-write-lock implementation that allows to use
    /// the <code>using</code> statement for releasing a lock.
    /// </summary>
    public partial class ReadWriteLock
    {
        private const int NoThread = 0;
        private readonly object sync = new object();

        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);

        private int lockCount;
        private int waitingReaders;
        private int waitingWriters;
        private int writerThreadId = NoThread;

        /// <summary>
        /// Acquires a read lock.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> that has to be disposed to release the lock.
        /// </returns>
        public IDisposable AcquireReadLock()
        {
            return this.AcquireReadLock(Timeout.Infinite);
        }

        /// <summary>
        /// Acquires a read lock.
        /// </summary>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that has to be disposed to release the lock.
        /// </returns>
        public IDisposable AcquireReadLock(int timeout)
        {
            this.EnterReadLock();
            return new Releaser(this.ExitReadLock);
        }

        /// <summary>
        /// Acquires a write lock.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> that has to be disposed to release the lock.
        /// </returns>
        public IDisposable AcquireWriteLock()
        {
            return this.AcquireWriteLock(Timeout.Infinite);
        }

        /// <summary>
        /// Acquires a write lock.
        /// </summary>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that has to be disposed to release the lock.
        /// </returns>
        public IDisposable AcquireWriteLock(int timeout)
        {
            this.EnterWriteLock();
            return new Releaser(this.ExitWriteLock);
        }

        private void EnterReadLock()
        {
            int currentId = Thread.CurrentThread.ManagedThreadId;

            Monitor.Enter(this.sync);
            try
            {
                // wait until there are no more pending writers, and no writer other than me has the lock
                if (!this.ReadLockPreCondition(currentId))
                {
                    try
                    {
                        this.waitingReaders++;
                        do
                        {
                            ////Monitor.Wait(this.sync);
                            Monitor.Exit(this.sync);
                            this.waitEvent.WaitOne();
                            Monitor.Enter(this.sync);
                        }
                        while (!this.ReadLockPreCondition(currentId));
                    }
                    finally
                    {
                        // make sure the waiting counter is updated correctly even in case of an exception
                        this.waitingReaders--;
                    }
                }

                // the following only gets executed if no exception was thrown:
                this.lockCount++;
            }
            finally
            {
                Monitor.Exit(this.sync);
            }
        }

        private void ExitReadLock()
        {
            Monitor.Enter(this.sync);
            try
            {
                if (this.lockCount > 0)
                {
                    this.lockCount--;
                    if (this.lockCount == 0)
                    {
                        this.PulseWaitingThreads();
                    }
                }
                else
                {
                    throw new ThreadStateException("Unbalanced acquire/release read lock detected");
                }
            }
            finally
            {
                Monitor.Exit(this.sync);
            }
        }

        private void EnterWriteLock()
        {
            int currentId = Thread.CurrentThread.ManagedThreadId;

            Monitor.Enter(this.sync);
            try
            {
                // Wait for other readers or writers to become ready
                if (!this.WriteLockPreCondition(currentId))
                {
                    try
                    {
                        this.waitingWriters++;
                        do
                        {
                            ////Monitor.Wait(this.sync);
                            Monitor.Exit(this.sync);
                            this.waitEvent.WaitOne();
                            Monitor.Enter(this.sync);
                        }
                        while (!this.WriteLockPreCondition(currentId));
                    }
                    finally
                    {
                        // make sure the waiting counter is updated correctly even in case of an exception
                        this.waitingWriters--;
                    }
                }

                // the following only gets executed if no exception was thrown:
                this.lockCount++;
                this.writerThreadId = currentId;
            }
            finally
            {
                Monitor.Exit(this.sync);
            }
        }

        private void ExitWriteLock()
        {
            int currentId = Thread.CurrentThread.ManagedThreadId;

            lock (this.sync)
            {
                if (this.lockCount > 0)
                {
                    if (this.writerThreadId == currentId)
                    {
                        // writelock was owned by me
                        this.lockCount--;

                        if (this.lockCount == 0)
                        {
                            this.writerThreadId = NoThread;
                            this.PulseWaitingThreads();
                        }
                    }
                    else
                    {
                        throw new ThreadStateException(
                            "The calling thread attempted to release a write lock "
                            + "but does not own the lock for the specified object");
                    }
                }
                else
                {
                    throw new ThreadStateException("Unbalanced acquire/release write lock detected");
                }
            }
        }

        private void PulseWaitingThreads()
        {
            if (this.waitingReaders > 0 || this.waitingWriters > 0)
            {
                ////Monitor.PulseAll(this.sync);
                this.waitEvent.Set();
            }
        }

        private bool WriteLockPreCondition(int currentThreadId)
        {
            return this.lockCount == 0 || this.writerThreadId == currentThreadId;
        }

        private bool ReadLockPreCondition(int currentThreadId)
        {
            return this.waitingWriters == 0
                && (this.writerThreadId == NoThread || this.writerThreadId == currentThreadId);
        }
    }
}
