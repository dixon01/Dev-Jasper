// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciMessageQueue.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Serialization
{
    using System;
    using System.Collections.Generic;
    using Messages;

    /// <summary>
    /// The message queue.
    /// </summary>
    public class EciMessageQueue
    {
        /// <summary>
        /// The messages.
        /// </summary>
        private readonly Queue<EciMessageBase> messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="EciMessageQueue"/> class.
        /// </summary>
        /// <param name="size">
        /// The queue size.
        /// </param>
        public EciMessageQueue(int size)
        {
            this.messages = new Queue<EciMessageBase>(size);
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public Queue<EciMessageBase> Messages
        {
            get
            {
                return this.messages;
            }
        }

        /// <summary>
        /// Processes a byte array and extract ECI packets..
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// True if the packet is complete and valid <see cref="bool"/>.
        /// </returns>
        public bool ProcessData(byte[] buffer)
        {
            // this needs to be rewritten to handle buffers with partial or multiple packets
            int s1 = Array.IndexOf(buffer, (byte)'<');
            int s2 = Array.IndexOf(buffer, (byte)'>');
            if (s1 >= 0 && s2 < buffer.Length && s1 < s2)
            {
                var packet = new EciBinaryPacket(buffer, s1, s2);
                if (!packet.ValidateCheckSum())
                {
                    return false;
                }

                EciMessageBase msg = EciSerializer.Deserialize(packet);
                if (msg == null)
                {
                    return false;
                }

                this.messages.Enqueue(msg);
                return true;
            }

            return false;
        }
    }
}