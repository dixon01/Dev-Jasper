// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractionRequestEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InteractionRequestEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Defines the event args of an interaction request.
    /// </summary>
    public class InteractionRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionRequestEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="callback">The callback.</param>
        public InteractionRequestEventArgs(Notification entity, Action callback)
        {
            this.Callback = callback;
            this.Entity = entity;
        }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        public Action Callback { get; private set; }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public Notification Entity { get; private set; }
    }
}