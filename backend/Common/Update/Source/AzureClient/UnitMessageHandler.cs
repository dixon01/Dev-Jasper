// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitMessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitMessageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.AzureClient
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The message handler which is responsible for update messages for one unit.
    /// </summary>
    internal class UnitMessageHandler : IDisposable
    {
        private readonly AzureUpdateClient client;

        private readonly Logger logger;

        private readonly IMessageDispatcher messageDispatcher;

        private readonly List<UpdateStateInfo> waitingFeedback = new List<UpdateStateInfo>();

        private readonly ITimer resendFeedbackTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitMessageHandler"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        public UnitMessageHandler(string unitName, AzureUpdateClient client)
        {
            this.client = client;
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + unitName);

            this.messageDispatcher =
                MessageDispatcher.Instance.GetNamedDispatcher(
                    new MediAddress(unitName, AzureUpdateClient.VirtualApplicationName));
            this.messageDispatcher.Subscribe<UpdateCommand>(this.HandleUpdateCommand);
            this.messageDispatcher.Subscribe<UpdateStateInfoAck>(this.HandleUpdateStateInfoAck);

            this.resendFeedbackTimer = TimerFactory.Current.CreateTimer("ResendFeedback");
            this.resendFeedbackTimer.AutoReset = true;
            this.resendFeedbackTimer.Interval = TimeSpan.FromMinutes(5);
            this.resendFeedbackTimer.Elapsed += (s, e) => this.ResendFeedback();
            this.resendFeedbackTimer.Enabled = true;
        }

        /// <summary>
        /// Event that is risen whenever a new update command received.
        /// </summary>
        public event EventHandler<MessageEventArgs<UpdateCommand>> UpdateCommandReceived;

        /// <summary>
        /// Sends feedback to the background system, taking care of acknowledges.
        /// </summary>
        /// <param name="feedback">
        /// The feedback.
        /// </param>
        public void SendFeedback(UpdateStateInfo feedback)
        {
            lock (this.waitingFeedback)
            {
                this.waitingFeedback.Add(feedback);
            }

            this.SendMessageToProvider(feedback);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.resendFeedbackTimer.Enabled = false;
            this.messageDispatcher.Unsubscribe<UpdateCommand>(this.HandleUpdateCommand);
            this.messageDispatcher.Unsubscribe<UpdateStateInfoAck>(this.HandleUpdateStateInfoAck);
            this.messageDispatcher.Dispose();
        }

        private void ResendFeedback()
        {
            UpdateStateInfo[] feedbacks;
            lock (this.waitingFeedback)
            {
                feedbacks = this.waitingFeedback.ToArray();
            }

            if (feedbacks.Length == 0)
            {
                return;
            }

            this.logger.Info("Resending {0} feedbacks", this.waitingFeedback);

            foreach (var feedback in feedbacks)
            {
                this.SendMessageToProvider(feedback);
            }
        }

        private void SendMessageToProvider(object message)
        {
            if (this.client.ProviderAddress == null)
            {
                this.logger.Warn("Don't know to whom to send message: {0}", message);
                return;
            }

            this.logger.Trace("Sending message to {0}: {1}", this.client.ProviderAddress, message);
            this.messageDispatcher.Send(this.client.ProviderAddress, message);
        }

        private void HandleUpdateCommand(object sender, MessageEventArgs<UpdateCommand> e)
        {
            this.logger.Debug("Received update command for {0} from {1}", e.Message.UnitId.UnitName, e.Source);
            var handler = this.UpdateCommandReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleUpdateStateInfoAck(object sender, MessageEventArgs<UpdateStateInfoAck> e)
        {
            this.logger.Trace(
                "Received acknowledge for update feedback: {0}[{1}] state: {2}",
                e.Message.UpdateId.BackgroundSystemGuid,
                e.Message.UpdateId.UpdateIndex,
                e.Message.UpdateState);
            lock (this.waitingFeedback)
            {
                this.waitingFeedback.RemoveAll(
                    f =>
                    f.UpdateId.Equals(e.Message.UpdateId)
                    && f.UnitId.Equals(e.Message.UnitId)
                    && f.State == e.Message.UpdateState);
            }
        }
    }
}