// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareWatchdogControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareWatchdogControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Watchdog
{
    /// <summary>
    /// The watchdog controller base class.
    /// </summary>
    public abstract class HardwareWatchdogControllerBase
    {
        private bool running;

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
        /// Actual implementation of <see cref="Start"/>.
        /// </summary>
        protected abstract void DoStart();

        /// <summary>
        /// Actual implementation of <see cref="Stop"/>.
        /// </summary>
        protected abstract void DoStop();
    }
}
