// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Options type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    using CommandLineParser.Arguments;

    /// <summary>
    /// The command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Default delay (in ms) between produced objects.
        /// </summary>
        internal const long DefaultDelayMs = 50;

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
            this.DelayMs = DefaultDelayMs;
            this.Type = ProducerConsumerQueueType.Default;
        }

        /// <summary>
        /// Gets or sets the delay in ms between produced objects.
        /// </summary>
        /// <value>
        /// The delay in ms between produced objects.
        /// </value>
        [ValueArgument(typeof(long), 'd', "delay")]
        public long DelayMs { get; set; }

        /// <summary>
        /// Gets or sets the type of the queue.
        /// </summary>
        /// <value>
        /// The type of the queue.
        /// </value>
        [ValueArgument(typeof(ProducerConsumerQueueType), 't', AllowMultiple = false)]
        public ProducerConsumerQueueType Type { get; set; }
    }
}
