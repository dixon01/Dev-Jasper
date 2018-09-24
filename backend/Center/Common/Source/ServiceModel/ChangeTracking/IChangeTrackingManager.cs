// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeTrackingManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IChangeTrackingManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    using Gorba.Center.Common.ServiceModel.Security;

    /// <summary>
    /// Defines the interface for the change tracking manager that can be used on the client side to work with change
    /// tracking objects.
    /// </summary>
    public interface IChangeTrackingManager : IDisposable
    {
        /// <summary>
        /// Changes the credentials used by this change tracking manager to access the background system.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        void ChangeCredentials(UserCredentials credentials);
    }
}