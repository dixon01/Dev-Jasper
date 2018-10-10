// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewsFeedContentProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.BackgroundSystem.Core.Dynamic.NewsFeed
{
    using System.Threading;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;

    /// <summary>
    /// The news feed content provider.
    /// </summary>
    internal abstract class NewsFeedContentProviderBase : LiveUpdateDynamicContentProviderBase
    {
        private readonly CancellationTokenSource cancelAll = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsFeedContentProviderBase"/> class.
        /// </summary>
        /// <param name="updateGroup">The update group.</param>
        protected NewsFeedContentProviderBase(
            UpdateGroupReadableModel updateGroup)
            : base(updateGroup)
        {
        }

        /// <summary>
        /// The do start.
        /// </summary>
        protected override void DoStart()
        {
        }

        /// <summary>
        /// The do stop.
        /// </summary>
        protected override void DoStop()
        {
            this.cancelAll.Cancel();
        }

        /// <summary>
        /// Checks if cancellation was requested.
        /// </summary>
        protected virtual void CheckCancelled()
        {
            this.cancelAll.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Enqueues a feed.
        /// </summary>
        /// <param name="feed">
        /// The feed.
        /// </param>
        protected virtual void Send(NewsFeed feed)
        {
            var liveUpdate = this.CreateQueuedMessage(feed);
            this.Send(liveUpdate);
        }
    }
}