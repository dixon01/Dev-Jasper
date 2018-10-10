// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileDialogInteractionManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OpenFileDialogInteractionManager type.
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
    /// Defines the interaction manager for <see cref="OpenFileDialogInteraction"/>s.
    /// </summary>
    public class OpenFileDialogInteractionManager : FileDialogInteractionManagerBase<OpenFileDialogInteraction>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenFileDialogInteractionManager"/> class.
        /// </summary>
        /// <param name="lastUsedDirectoryPaths">
        /// The directory paths for the different initial directory locations. The default value is null.
        /// </param>
        public OpenFileDialogInteractionManager(
            Dictionary<DialogDirectoryType, string> lastUsedDirectoryPaths = null)
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
            OpenFileDialogInteraction promptNotification, Action<OpenFileDialogInteraction> callback = null)
        {
            var initialDirectory = this.GetInitialDirectory(promptNotification);

            var openFileDialog = new OpenFileDialog
                {
                    RestoreDirectory = true,
                    Title = promptNotification.Title,
                    DefaultExt = promptNotification.DefaultExtension,
                    AddExtension = true,
                    Multiselect = promptNotification.MultiSelect,
                    Filter = promptNotification.Filter,
                    InitialDirectory = initialDirectory
                };

            var result = openFileDialog.ShowDialog();
            if (callback == null)
            {
                return;
            }

            promptNotification.Confirmed = result.Value;
            if (promptNotification.MultiSelect)
            {
                promptNotification.FileNames = openFileDialog.FileNames;
            }

            promptNotification.FileName = openFileDialog.FileName;

            if (result.Value)
            {
                try
                {
                    this.UpdateLastUsedDirectory(promptNotification, openFileDialog);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    var message = string.Format(
                        "Error getting the directory from the path '{0}'.", openFileDialog.FileName);
                    Logger.Error(exception, message);
                }
            }

            callback(promptNotification);
        }
    }
}