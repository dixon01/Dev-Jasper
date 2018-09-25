// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    using System.Threading;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Simple implementation of <see cref="IFrameController"/>.
    /// </summary>
    internal class FrameController : IFrameController
    {
        private static readonly Logger Logger = LogHelper.GetLogger<FrameController>();

        private int lastSentId;

        private uint lastReceivedId;

        /// <summary>
        /// Gets the last acknowledged frame id.
        /// </summary>
        public uint LastAcknowledgedFrameId { get; private set; }

        /// <summary>
        /// Verifies an incoming frame.
        /// </summary>
        /// <param name="info">
        /// The frame info.
        /// </param>
        /// <returns>
        /// The <see cref="FrameCheck"/> result.
        /// </returns>
        public FrameCheck VerifyIncoming(FrameInfo info)
        {
            if (info.AckFrameId > this.LastAcknowledgedFrameId)
            {
                this.LastAcknowledgedFrameId = info.AckFrameId;
                Logger.Trace("Acknowledged frame {0}", info.AckFrameId);
            }

            if (info.SendFrameId == this.lastReceivedId + 1)
            {
                Logger.Trace("Received frame {0}", info.SendFrameId);
                this.lastReceivedId++;
                return FrameCheck.Ok;
            }

            if (info.SendFrameId <= this.lastReceivedId)
            {
                Logger.Debug("Received frame {0} twice", info.SendFrameId);
                return FrameCheck.DuplicateFrame;
            }

            Logger.Warn("Missing frame: previous: {0} current: {1}", this.lastReceivedId, info.SendFrameId);
            return FrameCheck.MissingFrame;
        }

        /// <summary>
        /// Gets the information for the next frame.
        /// </summary>
        /// <returns>
        /// A new <see cref="FrameInfo"/>.
        /// </returns>
        public FrameInfo GetNextFrameInfo()
        {
            var sendId = Interlocked.Increment(ref this.lastSentId);
            return new FrameInfo((uint)sendId, this.lastReceivedId);
        }
    }
}