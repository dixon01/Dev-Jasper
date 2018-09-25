// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Consumer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Consumer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The consumer in the producer/consumer model.
    /// </summary>
    public class Consumer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Options options;

        private readonly string start;

        private IProducerConsumerQueue<TestToken> producerConsumerQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="start">The start date and time.</param>
        public Consumer(Options options, string start)
        {
            this.options = options;
            this.start = start;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>The producer/consumer queue created.</returns>
        public IProducerConsumerQueue<TestToken> Start()
        {
            this.producerConsumerQueue = ProducerConsumerQueueFactory<TestToken>.Current.Create(
                "Consumer", this.Consume, 1000);
            this.producerConsumerQueue.StartConsumer();
            Logger.Info(this.options.Type, this.start, "Consumer started");
            return this.producerConsumerQueue;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            Logger.Info(this.options.Type, this.start, "Stopping consumer");
            this.producerConsumerQueue.StopConsumer();
            Logger.Info(this.options.Type, this.start, "Consumer stopped");
        }

        private void Consume(TestToken testToken)
        {
            Logger.Debug(this.options.Type, this.start, "Consuming {0}", testToken);
        }
    }
}