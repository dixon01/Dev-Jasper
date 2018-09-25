// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedOperationProducerConsumerQueue{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OrderedOperationProducerConsumerQueue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RxLibrary
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Text;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Defines a producer-consumer queue that enqueus 
    /// </summary>
    /// <typeparam name="T">The type of the items in the queue.</typeparam>
    public class OrderedOperationProducerConsumerQueue<T> : IProducerConsumerQueue<QueuedOperation<T>>
        where T : class 
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IScheduler consumerScheduler;

        private readonly IScheduler producerScheduler;

        private readonly ISubject<QueuedOperation<T>> operations = new Subject<QueuedOperation<T>>();

        private readonly object locker = new object();

        private readonly ConcurrentDictionary<string, Action<IEnumerable<T>>> actions = new ConcurrentDictionary<string, Action<IEnumerable<T>>>();

        private IDisposable subscription;

        private volatile bool isStarted;

        public OrderedOperationProducerConsumerQueue(IScheduler producerScheduler, IScheduler consumerScheduler)
        {
            this.producerScheduler = producerScheduler;
            this.consumerScheduler = consumerScheduler;
        }

        public OrderedOperationProducerConsumerQueue(IScheduler producerScheduler)
            : this(producerScheduler, Scheduler.CurrentThread)
        {
        }

        public OrderedOperationProducerConsumerQueue()
            : this(new EventLoopScheduler())
        {
        }

        public IScheduler ConsumerScheduler { get { return this.consumerScheduler; } }

        public IScheduler ProducerSchduler { get { return this.producerScheduler; } }

        public int Count { get
        {
            return 0;
        }
        }

        public int Capacity { get
        {
            return 0;
        }
        }

        public bool Enqueue(QueuedOperation<T> obj)
        {
            if (!this.isStarted)
            {
                throw new ApplicationException("Queue not started");
            }

            this.operations.OnNext(obj);
            return true;
        }

        public IDisposable Register(string type, Action<IEnumerable<T>> action)
        {
            var registered = this.actions.TryAdd(type, action);
            if (registered)
            {
                return null;
            }

            return new Registration(this.actions, type);
        }

        public void StartConsumer()
        {
            if (this.isStarted)
            {
                return;
            }

            lock (this.locker)
            {
                if (this.isStarted)
                {
                    return;
                }

                this.CreateSubscriptionWithRunsExtension();

                this.isStarted = true;
            }
        }

        public void StopConsumer()
        {
            if(!this.isStarted)
            {
                return;
            }

            lock (this.locker)
            {
                if (!this.isStarted)
                {
                    return;
                }

                this.isStarted = false;
            }

            this.subscription.Dispose();
            this.subscription = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void CreateSubscriptionWithBuffer()
        {
            this.subscription = this.operations
                .ObserveOn(this.producerScheduler)
                .Timestamp()
                .Window(TimeSpan.FromSeconds(8), 5000)
                .Select(o => o.Aggregate(new OperationsBufferContext(), (acc, cur) => acc.Add(cur)))
                .Merge()
                .SubscribeOn(this.consumerScheduler)
                .Subscribe(this.Execute, this.OnError, this.OnCompleted);
        }

        private void CreateSubscriptionWithRunsExtension()
        {
            this.subscription = this.operations
                .Timestamp()
                .ObserveOn(this.producerScheduler)
                .Buffer(TimeSpan.FromSeconds(8), 5000)
                .Select(o => o.ToRuns(timestamped => timestamped.Value.Name).Where(list => list.Count > 0))
                .SubscribeOn(this.consumerScheduler)
                .Subscribe(this.Execute, this.OnError, this.OnCompleted);
        }

        private void Execute(IEnumerable<IList<Timestamped<QueuedOperation<T>>>> entries)
        {
            var list = entries.ToList();
            Logger.Debug(list.ToString());
            foreach (var operation in list)
            {
                var items = operation.ToList();
                Action<IEnumerable<T>> action;
                if (this.actions.TryGetValue(items.First().Value.Name, out action))
                {
                    try
                    {
                        var data = items.Select(t => t.Value.Value);
                        action(data);
                    }
                    catch (Exception exception)
                    {
                        Logger.ErrorException("Error while executing an action", exception);
                    }
                }
            }
        }

        private void Execute(OperationsBufferContext operations)
        {
            Logger.Debug(operations.ToString());
            foreach (var operation in operations.Operations)
            {
                var list = operation.ToList();
                Action<IEnumerable<T>> action;
                if (this.actions.TryGetValue(list.First().Value.Name, out action))
                {
                    try
                    {
                        var data = list.Select(t => t.Value.Value);
                        action(data);
                    }
                    catch (Exception exception)
                    {
                        Logger.ErrorException("Error while executing an action", exception);
                    }
                }
            }
        }

        private void OnError(Exception exception)
        {
            Logger.ErrorException("Error during operations", exception);
        }

        private void OnCompleted()
        {
            Logger.Info("Stream of operations completed");
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.subscription != null)
                {
                    this.subscription.Dispose();
                    this.subscription = null;
                }
            }
        }

        private class Registration : IDisposable
        {
            private readonly ConcurrentDictionary<string, Action<IEnumerable<T>>> actions;

            private readonly string type;

            public Registration(ConcurrentDictionary<string, Action<IEnumerable<T>>> actions, string type)
            {
                this.actions = actions;
                this.type = type;
            }

            public void Dispose()
            {
                this.Dispose(true);
            }

            public void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Action<IEnumerable<T>> action;
                    this.actions.TryRemove(this.type, out action);
                }
            }
        }

        private class OperationsBufferContext
        {
            private readonly List<IList<Timestamped<QueuedOperation<T>>>> operations = new List<IList<Timestamped<QueuedOperation<T>>>>();

            private readonly object locker = new object();

            private string currentType;

            public OperationsBufferContext()
            {
            }

            public OperationsBufferContext Add(Timestamped<QueuedOperation<T>> operation)
            {
                lock (this.locker)
                {
                    var last = this.operations.LastOrDefault();
                    if (last == null || !string.Equals(this.currentType, operation.Value.Name, StringComparison.InvariantCulture))
                    {
                        this.currentType = operation.Value.Name;
                        last = new List<Timestamped<QueuedOperation<T>>>();
                        this.operations.Add(last);
                    }

                    last.Add(operation);
                }
                return this;
            }

            public IEnumerable<IList<Timestamped<QueuedOperation<T>>>> Operations { get { return this.operations.AsReadOnly(); } }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder("[");
                if (operations.Count > 0)
                {
                    stringBuilder.Append(Display(operations.First()));
                    foreach (var item in operations.Skip(1))
                    {
                        stringBuilder.AppendFormat(", {0}", Display(item));
                    }
                }

                stringBuilder.Append("]");
                return stringBuilder.ToString();
            }

            private static string Display(IList<Timestamped<QueuedOperation<T>>> operations)
            {
                var stringBuilder = new StringBuilder("[");
                Func<IList<Timestamped<QueuedOperation<T>>>, string> display = o =>
                    {
                        var list = o.ToList();
                        var first = list.FirstOrDefault();
                        if (first == null)
	                    {
                            return string.Empty;
	                    }

                        var ok = list.All(op => op.Value.Name.Equals(first.Value.Name, StringComparison.InvariantCulture));

                        return string.Format("{{Operations {0}:{1}, {2}}}", first.Value.Name, list.Count, ok);
                    };
                Func<Timestamped<QueuedOperation<T>>, string> displaySingle = t =>
                    {
                        return string.Format("{0}: {1} ({2})", t.Timestamp, t.Value.Name, t.Value.Value);
                    };
                var firstOperation = operations.FirstOrDefault();
                if (firstOperation != null)
                {
                    stringBuilder.Append(displaySingle(firstOperation));
                    foreach (var item in operations.Skip(1))
                    {
                        stringBuilder.AppendFormat(", {0}", displaySingle(item));
                    }
                }

                stringBuilder.Append("]");
                return stringBuilder.ToString();
            }
        }
    }
}
