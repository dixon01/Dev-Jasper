// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserCredentials.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserCredentials type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Security
{
    using System.Diagnostics;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the user credentials used for authentication between clients and BackgroundSystem.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{Username}, {HashedPassword}")]
    public class UserCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentials"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="hashedPassword">
        /// The hashed password.
        /// </param>
        public UserCredentials(string username, string hashedPassword)
        {
            this.Username = username;
            this.HashedPassword = hashedPassword;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentials"/> class.
        /// </summary>
        public UserCredentials()
        {
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the hash of the password (evaluated using the MD5 algorithm).
        /// </summary>
        [DataMember]
        public string HashedPassword { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            var other = obj as UserCredentials;
            return other != null && string.Equals(this.Username, other.Username)
                   && string.Equals(this.HashedPassword, other.HashedPassword);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Username.GetHashCode() ^ this.HashedPassword.GetHashCode();
        }
    }
}