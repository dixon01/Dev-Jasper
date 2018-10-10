// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuNavigationParameters.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The menu navigation parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    /// <summary>
    /// The menu navigation parameters.
    /// </summary>
    public class MenuNavigationParameters
    {
        /// <summary>
        /// The main menu entries.
        /// </summary>
        public enum MainMenuEntries
        {
            /// <summary>
            /// The default.
            /// </summary>
            None,

            /// <summary>
            /// The file new.
            /// </summary>
            FileNew,

            /// <summary>
            /// The file open.
            /// </summary>
            FileOpen,

            /// <summary>
            /// The file import.
            /// </summary>
            FileImport,

            /// <summary>
            /// The file export.
            /// </summary>
            FileExport,

            /// <summary>
            /// The file resource manager.
            /// </summary>
            FileResourceManager,

            /// <summary>
            /// The file replacement.
            /// </summary>
            FileReplacement,

            /// <summary>
            /// The file formula manager.
            /// </summary>
            FileFormulaManager,

            /// <summary>
            /// The file save as.
            /// </summary>
            FileSaveAs
        }

        /// <summary>
        /// Gets or sets the root tab.
        /// </summary>
        public MainMenuEntries Root { get; set; }

        /// <summary>
        /// Gets or sets the sub menu.
        /// </summary>
        public string SubMenu { get; set; }
    }
}
