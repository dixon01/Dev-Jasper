// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using NLog;

    using RxLibrary;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly ISubject<Item> Items = new Subject<Item>();

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">
        /// The command line args.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Start");
            var queue = TestBufferedQueueStart();
            Produce();
            TestBufferedQueueStop(queue);
        }

        private static OrderedOperationProducerConsumerQueue<Item> TestBufferedQueueStart()
        {
            var queue = new OrderedOperationProducerConsumerQueue<Item>();
            queue.Register("Flagged", items => WriteOperations("Flagged", items));
            queue.Register("Unflagged", items => WriteOperations("Unflagged", items));
            queue.StartConsumer();
            Items.Subscribe(
                item => queue.Enqueue(new QueuedOperation<Item>(item.IsFlagged ? "Flagged" : "Unflagged", item)));
            return queue;
        }

        private static void TestBufferedQueueStop(OrderedOperationProducerConsumerQueue<Item> queue)
        {
            Console.ReadKey();
            queue.StopConsumer();
        }

        private static void WriteOperations(string name, IEnumerable<Item> operations)
        {
            Logger.Info("Operations '{0}':", name);
            foreach (var operation in operations)
            {
                WriteOperation(operation);
            }
        }

        private static void WriteOperation(Item item)
        {
            Logger.Info("Item: {0}", item);
        }

        private static void TestSimpleQueue()
        {
            var queue = new ReactiveProducerConsumerQueue<Item>(item => Logger.Info("Read: {0}", item));
            queue.StartConsumer();
            Items.Subscribe(item => queue.Enqueue(item));
            Console.ReadKey();
            queue.StopConsumer();
        }

        /// <summary>
        /// Produces items.
        /// </summary>
        internal static void Produce()
        {
            var rnd = new Random();
            var index = 1;
            Func<Item> createItem = () =>
                { return new Item(index++, rnd.NextDouble() < 0.6); };
            var producer = Observable.Interval(TimeSpan.FromMilliseconds(10)).StartWith(-10).Subscribe(l => Items.OnNext(createItem()));
            var singleSource = Items.Publish().RefCount();
        }

        private class Item
        {
            public Item(long value, bool isFlagged)
            {
                this.IsFlagged = isFlagged;
                this.Value = value;
            }

            public bool IsFlagged { get; private set; }

            protected long Value { get; private set; }

            public override string ToString()
            {
                return string.Format("{0}: {1}", this.IsFlagged ? "Flagged" : "Unflagged", this.Value);
            }
        }
    }
}
