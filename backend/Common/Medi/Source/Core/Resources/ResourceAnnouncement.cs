// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceAnnouncement.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceAnnouncement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    /// <summary>
    /// Message that announces that a resource is coming soon.
    /// </summary>
    public class ResourceAnnouncement : IResourceMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAnnouncement"/> class.
        /// </summary>
        public ResourceAnnouncement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAnnouncement"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public ResourceAnnouncement(ResourceId id)
        {
            this.Id = id;
            this.IsTemporary = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAnnouncement"/> class.
        /// This constructor is to be used for temporary (file) transfers.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="originalName">
        /// The original file name.
        /// </param>
        public ResourceAnnouncement(ResourceId id, string originalName)
        {
            this.Id = id;
            this.IsTemporary = true;
            this.OriginalName = originalName;
        }

        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        public ResourceId Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is only temporary.
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// This is only used for temporary file transfers.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("ResourceAnnouncement[{0}]", this.Id);
        }
    }
}
