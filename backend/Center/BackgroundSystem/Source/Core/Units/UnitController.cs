// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Units
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.BackgroundSystem.Core.Dynamic;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.GorbaProtocol;
    using Gorba.Common.Protocols.GorbaProtocol.Messages;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The unit observer which handles the connection state.
    /// </summary>
    public class UnitController : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly LiveUpdatesBucket liveUpdatesBucket;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitController"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        public UnitController(UnitReadableModel unit)
        {
            this.Model = unit;
            this.liveUpdatesBucket = new LiveUpdatesBucket(unit.Id);
            unit.PropertyChanged += this.OnUnitPropertyChanged;
            MessageDispatcher.Instance.Subscribe<GorbaMessageAck>(this.HandleUpdateStateInfo);
        }

        /// <summary>
        /// Gets the unit readable model.
        /// </summary>
        public UnitReadableModel Model { get; private set; }

        /// <summary>
        /// Enqueues a message to be sent to the unit.
        /// </summary>
        /// <param name="item">
        ///     The message.
        /// </param>
        public void EnqueueLiveUpdate(QueuedMessage item)
        {
            this.liveUpdatesBucket.Add(item);
            this.Send(item);
        }

        /// <summary>
        /// Tries to resend all commands that are not yet acknowledged.
        /// <see cref="NewsFeedMessage"/>s require special handling: even if acknowledged, they're kept until their
        /// validity expires; this allows re-sending last valid news feed after a unit restart.
        /// </summary>
        public void ResendLiveUpdates()
        {
            if (!this.Model.IsConnected)
            {
                Logger.Trace("Unit {0} is not connected. Can't resend live update messages", this.Model.Name);
                return;
            }

            var updates = this.liveUpdatesBucket.GetValidMessages().ToList();
            foreach (var update in updates)
            {
                this.Send(update);
            }

            Logger.Trace("Resent {0} update command(s) to {1}", updates.Count, this.Model.Name);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Model.PropertyChanged -= this.OnUnitPropertyChanged;
            MessageDispatcher.Instance.Unsubscribe<GorbaMessageAck>(this.HandleUpdateStateInfo);
        }

        private MediAddress GetAddress(string applicationName)
        {
            return new MediAddress(this.Model.Name, applicationName);
        }

        /// <summary>
        /// Tries to (re)send the messages to the unit.
        /// </summary>
        /// <param name="queuedMessage">The live update message to send.</param>
        private void Send(QueuedMessage queuedMessage)
        {
            if (!this.Model.IsConnected)
            {
                Logger.Trace("Unit {0} is not connected. Can't send live update {1}", this.Model.Name, queuedMessage);
                return;
            }

            var address = this.GetAddress(queuedMessage.ApplicationName);
            MessageDispatcher.Instance.Send(address, queuedMessage.Message);
            Logger.Trace("Sent Message {0} to unit {1}", queuedMessage.Message, this.Model.Name);
        }

        private void HandleUpdateStateInfo(object sender, MessageEventArgs<GorbaMessageAck> e)
        {
            if (!this.Model.Name.Equals(e.Source.Unit, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Logger.Trace("Received acknowledge for {0}", e.Message.Id);
            if (this.liveUpdatesBucket.TryRemove(e.Message.Id))
            {
                Logger.Debug("Removed acknowledged live update: {0}", e.Message);
                return;
            }

            Logger.Debug("Couldn't remove live update {0}", e.Message.Id);
        }

        private void OnUnitPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "IsConnected"
                || !this.Model.IsConnected)
            {
                return;
            }

            this.ResendLiveUpdates();
        }

        /// <summary>
        /// Caches the live updates messages to be sent to the unit.
        /// <see cref="NewsFeedMessage"/>s are not removed if they're still valid. This makes possible to resend feeds
        /// after a unit restart.
        /// </summary>
        internal class LiveUpdatesBucket : IDisposable
        {
            private readonly Dictionary<Guid, QueuedMessage> messages = new Dictionary<Guid, QueuedMessage>();

            private readonly Dictionary<int, Guid> newsFeeds = new Dictionary<int, Guid>();

            private readonly Logger logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="LiveUpdatesBucket"/> class.
            /// </summary>
            /// <param name="unitId">
            /// The unit id.
            /// </param>
            public LiveUpdatesBucket(int unitId)
            {
                this.logger = LogManager.GetLogger(this.GetType().FullName + "<" + unitId + ">");
            }

            /// <summary>
            /// Adds the message to the cache.
            /// </summary>
            /// <param name="queuedMessage">
            /// The queued message.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// The <paramref name="queuedMessage"/> argument is null.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// The inner <see cref="QueuedMessage.Message"/> property of <paramref name="queuedMessage"/> is null.
            /// </exception>
            public void Add(QueuedMessage queuedMessage)
            {
                if (queuedMessage == null)
                {
                    throw new ArgumentNullException("queuedMessage");
                }

                if (queuedMessage.Message == null)
                {
                    throw new ArgumentException("Inner message can't be null", "queuedMessage");
                }

                lock (this.messages)
                {
                    var feedMessage = queuedMessage.Message as NewsFeedMessage;
                    if (feedMessage != null)
                    {
                        if (this.newsFeeds.ContainsKey(feedMessage.FeedId))
                        {
                            var existingFeed = this.newsFeeds[feedMessage.FeedId];
                            this.logger.Trace(
                                "Removing existing message {0} for feed {1}",
                                existingFeed,
                                feedMessage.FeedId);
                            this.messages.Remove(existingFeed);
                        }

                        this.newsFeeds[feedMessage.FeedId] = feedMessage.Id;
                    }

                    this.logger.Trace(
                        "Added message {0} of type '{1}'",
                        queuedMessage.Message.Id,
                        queuedMessage.Message.GetType().FullName);
                    this.messages[queuedMessage.Message.Id] = queuedMessage;
                }
            }

            /// <summary>
            /// Removes the expired messages and return the remaining valid ones.
            /// </summary>
            /// <returns>
            /// The valid messages.
            /// </returns>
            public IEnumerable<QueuedMessage> GetValidMessages()
            {
                lock (this.messages)
                {
                    var utcNow = TimeProvider.Current.UtcNow;
                    var discarded =
                        this.messages.Values.Where(u => u.Message.ValidUntil < utcNow)
                            .Select(update => update.Message.Id)
                            .ToList();
                    foreach (var id in discarded)
                    {
                        this.TryRemove(id);
                    }

                    return this.messages.Values.OrderBy(message => message.Timestamp);
                }
            }

            /// <summary>
            /// Removes a message if it is expired, or if it's not a <see cref="NewsFeedMessage"/>.
            /// </summary>
            /// <param name="id">
            /// The id of the message to remove.
            /// </param>
            /// <returns>
            /// <c>true</c> if the message was removed; otherwise, <c>false</c>.
            /// </returns>
            public bool TryRemove(Guid id)
            {
                lock (this.messages)
                {
                    if (!this.messages.ContainsKey(id))
                    {
                        this.logger.Debug("Message {0} not found", id);
                        return false;
                    }

                    if (this.messages[id].Message.ValidUntil < TimeProvider.Current.UtcNow)
                    {
                        this.logger.Trace("Removed expired message {0}", id);
                        this.messages.Remove(id);
                        return true;
                    }

                    var feedMessage = this.messages[id].Message as NewsFeedMessage;
                    if (feedMessage != null)
                    {
                        this.logger.Trace("Message {0} is a (valid) NewsFeedMessage and won't be removed", id);
                        return false;
                    }

                    this.messages.Remove(id);
                    return true;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                this.Dispose(true);
            }

            private void Dispose(bool isDisposing)
            {
                if (isDisposing)
                {
                    // dispose managed resources
                }

                lock (this.messages)
                {
                    this.newsFeeds.Clear();
                    this.messages.Clear();
                }
            }
        }
    }
}