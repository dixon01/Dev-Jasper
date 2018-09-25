// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO004Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO004Handler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Telegram handler for GO004 message telegram.
    /// </summary>
    public class GO004Handler : TelegramHandler<GO004>, IManageableTable
    {
        private readonly SortedMap<int, MessageInfo> messages = new SortedMap<int, MessageInfo>();

        private readonly ITimer updateTimer;

        private GO004Config config;

        private GenericUsageHandler textUsage;
        private GenericUsageHandler typeUsage;
        private GenericUsageHandler titleUsage;

        private int lastMessageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO004Handler"/> class.
        /// </summary>
        public GO004Handler()
            : base(10)
        {
            this.updateTimer = TimerFactory.Current.CreateTimer("UpdateTimer");
            this.updateTimer.AutoReset = false;
            this.updateTimer.Elapsed += (s, e) => this.SendMessages();

            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            persistenceService.Saving += (s, e) => this.Save();
            var context = persistenceService.GetContext<List<MessageInfo>>();
            if (context.Value != null && context.Valid)
            {
                foreach (var message in context.Value)
                {
                    this.messages[message.Index] = message;
                }
            }
        }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="telegramConfig">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig telegramConfig, IIbisConfigContext configContext)
        {
            base.Configure(telegramConfig, configContext);

            this.config = (GO004Config)telegramConfig;

            var dictionary = configContext.Dictionary;
            this.textUsage = new GenericUsageHandler(this.config.UsedFor, dictionary);
            this.typeUsage = new GenericUsageHandler(this.config.UsedForType, dictionary);
            this.titleUsage = new GenericUsageHandler(this.config.UsedForTitle, dictionary);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var message in this.messages.Values)
            {
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<int>("Message index", message.Index, true),
                                     new ManagementProperty<int>("Message type", message.MessageType, true),
                                     new ManagementProperty<int>(
                                         "Message start validity since midnight", message.StartSeconds, true),
                                     new ManagementProperty<int>(
                                         "Message stop validity since midnight", message.EndSeconds, true),
                                     new ManagementProperty<string>("Message title", message.Title, true),
                                     new ManagementProperty<string>("Message text", message.Text, true),
                                 };
            }
        }

        /// <summary>
        /// Handles the telegram and generates Ximple if needed.
        /// This method needs to be implemented by subclasses to
        /// create the Ximple object for the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected override void HandleInput(GO004 telegram)
        {
            if (telegram.MessageIndex == 0)
            {
                // delete all messages
                this.messages.Clear();
                this.SendMessages();
                return;
            }

            var parts = telegram.MessageParts;
            if (parts == null || parts.Length == 0 || (parts.Length == 1 && string.IsNullOrEmpty(parts[0]))
                || telegram.TimeRange < 0)
            {
                // delete the given message
                this.messages.Remove(telegram.MessageIndex);
                this.SendMessages();
                return;
            }

            var text = parts.Length > 1 ? string.Join("[br]", parts, 1, parts.Length - 1) : string.Empty;
            var message = new MessageInfo
                {
                    Index = telegram.MessageIndex,
                    MessageType = telegram.MessageType,
                    StartSeconds = TimeToSeconds(telegram.TimeRange / 10000),
                    EndSeconds = TimeToSeconds(telegram.TimeRange % 10000),
                    Title = parts[0],
                    Text = text
                };
            this.messages[telegram.MessageIndex] = message;
            this.SendMessages();
        }

        /// <summary>
        /// Converts an int in HHMM format to the number of seconds.
        /// </summary>
        /// <param name="hhmm">an integer in HHMM format</param>
        /// <returns>the number of seconds on the given day</returns>
        private static int TimeToSeconds(int hhmm)
        {
            if (hhmm == 9999)
            {
                return -1;
            }

            int hours = hhmm / 100;
            int minutes = hhmm % 100;
            return (hours * 3600) + (minutes * 60);
        }

        private void SendMessages()
        {
            this.updateTimer.Enabled = false;

            var nowSeconds = (int)TimeProvider.Current.Now.TimeOfDay.TotalSeconds;
            int nextUpdate = TimeToSeconds(2400); // always trigger an update after midnight

            var ximple = new Ximple();
            int index = 0;
            foreach (var message in this.messages.Values)
            {
                if (this.AddMessage(ximple, message, index, nowSeconds))
                {
                    index++;
                }

                if (message.StartSeconds > nowSeconds && nextUpdate > message.StartSeconds)
                {
                    nextUpdate = message.StartSeconds;
                }

                if (message.EndSeconds > nowSeconds && nextUpdate > message.EndSeconds)
                {
                    nextUpdate = message.EndSeconds;
                }
            }

            int count = index;
            while (index < this.lastMessageCount)
            {
                this.AddMessage(ximple, MessageInfo.Empty, index++, nowSeconds);
            }

            this.lastMessageCount = count;

            // restart the timer (add 5 seconds just to be sure we are notified after the right time)
            this.updateTimer.Interval = TimeSpan.FromSeconds(nextUpdate - nowSeconds + 5);
            this.updateTimer.Enabled = true;

            if (ximple.Cells.Count == 0)
            {
                return;
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private bool AddMessage(Ximple ximple, MessageInfo message, int index, int nowSeconds)
        {
            if (!message.IsValid(nowSeconds))
            {
                return false;
            }

            var type = message.MessageType.ToString(CultureInfo.InvariantCulture);
            if (message.MessageType < 0)
            {
                type = string.Empty;
            }

            this.textUsage.AddCell(ximple, message.Text, index);
            this.typeUsage.AddCell(ximple, type, index);
            this.titleUsage.AddCell(ximple, message.Title, index);
            return true;
        }

        private void Save()
        {
            var context = ServiceLocator.Current.GetInstance<IPersistenceService>().GetContext<List<MessageInfo>>();
            context.Value = new List<MessageInfo>(this.messages.Values);
        }

        /// <summary>
        /// Information about a message.
        /// This class is only public to support XML serialization.
        /// </summary>
        public class MessageInfo
        {
            /// <summary>
            /// Empty message info used to clear data.
            /// </summary>
            public static readonly MessageInfo Empty = new MessageInfo
                { StartSeconds = -1, MessageType = -1, Title = string.Empty, Text = string.Empty };

            /// <summary>
            /// Gets or sets the index of this stop in the stop list.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the message type.
            /// </summary>
            public int MessageType { get; set; }

            /// <summary>
            /// Gets or sets the seconds since midnight
            /// for the start of the validity of this message.
            /// </summary>
            public int StartSeconds { get; set; }

            /// <summary>
            /// Gets or sets the seconds since midnight
            /// for the end of the validity of this message.
            /// </summary>
            public int EndSeconds { get; set; }

            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the message text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Checks if this message should be displayed
            /// at the given time.
            /// </summary>
            /// <param name="now">
            /// The seconds since midnight of the current time.
            /// </param>
            /// <returns>
            /// true if the message should be displayed.
            /// </returns>
            public bool IsValid(int now)
            {
                if (this.StartSeconds < 0 || this.EndSeconds < 0)
                {
                    return true;
                }

                if (this.StartSeconds <= this.EndSeconds)
                {
                    return now >= this.StartSeconds && now <= this.EndSeconds;
                }

                return now >= this.StartSeconds || now <= this.EndSeconds;
            }
        }
    }
}
