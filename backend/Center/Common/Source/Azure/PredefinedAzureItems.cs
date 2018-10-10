// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredefinedAzureItems.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PredefinedAzureItems type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Azure
{
    /// <summary>
    /// Defines the predefined items.
    /// </summary>
    public static class PredefinedAzureItems
    {
        /// <summary>
        /// The Azure settings.
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// Value for the connection limit.
            /// </summary>
            public static readonly string ConnectionLimit = "Wcf.ConnectionLimit";

            /// <summary>
            /// Connection string for the Center data context.
            /// </summary>
            public static readonly string CenterDataContext = "BackgroundSystem.CenterDataContext";

            /// <summary>
            /// Min log level for database log entries.
            /// </summary>
            public static readonly string LogLevel = "Diagnostics.LogLevel";

            /// <summary>
            /// The connection string of the azure storage.
            /// </summary>
            public static readonly string StorageConnectionString =
                "Center.StorageConnectionString";

            /// <summary>
            /// Connection string for notifications.
            /// </summary>
            public static readonly string NotificationsConnectionString =
                "BackgroundSystem.NotificationsConnectionString";

            /// <summary>
            /// Value for the host address.
            /// </summary>
            public static readonly string Host = "BackgroundSystem.Host";
        }

        /// <summary>
        /// Azure local storage resources.
        /// </summary>
        public static class LocalStorage
        {
            /// <summary>
            /// Local storage used for logs.
            /// </summary>
            public static readonly string Logs = "Logs";

            /// <summary>
            /// Local storage used for resources.
            /// </summary>
            public static readonly string Resources = "Resources";
        }
    }
}