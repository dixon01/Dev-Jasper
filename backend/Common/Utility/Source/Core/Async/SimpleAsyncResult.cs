// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleAsyncResult.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleAsyncResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Async
{
    using System;

    /// <summary>
    /// Simple one-value implementation of <see cref="IAsyncResult"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value of this asynchronous result.
    /// </typeparam>
    public class SimpleAsyncResult<T> : AsyncResultBase
    {
        private T resultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleAsyncResult{T}"/> class.
        /// </summary>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        public SimpleAsyncResult(AsyncCallback callback, object state)
            : base(callback, state)
        {
        }

        /// <summary>
        /// Gets the value provided in <see cref="Complete(T,bool)"/>.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// if the asynchronous request failed. Have a look at the inner exception for more details.
        /// </exception>
        public T Value
        {
            get
            {
                this.WaitForCompletionAndVerify();
                return this.resultValue;
            }
        }

        /// <summary>
        /// Completes this request with the given value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="synchronously">
        /// A flag indicating whether the asynchronous operation completed synchronously.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If this request has already been completed.
        /// </exception>
        public void Complete(T value, bool synchronously)
        {
            this.resultValue = value;
            this.Complete(synchronously);
        }
    }
}