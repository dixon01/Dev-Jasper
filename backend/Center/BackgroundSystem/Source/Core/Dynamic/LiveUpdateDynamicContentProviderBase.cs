// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveUpdateDynamicContentProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic
{
    using System;

    using Gorba.Center.BackgroundSystem.Core.Units;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Common.Protocols.GorbaProtocol;
    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// Defines the base class for dynamic content provider that need to send live updates to units.
    /// </summary>
    internal abstract class LiveUpdateDynamicContentProviderBase : DynamicContentProviderBase
    {
        private const string GorbaProtocolApplication = "GorbaProtocol";

        private readonly IUnitManager unitManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveUpdateDynamicContentProviderBase"/> class.
        /// </summary>
        /// <param name="updateGroup">The update group.</param>
        protected LiveUpdateDynamicContentProviderBase(UpdateGroupReadableModel updateGroup)
        {
            this.unitManager = DependencyResolver.Current.Get<IUnitManager>();
            this.UpdateGroup = updateGroup;
        }

        /// <summary>
        /// Gets the message dispatcher for the update group this provider was created for.
        /// </summary>
        public UpdateGroupReadableModel UpdateGroup { get; private set; }

        /// <summary>
        /// The create folder structure.
        /// </summary>
        /// <returns>
        /// The <see cref="UpdateFolderStructure"/>.
        /// </returns>
        public override UpdateFolderStructure CreateFolderStructure()
        {
            return new UpdateFolderStructure();
        }

        /// <summary>
        /// Sends a message to the group.
        /// </summary>
        /// <param name="update">
        /// The message.
        /// </param>
        protected virtual void Send(QueuedMessage update)
        {
            try
            {
                this.Logger.Trace("Sending live update {0}", update);
                this.unitManager.SendLiveUpdate(update);
            }
            catch (Exception exception)
            {
                this.Logger.Warn("Error while sending live update {0}", exception);
            }
        }

        /// <summary>
        /// Creates a live update token for the given content.
        /// </summary>
        /// <param name="feed">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="QueuedMessage"/>.
        /// </returns>
        protected virtual QueuedMessage CreateQueuedMessage(NewsFeed.NewsFeed feed)
        {
            var message = this.CreateMessage(feed);
            return new QueuedMessage(GorbaProtocolApplication, this.UpdateGroup.Id, message);
        }

        /// <summary>
        /// Creates the message to be sent.
        /// </summary>
        /// <param name="feed">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="GorbaMessage"/>.
        /// </returns>
        protected abstract GorbaMessage CreateMessage(NewsFeed.NewsFeed feed);
    }
}