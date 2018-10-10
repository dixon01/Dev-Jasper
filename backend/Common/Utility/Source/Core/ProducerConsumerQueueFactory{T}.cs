// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProducerConsumerQueueFactory{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProducerConsumerQueueFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    using NLog;

    /// <summary>
    /// Factory for <see cref="IProducerConsumerQueue&lt;T&gt;"/> objects.
    /// </summary>
    /// <typeparam name="T">Type of the items in the queue.</typeparam>
    public abstract class ProducerConsumerQueueFactory<T>
        where T : class
    {
        // ReSharper disable StaticFieldInGenericType
        private static readonly Logger Logger = LogHelper.GetLogger<ProducerConsumerQueueFactory<T>>();

        // ReSharper restore StaticFieldInGenericType
        static ProducerConsumerQueueFactory()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current producer-consumer queue factory.
        /// The default value creates a <see cref="NamedProducerConsumerQueue&lt;T&gt;"/>.
        /// </summary>
        public static ProducerConsumerQueueFactory<T> Current { get; private set; }

        /// <summary>
        /// Resets the <see cref="Current"/> factory to the default value.
        /// </summary>
        public static void Reset()
        {
            Current = new DefaultProducerConsumerQueueFactory();
        }

        /// <summary>
        /// Sets the specified factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public static void Set(ProducerConsumerQueueFactory<T> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory", "The factory object can't be null");
            }

            Current = factory;
        }

        /// <summary>
        /// Creates a new named producer-consumer queue.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="consumerCallback">The consumer callback.</param>
        /// <param name="capacity">The capacity.</param>
        /// <returns>
        /// The <see cref="IProducerConsumerQueue&lt;T&gt;"/> implementation.
        /// </returns>
        public abstract IProducerConsumerQueue<T> Create(string name, Action<T> consumerCallback, int capacity);

        /// <summary>
        /// <see cref="ProducerConsumerQueueFactory&lt;T&gt;"/> useful for testing.
        /// It executes operations immediately when enqueued.
        /// </summary>
        public class ImmediateProducerConsumerQueueFactory : ProducerConsumerQueueFactory<T>
        {
            /// <summary>
            /// Creates the specified name.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="consumerCallback">The consumer callback.</param>
            /// <param name="capacity">The capacity.</param>
            /// <returns>An immediate producer-consumer queue.</returns>
            public override IProducerConsumerQueue<T> Create(string name, Action<T> consumerCallback, int capacity)
            {
                return new ImmediateProducerConsumerQueue(name, consumerCallback, capacity);
            }

            private sealed class ImmediateProducerConsumerQueue : IProducerConsumerQueue<T>
            {
                private readonly Action<T> consumerCallback;

                public ImmediateProducerConsumerQueue(string name, Action<T> consumerCallback, int capacity)
                {
                    this.consumerCallback = consumerCallback;
                }

                /// <summary>
                /// Gets the capacity of this queue. If it is reached, <see cref="Enqueue"/>
                /// returns false.
                /// </summary>
                public int Capacity { get; private set; }

                /// <summary>
                /// Gets the number of items in the queue.
                /// </summary>
                public int Count { get; private set; }

                /// <summary>
                /// Starts the consumer.
                /// </summary>
                public void StartConsumer()
                {
                }

                /// <summary>
                /// Stops the consumer.
                /// </summary>
                public void StopConsumer()
                {
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
                    this.consumerCallback(obj);
                    return true;
                }

                /// <summary>
                /// Performs application-defined tasks associated with
                /// freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    // Nothing to dispose
                }
            }
        }

        private class DefaultProducerConsumerQueueFactory : ProducerConsumerQueueFactory<T>
        {
            public override IProducerConsumerQueue<T> Create(string name, Action<T> consumerCallback, int capacity)
            {
                Logger.Info("Creating named producer consumer queue '{0}' for type {1}", name, typeof(T).FullName);
                return new NamedProducerConsumerQueue<T>(name, consumerCallback, capacity);
            }
        }
    }
}