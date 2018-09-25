// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewsFeedUpdateMessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Protran.GorbaProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Protocols.GorbaProtocol.Messages;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.BbCode;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The live update gorba message handler.
    /// </summary>
    internal class NewsFeedUpdateMessageHandler : MessageHandlerBase<NewsFeedMessage>
    {
        private const string TimerNameFormat = "NewsFeedUpdate_{0}";

        private const string TimerNameRegexPattern = @"^NewsFeedUpdate_(?<" + FeedIdRegexGroupName + @">[\d]+)$";

        private const string FeedIdRegexGroupName = "FeedId";

        private const int TableNumber = 5000;

        private static readonly Regex TimerNameRegex = new Regex(TimerNameRegexPattern);

        private readonly Dictionary<int, IDeadlineTimer> validityTimer = new Dictionary<int, IDeadlineTimer>();

        private readonly Dictionary<int, string> lastFeeds = new Dictionary<int, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsFeedUpdateMessageHandler"/> class,
        /// also resends persisted messages.
        /// </summary>
        public NewsFeedUpdateMessageHandler()
        {
            this.ResendStoredMessages();
        }

        /// <summary>
        /// Processes the message, creating <see cref="Ximple"/> messages if required.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public override void ProcessMessage(NewsFeedMessage message)
        {
            this.Logger.Trace("Processing message {0}", message);
            var timer = this.GetOrCreateTimer(message.FeedId);
            timer.UtcDeadline = message.ValidUntil;
            timer.Enabled = true;

            var feedMessagePersistence = ServiceLocator.Current.GetInstance<IPersistenceService>();
            var storedMessage = feedMessagePersistence.GetContext<FeedMessagePersistance>(FeedIdToKey(message.FeedId));
            var content = string.IsNullOrEmpty(message.Content) ? string.Empty : BbParser.EscapeBbCode(message.Content);

            lock (this.lastFeeds)
            {
                if (this.lastFeeds.ContainsKey(message.FeedId)
                    && string.Equals(content, this.lastFeeds[message.FeedId], StringComparison.Ordinal))
                {
                    this.Logger.Trace("content of message for feed {0} didn't change. Only reset validity timer.");
                    storedMessage.Revalidate();
                    return;
                }

                this.lastFeeds[message.FeedId] = content;
            }

            this.Logger.Trace("Content changed for feed {0}. Creating new ximple.");
            var ximple = new Ximple();
            ximple.Cells.Add(new XimpleCell
                                 {
                                     TableNumber = TableNumber,
                                     Value = content,
                                     RowNumber = message.FeedId
                                 });
            this.RaiseXimpleCreated(ximple);

            storedMessage.Validity = message.ValidUntil.Subtract(DateTime.Now);
            storedMessage.Value = new FeedMessagePersistance { FeedMessage = content };
        }

        private static string FeedIdToKey(int feedId)
        {
            return "NewsFeed_" + feedId;
        }

        private void ResendStoredMessages()
        {
            var feedMessagePersistence = ServiceLocator.Current.GetInstance<IPersistenceService>();

            var feedId = 0;
            while (true)
            {
                var feedMessage = feedMessagePersistence.GetContext<FeedMessagePersistance>(FeedIdToKey(feedId));

                if (feedMessage.Value != null && feedMessage.Valid)
                {
                    // resend
                    this.Logger.Trace("Resend feed {0}. Creating new ximple.", feedId);
                    var ximple = new Ximple();
                    ximple.Cells.Add(new XimpleCell
                    {
                        TableNumber = TableNumber,
                        Value = feedMessage.Value.FeedMessage,
                        RowNumber = feedId
                    });
                    this.RaiseXimpleCreated(ximple);

                    feedId++;
                }
                else
                {
                    // no content so feed not used, invalidate so it will not be persisted
                    feedMessage.Validity = TimeSpan.Zero;
                    break;
                }
            }
        }

        private void OnValidityTimerElapsed(object sender, EventArgs eventArgs)
        {
            var timer = (IDeadlineTimer)sender;
            timer.Enabled = false;
            this.Logger.Trace("Deadline timer '{0}' elapsed", timer.Name);
            var match = TimerNameRegex.Match(timer.Name);
            var feedId = int.Parse(match.Groups[FeedIdRegexGroupName].Value);
            lock (this.validityTimer)
            {
                this.validityTimer.Remove(feedId);
                timer.Dispose();
            }

            var ximple = new Ximple();
            ximple.Cells.Add(
                new XimpleCell
                    {
                        TableNumber = TableNumber, RowNumber = feedId, ColumnNumber = 0, Value = string.Empty
                    });
            this.Logger.Debug("Validity expired for feed {0}. Sending ximple with empty content", feedId);
            this.RaiseXimpleCreated(ximple);
        }

        private IDeadlineTimer GetOrCreateTimer(int id)
        {
            lock (this.validityTimer)
            {
                if (this.validityTimer.ContainsKey(id))
                {
                    return this.validityTimer[id];
                }

                var timer = TimerFactory.Current.CreateDeadlineTimer(string.Format(TimerNameFormat, id));
                timer.Elapsed += this.OnValidityTimerElapsed;
                this.validityTimer.Add(id, timer);
                return timer;
            }
        }
    }
}