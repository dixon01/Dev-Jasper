// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadWriteLock.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadWriteLock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Utility
{
    using System;
    using System.Threading;

    /// <summary>
    /// Wrapper for <see cref="ReaderWriterLock"/> that allows to use
    /// the <code>using</code> statement for releasing a lock.
    /// </summary>
    public partial class ReadWriteLock
    {
        private readonly ReaderWriterLock locker = new ReaderWriterLock();

        /// <summary>
        /// Acquires a read lock.
        /// </summary>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> that has to be disposed to release the lock.
        /// </returns>
        public IDisposable AcquireReadLock(int timeout = Timeout.Infinite)
        {
            this.locker.AcquireReaderLock(timeout);
            return new Releaser(() => this.locker.ReleaseReaderLock());
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
        public IDisposable AcquireWriteLock(int timeout = Timeout.Infinite)
        {
            this.locker.AcquireWriterLock(timeout);
            return new Releaser(() => this.locker.ReleaseWriterLock());
        }
    }
}
