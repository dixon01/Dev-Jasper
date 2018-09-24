// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CyclicManager.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;
    using System.Timers;

    using Gorba.Common.Protocols.Isi.Messages;

    /// <summary>
    /// Object tasked to send cyclicylly ISI put messages
    /// to the remote ISI server accordingly to the received
    /// ISI get messages having the "Cyclic" feature.
    /// </summary>
    public class CyclicManager
    {
        private readonly IsiGet isiGet;

        private readonly Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicManager"/> class.
        /// </summary>
        /// <param name="isiGet">
        /// The IsiGet message.
        /// </param>
        public CyclicManager(IsiGet isiGet)
        {
            this.isiGet = isiGet;
            this.timer = new Timer(isiGet.Cyclic * 1000);
            this.timer.Elapsed += this.OnTimerElapsed;
        }

        /// <summary>
        /// Event that is fired every time an update for this message is required.
        /// </summary>
        public event EventHandler UpdateRequested;

        /// <summary>
        /// Gets the ID of this message.
        /// </summary>
        public int Id
        {
            get
            {
                return this.isiGet.IsiId;
            }
        }

        /// <summary>
        /// Starts the request manager.
        /// </summary>
        public void Start()
        {
            this.timer.Start();
        }

        /// <summary>
        /// Stops the request manager.
        /// </summary>
        public void Stop()
        {
            this.timer.Stop();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var handler = this.UpdateRequested;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
