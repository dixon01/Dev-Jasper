// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileDialogInteraction.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OpenFileDialogInteraction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog
{
    /// <summary>
    /// Defines the interaction dialog to open files.
    /// </summary>
    public class OpenFileDialogInteraction : FileDialogInteractionBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the user can select multiple files.
        /// </summary>
        public bool MultiSelect { get; set; }
    }
}