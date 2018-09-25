// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncResultBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncResultBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Async
{
    using System;
    using System.Threading;

    /// <summary>
    /// Simple base class for <see cref="IAsyncResult"/> using an <see cref="AutoResetEvent"/>
    /// as the wait handle.
    /// Subclasses will have to implement their own Complete() method which will call
    /// <see cref="Complete"/>.
    /// </summary>
    public abstract class AsyncResultBase : IAsyncResult
    {
        private readonly ManualResetEvent waitHandle = new ManualResetEvent(false);

        private readonly AsyncCallback callback;

        private Exception resultException;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResultBase"/> class.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        protected AsyncResultBase(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.AsyncState = state;
        }

        /// <summary>
        /// Gets a value indicating whether the operation has completed.
        /// This flag is set to true by <see cref="Complete"/>.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets the wait handle.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return this.waitHandle;
            }
        }

        /// <summary>
        /// Gets the state object provided in the constructor.
        /// </summary>
        public object AsyncState { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this operation completed synchronously.
        /// This implementation always returns false.
        /// </summary>
        public bool CompletedSynchronously { get; private set; }

        /// <summary>
        /// Waits until the request has completed and verifies that the request
        /// completed without an exception. If there was an exception, it will be thrown by this method.
        /// </summary>
        /// <exception cref="Exception">
        /// Throws an exception if <see cref="CompleteException"/> was called.
        /// </exception>
        public void WaitForCompletionAndVerify()
        {
            if (!this.IsCompleted)
            {
                this.AsyncWaitHandle.WaitOne();
            }

            if (this.resultException != null)
            {
                throw this.resultException;
            }
        }

        /// <summary>
        /// Completes this request with an exception.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="synchronously">
        /// A flag indicating whether the asynchronous operation completed synchronously.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If this request has already been completed.
        /// </exception>
        public void CompleteException(Exception exception, bool synchronously)
        {
            this.resultException = exception;
            this.Complete(synchronously);
        }

        /// <summary>
        /// Tries to complete this request with an exception.
        /// If the request was previously completed, this method does nothing
        /// and returns false.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="synchronously">
        /// A flag indicating whether the asynchronous operation completed synchronously.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If this request has already been completed.
        /// </exception>
        /// <returns>
        /// A flag telling if the request was completed through this method.
        /// If false is returned, the request was completed previously.
        /// </returns>
        public bool TryCompleteException(Exception exception, bool synchronously)
        {
            if (this.IsCompleted)
            {
                return false;
            }

            this.resultException = exception;
            this.Complete(synchronously);
            return true;
        }

        /// <summary>
        /// Method to be called by subclasses when the request was completed.
        /// Sets <see cref="IsCompleted"/> to true, sets the wait handle and
        /// calls the callback given in the constructor with this object as
        /// an argument.
        /// </summary>
        /// <param name="synchronously">
        /// The value to be set in <see cref="CompletedSynchronously"/>.
        /// </param>
        protected void Complete(bool synchronously)
        {
            if (this.IsCompleted)
            {
                throw new NotSupportedException("Can't complete async result twice");
            }

            this.CompletedSynchronously = synchronously;
            this.IsCompleted = true;
            this.waitHandle.Set();

            if (this.callback != null)
            {
                this.callback(this);
            }
        }
    }
}
