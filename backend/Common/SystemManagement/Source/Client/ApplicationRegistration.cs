// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationRegistration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.SystemManager.ServiceModel.Messages;

    using NLog;

    /// <summary>
    /// The application registration implementation.
    /// </summary>
    internal class ApplicationRegistration : IApplicationRegistration, IManageableObject
    {
        private static readonly TimeSpan RegistrationRetryTimeout = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan StateUpdateRetryTimeout = TimeSpan.FromSeconds(2);

        private readonly Logger logger;

        private readonly SystemManagerClient client;

        private readonly int processId;

        private readonly ITimer registrationRetryTimer;
        private readonly ITimer stateUpdateRetryTimer;

        private readonly ShutdownCatcher shutdownCatcher;

        private string applicaitonId;

        private string ApplicaitonId
        {
            get
            {
                return this.applicaitonId;
            }
            set
            {
                this.applicaitonId = value;
                logger.Info("ApplicationId Changed = {0}", this.applicaitonId);
            }
        }

        private ApplicationState state;

        private bool registrationStarted;

        private bool exitRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRegistration"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the application.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        public ApplicationRegistration(string name, SystemManagerClient client)
        {
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + name.Replace('.', '_'));
            this.Name = name;
            this.client = client;

            var process = Process.GetCurrentProcess();
            this.processId = process.Id;
            logger.Info("Initialize the Application Registration for {0}, Process Id {1}", this.Name, this.processId);

            this.registrationRetryTimer = TimerFactory.Current.CreateTimer("Registration-" + name);
            this.stateUpdateRetryTimer = TimerFactory.Current.CreateTimer("StateUpdate-" + name);

            this.shutdownCatcher = new ShutdownCatcher();
        }

        /// <summary>
        /// Event that is fired when the registration
        /// (started with <see cref="Register"/>) was successful.
        /// </summary>
        public event EventHandler Registered;

        /// <summary>
        /// Event that is fired when the watchdog was kicked.
        /// You can set <see cref="CancelEventArgs.Cancel"/> to true to
        /// prevent the client from sending a response to the request.
        /// This will then at some point trigger a re-launch from the System Manager.
        /// </summary>
        public event CancelEventHandler WatchdogKicked;

        /// <summary>
        /// Event that is fired when a application re-launch was requested by System Manager.
        /// </summary>
        public event EventHandler RelaunchRequested;

        /// <summary>
        /// Event that is fired when an application exit was requested by System Manager.
        /// </summary>
        public event EventHandler ExitRequested;

        /// <summary>
        /// Gets the name of this registration.
        /// This is the same as the argument given to <see cref="SystemManagerClient.CreateRegistration"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the state of this application.
        /// </summary>
        public ApplicationState State
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

                this.logger.Info("Application {0} State changed from {1} to {2}", this.Name, this.state, value);
                this.state = value;

                this.SendStateChangeMessage(value);
                this.stateUpdateRetryTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Gets the information about this application.
        /// </summary>
        public ApplicationInfo Info { get; private set; }

        /// <summary>
        /// Initializes this object.
        /// This method has to be called before any other in this class.
        /// </summary>
        public void Initialize()
        {
            logger.Info("Initialize() Enter for application {0}", this.Name);
            this.registrationRetryTimer.Interval = RegistrationRetryTimeout;
            this.registrationRetryTimer.AutoReset = true;
            this.registrationRetryTimer.Elapsed += (s, e) => this.SendRegisterMessage();

            this.stateUpdateRetryTimer.Interval = StateUpdateRetryTimeout;
            this.stateUpdateRetryTimer.AutoReset = true;
            this.stateUpdateRetryTimer.Elapsed += (s, e) => this.SendStateChangeMessage(this.State);

            this.shutdownCatcher.ShuttingDown += this.ShutdownCatcherOnShuttingDown;

            MessageDispatcher.Instance.Subscribe<RegisterAcknowledge>(this.HandleRegisterAcknowledge);
            MessageDispatcher.Instance.Subscribe<StateChangeAcknowledge>(this.HandleStateChangeAcknowledge);
            MessageDispatcher.Instance.Subscribe<AliveRequest>(this.HandleAliveRequest);
            MessageDispatcher.Instance.Subscribe<ExitApplicationCommand>(this.HandleExitApplicationCommand);
        }

        /// <summary>
        /// Begins to register this registration with the System Manager.
        /// When the registration was successful, <see cref="IApplicationRegistration.Registered"/> will be fired.
        /// </summary>
        public void Register()
        {
            if (this.registrationStarted)
            {
                logger.Error("REgister cannot be called again !!!");
                throw new NotSupportedException("Can't call Register() twice");
            }

            this.registrationStarted = true;
            this.registrationRetryTimer.Enabled = true;
            this.State = ApplicationState.Starting;
            this.SendRegisterMessage();
        }

        /// <summary>
        /// Sets the state of this application to <see cref="ApplicationState.Running"/>.
        /// This should be called once the application's main functionality is running
        /// (it's not starting up anymore).
        /// </summary>
        public void SetRunning()
        {
            this.State = ApplicationState.Running;
        }

        /// <summary>
        /// Sets the state of this application to <see cref="ApplicationState.Exiting"/>.
        /// This should be called once the application received the <see cref="ExitRequested"/>
        /// event and is exiting.
        /// </summary>
        public void SetExiting()
        {
            this.State = ApplicationState.Exiting;
        }

        /// <summary>
        /// Asks the System Manager to exit this application with the given reason.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Exit(string reason)
        {
            if (this.Info == null)
            {
                this.logger.Warn("Can't exit using System Manager ({0}), requesting application to exit", reason);
                this.RaiseExitRequested(EventArgs.Empty);
                return;
            }

            this.client.ExitApplication(this.Info, reason);
        }

        /// <summary>
        /// Asks the System Manager to re-launch this application with the given reason.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Relaunch(string reason)
        {
            if (this.Info == null)
            {
                this.logger.Warn("Can't relaunch ({0}): not connected to System Manager", reason);
                return;
            }

            this.client.RelaunchApplication(this.Info, reason);
        }

        /// <summary>
        /// Deregisters this application from the System Manager.
        /// This will set the <see cref="IApplicationRegistration.State"/> to <see cref="ApplicationState.Exiting"/>.
        /// </summary>
        public void Deregister()
        {
            if (!this.client.Deregister(this))
            {
                return;
            }

            this.shutdownCatcher.Stop();

            this.State = ApplicationState.Exiting;

            this.registrationRetryTimer.Dispose();
            this.stateUpdateRetryTimer.Dispose();

            MessageDispatcher.Instance.Unsubscribe<RegisterAcknowledge>(this.HandleRegisterAcknowledge);
            MessageDispatcher.Instance.Unsubscribe<StateChangeAcknowledge>(this.HandleStateChangeAcknowledge);
            MessageDispatcher.Instance.Unsubscribe<AliveRequest>(this.HandleAliveRequest);
            MessageDispatcher.Instance.Unsubscribe<ExitApplicationCommand>(this.HandleExitApplicationCommand);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            var registrationState = this.Info != null
                                        ? "Registered"
                                        : (this.registrationStarted ? "Registering" : "Not registered");

            yield return new ManagementProperty<string>(
                    "Process ID", this.processId.ToString(CultureInfo.InvariantCulture), true);
            yield return new ManagementProperty<string>("Application State", this.State.ToString(), true);
            yield return new ManagementProperty<string>("Registration State", registrationState, true);

            if (this.Info == null)
            {
                yield break;
            }

            yield return new ManagementProperty<string>("Version", this.Info.Version, true);
            yield return new ManagementProperty<string>("Path", this.Info.Path, true);
        }

        /// <summary>
        /// Creates a new loop observer with the given name and a timeout.
        /// </summary>
        /// <param name="name">
        /// The name of the observer, used for logging.
        /// </param>
        /// <param name="timeout">
        /// The timeout after which the observer should ask the application to restart.
        /// </param>
        /// <returns>
        /// The new <see cref="ILoopObserver"/>.
        /// </returns>
        public ILoopObserver CreateLoopObserver(string name, TimeSpan timeout)
        {
            return new LoopObserver(name, timeout, this);
        }

        /// <summary>
        /// Raises the <see cref="Registered"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseRegistered(EventArgs e)
        {
            var handler = this.Registered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="WatchdogKicked"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseWatchdogKicked(CancelEventArgs e)
        {
            var handler = this.WatchdogKicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RelaunchRequested"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseRelaunchRequested(EventArgs e)
        {
            var handler = this.RelaunchRequested;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ExitRequested"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseExitRequested(EventArgs e)
        {
            lock (this)
            {
                if (this.exitRequested)
                {
                    return;
                }

                this.exitRequested = true;
            }

            this.logger.Debug("Raising ExitRequested event");
            var handler = this.ExitRequested;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SendRegisterMessage()
        {
            MessageDispatcher.Instance.Send(
                this.client.SystemManagerAddress,
                new RegisterMessage { Name = this.Name, ProcessId = this.processId });
        }

        private void SendStateChangeMessage(ApplicationState value)
        {
            if (this.ApplicaitonId == null)
            {
#if DEBUG
                logger.Trace("SendStateChangeMessage() Ignored applicationId == null, App not registerd yetr");
#endif
                return;
            }

            logger.Info("MessageDispatch to SystemManager for Application Id {0} State was changed to {1}", this.ApplicaitonId, value);
            MessageDispatcher.Instance.Send(
                this.client.SystemManagerAddress,
                new StateChangeMessage { ApplicationId = this.ApplicaitonId, State = MessageConverter.Convert(value) });
        }

        private void ShutdownCatcherOnShuttingDown(object sender, EventArgs e)
        {
            this.logger.Info("Got shutdown event");

            var waitEvent = new ManualResetEvent(false);
            this.ExitRequested += (o, args) => waitEvent.Set();
            this.client.Reboot("Application got shutdown event");

            // wait for 20 seconds (we should get the shutdown command from SM)
            if (!waitEvent.WaitOne(20 * 1000, false))
            {
                // we didn't get the exit command, so let's exit ourselves
                this.RaiseExitRequested(e);
            }

            // wait for forever (this thread is in background mode, so it will get aborted once the application exits)
            new AutoResetEvent(false).WaitOne();
        }

        private void HandleRegisterAcknowledge(object sender, MessageEventArgs<RegisterAcknowledge> e)
        {
            if (e.Message.Request.Name != this.Name || e.Message.Request.ProcessId != this.processId)
            {
                return;
            }

            if (this.ApplicaitonId != null)
            {
                this.logger.Warn(
                    "Got a second register acknowledge, ignoring it; current appId={0}, second appId={1}",
                    this.ApplicaitonId,
                    e.Message.ApplicationId);
                return;
            }

            this.ApplicaitonId = e.Message.ApplicationId;
            this.Info = new ApplicationInfo(e.Message.Info);
            this.registrationRetryTimer.Enabled = false;
            this.shutdownCatcher.Start(null);
            this.RaiseRegistered(EventArgs.Empty);
        }

        private void HandleStateChangeAcknowledge(object sender, MessageEventArgs<StateChangeAcknowledge> e)
        {
            if (e.Message.ApplicationId != this.ApplicaitonId
                || MessageConverter.Convert(e.Message.State) != this.State)
            {
                return;
            }

            // System Manager received the current state, we can stop resending it
            this.stateUpdateRetryTimer.Enabled = false;
        }

        private void HandleAliveRequest(object sender, MessageEventArgs<AliveRequest> e)
        {
            if (e.Message.ApplicationId != this.ApplicaitonId)
            {
                return;
            }

            var args = new CancelEventArgs(false);
            this.RaiseWatchdogKicked(args);

            if (args.Cancel)
            {
                return;
            }

            MessageDispatcher.Instance.Send(
                e.Source, new AliveResponse { ApplicationId = this.ApplicaitonId, RequestId = e.Message.RequestId });
        }

        private void HandleExitApplicationCommand(object sender, MessageEventArgs<ExitApplicationCommand> e)
        {
            if (e.Message.ApplicationId != this.ApplicaitonId)
            {
                this.logger.Warn("Got exit command that wasn't meant for me!");
                return;
            }

            ThreadPool.QueueUserWorkItem(s => this.RaiseExitRequested(EventArgs.Empty));
        }
    }
}