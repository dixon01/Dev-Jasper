// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationManagerUtility.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotificationManagerUtility type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Notifications
{
    using System;
    using System.Linq;

    /// <summary>
    /// Utility for notification managers.
    /// </summary>
    public static class NotificationManagerUtility
    {
        private const int SessionIdLength = 8;

        private static readonly Random Random = new Random();

        /// <summary>
        /// Gets the application name that can be used for creating subscriptions.
        /// </summary>
        /// <returns>
        /// The name that can be used for subscriptions.
        /// </returns>
        public static string GetApplicationName()
        {
            if (AppDomain.CurrentDomain.FriendlyName.Contains('/'))
            {
                return System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName();
            }

            return AppDomain.CurrentDomain.FriendlyName.Replace(".exe", string.Empty).Replace(" ", string.Empty);
        }

        /// <summary>
        /// Generates a unique string that can be used as session id.
        /// </summary>
        /// <returns>
        /// The unique session id.
        /// </returns>
        public static string GenerateUniqueSessionId()
        {
            const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(
                Enumerable.Repeat(Chars, SessionIdLength)
                          .Select(s => s[Random.Next(s.Length)])
                          .ToArray());
        }
    }
}