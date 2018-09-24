// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadyGateFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadyGateFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    /// <summary>
    /// Factory to create <see cref="IReadyGate"/> instances.
    /// </summary>
    public abstract class ReadyGateFactory
    {
        static ReadyGateFactory()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static ReadyGateFactory Current { get; private set; }

        /// <summary>
        /// Resets the current factory to the default one.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultReadyGateFactory.Instance);
        }

        /// <summary>
        /// Sets the current factory.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void SetCurrent(ReadyGateFactory instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Creates a factory.
        /// </summary>
        /// <param name="name">The name of the ready gate.</param>
        /// <param name="postNotificationAsync">
        /// The post notification async.
        /// </param>
        /// <returns>
        /// The <see cref="IReadyGate"/>.
        /// </returns>
        public abstract IReadyGate Create(string name, Func<Notification, Task<Guid>> postNotificationAsync);

        private sealed class DefaultReadyGateFactory : ReadyGateFactory
        {
            static DefaultReadyGateFactory()
            {
                Instance = new DefaultReadyGateFactory();
            }

            public static DefaultReadyGateFactory Instance { get; private set; }

            public override IReadyGate Create(string name, Func<Notification, Task<Guid>> postNotificationAsync)
            {
                return new ReadyGate(name, postNotificationAsync);
            }
        }
    }
}
