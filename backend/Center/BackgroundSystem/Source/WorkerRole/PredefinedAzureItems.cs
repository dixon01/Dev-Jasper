// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredefinedAzureItems.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PredefinedAzureItems type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole
{
    /// <summary>
    /// Defines the predefined items.
    /// </summary>
    public static class PredefinedAzureItems
    {
        /// <summary>
        /// Azure endpoints.
        /// </summary>
        public static class Endpoints
        {
            /// <summary>
            /// Base address used for service endpoints.
            /// </summary>
            public static readonly string Services = "BackgroundSystem.Services";
        }

        /// <summary>
        /// Azure settings.
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// The tenant string used to access the Azure Active Directory.
            /// </summary>
            public static readonly string Tenant = "ActiveDirectory.Tenant";

            /// <summary>
            /// The client id string used to validate a user against Azure Active Directory.
            /// </summary>
            public static readonly string ClientId = "ActiveDirectory.ClientId";

            /// <summary>
            /// The resource url used for validation.
            /// </summary>
            public static readonly string ResourceUrl = "ActiveDirectory.AuthorizationUrl";

            /// <summary>
            /// Flag to decide if the portal host should also be started (value = true) or not (value = false).
            /// </summary>
            public static readonly string StartPortalHost = "BackgroundSystem.StartPortalHost";

            /// <summary>
            /// Value to indicate whether the portal should provide the links to the Beta version (<c>true</c>) or
            /// official version (<c>false</c>).
            /// </summary>
            public static readonly string ClickOnceBeta = "Portal.ClickOnceUseBeta";
        }
    }
}