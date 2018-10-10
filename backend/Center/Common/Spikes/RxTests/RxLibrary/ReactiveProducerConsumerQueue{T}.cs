// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactiveProducerConsumerQueue{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReactiveProducerConsumerQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RxLibrary
{
    using System;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Defines a producer-consumer queue implemented with the Rx framework.
    /// </summary>
    /// <typeparam name="T">The type of the items in the queue.</typeparam>
    public class ReactiveProducerConsumerQueue<T> : IProducerConsumerQueue<T>
        where T : class
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Subject<T> messages = new Subject<T>();

        private readonly object locker = new object();

        private readonly IScheduler consumerScheduler;

        private readonly IScheduler producerScheduler;

        private readonly Action<T> consumerCallback;

        private bool isStarted;

        private IDisposable subscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveProducerConsumerQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="consumerCallback">The consumer callback.</param>
        /// <param name="producerScheduler">The producer scheduler.</param>
        /// <param name="consumerScheduler">The consumer scheduler.</param>
        public ReactiveProducerConsumerQueue(Action<T> consumerCallback, IScheduler producerScheduler, IScheduler consumerScheduler)
        {
            this.producerScheduler = producerScheduler;
            this.consumerScheduler = consumerScheduler;
            this.consumerCallback = consumerCallback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveProducerConsumerQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="consumerCallback">The consumer callback.</param>
        /// <param name="producerScheduler">The producer scheduler.</param>
        public ReactiveProducerConsumerQueue(Action<T> consumerCallback, IScheduler producerScheduler)
            : this(consumerCallback, producerScheduler, new EventLoopScheduler())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveProducerConsumerQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="consumerCallback">The consumer callback.</param>
        public ReactiveProducerConsumerQueue(Action<T> consumerCallback)
            : this(consumerCallback, new EventLoopScheduler())
        {
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Starts the consumer.
        /// </summary>
        public void StartConsumer()
        {
            Logger.Info("Request to start the queue.");
            if (this.isStarted)
            {
                Logger.Trace("The queue was already started. This start request will be ignored.");
                return;
            }

            lock (this.locker)
            {
                if (this.isStarted)
                {
                    Logger.Trace("The queue was already started. This start request will be ignored.");
                    return;
                }

                this.isStarted = true;
            }

            this.subscription =
                this.messages.ObserveOn(this.producerScheduler)
                    .Timestamp()
                    .SubscribeOn(this.consumerScheduler)
                    .Subscribe(this.Dequeue);
        }

        /// <summary>
        /// Stops the consumer.
        /// </summary>
        public void StopConsumer()
        {
            Logger.Info("Request to stop the queue manager.");
            if (!this.isStarted)
            {
                Logger.Trace("The queue manager was already stopped. This request will be ignored.");
                return;
            }

            lock (this.locker)
            {
                if (!this.isStarted)
                {
                    Logger.Trace("The queue manager was already stopped. This request will be ignored.");
                    return;
                }

                this.isStarted = false;
            }

            this.TrySubscriptionDisposal();
        }

        /// <summary>
        /// Enqueues the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns><c>true</c> if the item was enqueued; otherwise, <c>false</c>.</returns>
        public bool Enqueue(T obj)
        {
            Logger.Trace("Enqueuing item {0}", obj);
            this.messages.OnNext(obj);
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dequeue(Timestamped<T> item)
        {
            this.consumerCallback(item.Value);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.TrySubscriptionDisposal();
            }
        }

        private void TrySubscriptionDisposal()
        {
            if (this.subscription != null)
            {
                this.subscription.Dispose();
                this.subscription = null;
            }
        }
    }
}
