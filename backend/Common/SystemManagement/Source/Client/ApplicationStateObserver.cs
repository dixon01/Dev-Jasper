// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationStateObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationStateObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.SystemManager.ServiceModel.Messages;

    /// <summary>
    /// Implementation of <see cref="IApplicationStateObserver"/>.
    /// This class is only to be used by <see cref="SystemManagerClient"/> directly.
    /// </summary>
    internal class ApplicationStateObserver : IApplicationStateObserver
    {
        private readonly string applicationId;

        private readonly IMessageDispatcher messageDispatcher;

        private readonly MediAddress systemManagerAddress;

        private Common.SystemManagement.ServiceModel.ApplicationState state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStateObserver"/> class.
        /// </summary>
        /// <param name="application">
        /// The application information.
        /// </param>
        /// <param name="messageDispatcher">
        /// The <see cref="MessageDispatcher"/> to use for communication with the remote System Manager.
        /// </param>
        /// <param name="systemManagerAddress">
        /// The address of the remote System Manager (actually of the controller named dispatcher).
        /// </param>
        public ApplicationStateObserver(
            ApplicationInfo application, IMessageDispatcher messageDispatcher, MediAddress systemManagerAddress)
        {
            this.applicationId = application.Id;
            this.state = application.State;
            this.ApplicationName = application.Name;

            this.messageDispatcher = messageDispatcher;
            this.systemManagerAddress = systemManagerAddress;

            this.messageDispatcher.Subscribe<StateChangeNotification>(this.HandleStateChangeNotification);
        }

        /// <summary>
        /// Event that is fired when <see cref="IApplicationStateObserver.State"/> changes.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        public Common.SystemManagement.ServiceModel.ApplicationState State
        {
            get
            {
                return this.state;
            }

            private set
            {
                if (this.state == value)
                {
                    return;
                }

                this.state = value;
                this.RaiseStateChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.messageDispatcher.Unsubscribe<StateChangeNotification>(this.HandleStateChangeNotification);
        }

        /// <summary>
        /// Raises the <see cref="StateChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStateChanged(EventArgs e)
        {
            var handler = this.StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleStateChangeNotification(object sender, MessageEventArgs<StateChangeNotification> e)
        {
            if (e.Message.ApplicationId == this.applicationId && e.Source.Equals(this.systemManagerAddress))
            {
                this.State = MessageConverter.Convert(e.Message.State);
            }
        }
    }
}