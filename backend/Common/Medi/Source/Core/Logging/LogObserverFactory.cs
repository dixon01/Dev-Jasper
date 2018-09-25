// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogObserverFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogObserverFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Logging
{
    using System;

    /// <summary>
    /// Default implementation of <see cref="ILogObserverFactory"/>.
    /// </summary>
    internal class LogObserverFactory : ILogObserverFactory
    {
        private readonly IMessageDispatcher messageDispatcher;

        private readonly ILogObserver localObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogObserverFactory"/> class.
        /// </summary>
        /// <param name="interceptLocalLogs">
        /// A flag indicating if the local log observer should be initialized.
        /// </param>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
        /// </param>
        public LogObserverFactory(bool interceptLocalLogs, IMessageDispatcher messageDispatcher)
        {
            this.messageDispatcher = messageDispatcher;
            if (!interceptLocalLogs)
            {
                return;
            }

            this.localObserver = new LocalLogObserver(this.messageDispatcher);
        }

        /// <summary>
        /// Gets the local log observer.
        /// </summary>
        public ILogObserver LocalObserver
        {
            get
            {
                if (this.localObserver == null)
                {
                    throw new NotSupportedException("Local log interception is turned off");
                }

                return this.localObserver;
            }
        }

        /// <summary>
        /// Creates a remote observer for the given address.
        /// </summary>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        /// <returns>
        /// The <see cref="ILogObserver"/>.
        /// </returns>
        public ILogObserver CreateRemoteObserver(MediAddress remoteAddress)
        {
            return new RemoteLogObserver(this.messageDispatcher, remoteAddress);
        }
    }
}