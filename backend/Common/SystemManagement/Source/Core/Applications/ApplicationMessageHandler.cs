// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationMessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationMessageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Messages;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Motion.SystemManager.ServiceModel.Messages;

    using NLog;

    using ApplicationInfo = Gorba.Motion.SystemManager.ServiceModel.Messages.ApplicationInfo;
    using ApplicationReasonInfo = Gorba.Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo;

    /// <summary>
    /// The Medi message handler for <see cref="ApplicationControllerBase"/>.
    /// </summary>
    public class ApplicationMessageHandler : IDisposable
    {
        private readonly Logger logger;
        private readonly ApplicationControllerBase controller;

        private readonly string applicationId;

        private int aliveCounter;

        private MediAddress clientAddress;

        private int lastAliveId;

        private IMessageDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMessageHandler"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        public ApplicationMessageHandler(ApplicationControllerBase controller)
        {
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + controller.Name.Replace('.', '_'));
            this.controller = controller;
            this.applicationId = controller.ApplicationId;

            this.dispatcher = MessageDispatcher.Instance.GetNamedDispatcher(Addresses.SystemManagerDispatcher);

            this.controller.StateChanged += this.ControllerOnStateChanged;

            this.dispatcher.Subscribe<RegisterMessage>(this.HandleRegisterMessage);
            this.dispatcher.Subscribe<StateChangeMessage>(this.HandleStateChangeMessage);
            this.dispatcher.Subscribe<AliveResponse>(this.HandleAliveResponse);
            this.dispatcher.Subscribe<RelaunchApplicationRequest>(this.HandleRelaunchApplicationRequest);
            this.dispatcher.Subscribe<ExitApplicationRequest>(this.HandleExitApplicationRequest);
            this.dispatcher.Subscribe<ApplicationStateRequest>(this.HandleApplicationStateRequest);
        }

        /// <summary>
        /// Event that is fired when a correct alive response was received after
        /// <see cref="SendAliveRequest"/> was called.
        /// </summary>
        public event EventHandler AliveResponseReceived;

        /// <summary>
        /// Sends an alive request to the application.
        /// </summary>
        /// <returns>
        /// True if the message was sent (i.e. the application has registered before).
        /// </returns>
        public bool SendAliveRequest()
        {
            if (this.clientAddress == null)
            {
                return false;
            }

            this.lastAliveId = Interlocked.Increment(ref this.aliveCounter);

            this.dispatcher.Send(
                this.clientAddress,
                new AliveRequest { ApplicationId = this.applicationId, RequestId = this.lastAliveId });
            return true;
        }

        /// <summary>
        /// Sends the application a command telling it to exit immediately.
        /// </summary>
        /// <returns>
        /// True if the message was sent (i.e. the application has registered before).
        /// </returns>
        public bool SendExitCommand()
        {
            if (this.clientAddress == null)
            {
                return false;
            }

            this.dispatcher.Send(this.clientAddress, new ExitApplicationCommand { ApplicationId = this.applicationId });
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.dispatcher.Unsubscribe<RegisterMessage>(this.HandleRegisterMessage);
            this.dispatcher.Unsubscribe<StateChangeMessage>(this.HandleStateChangeMessage);
            this.dispatcher.Unsubscribe<AliveResponse>(this.HandleAliveResponse);
            this.dispatcher.Unsubscribe<RelaunchApplicationRequest>(this.HandleRelaunchApplicationRequest);
            this.dispatcher.Unsubscribe<ExitApplicationRequest>(this.HandleExitApplicationRequest);
            this.dispatcher.Unsubscribe<ApplicationStateRequest>(this.HandleApplicationStateRequest);

            this.controller.StateChanged -= this.ControllerOnStateChanged;
        }

        /// <summary>
        /// Raises the <see cref="AliveResponseReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseAliveResponseReceived(EventArgs e)
        {
            var handler = this.AliveResponseReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private StateChangeNotification CreateStateChangeNotification()
        {
            return new StateChangeNotification
                       {
                           ApplicationId = this.applicationId,
                           State = MessageConverter.Convert(this.controller.State)
                       };
        }

        private void ControllerOnStateChanged(object sender, EventArgs eventArgs)
        {
            this.dispatcher.Broadcast(this.CreateStateChangeNotification());
        }

        private void HandleRegisterMessage(object sender, MessageEventArgs<RegisterMessage> e)
        {
            var procController = this.controller as ProcessApplicationController;
            if (procController != null)
            {
                if (e.Message.ProcessId != procController.ProcessId)
                {
                    // not for us
                    return;
                }
            }
            else
            {
                if (e.Message.Name != this.controller.Name)
                {
                    // not for us
                    return;
                }
            }

            this.logger.Debug("Got registration from {0}", e.Source);
            this.clientAddress = e.Source;
            this.dispatcher.Send(
                this.clientAddress,
                new RegisterAcknowledge
                    {
                        ApplicationId = this.applicationId,
                        Info = this.controller.CreateApplicationInfo().ToMessage(),
                        Request = e.Message
                    });
        }

        private void HandleStateChangeMessage(object sender, MessageEventArgs<StateChangeMessage> e)
        {
            if (e.Message.ApplicationId != this.applicationId)
            {
                return;
            }

            if (this.controller.State == ApplicationState.AwaitingLaunch
                || this.controller.State == ApplicationState.Exiting
                || this.controller.State == ApplicationState.Unknown)
            {
                // we get a notification in a state where we are not expected to get a state change
                this.logger.Warn("Got state change to {0} in {1}, ignoring it", e.Message.State, this.controller.State);
                return;
            }

            this.dispatcher.Send(
                e.Source,
                new StateChangeAcknowledge { ApplicationId = this.applicationId, State = e.Message.State });

            this.controller.State = MessageConverter.Convert(e.Message.State);
        }

        private void HandleAliveResponse(object sender, MessageEventArgs<AliveResponse> e)
        {
            if (e.Message.ApplicationId != this.applicationId || e.Message.RequestId != this.lastAliveId)
            {
                return;
            }

            this.lastAliveId = -1;
            this.RaiseAliveResponseReceived(EventArgs.Empty);
        }

        private void HandleExitApplicationRequest(object sender, MessageEventArgs<ExitApplicationRequest> e)
        {
            if (e.Message.ApplicationId != this.applicationId)
            {
                return;
            }

            this.controller.RequestExit(string.Format("From {0}: {1}", e.Source, e.Message.Reason));
        }

        private void HandleRelaunchApplicationRequest(object sender, MessageEventArgs<RelaunchApplicationRequest> e)
        {
            if (e.Message.ApplicationId != this.applicationId)
            {
                return;
            }

            var attribute = e.Source.Application.IndexOf("Update", StringComparison.InvariantCultureIgnoreCase) >= 0
                                ? ApplicationRelaunchAttribute.SoftwareUpdate
                                : ApplicationRelaunchAttribute.User;
            this.controller.RequestRelaunch(attribute, string.Format("From {0}: {1}", e.Source, e.Message.Reason));
        }

        private void HandleApplicationStateRequest(object sender, MessageEventArgs<ApplicationStateRequest> e)
        {
            if (e.Message.ApplicationId != this.applicationId)
            {
                return;
            }

            this.dispatcher.Send(e.Source, this.CreateStateChangeNotification());
        }
    }
}
