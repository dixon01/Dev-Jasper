// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressBoxControl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProgressBoxControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.C74.Controls
{
    using System;

    /// <summary>
    /// The progress box control.
    /// </summary>
    public partial class ProgressBoxControl : PopupControl
    {
        private const int SpeedFactor = 10;

        private int ticksCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBoxControl"/> class.
        /// </summary>
        public ProgressBoxControl()
        {
            this.InitializeComponent();

            this.timer.Interval = 1000 / SpeedFactor;
        }

        /// <summary>
        /// Event that is fired when the progress box is stopped (<see cref="Stop"/>) or
        /// the maximum time has expired.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Starts the progress bar.
        /// </summary>
        /// <param name="maxTime">
        /// The maximum wait time until the progress box is closed.
        /// </param>
        public void Start(TimeSpan maxTime)
        {
            this.Stop();

            this.ticksCounter = 0;
            this.progressBar.Value = 0;
            this.progressBar.Maximum = (int)(maxTime.TotalSeconds * SpeedFactor);
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Stops the progress bar and closes this progress box (raises the <see cref="Stopped"/> event).
        /// </summary>
        public void Stop()
        {
            if (!this.timer.Enabled)
            {
                return;
            }

            this.timer.Enabled = false;
            this.RaiseStopped();
        }

        /// <summary>
        /// Raises the <see cref="Stopped"/> event.
        /// </summary>
        protected virtual void RaiseStopped()
        {
            var handler = this.Stopped;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (++this.ticksCounter >= this.progressBar.Maximum)
            {
                this.Stop();
            }
            else
            {
                this.progressBar.Value = this.ticksCounter;
            }
        }
    }
}
