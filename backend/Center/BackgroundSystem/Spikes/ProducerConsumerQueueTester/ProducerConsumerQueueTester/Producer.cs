// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Producer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Producer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Producer in the producer/consumer model.
    /// </summary>
    public class Producer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Options options;

        private readonly string start;

        private readonly IProducerConsumerQueue<TestToken> producerConsumerQueue;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="start">The start date and time.</param>
        /// <param name="queue">The queue.</param>
        public Producer(Options options, string start, IProducerConsumerQueue<TestToken> queue)
        {
            this.options = options;
            this.start = start;
            this.producerConsumerQueue = queue;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            var task = new Task(
                this.InternalStart, this.cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            task.ContinueWith(this.OnTaskCompleted);
            task.Start();
            Logger.Info(this.options.Type, this.start, "Producer started");
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
        }

        private void InternalStart()
        {
            while (!this.cancellationTokenSource.Token.IsCancellationRequested)
            {
                var testToken = new TestToken { Time = DateTime.UtcNow.Ticks };
                Logger.Trace(this.options.Type, this.start, "Produced token {0}", testToken);
                this.producerConsumerQueue.Enqueue(testToken);
                Thread.Sleep(TimeSpan.FromMilliseconds(50));
            }
        }

        private void OnTaskCompleted(Task previousTask)
        {
            if (previousTask.Exception == null)
            {
                Logger.Info(this.options.Type, this.start, "Producer completed");
                return;
            }

            Logger.WarnException("Producer completed with errors", previousTask.Exception.Flatten());
        }
    }
}