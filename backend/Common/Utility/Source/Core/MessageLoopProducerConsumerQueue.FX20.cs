// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageLoopProducerConsumerQueue.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageLoopProducerConsumerQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Producer-consumer queue that uses a windows message loop to consume the items added to the queue.
    /// The consumption is therefore guaranteed to run inside a windows message loop,
    /// which is for example needed for COM.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the objects to be stored in this queue.
    /// </typeparam>
    public partial class MessageLoopProducerConsumerQueue<T> : IProducerConsumerQueue<T>
        where T : class
    {
        private readonly Action<T> consumer;

        private readonly ManualResetEvent waitConsumerReady = new ManualResetEvent(false);

        private Thread thread;

        private MessageHandler messageHandler;

        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageLoopProducerConsumerQueue{T}"/> class.
        /// </summary>
        /// <param name="consumer">
        /// The consumer.
        /// </param>
        public MessageLoopProducerConsumerQueue(Action<T> consumer)
        {
            this.consumer = consumer;
            this.Capacity = int.MaxValue;
        }

        /// <summary>
        /// Gets the capacity of this queue. If it is reached, <see cref="IProducerConsumerQueue{T}.Enqueue"/>
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
                return this.count;
            }
        }

        /// <summary>
        /// Starts a consumer thread that will consume objects
        /// enqueued using <see cref="ProducerConsumerQueue{T}.Enqueue"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If <see cref="ProducerConsumerQueue{T}.StartConsumer"/> was already called before without
        /// calling <see cref="ProducerConsumerQueue{T}.StopConsumer"/> afterwards.
        /// </exception>
        public void StartConsumer()
        {
            if (this.thread != null)
            {
                return;
            }

            this.thread = new Thread(this.Run) { IsBackground = true };
            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.Start();

            this.waitConsumerReady.WaitOne();
        }

        /// <summary>
        /// Stops the consumer thread that was started by <see cref="ProducerConsumerQueue{T}.StartConsumer"/>.
        /// </summary>
        public void StopConsumer()
        {
            if (this.thread == null)
            {
                return;
            }

            this.waitConsumerReady.Reset();
            this.messageHandler.Invoke(new ThreadStart(Application.Exit));
            this.messageHandler = null;
            this.thread = null;
        }

        /// <summary>
        /// Enqueues an object inside the queue.
        /// </summary>
        /// <param name="obj">
        /// The object to be enqueued.
        /// </param>
        /// <returns>
        /// Whether the object was successfully enqueued.
        /// If the queue is full (i.e. <see cref="ProducerConsumerQueue{T}.Capacity"/>
        /// is reached), this method returns false and discards
        /// the given object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the object to be enqueued is null.
        /// </exception>
        public bool Enqueue(T obj)
        {
            Interlocked.Increment(ref this.count);
            this.messageHandler.BeginInvoke(
                new ThreadStart(
                    () =>
                        {
                            Interlocked.Decrement(ref this.count);
                            this.consumer(obj);
                        }));
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.StopConsumer();
        }

        private void Run()
        {
            using (this.messageHandler = new MessageHandler())
            {
                this.waitConsumerReady.Set();
                Application.Run();
            }
        }

        private sealed class MessageHandler : Form
        {
            public MessageHandler()
            {
                this.CreateHandle();
            }
        }
    }
}
