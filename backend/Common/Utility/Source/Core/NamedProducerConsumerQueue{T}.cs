// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedProducerConsumerQueue{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NamedProducerConsumerQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Defines a <see cref="ProducerConsumerQueue&lt;T&gt;"/> identified by a name.
    /// </summary>
    /// <typeparam name="T">The type of the items queued.</typeparam>
    public class NamedProducerConsumerQueue<T> : ProducerConsumerQueue<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedProducerConsumerQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="consumerCallback">The consumer callback.</param>
        /// <param name="capacity">The capacity.</param>
        public NamedProducerConsumerQueue(string name, Action<T> consumerCallback, int capacity)
            : base(consumerCallback, capacity)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        public string Name { get; private set; }
    }
}