// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Component.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Component type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.TestApps.ComponentTest
{
    using System;
    using System.ComponentModel;
    using System.Threading;

    using Gorba.Common.SystemManagement.Client;

    /// <summary>
    /// Example component implementing <see cref="IApplication"/>.
    /// </summary>
    public class Component : IApplication
    {
        private Thread thread;

        private string appName;

        private IApplicationRegistration registration;

        /// <summary>
        /// Configures this component.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Configure(string name)
        {
            this.appName = name;
        }

        /// <summary>
        /// Starts this component.
        /// </summary>
        public void Start()
        {
            if (this.registration != null)
            {
                return;
            }

            this.registration = SystemManagerClient.Instance.CreateRegistration(this.appName);
            this.registration.Registered += this.RegistrationOnRegistered;
            this.registration.WatchdogKicked += this.RegistrationOnWatchdogKicked;

            Console.WriteLine("Starting Test Component {0}", this.appName);
            this.registration.Register();
            this.thread = new Thread(this.Run) { IsBackground = true };
            this.thread.Start();
        }

        /// <summary>
        /// Stops this component.
        /// </summary>
        public void Stop()
        {
            Console.WriteLine("Stopping Test Component {0}", this.appName);
            this.thread = null;
        }

        private void Run()
        {
            this.registration.SetRunning();

            while (this.thread != null)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Hello from Test Component {0}", this.appName);
            }

            this.registration.Deregister();
            this.registration = null;
        }

        private void RegistrationOnRegistered(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Component registered");
        }

        private void RegistrationOnWatchdogKicked(object sender, CancelEventArgs e)
        {
            Console.WriteLine("Watchdog was kicked for {0}", this.appName);
        }
    }
}
