// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientCommandCompositionKeys.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the command keys used for composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client
{
    /// <summary>
    /// Defines the command keys used for composition.
    /// </summary>
    public static class ClientCommandCompositionKeys
    {
        /// <summary>
        /// The login command.
        /// </summary>
        public const string Login = "Client.Login";

        /// <summary>
        /// The logout command.
        /// </summary>
        public const string Logout = "Client.Logout";

        /// <summary>
        /// The offline mode command.
        /// </summary>
        public const string OfflineMode = "Client.OfflineMode";

        /// <summary>
        /// The tenant selection command.
        /// </summary>
        public const string TenantSelection = "Client.TenantSelection";

        /// <summary>
        /// The cancel tenant selection command.
        /// </summary>
        public const string CancelTenantSelection = "Client.CancelTenantSelection";

        /// <summary>
        /// The change password command.
        /// </summary>
        public const string ChangePassword = "Client.ChangePassword";

        /// <summary>
        /// The cancel update command.
        /// </summary>
        public const string CancelUpdate = "Client.CancelUpdate";

        /// <summary>
        /// Application commands.
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// The Exit command.
            /// </summary>
            public const string Exit = "Client.Application.Exit";
        }
    }
}
