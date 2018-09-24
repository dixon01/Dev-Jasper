// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareAbstractionBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareAbstractionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Hal
{
    using System;

    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Motion.SystemManager.Core.Watchdog;

    /// <summary>
    /// The base class for the different HALs.
    /// </summary>
    public abstract partial class HardwareAbstractionBase
    {
        private bool running;

        private bool shutdownInitiated;

        /// <summary>
        /// Event that is fired when the hardware requests a shutdown (e.g. through ignition).
        /// </summary>
        public event EventHandler ShutdownRequested;

        /// <summary>
        /// Gets or sets the watchdog (can be null).
        /// </summary>
        public HardwareWatchdogControllerBase Watchdog { get; protected set; }

        /// <summary>
        /// Gets the serial number or null if it is unknown.
        /// </summary>
        public abstract string SerialNumber { get; }

        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        protected SystemConfig Config { get; private set; }

        /// <summary>
        /// Creates a new instance of the HAL for the current hardware.
        /// </summary>
        /// <param name="config">
        /// The system configuration.
        /// </param>
        /// <returns>
        /// A subclass of <see cref="HardwareAbstractionBase"/>.
        /// </returns>
        public static HardwareAbstractionBase Create(SystemConfig config)
        {
            var hardwareAbstraction = CreateHardwareLayer(config);
            hardwareAbstraction.Configure(config);
            return hardwareAbstraction;
        }

        /// <summary>
        /// Starts this controller and regularly kicks the hardware watchdog.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.DoStart();
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            this.DoStop();
        }

        /// <summary>
        /// Configures this HAL.
        /// </summary>
        /// <param name="config">
        /// The system configuration.
        /// </param>
        protected virtual void Configure(SystemConfig config)
        {
            this.Config = config;
        }

        /// <summary>
        /// Actual implementation of <see cref="Start"/>.
        /// </summary>
        protected virtual void DoStart()
        {
            if (this.Config.KickWatchdog)
            {
                this.Watchdog.Start();
            }
        }

        /// <summary>
        /// Actual implementation of <see cref="Stop"/>.
        /// </summary>
        protected virtual void DoStop()
        {
            if (this.Config.KickWatchdog)
            {
                this.Watchdog.Stop();
            }
        }

        /// <summary>
        /// Raises the <see cref="ShutdownRequested"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseShutdownRequested(EventArgs e)
        {
            if (this.shutdownInitiated)
            {
                return;
            }

            this.shutdownInitiated = true;
            var handler = this.ShutdownRequested;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
