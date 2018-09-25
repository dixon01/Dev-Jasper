// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileDialogInteractionManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SaveFileDialogInteractionManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog
{
    using System;
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework.Interaction;

    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// Defines the interaction manager for <see cref="SaveFileDialogInteraction"/>s.
    /// </summary>
    public class SaveFileDialogInteractionManager : FileDialogInteractionManagerBase<SaveFileDialogInteraction>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveFileDialogInteractionManager"/> class.
        /// </summary>
        /// <param name="lastUsedDirectoryPaths">
        /// The directory paths.
        /// </param>
        public SaveFileDialogInteractionManager(Dictionary<DialogDirectoryType, string> lastUsedDirectoryPaths = null)
            : base(lastUsedDirectoryPaths)
        {
        }

        /// <summary>
        /// Gets the or create interaction request.
        /// </summary>
        /// <returns>The interaction request.</returns>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public override IInteractionRequest GetOrCreateInteractionRequest()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Raises the specified prompt notification.
        /// </summary>
        /// <param name="promptNotification">The prompt notification.</param>
        /// <param name="callback">The callback.</param>
        public override void Raise(
            SaveFileDialogInteraction promptNotification, Action<SaveFileDialogInteraction> callback = null)
        {
            var initialDirectory = this.GetInitialDirectory(promptNotification);

            var saveFileDialog = new SaveFileDialog
                {
                    OverwritePrompt = promptNotification.OverwritePrompt,
                    RestoreDirectory = true,
                    Title = promptNotification.Title,
                    DefaultExt = promptNotification.DefaultExtension,
                    AddExtension = true,
                    Filter = promptNotification.Filter,
                    InitialDirectory = initialDirectory,
                };

            if (!string.IsNullOrWhiteSpace(promptNotification.FileName))
            {
                saveFileDialog.FileName = promptNotification.FileName;
            }

            var result = saveFileDialog.ShowDialog();
            if (callback == null)
            {
                return;
            }

            promptNotification.Confirmed = result.Value;
            promptNotification.FileName = saveFileDialog.FileName;
            if (result.Value)
            {
                try
                {
                    this.UpdateLastUsedDirectory(promptNotification, saveFileDialog);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    var message = string.Format(
                        "Error getting the directory from the path '{0}'.", saveFileDialog.FileName);
                    Logger.Error(exception, message);
                }
            }

            callback(promptNotification);
        }
    }
}