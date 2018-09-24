// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Message type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Message
{
    using System;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// A message shown on the UI.
    /// </summary>
    internal class Message
    {
        private readonly int id;
        private readonly string message;
        private readonly MessageHandler msgHandler;
        private readonly DateTime timeReceived;
        private readonly MessageType type = MessageType.Info;

        private readonly ITimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name = "message">
        /// Message to show
        /// </param>
        /// <param name = "type">
        /// type of the message
        /// </param>
        /// <param name = "id">
        /// the id of this message
        /// </param>
        /// <param name = "timeToShow">
        /// time to show the message in seconds. Hint: if type is Confirm, timeToShow has no influence
        /// </param>
        /// <param name = "msgHandler">
        /// Message handler
        /// </param>
        public Message(string message, MessageType type, int id, int timeToShow, MessageHandler msgHandler)
        {
            this.message = message;
            this.type = type;
            this.id = id;
            this.msgHandler = msgHandler;

            this.timeReceived = TimeProvider.Current.Now;

            // Instruction messages have no time. Needs driver confirmation (press icon)
            if (this.type == MessageType.Instruction)
            {
                return;
            }

            this.timer = TimerFactory.Current.CreateTimer("Message-" + id);
            this.timer.AutoReset = false;
            this.timer.Interval = TimeSpan.FromSeconds(timeToShow);
            this.timer.Elapsed += this.TimerOnElapsed;
        }

        /// <summary>
        /// Shows this message.
        /// </summary>
        public void Show()
        {
            if (this.timer != null)
            {
                this.timer.Enabled = true;
            }

            this.msgHandler.MessageField.ShowMessage(this.type, this.message, this.id);
        }

        /// <summary>
        /// Creates the icon list entry for this message.
        /// </summary>
        /// <returns>
        /// The <see cref="MessageListEntry"/>.
        /// </returns>
        internal MessageListEntry CreateIconListEntry()
        {
            return new MessageListEntry(this.type, this.timeReceived, this.message);
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.timer.Dispose();
            this.msgHandler.ShownDone(this.id);
        }
    }
}