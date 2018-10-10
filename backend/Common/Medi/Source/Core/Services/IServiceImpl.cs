// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceImpl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Services
{
    /// <summary>
    /// Medi service implementation interface.
    /// All services must implement this interface, so they can be started and stopped by Medi.
    /// </summary>
    internal interface IServiceImpl : IService
    {
        /// <summary>
        /// Starts this service.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that owns this service.
        /// </param>
        void Start(IMessageDispatcherImpl dispatcher);

        /// <summary>
        /// Stops this service.
        /// </summary>
        void Stop();
    }
}