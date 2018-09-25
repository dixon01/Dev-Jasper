// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProducerConsumerQueue.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProducerConsumerQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Simple generic producer-consumer queue that has its own consumer
    /// thread which can be started and stopped using
    /// <see cref="StartConsumer"/> and <see cref="StopConsumer"/>.
    /// When an object has been consumed, a callback is notified.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the objects to be stored in this queue.
    /// </typeparam>
    public interface IProducerConsumerQueue<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// Gets the capacity of this queue. If it is reached, <see cref="Enqueue"/>
        /// returns false.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Starts a consumer thread that will consume objects
        /// enqueued using <see cref="ProducerConsumerQueue{T}.Enqueue"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If <see cref="ProducerConsumerQueue{T}.StartConsumer"/> was already called before without
        /// calling <see cref="ProducerConsumerQueue{T}.StopConsumer"/> afterwards.
        /// </exception>
        void StartConsumer();

        /// <summary>
        /// Stops the consumer thread that was started by <see cref="ProducerConsumerQueue{T}.StartConsumer"/>.
        /// </summary>
        void StopConsumer();

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
        bool Enqueue(T obj);
    }
}