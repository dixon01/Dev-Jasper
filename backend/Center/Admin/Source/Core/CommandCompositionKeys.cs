// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandCompositionKeys.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommandCompositionKeys type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core
{
    /// <summary>
    /// The command composition keys.
    /// </summary>
    public static class CommandCompositionKeys
    {
        /// <summary>
        /// The shell keys.
        /// </summary>
        public static class Shell
        {
            /// <summary>
            /// The UI keys.
            /// </summary>
            public static class UI
            {
                /// <summary>
                /// The ShowFileMenu command.
                /// </summary>
                public const string ShowFileMenu = "Admin.Shell.UI.ShowFileMenu";

                /// <summary>
                /// The ShowAboutScreen command.
                /// </summary>
                public const string ShowAboutScreen = "Admin.Shell.UI.ShowAboutScreen";

                /// <summary>
                /// The ShowOptionsDialog command.
                /// </summary>
                public const string ShowOptionsDialog = "Admin.Shell.UI.ShowOptionsDialog";
            }

            /// <summary>
            /// The navigator keys.
            /// </summary>
            public static class Navigator
            {
                /// <summary>
                /// The go home command.
                /// </summary>
                public const string GoHome = "Admin.Shell.Navigator.GoHome";
            }

            /// <summary>
            /// The editor keys.
            /// </summary>
            public static class Editor
            {
                /// <summary>
                /// The save command.
                /// </summary>
                public const string Save = "Admin.Shell.Editor.Save";

                /// <summary>
                /// The create command.
                /// </summary>
                public const string Create = "Admin.Shell.Editor.Create";

                /// <summary>
                /// The cancel edit command.
                /// </summary>
                public const string CancelEdit = "Admin.Shell.Editor.CancelEdit";

                /// <summary>
                /// The update property display command.
                /// </summary>
                public const string UpdatePropertyDisplay = "Admin.Shell.Editor.UpdatePropertyDisplay";

                /// <summary>
                /// The commands for the software version editor.
                /// </summary>
                public static class PackageVersion
                {
                    /// <summary>
                    /// The add folder command.
                    /// </summary>
                    public const string AddFolder = "Admin.Shell.Editor.PackageVersion.AddFolder";

                    /// <summary>
                    /// The add file command.
                    /// </summary>
                    public const string AddFile = "Admin.Shell.Editor.PackageVersion.AddFile";

                    /// <summary>
                    /// The delete folder/file command.
                    /// </summary>
                    public const string DeleteItem = "Admin.Shell.Editor.PackageVersion.DeleteItem";
                }
            }

            /// <summary>
            /// The removable media keys.
            /// </summary>
            public static class RemovableMedia
            {
                /// <summary>
                /// The import feedback command.
                /// </summary>
                public const string ImportFeedback = "Admin.Shell.RemovableMedia.ImportFeedback";

                /// <summary>
                /// The export updates command.
                /// </summary>
                public const string ExportUpdates = "Admin.Shell.RemovableMedia.ExportUpdates";

                /// <summary>
                /// The cancel operation command.
                /// </summary>
                public const string CancelOperation = "Admin.Shell.RemovableMedia.CancelOperation";
            }

            /// <summary>
            /// The background system settings keys.
            /// </summary>
            public static class BackgroundSystemSettings
            {
                /// <summary>
                /// The toggle maintenance mode command.
                /// </summary>
                public const string ToggleMaintenanceMode =
                    "Admin.Shell.BackgroundSystemSettings.ToggleMaintenanceMode";

                /// <summary>
                /// The update azure provider command.
                /// </summary>
                public const string UpdateSettings = "Admin.Shell.BackgroundSystemSettings.UpdateSettings";
            }

            /// <summary>
            /// The widget keys.
            /// </summary>
            public static class Widgets
            {
                /// <summary>
                /// The command to navigate to a recently edited entity.
                /// </summary>
                public const string NavigateToRecentEntity = "Admin.Shell.Widgets.NavigateToRecentEntity";
            }
        }

        /// <summary>
        /// The entity keys.
        /// </summary>
        public static class Entities
        {
            /// <summary>
            /// The add entity command.
            /// </summary>
            public const string Add = "Admin.Entities.Add";

            /// <summary>
            /// The edit entity command.
            /// </summary>
            public const string Edit = "Admin.Entities.Edit";

            /// <summary>
            /// The copy entity command.
            /// </summary>
            public const string Copy = "Admin.Entities.Copy";

            /// <summary>
            /// The delete entity command.
            /// </summary>
            public const string Delete = "Admin.Entities.Delete";

            /// <summary>
            /// The filter column command.
            /// </summary>
            public const string FilterColumn = "Admin.Entities.FilterColumn";

            /// <summary>
            /// The update column visibility command.
            /// </summary>
            public const string UpdateColumnVisibility = "Admin.Entities.UpdateColumnVisibility";

            /// <summary>
            /// The navigate to entity command.
            /// </summary>
            public const string NavigateTo = "Admin.Entities.NavigateTo";

            /// <summary>
            /// The load entity details command.
            /// </summary>
            public const string LoadDetails = "Admin.Entities.LoadDetails";

            /// <summary>
            /// The unit configuration commands.
            /// </summary>
            public static class UnitConfiguration
            {
                /// <summary>
                /// The edit.
                /// </summary>
                // ReSharper disable once MemberHidesStaticFromOuterClass
                public const string Edit = "Admin.Entities.UnitConfiguration.Edit";
            }
        }

        /// <summary>
        /// The unit configurator keys.
        /// </summary>
        public static class UnitConfig
        {
            /// <summary>
            /// The navigate to part command.
            /// </summary>
            public const string NavigateToPart = "Admin.UnitConfig.NavigateToPart";

            /// <summary>
            /// The navigate to previous command.
            /// </summary>
            public const string NavigateToPrevious = "Admin.UnitConfig.NavigateToPrevious";

            /// <summary>
            /// The navigate to next command.
            /// </summary>
            public const string NavigateToNext = "Admin.UnitConfig.NavigateToNext";

            /// <summary>
            /// The commit command.
            /// </summary>
            public const string Commit = "Admin.UnitConfig.Commit";

            /// <summary>
            /// The commit and load.
            /// </summary>
            public const string CommitAndLoad = "Admin.UnitConfig.CommitAndLoad";

            /// <summary>
            /// The load version.
            /// </summary>
            public const string LoadVersion = "Admin.UnitConfig.LoadVersion";

            /// <summary>
            /// The cancel command.
            /// </summary>
            public const string Cancel = "Admin.UnitConfig.Cancel";

            /// <summary>
            /// The export command.
            /// </summary>
            public const string Export = "Admin.UnitConfig.Export";
        }
    }
}
