// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotAuthenticatedException.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotAuthenticatedException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Exceptions
{
    using System;

    /// <summary>
    /// Exception determining a failed attempt of authentication.
    /// </summary>
    [Serializable]
    public class NotAuthenticatedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthenticatedException"/> class.
        /// </summary>
        /// <param name="login">
        ///     The login.
        /// </param>
        /// <param name="remoteAddress">
        ///     The remote address.
        /// </param>
        public NotAuthenticatedException(string login = null, string remoteAddress = null)
        {
            this.RemoteAddress = remoteAddress;
            this.Login = login;
        }

        /// <summary>
        /// Gets the login used for the authentication attempt.
        /// </summary>
        public string Login { get; private set; }

        /// <summary>
        /// Gets the remote address of the client trying to authenticate.
        /// </summary>
        /// <value>
        /// The address of the client trying to authenticate if the attempt was done remotely; otherwise, <c>null</c>.
        /// </value>
        public string RemoteAddress { get; private set; }
    }
}