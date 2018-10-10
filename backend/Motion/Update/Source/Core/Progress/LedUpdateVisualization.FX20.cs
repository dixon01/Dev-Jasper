// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedUpdateVisualization.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LedUpdateVisualization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Progress
{
    using System;

    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Mgi.IO;

    using NLog;

    /// <summary>
    /// Update visualization that drives the "Update LED" on the PC-2.
    /// </summary>
    public partial class LedUpdateVisualization : IUpdateVisualization
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan defaultBlinkInterval = TimeSpan.FromSeconds(0.4);
        private readonly TimeSpan errorBlinkInterval = TimeSpan.FromSeconds(0.1);

        private readonly IInputOutputManager manager;

        private readonly ITimer initialOffTimer;

        private readonly ITimer blinkTimer;

        private LedState ledState = LedState.Undefined;

        private bool ledOn;

        private bool hasErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedUpdateVisualization"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public LedUpdateVisualization(LedVisualizationConfig config)
        {
            this.manager = InputOutputManagerFactory.Create();
            if (this.manager.UpdateLed == null)
            {
                throw new NotSupportedException("The current hardware doesn't have an Update LED");
            }

            if (config.DefaultFrequency > 0)
            {
                this.defaultBlinkInterval = TimeSpan.FromSeconds(0.5 / config.DefaultFrequency);
            }

            if (config.ErrorFrequency > 0)
            {
                this.errorBlinkInterval = TimeSpan.FromSeconds(0.5 / config.ErrorFrequency);
            }

            // this timer is only used to turn of the update LED in case it was accidentally on when starting up;
            // it will only be turned off after 20 seconds if there is no progress started within that time frame
            this.initialOffTimer = TimerFactory.Current.CreateTimer("UpdateLedOff");
            this.initialOffTimer.AutoReset = false;
            this.initialOffTimer.Interval = TimeSpan.FromSeconds(20);
            this.initialOffTimer.Elapsed += (sender, args) => this.SetLedState(LedState.Off);

            this.blinkTimer = TimerFactory.Current.CreateTimer("UpdateLedBlink");
            this.blinkTimer.AutoReset = true;
            this.blinkTimer.Interval = this.defaultBlinkInterval;
            this.blinkTimer.Elapsed += this.BlinkTimerOnElapsed;

            this.initialOffTimer.Enabled = true;
        }

        private enum LedState
        {
            Undefined,
            Off,
            On,
            Blink,
            Error
        }

        private bool LedOn
        {
            get
            {
                return this.ledOn;
            }

            set
            {
                if (this.ledOn == value)
                {
                    return;
                }

                Logger.Trace("Switching LED {0}", value ? "on" : "off");
                this.ledOn = value;
                this.manager.UpdateLed.Write(this.ledOn);
            }
        }

        /// <summary>
        /// Shows this visualization.
        /// </summary>
        public void Show()
        {
            this.initialOffTimer.Enabled = false;
            this.hasErrors = false;
            this.SetLedState(LedState.On);
        }

        /// <summary>
        /// Updates the progress shown with the visualization.
        /// </summary>
        /// <param name="progress">
        /// The progress in percentage (0.0...0.1).
        /// </param>
        /// <param name="stage">
        /// The update stage.
        /// </param>
        /// <param name="note">
        /// The details about the current state of the progress.
        /// </param>
        public void UpdateProgress(double progress, UpdateStage stage, string note)
        {
            switch (stage)
            {
                case UpdateStage.ReceivingUpdate:
                case UpdateStage.SendingFeedback:
                case UpdateStage.ForwardingUpdate:
                case UpdateStage.ForwardingFeedback:
                    this.SetLedState(LedState.Blink);
                    break;
                case UpdateStage.Installing:
                    this.SetLedState(LedState.On);
                    break;
            }
        }

        /// <summary>
        /// Adds an error message to this visualization.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message, never null.
        /// </param>
        public void AddErrorMessage(string errorMessage)
        {
            this.hasErrors = true;
        }

        /// <summary>
        /// Tells the visualization that the current progress has finished.
        /// </summary>
        public void Finished()
        {
            this.SetLedState(this.hasErrors ? LedState.Error : LedState.On);
        }

        /// <summary>
        /// Hides this visualization.
        /// </summary>
        public void Hide()
        {
            this.SetLedState(LedState.Off);
        }

        /// <summary>
        /// Adds a success message to this visualization.
        /// </summary>
        /// <param name="successMessage">
        /// The success message, never null.
        /// </param>
        public void AddSuccessMessage(string successMessage)
        {
        }

        private void SetLedState(LedState state)
        {
            if (this.ledState == state)
            {
                return;
            }

            Logger.Debug("Changing LED state to {0}", state);
            this.ledState = state;
            this.blinkTimer.Enabled = state == LedState.Blink || state == LedState.Error;
            this.blinkTimer.Interval = state == LedState.Error ? this.errorBlinkInterval : this.defaultBlinkInterval;
            this.LedOn = state == LedState.On;
        }

        private void BlinkTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.LedOn = !this.LedOn;
        }
    }
}