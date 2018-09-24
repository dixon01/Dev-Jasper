// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AHandler.Messages.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Text;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Telegram handler part related to messages for DS021a that
    /// handles connection information according to their <see cref="DS021AConfig"/>.
    /// </summary>
    public partial class DS021AHandler
    {
        private readonly SortedMap<int, string> messageParts = new SortedMap<int, string>();

        private ITimer messageFlushTimer;

        private GenericUsageHandler textUsage;

        private void ConfigureNewsticker()
        {
            this.messageFlushTimer = TimerFactory.Current.CreateTimer("FlushMessages");
            this.messageFlushTimer.AutoReset = false;
            this.messageFlushTimer.Elapsed += (s, e) => this.FlushMessage();
            if (this.config.FlushTimeout > TimeSpan.Zero)
            {
                this.messageFlushTimer.Interval = this.config.FlushTimeout;
            }

            this.textUsage = new GenericUsageHandler(this.config.UsedForText, this.Dictionary);
        }

        /// <summary>
        /// Entry point for message handling of DS021a 'S' variant.
        /// </summary>
        /// <param name="index">the <code>Sx</code> index</param>
        /// <param name="text">the text following x04 in the payload of the telegram</param>
        private void HandleMessageTelegram(int index, string text)
        {
            if (index < 0 || index > 9)
            {
                this.Logger.Warn("Received invalid message index: {0}", index);
                return;
            }

            this.messageFlushTimer.Enabled = false;

            if (index == 0)
            {
                this.ClearMessages();
                return;
            }

            var textChanged = this.messageParts.ContainsKey(index);

            if (text.Trim(' ').Length == 0)
            {
                // delete message part
                this.messageParts.Remove(index);
            }
            else
            {
                this.messageParts[index] = text;
            }

            if (index == 9 || textChanged)
            {
                this.FlushMessage();
            }
            else
            {
                this.messageFlushTimer.Enabled = true;
            }
        }

        private void ClearMessages()
        {
            this.messageParts.Clear();
            this.FlushMessage();
        }

        private void FlushMessage()
        {
            var message = new StringBuilder();
            foreach (var part in this.messageParts.Values)
            {
                message.Append(part);
            }

            var ximple = new Ximple();
            this.textUsage.AddCell(ximple, message.ToString());
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}