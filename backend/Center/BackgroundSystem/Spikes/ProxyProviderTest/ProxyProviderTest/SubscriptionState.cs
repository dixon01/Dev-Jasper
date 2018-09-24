// -----------------------------------------------------------------------
// <copyright file="SubscriptionState.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ProxyProviderTest
{
    using System;
    using System.Diagnostics.Contracts;

    using NLog;

    /// <summary>
    /// This class contains state information for a subscription to events coming from the Comms.
    /// Disposing the object disposes the event stream. 
    /// </summary>
    internal class SubscriptionState : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable[] observable;

        private readonly Guid commsSubscriptionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionState"/> class.
        /// </summary>
        /// <param name="commsSubscriptionId">The comms subscription id.</param>
        /// <param name="observable">The observable stream.</param>
        public SubscriptionState(Guid commsSubscriptionId, params IDisposable[] observable)
        {
            Contract.Requires(observable != null, "The observable event stream can't be null.");
            Contract.Ensures(
                this.observable != null, "The observable event stream field must be assigned with the parameter.");
            this.commsSubscriptionId = commsSubscriptionId;
            this.observable = observable;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SubscriptionState"/> class.
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SubscriptionState"/> is reclaimed by garbage collection.
        /// </summary>
        ~SubscriptionState()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Logger.Trace("Disposing comms subscription {0}", this.commsSubscriptionId);
                foreach (var disposable in this.observable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
