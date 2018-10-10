// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MgiHardwareAbstraction.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MgiHardwareAbstraction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Hal
{
    using System;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Mgi.AtmelControl;
    using Gorba.Motion.SystemManager.Core.Watchdog;

    using NLog;

    /// <summary>
    /// The HAL for MGI Topbox.
    /// </summary>
    public partial class MgiHardwareAbstraction : HardwareAbstractionBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AtmelControlClient atmelClient;

        private readonly ITimer holdTimer;

        private readonly ManualResetEvent startWait = new ManualResetEvent(false);

        private bool ignition;

        private string serialNumber;

        private bool initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="MgiHardwareAbstraction"/> class.
        /// </summary>
        public MgiHardwareAbstraction()
        {
            try
            {
                this.Watchdog = new KontronWatchdogController();

                this.atmelClient = new AtmelControlClient();
                this.atmelClient.ConnectedChanged += this.OnAtmelControlClientConnectionChanged;

                this.holdTimer = TimerFactory.Current.CreateTimer("IgnitionHold");
                this.holdTimer.AutoReset = false;
                this.holdTimer.Elapsed += (s, e) => this.RaiseShutdownRequested(e);
                this.initialized = true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to create KontronWatchdogController! {0}", ex.Message);
            }
        }

        /// <summary>
        /// Gets the serial number or null if it is unknown.
        /// </summary>
        public override string SerialNumber
        {
            get
            {
                return this.serialNumber;
            }
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareAbstractionBase.Start"/>.
        /// </summary>
        protected override void DoStart()
        {
            base.DoStart();
            if (this.initialized)
            {
                this.atmelClient.Connect();

                // wait until we know the ignition state
                Logger.Info("Waiting for ignition");
                if (this.startWait.WaitOne(TimeSpan.FromSeconds(10), false) && !this.ignition)
                {
                    // we got the current ignition state and it is "off", so we need to exit immediately
                    Logger.Warn("Ignition is off, exit immediately");
                    this.RaiseShutdownRequested(EventArgs.Empty);
                    throw new IOException("Ignition is turned off during start-up");
                }
            }
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareAbstractionBase.Stop"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.atmelClient?.Close();
            base.DoStop();
        }

        private void OnAtmelControlClientConnectionChanged(object sender, EventArgs args)
        {
            if (this.atmelClient.Connected)
            {
                Logger.Info("Connected to AtmelControl");
                this.atmelClient.RegisterObject<InfovisionInputState>(this.ReadInputState);
                this.atmelClient.RegisterObject<InfovisionSystemState>(this.ReadSystemState);
            }
            else
            {
                Logger.Info("Disconnected from AtmelControl");
            }
        }

        private void ReadSystemState(InfovisionSystemState state)
        {
            this.serialNumber = state.Serial ?? this.serialNumber;
        }

        private void ReadInputState(InfovisionInputState state)
        {
            if (state.Ignition == null)
            {
                return;
            }

            Logger.Info("Ignition changed to: {0}", state.Ignition.Value);
            this.UpdateIgnition(state.Ignition.Value == 1);
            this.startWait.Set();
        }

        private void UpdateIgnition(bool value)
        {
            if (this.ignition == value)
            {
                return;
            }

            this.ignition = value;
            if (value)
            {
                this.holdTimer.Enabled = false;
            }
            else if (this.Config.IgnitionHoldTime <= TimeSpan.Zero)
            {
                Logger.Info("Shutting down immediately");
                this.RaiseShutdownRequested(EventArgs.Empty);
            }
            else
            {
                Logger.Info("Shutting down in {0}", this.Config.IgnitionHoldTime);
                this.holdTimer.Interval = this.Config.IgnitionHoldTime;
                this.holdTimer.Enabled = true;
            }
        }
    }
}