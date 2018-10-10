// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandCompositionKeys.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CommandCompositionKeys.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core
{
    /// <summary>
    /// The CommandCompositionKeys.
    /// </summary>
    public class CommandCompositionKeys
    {
        /// <summary>
        /// Commands within the Shell scope.
        /// </summary>
        public static class Shell
        {
            /// <summary>
            /// Commands within the UI scope.
            /// </summary>
            public static class UI
            {
                /// <summary>
                /// The ShowFileMenu command.
                /// </summary>
                public const string ShowFileMenu = "Diag.Shell.UI.ShowFileMenu";

                /// <summary>
                /// The ShowUnitMenu command.
                /// </summary>
                public const string ShowUnitMenu = "Diag.Shell.UI.ShowUnitMenu";

                /// <summary>
                /// The ShowApplicationMenu command.
                /// </summary>
                public const string ShowApplicationMenu = "Diag.Shell.UI.ShowApplicationMenu";

                /// <summary>
                /// The ShowViewMenu command.
                /// </summary>
                public const string ShowViewMenu = "Diag.Shell.UI.ShowViewMenu";

                /// <summary>
                /// The Reset command.
                /// </summary>
                public const string Reset = "Diag.Shell.UI.Reset";

                /// <summary>
                /// The ShowAboutScreen command.
                /// </summary>
                public const string ShowAboutScreen = "Diag.Shell.UI.ShowAboutScreen";

                /// <summary>
                /// The ShowOptionsDialog command.
                /// </summary>
                public const string ShowOptionsDialog = "Diag.Shell.UI.ShowOptionsDialog";
            }
        }

        /// <summary>
        /// Commands within the Unit scope.
        /// </summary>
        public static class Unit
        {
            /// <summary>
            /// The command to refresh the list of all (discovered) units.
            /// </summary>
            public const string RefreshAllUnits = "Diag.Unit.RefreshAllUnits";

            /// <summary>
            /// The command to show a prompt the user for adding a unit.
            /// </summary>
            public const string RequestAdd = "Diag.Unit.RequestAdd";

            /// <summary>
            /// The command to Announce a unit.
            /// </summary>
            public const string Announce = "Diag.Unit.Announce";

            /// <summary>
            /// The command to Reboot a unit.
            /// </summary>
            public const string Reboot = "Diag.Unit.Reboot";

            /// <summary>
            /// The command to Remove a unit.
            /// </summary>
            public const string Remove = "Diag.Unit.Remove";

            /// <summary>
            /// The command to toggle the Connection of a unit.
            /// </summary>
            public const string ToggleConnect = "Diag.Unit.ToggleConnect";

            /// <summary>
            /// The command to toggle the Favorite state of a unit.
            /// </summary>
            public const string ToggleFavorite = "Diag.Unit.ToggleFavorite";

            /// <summary>
            /// The command to Connect a unit.
            /// </summary>
            public const string Connect = "Diag.Unit.Connect";

            /// <summary>
            /// The command to Disconnect a unit.
            /// </summary>
            public const string Disconnect = "Diag.Unit.Disconnect";

            /// <summary>
            /// The command to EditIPSettings a unit.
            /// </summary>
            public const string EditIpSettings = "Diag.Unit.EditIPSettings";

            /// <summary>
            /// The command to LoadFileSystemFolder a unit.
            /// </summary>
            public const string LoadFileSystemFolder = "Diag.Unit.LoadFileSystemFolder";

            /// <summary>
            /// The command to OpenRemoteFile a unit.
            /// </summary>
            public const string OpenRemoteFile = "Diag.Unit.OpenRemoteFile";

            /// <summary>
            /// The command to DownloadRemoteFile a unit.
            /// </summary>
            public const string DownloadRemoteFile = "Diag.Unit.DownloadRemoteFile";

            /// <summary>
            /// The command to CancelRemoteFileDownload a unit.
            /// </summary>
            public const string CancelRemoteFileDownload = "Diag.Unit.CancelRemoteFileDownload";
        }

        /// <summary>
        /// Commands within the UnitApplication scope.
        /// </summary>
        public static class UnitApplication
        {
            /// <summary>
            /// The command to Launch an application on an unit.
            /// </summary>
            public const string Launch = "Diag.UnitApplication.Launch";

            /// <summary>
            /// The command to re-launch an application on an unit.
            /// </summary>
            public const string Relaunch = "Diag.UnitApplication.Relaunch";

            /// <summary>
            /// The command to End an application on an unit.
            /// </summary>
            public const string End = "Diag.UnitApplication.End";

            /// <summary>
            /// The command to load the children of a node in the medi tree of an application.
            /// </summary>
            public const string LoadMediTreeChildren = "Diag.UnitApplication.LoadMediTreeChildren";
        }
    }
}