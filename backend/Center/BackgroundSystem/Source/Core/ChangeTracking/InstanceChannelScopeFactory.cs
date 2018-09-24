// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceChannelScopeFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstanceChannelScopeFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.ChangeTracking
{
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// An implementation of the <see cref="ChannelScopeFactory&lt;T&gt;"/> that always returns an
    /// <see cref="InstanceChannelScope"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    public class InstanceChannelScopeFactory<T> : ChannelScopeFactory<T>
        where T : class
    {
        private readonly T instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceChannelScopeFactory{T}"/> class.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public InstanceChannelScopeFactory(T instance)
        {
            this.instance = instance;
        }

        /// <summary>
        /// Creates a new channel with the given <paramref name="userCredentials"/> and wraps it in a scope disposable
        /// object.
        /// </summary>
        /// <param name="userCredentials">
        /// The user credentials.
        /// </param>
        /// <returns>A new channel scope.</returns>
        /// <remarks>
        /// The channel scope returned by this factory doesn't dispose anything.
        /// </remarks>
        public override ChannelScope<T> Create(UserCredentials userCredentials)
        {
            return new InstanceChannelScope(this.instance);
        }

        private class InstanceChannelScope : ChannelScope<T>
        {
            public InstanceChannelScope(T instance)
                : base(instance)
            {
            }

            protected override void Dispose(bool isDisposing)
            {
                // Nothing to dispose
            }
        }
    }
}
