// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendMessageQueue.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SendMessageQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// A queue for <see cref="SendMessageBuffer"/>s that allows to re-enqueue
    /// non-acknowledged buffers when reconnecting a session.
    /// </summary>
    internal class SendMessageQueue
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SendMessageQueue>();

        private readonly LinkedList<SendMessageBuffer> sending = new LinkedList<SendMessageBuffer>();
        private readonly LinkedList<SendMessageBuffer> sent = new LinkedList<SendMessageBuffer>();

        /// <summary>
        /// Gets or sets a value indicating whether framing enabled.
        /// If framing is disabled, this class just acts as a normal queue.
        /// </summary>
        public bool FramingEnabled { get; set; }

        /// <summary>
        /// Gets the number of unsent buffers in this queue.
        /// This property doesn't count the non-acknowledged buffers still
        /// kept in the "sent" queue.
        /// </summary>
        public int Count
        {
            get
            {
                return this.sending.Count;
            }
        }

        /// <summary>
        /// Enqueues a new buffer for sending.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public void Enqueue(SendMessageBuffer buffer)
        {
            this.sending.AddFirst(buffer);
            Logger.Trace(
                "Enqueued buffer with {0} bytes, now {1} buffers in sending, {2} buffers in sent",
                buffer.Count,
                this.sending.Count,
                this.sent.Count);
        }

        /// <summary>
        /// De-queues the next buffer to be sent.
        /// </summary>
        /// <returns>
        /// The oldest <see cref="SendMessageBuffer"/> enqueued in this queue.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If this queue is empty.
        /// </exception>
        public SendMessageBuffer Dequeue()
        {
            var node = this.sending.Last;
            this.sending.RemoveLast();
            if (this.FramingEnabled)
            {
                this.sent.AddFirst(node);
            }

            Logger.Trace(
                "Dequeued buffer with {0} bytes, now {1} buffers in sending, {2} buffers in sent",
                node.Value.Count,
                this.sending.Count,
                this.sent.Count);

            return node.Value;
        }

        /// <summary>
        /// Acknowledges a given frame id.
        /// This removes all acknowledged buffers with lower or equal
        /// frame ids from the "sent" queue. Those messages won't be
        /// resent if the queue is <see cref="Restart"/>ed.
        /// </summary>
        /// <param name="ackFrameId">
        /// The acknowledged frame id.
        /// </param>
        public void Acknowledge(uint ackFrameId)
        {
            if (!this.FramingEnabled)
            {
                return;
            }

            if (this.sent.Count == 0)
            {
                return;
            }

            if (this.sent.First.Value.FrameId < ackFrameId)
            {
                var message = string.Format(
                    "skipping acknowledging a higher frame number: {0} < {1}",
                    this.sent.First.Value.FrameId,
                    ackFrameId);
                Logger.Warn(message);

                // This exception is causing a problem in the master MeDi
                // host application in case of out of order frame acks.
                // throw new ArgumentOutOfRangeException("ackFrameId", message);
            }

            LinkedListNode<SendMessageBuffer> previous;
            for (var node = this.sent.Last; node != null && node.Value.FrameId <= ackFrameId; node = previous)
            {
                previous = node.Previous;
                this.sent.Remove(node);
            }

            Logger.Trace(
                "Acknowledged frame id {0}, now {1} buffers in sending, {2} buffers in sent",
                ackFrameId,
                this.sending.Count,
                this.sent.Count);
        }

        /// <summary>
        /// Restarts this queue by re-enqueueing all non-acknowledged buffers.
        /// </summary>
        public void Restart()
        {
            for (var node = this.sent.First; node != null; node = node.Next)
            {
                this.sending.AddLast(node.Value);
            }

            this.sent.Clear();

            Logger.Trace(
                "Restarted queue, now {0} buffers in sending, {1} buffers in sent",
                this.sending.Count,
                this.sent.Count);
        }

        /// <summary>
        /// Completely clears this queue by removing all items from both internal
        /// queues.
        /// </summary>
        public void Clear()
        {
            this.sent.Clear();
            this.sending.Clear();

            Logger.Trace(
                "Cleared queue, now {0} buffers in sending, {1} buffers in sent",
                this.sending.Count,
                this.sent.Count);
        }
    }
}