// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpMessageQueue.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that provides a message queue for the
//   HTTP communication Layer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;
    using System.Collections.Concurrent;

    using NLog;

    /// <summary>
    /// Class HttpMessageQueue provides a message queue for the 
    /// HTTP communication Layer.
    /// </summary>
    public class HttpMessageQueue
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// message queue
        /// </summary>
        private readonly ConcurrentQueue<string> queue;

        /// <summary>
        /// Initializes a new instance of the HttpMessageQueue class.
        /// </summary>
        public HttpMessageQueue()
        {
            new object();
            this.queue = new ConcurrentQueue<string>();
        }

        /// <summary>
        /// Gets the number of elements contained in the Queue.
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                return this.queue.Count;
            }
        }

        /// <summary>
        /// Adds a message to the end of the Queue.
        /// </summary>
        /// <param name="message">
        /// message to enqueue
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// throws ArgumentNullException if parameter message is null
        /// </exception>
        public void Enqueue(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.queue.Enqueue(message);

            Logger.Trace("Enqueue: {0} messages in queue", this.queue.Count);
        }

        /// <summary>
        /// Returns the message at the beginning of the Queue and removes the message from the queue.
        /// </summary>
        /// <returns>
        /// dequeued message
        /// </returns>
        public string Dequeue()
        {
            string value;
            if (this.queue.TryDequeue(out value))
            {
                Logger.Trace("Dequeue: {0} messages left in queue", this.queue.Count);
            }
            else
            {
                Logger.Trace("Dequeue: The queue is empty");
                value = null;
            }

            return value;
        }

        /// <summary>
        /// Returns the message at the beginning of the Queue without removing the message from the queue.
        /// </summary>
        /// <returns>
        /// dequeued message
        /// </returns>
        public string Peek()
        {
            string value;
            if (this.queue.TryPeek(out value))
            {
                Logger.Trace("Peek: {0} messages in the queue.", this.queue.Count);
            }
            else
            {
                Logger.Trace("Peek: The queue is empty");
                value = null;
            }

            return value;
        }
    }
}