// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionManager{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionManager&lt;T&gt; type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Defines the manager for interactions with prompts of the specified <typeparamref name="T"/> type.
    /// </summary>
    /// <typeparam name="T">The type of the prompt notification.</typeparam>
    public abstract class InteractionManager<T>
        where T : PromptNotification
    {
        static InteractionManager()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static InteractionManager<T> Current { get; private set; }

        /// <summary>
        /// Resets the current factory instance to the default one.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultInteractionManager.Instance);
        }

        /// <summary>
        /// Sets the current factory instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public static void SetCurrent(InteractionManager<T> instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", "The conflict manager instance can't be null");
            }

            Current = instance;
        }

        /// <summary>
        /// Gets or creates an interaction request for the given prompt notification.
        /// </summary>
        /// <returns>
        /// An interaction to be used for the specified prompt notification type.
        /// </returns>
        public abstract IInteractionRequest GetOrCreateInteractionRequest();

        /// <summary>
        /// Raises the specified prompt notification.
        /// </summary>
        /// <param name="promptNotification">The prompt notification.</param>
        /// <param name="callback">The callback.</param>
        public abstract void Raise(T promptNotification, Action<T> callback = null);

        private sealed class DefaultInteractionManager : InteractionManager<T>
        {
            private readonly ConcurrentDictionary<Type, IInteractionRequest> interactionRequests =
                new ConcurrentDictionary<Type, IInteractionRequest>();

            static DefaultInteractionManager()
            {
                Instance = new DefaultInteractionManager();
            }

            /// <summary>
            /// Gets the instance.
            /// </summary>
            /// <value>
            /// The instance.
            /// </value>
            public static DefaultInteractionManager Instance { get; private set; }

            /// <summary>
            /// Gets or creates an interaction request for the given prompt notification.
            /// </summary>
            /// <returns>
            /// An interaction to be used for the specified prompt notification type.
            /// </returns>
            public override IInteractionRequest GetOrCreateInteractionRequest()
            {
                var request = this.interactionRequests.GetOrAdd(typeof(T), this.ValueFactory);
                return request;
            }

            /// <summary>
            /// Raises the specified prompt notification.
            /// </summary>
            /// <param name="promptNotification">The prompt notification.</param>
            /// <param name="callback">The callback.</param>
            public override void Raise(T promptNotification, Action<T> callback = null)
            {
                if (!this.interactionRequests.ContainsKey(typeof(T)))
                {
                    return;
                }

                var request = this.interactionRequests[typeof(T)];
                if (request == null)
                {
                    return;
                }

                Action action = () =>
                {
                    if (callback != null)
                    {
                        callback(promptNotification);
                    }
                };
                request.Raise(promptNotification, action);
            }

            private IInteractionRequest ValueFactory(Type type)
            {
                return new InteractionRequest<T>();
            }
        }
    }
}