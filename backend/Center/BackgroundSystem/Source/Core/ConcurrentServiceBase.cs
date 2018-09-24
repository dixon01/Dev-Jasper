// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentServiceBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines a base class for concurrent services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;

    using NLog;

    /// <summary>
    /// Defines a base class for concurrent services.
    /// </summary>
    public abstract class ConcurrentServiceBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly AsyncReaderWriterLock locker = new AsyncReaderWriterLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentServiceBase"/> class.
        /// </summary>
        protected ConcurrentServiceBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Logs details about an occurred exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual void OnError(Exception exception)
        {
            this.Logger.Error(exception, "Error occurred while executing an operation on the inner service");
        }

        /// <summary>
        /// Acquires a reader lock.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited with the <see cref="AsyncReaderWriterLock.Releaser"/> that should
        /// be used in a <c>using</c> statement.
        /// </returns>
        protected virtual Task<AsyncReaderWriterLock.Releaser> AcquireReaderLockAsync()
        {
            return this.locker.ReaderLockAsync();
        }

        /// <summary>
        /// Acquires a writer lock.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited with the <see cref="AsyncReaderWriterLock.Releaser"/> that should
        /// be used in a <c>using</c> statement.
        /// </returns>
        protected virtual Task<AsyncReaderWriterLock.Releaser> AcquireWriterLockAsync()
        {
            return this.locker.ReaderLockAsync();
        }
    }
}
