// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelScopeFactory{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelScopeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client
{
    using System;
    using System.Linq;
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    /// <summary>
    /// Factory to create <see cref="IDisposable"/> wrapper around channels for services of type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service. It must be a valid WCF service.</typeparam>
    public abstract class ChannelScopeFactory<T>
        where T : class
    {
        static ChannelScopeFactory()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static ChannelScopeFactory<T> Current { get; private set; }

        /// <summary>
        /// Resets the current factory to the default one which uses a default configuration entry for the specified
        /// service.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultChannelScopeFactory.Instance);
        }

        /// <summary>
        /// Replaces the current factory with the given one.
        /// </summary>
        /// <param name="instance">The instance to be set as current factory.</param>
        public static void SetCurrent(ChannelScopeFactory<T> instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Creates a new channel with the given <paramref name="userCredentials"/> and wraps it in a scope disposable
        /// object.
        /// </summary>
        /// <param name="userCredentials">
        /// The user credentials.
        /// </param>
        /// <returns>A new channel scope.</returns>
        public abstract ChannelScope<T> Create(UserCredentials userCredentials);

        /// <summary>
        /// Defines an abstract channel scope factory that internally uses a channel factory.
        /// </summary>
        public abstract class ChannelFactoryChannelScopeFactory : ChannelScopeFactory<T>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private UserCredentials currentCredentials;

            /// <summary>
            /// Gets the internal channel factory.
            /// </summary>
            protected ChannelFactory<T> InternalFactory { get; private set; }

            /// <summary>
            /// Creates a new channel with the given <paramref name="userCredentials"/> and wraps it in a scope
            /// disposable object.
            /// </summary>
            /// <param name="userCredentials">
            /// The user credentials.
            /// </param>
            /// <returns>A new channel scope.</returns>
            public override ChannelScope<T> Create(UserCredentials userCredentials)
            {
                this.EnsureChannelFactory(userCredentials);
                var channel = this.InternalFactory.CreateChannel();
                return new ChannelScope<T>(channel);
            }

            /// <summary>
            /// Creates the internal channel factory.
            /// </summary>
            /// <returns>
            /// The internal <see cref="ChannelFactory"/>.
            /// </returns>
            protected abstract ChannelFactory<T> CreateInternalChannelFactory();

            /// <summary>
            /// Ensures that the internal channel factory is created and in a valid state. In any of these cases it
            /// creates a new channel factory.
            /// </summary>
            /// <param name="userCredentials">
            /// The user credentials.
            /// </param>
            protected void EnsureChannelFactory(UserCredentials userCredentials = null)
            {
                var validStates = new[]
                    {
                        CommunicationState.Created, CommunicationState.Opened, CommunicationState.Opening
                    };
                if (this.InternalFactory != null && validStates.Contains(this.InternalFactory.State)
                    && object.Equals(this.currentCredentials, userCredentials))
                {
                    return;
                }

                lock (this)
                {
                    if (this.InternalFactory != null && validStates.Contains(this.InternalFactory.State)
                        && object.Equals(this.currentCredentials, userCredentials))
                    {
                        return;
                    }

                    if (this.InternalFactory != null)
                    {
                        this.InternalFactory.BeginClose(this.Closed, this.InternalFactory);
                    }

                    this.InternalFactory = this.CreateInternalChannelFactory();
                    this.currentCredentials = userCredentials;
                    if (userCredentials == null)
                    {
                        return;
                    }

                    this.InternalFactory.SetLoginCredentials(userCredentials);
                }
            }

            private void Closed(IAsyncResult ar)
            {
                var channelFactory = (ChannelFactory<T>)ar.AsyncState;
                try
                {
                    channelFactory.EndClose(ar);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't properly close old channel factory");
                }
            }
        }

        /// <summary>
        /// The default channel scope factory uses the app config to find a valid configuration for the channel factory
        /// (using the '*' wildcard).
        /// </summary>
        private sealed class DefaultChannelScopeFactory : ChannelFactoryChannelScopeFactory
        {
            private static readonly Lazy<DefaultChannelScopeFactory> LazyInstance =
                new Lazy<DefaultChannelScopeFactory>(CreateInstance);

            private DefaultChannelScopeFactory()
            {
            }

            public static DefaultChannelScopeFactory Instance
            {
                get
                {
                    return LazyInstance.Value;
                }
            }

            protected override ChannelFactory<T> CreateInternalChannelFactory()
            {
                return new ChannelFactory<T>("*");
            }

            private static DefaultChannelScopeFactory CreateInstance()
            {
                return new DefaultChannelScopeFactory();
            }
        }
    }
}