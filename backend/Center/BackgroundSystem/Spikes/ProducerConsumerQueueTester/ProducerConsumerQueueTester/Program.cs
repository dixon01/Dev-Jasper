// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    using System;

    using Gorba.Center.Common.Core.Queues;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args)
        {
            var options = new Options();
            var start = DateTime.Now.ToString("yyyyMMdd_hhmmss");
            var commandLineParser = new CommandLineParser.CommandLineParser();
            commandLineParser.ExtractArgumentAttributes(options);
            var started = false;
            Monitor monitor = null;
            Consumer consumer = null;
            Producer producer = null;
            try
            {
                commandLineParser.CheckMandatoryArguments = true;
                commandLineParser.ParseCommandLine(args);
                if (options.DelayMs == 0)
                {
                    options.DelayMs = Options.DefaultDelayMs;
                }

                Logger.Info(
                    options.Type, start, "Starting with type {0} and delay of {1} ms", options.Type, options.DelayMs);
                SetupProducerConsumerQueueFactory(options, start);
                monitor = new Monitor(options, start);
                monitor.Start();
                consumer = new Consumer(options, start);
                var queue = consumer.Start();
                producer = new Producer(options, start, queue);
                producer.Start();
                started = true;
            }
            catch (CommandLineParser.Exceptions.CommandLineException exception)
            {
                Logger.ErrorException("Error during parsing of arguments", exception);
                commandLineParser.ShowUsage();
            }

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Type ctrl + q to exit");
                key = Console.ReadKey();
            }
            while (key.Key != ConsoleKey.Q && !key.Modifiers.HasFlag(ConsoleModifiers.Control));
            if (!started)
            {
                return;
            }

            producer.Stop();
            consumer.Stop();
            monitor.Stop();
        }

        private static void SetupProducerConsumerQueueFactory(Options options, string start)
        {
            switch (options.Type)
            {
                case ProducerConsumerQueueType.Default:
                    Logger.Debug(options.Type, start, "Using default factory");
                    break;
                case ProducerConsumerQueueType.Reactive:
                    Logger.Debug(options.Type, start, "Setting up Reactive factory");
                    ProducerConsumerQueueFactory<TestToken>.Set(new ReactiveProducerConsumerQueueFactory<TestToken>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("options", "Queue type unknown");
            }
        }
    }
}