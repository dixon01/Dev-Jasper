// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDialogInteractionBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileDialogInteractionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// Defines the base class for <see cref="PromptNotification"/>s related to files.
    /// </summary>
    public abstract class FileDialogInteractionBase : PromptNotification
    {
        /// <summary>
        /// Gets or sets a value indicating whether the file extension should be automatically added.
        /// </summary>
        /// <value>
        ///   <c>true</c> if file extension should be automatically added; otherwise, <c>false</c>.
        /// </value>
        public bool AddExtension { get; set; }

        /// <summary>
        /// Gets or sets the default extension.
        /// </summary>
        /// <value>
        /// The default extension.
        /// </value>
        public string DefaultExtension { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the selected file names.
        /// </summary>
        public IEnumerable<string> FileNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the last directory location should be restored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the last directory location should be restored; otherwise, <c>false</c>.
        /// </value>
        public bool RestoreDirectory { get; set; }

        /// <summary>
        /// Gets or sets the dialog directory type.
        /// </summary>
        public DialogDirectoryType DirectoryType { get; set; }
    }
}