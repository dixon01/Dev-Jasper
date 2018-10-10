// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelScope{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelScope type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client
{
    using System;
    using System.ServiceModel;

    using NLog;

    /// <summary>
    /// Wrapper around a channel.
    /// When disposed, the inner channel will be closed.
    /// </summary>
    /// <typeparam name="T">The type of the channel.</typeparam>
    public class ChannelScope<T> : IDisposable
        where T : class
    {
        private readonly Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelScope{T}"/> class.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        protected internal ChannelScope(T channel)
            : this()
        {
            this.Channel = channel;
        }

        private ChannelScope()
        {
            this.logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the inner channel.
        /// </summary>
        public T Channel { get; private set; }

        /// <summary>
        /// Creates a new scope for the specified channel.
        /// </summary>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// The <see cref="ChannelScope&lt;T&gt;"/>.
        /// </returns>
        public static ChannelScope<T> Create(T channel)
        {
            return new ChannelScope<T>(channel);
        }

        /// <summary>
        /// Disposes this object, closing the inner channel.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="isDisposing">
        /// Flag used by the Dispose pattern.
        /// </param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            var channel = this.Channel as ICommunicationObject;
            if (channel == null)
            {
                this.logger.Trace("The channel is null. Nothing to dispose.");
                return;
            }

            try
            {
                this.logger.Trace("Closing the channel.");
                channel.Close();
            }
            catch (CommunicationException exception)
            {
                this.logger.Debug("Communication exception while closing the channel. Aborting it", exception);
                channel.Abort();
            }
            catch (TimeoutException exception)
            {
                this.logger.Debug("Timeout exception while closing the channel. Aborting it", exception);
                channel.Abort();
            }
            catch (Exception exception)
            {
                this.logger.Debug("Generic exception while closing the channel. Aborting it", exception);
                channel.Abort();

                throw;
            }
        }
    }
}