// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInteractionRequest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IInteractionRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Defines an interaction request.
    /// </summary>
    public interface IInteractionRequest
    {
        /// <summary>
        /// Occurs when an interaction request is raised.
        /// </summary>
        event EventHandler<InteractionRequestEventArgs> Raised;

        /// <summary>
        /// Raises this request.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="callback">The callback.</param>
        void Raise(Notification notification, Action callback);
    }
}