// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredefinedAzureItems.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PredefinedAzureItems type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host
{
    /// <summary>
    /// Defines the predefined items.
    /// </summary>
    public static class PredefinedAzureItems
    {
        /// <summary>
        /// The endpoints.
        /// </summary>
        public static class Endpoints
        {
            /// <summary>
            /// The Http endpoint.
            /// </summary>
            public static readonly string HttpEndpoint = "Portal.HttpEndpoint";

            /// <summary>
            /// The Https endpoint.
            /// </summary>
            public static readonly string HttpsEndpoint = "Portal.HttpsEndpoint";
        }

        /// <summary>
        /// The Azure settings.
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// Value for the connection limit.
            /// </summary>
            public static readonly string ConnectionLimit = "Portal.ConnectionLimit";

            /// <summary>
            /// Value for the enable https flag.
            /// </summary>
            public static readonly string EnableHttps = "Portal.EnableHttps";
        }

        /// <summary>
        /// Azure local storage resources.
        /// </summary>
        public static class Storage
        {
            /// <summary>
            /// Connection string for notifications.
            /// </summary>
            public static readonly string AppData = "Portal.AppData";
        }
    }
}