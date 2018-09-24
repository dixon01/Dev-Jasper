// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnouncementHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnnouncementHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Bus.Core.Data;

    /// <summary>
    /// The announcement handler that triggers announcements in the right moment.
    /// </summary>
    internal class AnnouncementHandler
    {
        private int currentIndex;

        private int announcementId;

        private float longitude;

        private float latitude;

        private int buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementHandler"/> class.
        /// </summary>
        public AnnouncementHandler()
        {
            this.Init();
        }

        /// <summary>
        /// (Re)initializes this handler.
        /// </summary>
        public void Init()
        {
            this.currentIndex = -1;
            this.announcementId = 0;
            this.longitude = 0;
            this.latitude = 0;
            this.buffer = 0;
        }

        /// <summary>
        /// Handles GPS position updates.
        /// </summary>
        /// <param name="lon">
        /// The longitude.
        /// </param>
        /// <param name="lat">
        /// The latitude.
        /// </param>
        public void HandlePosition(float lon, float lat)
        {
            if (this.currentIndex < 0)
            {
                return;
            }

            var distance = Wgs84.GetDistance(this.latitude, this.longitude, lat, lon);
            if (distance <= this.buffer)
            {
                MessageDispatcher.Instance.Broadcast(new evAnnouncement(this.announcementId));
                this.Init();
            }
        }

        /// <summary>
        /// Adds an announcement for the given route and index.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="idx">
        /// The index.
        /// </param>
        /// <param name="userParameters">
        /// The user parameters.
        /// </param>
        public void Add(RouteInfo route, int idx, ParamIti userParameters)
        {
            if (userParameters.MinDistanceAnnouncement <= 0 || userParameters.BufferAnnouncement <= 0)
            {
                // feature deactivated
                return;
            }

            if (route == null || idx < 0 || route.GetTerminusIndex() <= idx)
            {
                return;
            }

            var p1 = route.Points[idx];
            var p2 = route.Points[idx + 1];
            var distance = Wgs84.GetDistance(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude);
            if (distance <= userParameters.MinDistanceAnnouncement)
            {
                return;
            }

            this.longitude = p2.Longitude;
            this.latitude = p2.Latitude;
            this.currentIndex = idx + 1;
            this.announcementId = p2.InteriorAnnouncementMp3;
            this.buffer = userParameters.BufferAnnouncement;
        }

        /// <summary>
        /// Removes the announcement for the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void Remove(int index)
        {
            if (this.currentIndex == index)
            {
                this.Init();
            }
        }
    }
}