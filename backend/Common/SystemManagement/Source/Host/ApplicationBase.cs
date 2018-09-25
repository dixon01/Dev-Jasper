// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host
{
    using System;
    using System.Threading;

    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Compatibility.Container;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Base class for applications.
    /// </summary>
    public abstract class ApplicationBase : IRunnableApplication
    {
        /// <summary>
        /// The logger for this class.
        /// </summary>
        protected readonly Logger Logger;

        private IApplicationRegistration registration;

        public EventHandler RegistrationCompleted;

        private bool registered;

        private string appName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBase"/> class.
        /// </summary>
        protected ApplicationBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);          
        }

        private void SetupRegistration(string name)
        {
            this.registration = SystemManagerClient.Instance.CreateRegistration(name);

            this.registration.Registered += (sender, args) => this.registered = true;
            this.registration.ExitRequested += this.RegistrationOnExitRequested;

            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            
            serviceContainer.RegisterInstance(name, this.registration);

            this.registration.Register();
            var handler = this.RegistrationCompleted;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Configure(string name)
        {
            this.Logger.Trace("Configuring application: {0}", name);
            this.appName = name;
            SetupRegistration(name);
        }

        /// <summary>
        /// Starts this application.
        /// </summary>
        /// <exception cref="ThreadStateException">The thread is dead. </exception>
        public virtual void Start()
        {
            var thread = new Thread(() => this.Run(new string[0]));
            thread.IsBackground = false;
            thread.Name = this.appName;

            thread.Start();
        }

        /// <summary>
        /// Stops this application.
        /// </summary>
        public void Stop()
        {
            this.Logger.Trace("Stopping application");
            this.registration.SetExiting();
            this.DoStop();
        }

        /// <summary>
        /// Runs this application.
        /// This method will not return until after <see cref="Stop"/> was called.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        public void Run(string[] args)
        {
            this.Logger.Trace("Starting application");
            this.DoRun(args);

            this.registration.Deregister();
        }

        /// <summary>
        /// Asks the System Manager to re-launch this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Relaunch(string reason)
        {
            this.Logger.Trace("Relaunching application: {0} - Reason: {1}'", this.appName, reason);
            if (this.registered)
            {
                this.registration.Relaunch(reason);
            }
            else
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Asks the System Manager to exit this application with the given reason.
        /// If this application was not registered with the System Manager, it will simply stop.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Exit(string reason)
        {
            this.Logger.Trace("Exiting application: '{0}'", reason);
            if (this.registered)
            {
                this.registration.Exit(reason);
            }
            else
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Sets the state of this application to <see cref="ApplicationState.Running"/>.
        /// This should be called once the application's main functionality is running
        /// (it's not starting up anymore).
        /// </summary>
        public void SetRunning()
        {
            if (this.registration != null)
            {
                this.Logger.Trace("SetRunning() Enter");
                this.registration.SetRunning();
            }
            else
            {
                this.Logger.Error("SetRunning() registration == null, cannot set the Application {0} to Running state!", this.appName);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="Run"/> method.
        /// This method should not return until after <see cref="Stop"/> was called.
        /// Implementing classes should either override <see cref="DoRun()"/> or this method.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        protected virtual void DoRun(string[] args)
        {
            this.DoRun();
        }

        /// <summary>
        /// Implementation of the <see cref="Run"/> method.
        /// This method should not return until after <see cref="Stop"/> was called.
        /// Implementing classes should either override <see cref="DoRun(string[])"/> or this method.
        /// </summary>
        protected virtual void DoRun()
        {
        }

        /// <summary>
        /// Implementation of the <see cref="Stop"/> method.
        /// This method should stop whatever is running in <see cref="DoRun()"/>.
        /// </summary>
        protected abstract void DoStop();

        private void RegistrationOnExitRequested(object sender, EventArgs eventArgs)
        {
            this.Logger.Info("Exit requested by System Manager");
            this.Stop();
        }
    }
}
