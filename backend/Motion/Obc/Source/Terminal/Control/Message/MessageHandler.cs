// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Message
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The message handler.
    /// </summary>
    internal class MessageHandler : IMessageField
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MessageHandler>();

        private readonly List<MessageListEntry> allMessages = new List<MessageListEntry>();

        private readonly object locker;

        private readonly IMessageField messageField;

        private readonly Queue<Message> messageQueue;

        private readonly Configurator configurator;

        private Message actualMessage;

        private string defaultText1 = string.Empty;

        private string defaultText2 = string.Empty;

        private int timeToShow = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal MessageHandler(IContext context)
        {
            this.messageQueue = new Queue<Message>();
            this.locker = new object();
            this.messageField = context.UiRoot.MessageField;
            this.configurator =
                new Configurator(PathManager.Instance.GetPath(FileType.Data, ConfigPaths.SavedTmpMessages));

            var iconBar = context.UiRoot.IconBar;
            foreach (var mle in this.allMessages)
            {
                switch (mle.IconType)
                {
                    case MessageType.Error:
                        iconBar.SetAlarmMessageIcon(true);
                        break;
                    case MessageType.Alarm:
                    case MessageType.Info:
                    case MessageType.Instruction:
                        iconBar.SetInformationMessageIcon(true);
                        break;
                }
            }

            this.messageField.Confirmed += this.MessageFieldConfirmedEvent;

            MessageDispatcher.Instance.Subscribe<evMessage>(this.EvMessageEvent);
            MessageDispatcher.Instance.Subscribe<evERGError>(this.EvErgErrorEvent);

            LanguageManager.Instance.CurrentLanguageChanged += this.CurrentLanguageChanged;
        }

        /// <summary>
        /// The confirmed event.
        /// </summary>
        public event EventHandler<IndexEventArgs> Confirmed;

        /// <summary>
        /// Gets or sets the time to show for info and error messages.
        /// Time to show a message in seconds. When this time is
        /// expired, the default text (see: ShowDefaultText) will be activated
        /// </summary>
        public int TimeToShow
        {
            get
            {
                return this.timeToShow;
            }

            set
            {
                this.timeToShow = value;
            }
        }

        /// <summary>
        /// Gets the message field.
        /// </summary>
        internal IMessageField MessageField
        {
            get
            {
                return this.messageField;
            }
        }

        /// <summary>
        /// Sets the destination text.
        /// </summary>
        /// <param name="txt">
        /// The text.
        /// </param>
        public void SetDestinationText(string txt)
        {
            this.defaultText2 = txt;
            this.ActivateDefaultText();
        }

        /// <summary>
        ///   Shows a message
        /// </summary>
        /// <param name = "type">type of the message</param>
        /// <param name = "text">message text</param>
        /// <param name = "msgId">the ID of this message</param>
        public void ShowMessage(MessageType type, string text, int msgId)
        {
            this.AddMessage(text, type, msgId);
        }

        /// <summary>
        /// Adds a message.
        /// </summary>
        /// <param name="text">
        /// The message text.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The message id.
        /// </param>
        public void AddMessage(string text, MessageType type, int id)
        {
            var message = new Message(text, type, id, this.timeToShow, this);
            this.allMessages.Insert(0, message.CreateIconListEntry());
            ////allMessages.Add(message.CreateIconListEntry());
            lock (this.locker)
            {
                if (this.actualMessage == null)
                {
                    this.actualMessage = message;
                    message.Show();
                }
                else
                {
                    this.messageQueue.Enqueue(message);
                }
            }

            try
            {
                this.configurator.Serialize(this.allMessages);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't serialize message list", ex);
            }
        }

        /// <summary>
        /// Hides the given message ID.
        /// </summary>
        /// <param name="msgId">
        /// The message ID.
        /// </param>
        internal void ShownDone(int msgId)
        {
            lock (this.locker)
            {
                if (this.messageQueue.Count > 0)
                {
                    this.actualMessage = this.messageQueue.Dequeue();
                    this.actualMessage.Show();
                }
                else
                {
                    this.actualMessage = null;
                    this.ActivateDefaultText();
                }
            }

            if (this.Confirmed != null)
            {
                this.Confirmed(this, new IndexEventArgs(msgId));
            }
        }

        /// <summary>
        /// Gets all messages.
        /// </summary>
        /// <returns>
        /// The list of messages.
        /// </returns>
        internal List<MessageListEntry> GetAllMessages()
        {
            return this.allMessages;
        }

        private void CurrentLanguageChanged(object sender, EventArgs e)
        {
            this.ActivateDefaultText();
        }

        private void ActivateDefaultText()
        {
            lock (this.locker)
            {
                if (this.actualMessage == null)
                {
                    if (this.defaultText2 == null)
                    {
                        this.defaultText1 = ml.ml_string(125, "No block loaded");
                    }
                    else if (this.defaultText2.Length == 0)
                    {
                        this.defaultText1 = ml.ml_string(125, "No block loaded");
                    }
                    else
                    {
                        this.defaultText1 = ml.ml_string(126, "Dest: ");
                    }

                    this.messageField.SetDestinationText(this.defaultText1 + this.defaultText2);
                }
            }
        }

        private void EvErgErrorEvent(object sender, MessageEventArgs<evERGError> e)
        {
            string errorDescription;
            switch (e.Message.ErrorCode)
            {
                case evERGError.E1_GENERAL_ERROR:
                    errorDescription = ml.ml_string(41, "ERG: Ticketing error"); // MLHIDE
                    break;
                case evERGError.E2_INVALID_DATA:
                    errorDescription = ml.ml_string(43, "ERG: Error cash box invalid data"); // MLHIDE
                    break;
                case evERGError.E3_TICKETING_ERROR:
                    errorDescription = ml.ml_string(44, "ERG: Error canceling machine"); // MLHIDE
                    break;
                default:
                    errorDescription = ml.ml_string(137, "ERG: Error Number: ") + e.Message.ErrorCode;
                    break;
            }

            if (errorDescription != null)
            {
                this.HandleMessageEvent(
                    33333 + e.Message.ErrorCode,
                    evMessage.Types.Text,
                    evMessage.SubTypes.Error,
                    evMessage.Messages.Undef,
                    errorDescription);
            }
        }

        private void EvMessageEvent(object sender, MessageEventArgs<evMessage> e)
        {
            if (e.Message.Destination != evMessage.Destinations.Driver)
            {
                return;
            }

            this.HandleMessageEvent(
                e.Message.MessageId,
                e.Message.MessageType,
                e.Message.SubType,
                e.Message.Message,
                e.Message.MessageText);
        }

        private void HandleMessageEvent(
            int messageId,
            evMessage.Types messageType,
            evMessage.SubTypes subType,
            evMessage.Messages message,
            string messageText)
        {
            MessageType type;
            switch (subType)
            {
                case evMessage.SubTypes.Error:
                    type = MessageType.Error;
                    break;
                case evMessage.SubTypes.Info:
                    type = MessageType.Info;
                    break;
                case evMessage.SubTypes.Instruction:
                    type = MessageType.Instruction;
                    break;
                case evMessage.SubTypes.System:
                    type = MessageType.Error;
                    break;
                default:
                    type = MessageType.Info;
                    break;
            }

            if (messageType == evMessage.Types.Predefined)
            {
                messageText = MessageStringDefinitions.GetMessageString(message);
            }

            this.AddMessage(messageText, type, messageId);
        }

        private void MessageFieldConfirmedEvent(object sender, IndexEventArgs e)
        {
            this.ShownDone(e.Index);
        }
    }
}