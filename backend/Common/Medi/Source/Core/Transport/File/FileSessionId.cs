// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSessionId.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSessionId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using System;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;

    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// Implementation of <see cref="ISessionId"/> for the file exchange protocol.
    /// </summary>
    internal class FileSessionId : ISessionId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSessionId"/> class.
        /// </summary>
        /// <param name="address">
        /// The address of the node for which to create this id.
        /// </param>
        public FileSessionId(MediAddress address)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", address.Unit, address.Application));
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            this.Hash = sb.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSessionId"/> class.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        public FileSessionId(string hash)
        {
            this.Hash = hash;
        }

        /// <summary>
        /// Gets the hash used to identify a session.
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>. </param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as FileSessionId);
        }

        /// <summary>
        /// Checks if the other session id is the same as this object.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// True if the session ids are the same.
        /// </returns>
        public bool Equals(ISessionId other)
        {
            return this.Equals(other as FileSessionId);
        }

        /// <summary>
        /// Checks if the given <see cref="FileSessionId"/> has the same <see cref="Hash"/>
        /// as this object.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// true if the two are the same.
        /// </returns>
        public bool Equals(FileSessionId other)
        {
            if (other == null || other.GetType() != this.GetType())
            {
                return false;
            }

            return this.Hash.Equals(other.Hash, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Hash.ToLower(CultureInfo.InvariantCulture).GetHashCode();
        }
    }
}
