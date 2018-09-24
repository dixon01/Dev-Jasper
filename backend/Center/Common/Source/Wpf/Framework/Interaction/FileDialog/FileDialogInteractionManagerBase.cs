// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDialogInteractionManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileDialogInteractionManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    using Microsoft.Win32;

    /// <summary>
    /// Defines a base class for file dialog interaction managers.
    /// </summary>
    /// <typeparam name="T">The type of the prompt notification.</typeparam>
    public abstract class FileDialogInteractionManagerBase<T> : InteractionManager<T>
        where T : PromptNotification
    {
        private readonly Dictionary<DialogDirectoryType, string> lastUsedDirectoryPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDialogInteractionManagerBase{T}"/> class.
        /// </summary>
        /// <param name="lastUsedDirectoryPaths">
        /// The last used directory paths.
        /// </param>
        protected FileDialogInteractionManagerBase(Dictionary<DialogDirectoryType, string> lastUsedDirectoryPaths)
        {
            if (lastUsedDirectoryPaths == null)
            {
                this.lastUsedDirectoryPaths = new Dictionary<DialogDirectoryType, string>();
                return;
            }

            this.lastUsedDirectoryPaths = lastUsedDirectoryPaths;
        }

        /// <summary>
        /// Updates the last used directory.
        /// </summary>
        /// <param name="promptNotification">
        /// The prompt notification.
        /// </param>
        /// <param name="fileDialog">
        /// The file dialog.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The directory type of the <paramref name="promptNotification"/> is an unknown value.
        /// </exception>
        protected void UpdateLastUsedDirectory(
            FileDialogInteractionBase promptNotification, FileDialog fileDialog)
        {
            if (promptNotification.DirectoryType == null)
            {
                return;
            }

            this.lastUsedDirectoryPaths[promptNotification.DirectoryType] = Path.GetDirectoryName(fileDialog.FileName);
        }

        /// <summary>
        /// Gets the initial directory.
        /// </summary>
        /// <param name="promptNotification">
        /// The prompt notification.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The directory type of the <paramref name="promptNotification"/> is an unknown value.
        /// </exception>
        protected string GetInitialDirectory(FileDialogInteractionBase promptNotification)
        {
            if (promptNotification.DirectoryType == null)
            {
                return null;
            }

            string initialDirectory;
            this.lastUsedDirectoryPaths.TryGetValue(promptNotification.DirectoryType, out initialDirectory);
            return initialDirectory;
        }
    }
}