// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceId.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Unique identifier for a resource.
    /// Don't create instances yourself, but use
    /// <see cref="IResourceService.RegisterResource(string,bool)"/> instead.
    /// This object can also be sent to remote peers to identify
    /// a resource, but make sure you called
    /// <see cref="IResourceService.SendResource(ResourceInfo,MediAddress)"/> beforehand.
    /// </summary>
    public class ResourceId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceId"/> class.
        /// </summary>
        public ResourceId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceId"/> class.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        public ResourceId(string hash)
        {
            this.Hash = hash;
        }

        /// <summary>
        /// Gets or sets the MD5 hash of the resource.
        /// </summary>
        [XmlAttribute]
        public string Hash { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is
        /// equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to
        /// the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare
        /// with the current <see cref="T:System.Object"/>. </param>
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return this.Hash.Equals(((ResourceId)obj).Hash, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Hash.ToLower().GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{{{0}}}", this.Hash);
        }
    }
}