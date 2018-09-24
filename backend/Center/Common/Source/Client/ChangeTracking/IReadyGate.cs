// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadyGate.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IReadyGate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Notifications;

    /// <summary>
    /// Defines the interface of a component responsible to verify if a service ready using
    /// <see cref="PingNotification"/>s and handling <see cref="PongNotification"/>s.
    /// </summary>
    public interface IReadyGate
    {
        /// <summary>
        /// Waits that the service is ready.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        Task WaitReadyAsync();

        /// <summary>
        /// Sends a <see cref="PingNotification"/> and waits the <see cref="PongNotification"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        Task PingPongAsync();

        /// <summary>
        /// Forwards a <see cref="PongNotification"/>.
        /// </summary>
        /// <param name="notification">
        /// The notification to be forwarded.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        Task PongAsync(PongNotification notification);
    }
}