// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommitVerificationBase{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommitVerificationBase&lt;T&gt; type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Exceptions;

    /// <summary>
    /// Defines a base class for commit verifications.
    /// </summary>
    /// <typeparam name="T">The type of the key of the entity.</typeparam>
    public abstract class CommitVerificationBase<T>
        where T : struct
    {
        private readonly TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();

        private readonly TimeSpan timeout = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Waits the completion.
        /// </summary>
        /// <returns>
        /// The identifier of the entity.
        /// </returns>
        protected virtual async Task<T> WaitCompletion()
        {
            var completion = this.taskCompletionSource.Task;
            var cancellationTokenSource = new CancellationTokenSource();
            var delay = Task.Delay(this.timeout, cancellationTokenSource.Token);
            await Task.WhenAny(completion, delay);
            if (delay.IsCompleted)
            {
                this.TrySetException("The operation didn't complete within the allowed time");
            }

            cancellationTokenSource.Cancel();
            return await this.taskCompletionSource.Task;
        }

        /// <summary>
        /// Tries to set the result on the completion source with the specified message.
        /// </summary>
        /// <param name="entityId">
        /// The id of the entity.
        /// </param>
        /// <returns>
        /// A flag indicating whether it was possible or not to set the result.
        /// </returns>
        protected virtual bool TrySetResult(T entityId)
        {
            return this.taskCompletionSource.TrySetResult(entityId);
        }

        /// <summary>
        /// Tries to set an exception on the completion source with the specified message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// A flag indicating whether it was possible or not to set the exception.
        /// </returns>
        protected virtual bool TrySetException(string message)
        {
            return this.taskCompletionSource.TrySetException(new ChangeTrackingException(message));
        }
    }
}