// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionRequest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Defines an interaction request for the specified prompt notification type.
    /// </summary>
    /// <typeparam name="T">The type of the prompt notification.</typeparam>
    public class InteractionRequest<T> : IInteractionRequest
        where T : PromptNotification
    {
        /// <summary>
        /// Occurs when an interaction request is raised.
        /// </summary>
        public event EventHandler<InteractionRequestEventArgs> Raised;

        /// <summary>
        /// Raises this request.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="callback">The callback.</param>
        public void Raise(Notification notification, Action callback)
        {
            var handler = this.Raised;
            if (handler == null)
            {
                return;
            }

            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(() => handler(this, new InteractionRequestEventArgs(notification, callback)));
        }
    }
}