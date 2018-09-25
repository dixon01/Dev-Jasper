// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisMaster.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Remote
{
    using System;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// This object represents an abstraction of the
    /// IBIS master: the board computer that sends (as master)
    /// IBIS telegrams to Protran.
    /// </summary>
    public class IbisMaster : RemoteComputer
    {
        /// <summary>
        /// Timer tasked to inspect if this remote board computer
        /// has sent some bytes to Protran within a specific timeout.
        /// </summary>
        private readonly ITimer timerCheckTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisMaster"/> class.
        /// </summary>
        /// <param name="connectionTimeout">
        /// The amount of seconds after them the connection with the IBIS
        /// master has to be considered lost, in case of no data reception.
        /// </param>
        public IbisMaster(TimeSpan connectionTimeout)
        {
            this.timerCheckTime = TimerFactory.Current.CreateTimer("CheckTime");
            this.timerCheckTime.Interval = connectionTimeout;
            this.timerCheckTime.AutoReset = false;
            this.timerCheckTime.Elapsed += this.CheckTime;
            this.ResetActivityNotifier();
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this remote computer has sent data or not.
        /// </summary>
        public override bool HasSentData
        {
            get
            {
                return this.Status == RemoteComputerStatus.Active;
            }

            set
            {
                if (value)
                {
                    this.Status = RemoteComputerStatus.Active;

                    // we have received right now bytes (at least one byte)
                    // from the remote IBIS computer. This means that it is
                    // active. So, now it's the time to start the timer that
                    // will advise us if the remote IBIS computer become inactive later.
                    this.ResetActivityNotifier();
                }
            }
        }

        /// <summary>
        /// Releases all the resources allocated by this object.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.timerCheckTime.Enabled = false;
            this.timerCheckTime.Dispose();
        }

        private void CheckTime(object sender, EventArgs e)
        {
            // if we enter in this function, it means that the timeout
            // is expired. This means also that we didn't receive any byte
            // since "this.connectionTimeout" seconds.
            // so I consider inactive this remote computer.
            this.Status = RemoteComputerStatus.Inactive;
        }

        /// <summary>
        /// Starts the timer that will notify us (asynchronously)
        /// if the remote computer doesn't have sent us any byte
        /// since "this.connectionTimeout" seconds.
        /// </summary>
        private void ResetActivityNotifier()
        {
            // first I make sure that the timer is stopped.
            this.timerCheckTime.Enabled = false;

            // and now I can really start the timer.
            this.timerCheckTime.Enabled = true;
        }
    }
}
