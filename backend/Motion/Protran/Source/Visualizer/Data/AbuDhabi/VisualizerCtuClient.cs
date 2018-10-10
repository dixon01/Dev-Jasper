// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerCtuClient.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.AbuDhabi
{
    using System;
    using System.Threading;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Common.Protocols.Ctu.Responses;
    using Gorba.Motion.Protran.AbuDhabi.Ctu;

    /// <summary>
    /// Subclass of Cu5Client that fakes the connection to the CU5
    /// for the visualizer.
    /// </summary>
    internal class VisualizerCtuClient : CtuClient
    {
        /// <summary>
        /// Triggers this object in order to make starting all its
        /// internal acitvities.
        /// </summary>
        public override void Start()
        {
            if (this.IsRunning)
            {
                return;
            }

            this.DeviceInfo = new DeviceInfoResponse
                {
                    DataVersion = "DataVersion", 
                    SerialNumber = 12345678,
                    SoftwareVersion = "VisualizerCTU"
                };

            this.IsRunning = true;

            ExecuteLater(1000, () => this.RaiseCommunicationStarted(EventArgs.Empty));
        }

        /// <summary>
        /// Stops all the activities with the remote CU5 device.
        /// </summary>
        public override void Stop()
        {
            if (!this.IsRunning)
            {
                return;
            }

            this.IsRunning = false;
        }

        /// <summary>
        /// Sends a CTU datagram to the CU5. This implementation does nothing.
        /// </summary>
        /// <param name="triplets">
        /// The triplets to send.
        /// </param>
        protected override void SendDatagram(params Triplet[] triplets)
        {
        }

        private static void ExecuteLater(int delay, ThreadStart task)
        {
            var timer = new Timer(s => task());
            timer.Change(delay, Timeout.Infinite);
        }
    }
}