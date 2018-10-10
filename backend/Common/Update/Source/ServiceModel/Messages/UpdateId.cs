// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateId.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The identifier for an update.
    /// </summary>
    public class UpdateId : IEquatable<UpdateId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateId"/> class.
        /// </summary>
        public UpdateId()
        {
            this.BackgroundSystemGuid = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateId"/> class.
        /// </summary>
        /// <param name="backgroundSystemGuid">
        /// The GUID of the Background System that created this update command.
        /// </param>
        /// <param name="updateIndex">
        /// The update index.
        /// </param>
        public UpdateId(string backgroundSystemGuid, int updateIndex)
        {
            this.BackgroundSystemGuid = backgroundSystemGuid;
            this.UpdateIndex = updateIndex;
        }

        /// <summary>
        /// Gets or sets the GUID of the Background System that created this update command.
        /// </summary>
        [XmlAttribute]
        public string BackgroundSystemGuid { get; set; }

        /// <summary>
        /// Gets or sets the update index which is incremented for each update and allows to
        /// order updates by creation time. The index is only unique if the
        /// <see cref="BackgroundSystemGuid"/> stays the same.
        /// </summary>
        [XmlAttribute]
        public int UpdateIndex { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(UpdateId other)
        {
            return other != null
                   && this.BackgroundSystemGuid == other.BackgroundSystemGuid
                   && this.UpdateIndex == other.UpdateIndex;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        public override bool Equals(object obj)
        {
            var other = obj as UpdateId;
            return this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.BackgroundSystemGuid.GetHashCode() ^ this.UpdateIndex;
        }
    }
}