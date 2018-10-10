namespace Luminator.AdhocMessaging.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public static class AsyncHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Execute's an async Task<T> method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task<T> method to execute</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(
                async _ =>
                    {
                        try
                        {
                            await task();
                        }
                        catch (Exception e)
                        {
                            synch.InnerException = e;
                            throw;
                        }
                        finally
                        {
                            synch.EndMessageLoop();
                        }
                    },
                null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        ///     Execute's an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            var ret = default(T);
            synch.Post(
                async _ =>
                    {
                        try
                        {
                            ret = await task();
                        }
                        catch (Exception e)
                        {
                            synch.InnerException = e;
                            throw;
                        }
                        finally
                        {
                            synch.EndMessageLoop();
                        }
                    },
                null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        #endregion

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            #region Fields

            private readonly Queue<Tuple<SendOrPostCallback, object>> items = new Queue<Tuple<SendOrPostCallback, object>>();

            private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

            private bool done;

            #endregion

            #region Public Properties

            public Exception InnerException { get; set; }

            #endregion

            #region Public Methods and Operators

            public void BeginMessageLoop()
            {
                while (!this.done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (this.items)
                    {
                        if (this.items.Count > 0) task = this.items.Dequeue();
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (this.InnerException != null) // the method threw an exeption
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", this.InnerException);
                    }
                    else
                    {
                        this.workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }

            public void EndMessageLoop()
            {
                this.Post(_ => this.done = true, null);
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (this.items)
                {
                    this.items.Enqueue(Tuple.Create(d, state));
                }
                this.workItemsWaiting.Set();
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            #endregion
        }
    }
}