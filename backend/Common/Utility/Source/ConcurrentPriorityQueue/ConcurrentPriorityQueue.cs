// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentPriorityQueue.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumeration of allowed priority for comms messages contained into a priority queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.ConcurrentPriorityQueue
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Enumeration of allowed priority for comms messages contained into a priority queue.
    /// </summary>
    [DataContract]
    public enum QueuePriority 
    {
        /// <summary>
        /// CommS mesasges are sent with the highest priority. 
        /// </summary>
        [EnumMember]
        High = 0,

        /// <summary>
        /// CommS messages are sent just after the highest priority message.
        /// </summary>
        [EnumMember]
        Normal = 1,

        /// <summary>
        /// CommS messages are sent with the lowest priority
        /// </summary>
        [EnumMember]
        Low = 2
    }

    /// <summary>
    /// Thread-safe priority queue. See <see cref="QueuePriority"/> for list of priorities.
    /// </summary>
    /// <typeparam name="T">T is a class type that will be handle by the concurrent priority queue.</typeparam>
    public class ConcurrentPriorityQueue<T> where T : class
    {
        /// <summary>
        /// Internal list queue to handle the 3 priority queues for each <see cref="QueuePriority"/>
        /// </summary>
        private readonly SortedList<QueuePriority, ConcurrentQueue<T>> priorityQueues = new SortedList<QueuePriority, ConcurrentQueue<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentPriorityQueue{T}"/> class. 
        /// <remarks>
        /// Instanciates the three internals ConcurrentQueue for each priority
        /// </remarks>
        /// </summary>
        public ConcurrentPriorityQueue()
        {
            foreach (QueuePriority priority in Enum.GetValues(typeof(QueuePriority)))
            {
                this.priorityQueues.Add(priority, new ConcurrentQueue<T>());
            }
        }

        /// <summary>
        /// Gets the total number of all items containing into internal queues.
        /// </summary>
        public int Count
        {
            get
            {
                return this.priorityQueues.Sum(keyValuePair => keyValuePair.Value.Count);
            }
        }

        /// <summary>
        /// Enqueues the Item in the priority queue.
        /// </summary>
        /// <param name="priority">Priority of the item.</param>
        /// <param name="item">Item of type T to be enqueued.</param>
        public void Push(QueuePriority priority, T item)
        {
            this.priorityQueues[priority].Enqueue(item);
        }

        /// <summary>
        /// Push the item with Normal priority
        /// </summary>
        /// <param name="item">Item of type T to be enqueued.</param>
        public void Push(T item)
        {
            this.priorityQueues[QueuePriority.Normal].Enqueue(item);
        }

        /// <summary>
        /// Try to dequeue the item of the queue with the highest priority. If all queues are empty, returns null.
        /// </summary>
        /// <returns>Returns and dequeues the T item with the highest priority.</returns>
        public T Pop()
        {
            foreach (QueuePriority priority in Enum.GetValues(typeof(QueuePriority)))
            {
                T obj;
                if (this.priorityQueues[priority].TryDequeue(out obj))
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Dequeues the first item of type T from the queue according to the given priority. If the queue is empty, 
        /// it doesn't continue into the upper priority queue but returns null.
        /// </summary>
        /// <param name="priority">Priority of the required returned item.</param>
        /// <returns>Returns the first T item from 'priority' queue</returns>
        public T PopPriority(QueuePriority priority)
        {
            T obj;
            return this.priorityQueues[priority].TryDequeue(out obj) ? obj : null;
        }

        /// <summary>
        /// Dequeues all items from the queue of given priority. 
        /// </summary>
        /// <param name="priority">Priority of the required returned list.</param>
        /// <returns>Returns an enumertaion containing all of items of T type from the 'priority' queue.</returns>
        public IEnumerable<T> PopAllPriority(QueuePriority priority)
        {
            var ret = new List<T>();
            T obj;
            do
            {
                obj = this.PopPriority(priority);
                if (obj != null)
                {
                    ret.Add(obj);
                }
            }
            while (obj != null);
            return ret;
        }

        /// <summary>
        /// Enables to look at the item at the head of highest the queue without removing it. 
        /// </summary>
        /// <returns>Returns the first T item with the highest priority without dequeue it.</returns>
        public T Peek()
        {
            foreach (QueuePriority priority in Enum.GetValues(typeof(QueuePriority)))
            {
                T obj;
                if (this.priorityQueues[priority].TryPeek(out obj))
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all items as IEnumerable list containing into all priority queues.
        /// </summary>
        /// <returns>IEnumerable list of T items.</returns>
        public IEnumerable<T> PeekAll()
        {
            var ret = new List<T>();
            foreach (var keyValuePair in this.priorityQueues)
            {
                ret.AddRange(keyValuePair.Value.AsEnumerable());
            }

            return ret;
        }

        /// <summary>
        /// Gest and dequeues all T items as IEnumerable list containing into all priority queues.
        /// </summary>
        /// <returns>IEnumerable list of T items.</returns>
        public IEnumerable<T> PopAll()
        {
            var ret = new List<T>();
            foreach (QueuePriority priority in Enum.GetValues(typeof(QueuePriority)))
            {
                var collection = this.PopAllPriority(priority);
                if (collection != null)
                {
                    ret.AddRange(collection);
                }
            }

            return ret;
        }
    }
}
