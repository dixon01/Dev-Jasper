// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Users.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Users type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WcfAuthenticatedSession.ServiceModel
{
    /// <summary>
    /// The users.
    /// </summary>
    public enum Users
    {
        /// <summary>
        /// The undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The reader.
        /// </summary>
        Reader = 1,

        /// <summary>
        /// The writer.
        /// </summary>
        Writer = 2,

        /// <summary>
        /// The god.
        /// </summary>
        God = 3,

        /// <summary>
        /// The unauthorized.
        /// </summary>
        Unauthorized = 4
    }
}