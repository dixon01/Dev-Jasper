// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProducerConsumerQueue{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProducerConsumerQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    using NLog;

    /// <summary>
    /// Simple generic producer-consumer queue that has its own consumer
    /// thread which can be started and stopped using
    /// <see cref="StartConsumer"/> and <see cref="StopConsumer"/>.
    /// When an object has been consumed, a callback is notified.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the objects to be stored in this queue.
    /// </typeparam>
    public class ProducerConsumerQueue<T> : IProducerConsumerQueue<T>
        where T : class
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ProducerConsumerQueue<T>>();

        private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        private readonly Queue<T> queue;

        private readonly Action<T> consumer;

        private bool running;

        private Thread consumerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerConsumerQueue{T}"/> class.
        /// </summary>
        /// <param name="consumerCallback">
        /// The callback that is called on the consumer thread
        /// when an object was enqueued in this queue.
        /// </param>
        /// <param name="capacity">
        /// The capacity of the queue. If it is reached, <see cref="Enqueue"/>
        /// returns false.
        /// </param>
        public ProducerConsumerQueue(Action<T> consumerCallback, int capacity)
        {
            this.consumer = consumerCallback;
            this.queue = new Queue<T>(capacity);
            this.Capacity = capacity;
        }

        /// <summary>
        /// Gets the capacity of this queue. If it is reached, <see cref="Enqueue"/>
        /// returns false.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        public int Count
        {
            get
            {
                return this.queue.Count;
            }
        }

        /// <summary>
        /// Starts a consumer thread that will consume objects
        /// enqueued using <see cref="Enqueue"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If <see cref="StartConsumer"/> was already called before without
        /// calling <see cref="StopConsumer"/> afterwards.
        /// </exception>
        public void StartConsumer()
        {
            if (this.running)
            {
                throw new NotSupportedException("Can't start consumer thread twice!");
            }

            Logger.Info("Consumer started");
            this.running = true;
            if (this.queue.Count == 0)
            {
                this.waitHandle.Reset();
            }

            this.consumerThread = this.CreateThread();
            this.consumerThread.Start();
        }

        /// <summary>
        /// Stops the consumer thread that was started by <see cref="StartConsumer"/>.
        /// </summary>
        public void StopConsumer()
        {
            if (!this.running)
            {
                return;
            }

            var thread = this.consumerThread;
            this.consumerThread = null;
            this.running = false;
            Logger.Info("Consumer stopped");
            this.waitHandle.Set();

            if (thread != null && thread != Thread.CurrentThread)
            {
                try
                {
                    thread.Join(100);
                }
                catch (ThreadStateException)
                {
                }
            }
        }

        /// <summary>
        /// Enqueues an object inside the queue.
        /// </summary>
        /// <param name="obj">
        /// The object to be enqueued.
        /// </param>
        /// <returns>
        /// Whether the object was successfully enqueued.
        /// If the queue is full (i.e. <see cref="Capacity"/>
        /// is reached), this method returns false and discards
        /// the given object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the object to be enqueued is null.
        /// </exception>
        public bool Enqueue(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            lock (((ICollection)this.queue).SyncRoot)
            {
                if (this.queue.Count > this.Capacity)
                {
                    // at the moment, the queue is full
                    Logger.Warn("Queue full. Discarding object {0}", obj);
                    return false;
                }

                // ok, it seems that I can really
                // enqueue the object.
                Logger.Trace("Enqueuing object {0}", obj);
                this.queue.Enqueue(obj);
            }

            // now it's the time to set the event.
            // the consumer will do its part.
            this.waitHandle.Set();

            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Creates the thread on which the queue will consume objects.
        /// </summary>
        /// <returns>
        /// The <see cref="Thread"/>.
        /// The default implementation creates a named background thread (MTA).
        /// </returns>
        protected virtual Thread CreateThread()
        {
            return new Thread(this.Consume)
                       {
                           Name = "ProducerConsumerQueue<" + typeof(T).Name + ">",
                           IsBackground = true
                       };
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.StopConsumer();
            }
        }

        /// <summary>
        /// Consumes the data stored into the queue,
        /// until a termination request is called.
        /// </summary>
        private void Consume()
        {
            while (this.running)
            {
                T obj = null;

                // consume the data inside the queue.
                lock (((ICollection)this.queue).SyncRoot)
                {
                    if (this.queue.Count > 0)
                    {
                        obj = this.queue.Dequeue();
                    }
                }

                if (obj == null && this.running)
                {
                    // nothing to do. I wait for
                    // the signal that somthing was enqueued.
                    this.waitHandle.WaitOne();
                }
                else
                {
                    Logger.Trace("Consuming object {0}", obj);
                    this.consumer(obj);
                }
            }

            // before terminating,
            // I flush the queue.
            lock (((ICollection)this.queue).SyncRoot)
            {
                this.queue.Clear();
            }
        }
    }
}