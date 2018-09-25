// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProducerConsumerQueueType.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProducerConsumerQueueType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    /// <summary>
    /// Defines the implemented types of queues.
    /// </summary>
    public enum ProducerConsumerQueueType
    {
        /// <summary>
        /// The default implementation.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Implementation with RX.
        /// </summary>
        Reactive = 1
    }
}
