// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileDialogInteraction.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SaveFileDialogInteraction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog
{
    /// <summary>
    /// Defines the interaction to save files.
    /// </summary>
    public class SaveFileDialogInteraction : FileDialogInteractionBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the overwrite prompt should be shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the overwrite prompt should be shown; otherwise, <c>false</c>.
        /// </value>
        public bool OverwritePrompt { get; set; }
    }
}