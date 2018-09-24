// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Permission.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Permission type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.ServiceModel
{
    /// <summary>
    /// Defines the permissions.
    /// </summary>
    public enum Permission
    {
        /// <summary>
        /// The read.
        /// </summary>
        Read = 0,

        /// <summary>
        /// The write.
        /// </summary>
        Write = 1
    }
}