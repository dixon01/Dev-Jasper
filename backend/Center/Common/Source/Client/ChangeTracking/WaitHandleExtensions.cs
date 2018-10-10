// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitHandleExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WaitHandleExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions for <see cref="WaitHandle"/>s.
    /// </summary>
    public static class WaitHandleExtensions
    {
        /// <summary>
        /// Gets a task corresponding to the <see cref="WaitHandle"/> with an infinite timeout.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>A <see cref="Task"/> corresponding to the given handle.</returns>
        public static Task AsTask(this WaitHandle handle)
        {
            return AsTask(handle, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Gets the last part of the dotted name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The last part of the name.
        /// </returns>
        public static string GetLastPart(this string name)
        {
            return name.Split('.').Last();
        }

        /// <summary>
        /// Gets a task corresponding to the <see cref="WaitHandle"/> with the specified timeout.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A <see cref="Task"/> corresponding to the given handle.</returns>
        public static Task AsTask(this WaitHandle handle, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<object>();
            var registration = ThreadPool.RegisterWaitForSingleObject(
                handle,
                (state, timedOut) =>
                    {
                        var localTcs = (TaskCompletionSource<object>)state;
                        if (timedOut)
                        {
                            localTcs.TrySetCanceled();
                        }
                        else
                        {
                            localTcs.TrySetResult(null);
                        }
                    },
                tcs,
                timeout,
                executeOnlyOnce: true);
            tcs.Task.ContinueWith(
                (_, state) => ((RegisteredWaitHandle)state).Unregister(null),
                registration,
                TaskScheduler.Default);
            return tcs.Task;
        }
    }
}