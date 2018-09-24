// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evAnnouncement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evAnnouncement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    public class evAnnouncement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evAnnouncement"/> class.
        /// </summary>
        /// <param name="announcement">
        /// The announcement.
        /// </param>
        public evAnnouncement(int announcement)
        {
            this.Announcement = announcement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evAnnouncement"/> class.
        /// </summary>
        public evAnnouncement()
        {
            this.Announcement = 0;
        }

        /// <summary>
        /// Gets or sets the announcement number.
        /// </summary>
        public int Announcement { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "evAnnouncement. id: " + this.Announcement;
        }
    }
}