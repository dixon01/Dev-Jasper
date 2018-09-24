// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Diag.Core.Controllers.App;
    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels.FileSystem;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Medi.FileSystem;
    using Gorba.Common.Utility.Core;

    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// Base class for all unit controllers.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of view model this unit controller uses.
    /// </typeparam>
    internal abstract class UnitControllerBase<TViewModel> : SynchronizableControllerBase, IUnitController
        where TViewModel : UnitViewModelBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private CancellationTokenSource downloadCancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitControllerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        protected UnitControllerBase(TViewModel viewModel)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
            this.ViewModel = viewModel;
            this.ApplicationControllers = new List<RemoteAppController>();
        }

        /// <summary>
        /// Gets the view model for the unit controlled by this controller.
        /// </summary>
        public TViewModel ViewModel { get; private set; }

        UnitViewModelBase IUnitController.ViewModel
        {
            get
            {
                return this.ViewModel;
            }
        }

        /// <summary>
        /// Gets the list of application controllers responsible for applications of this unit.
        /// </summary>
        public List<RemoteAppController> ApplicationControllers { get; private set; }

        /// <summary>
        /// Connects to the unit.
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// Disconnects from the unit.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Checks if the given file can be opened (i.e. there is an application assigned to that extension).
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// True if the file can be opened.
        /// </returns>
        public bool CanOpenRemoteFile(FileViewModel file)
        {
            if (file == null || file.Name == null
                || file.Name.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            try
            {
                var ext = this.GetExtension(file.Name);
                if (string.IsNullOrEmpty(ext))
                {
                    return false;
                }

                ext = ext.ToLower();

                string programId;
                using (var extKey = Registry.ClassesRoot.OpenSubKey(ext))
                {
                    if (extKey == null)
                    {
                        return false;
                    }

                    programId = extKey.GetValue(null).ToString();
                }

                using (var commandKey = Registry.ClassesRoot.OpenSubKey(programId + @"\shell\open\command"))
                {
                    return commandKey != null;
                }
            }
            catch (Exception ex)
            {
                this.Logger.WarnException("Couldn't find shell application for " + file.Name, ex);
                return true;
            }
        }

        /// <summary>
        /// Downloads and then opens a remote file with the default application.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public async void OpenRemoteFile(FileViewModel file)
        {
            this.SetDownloading();
            try
            {
                await this.OpenRemoteFileAsync(file, this.downloadCancellationToken.Token);
            }
            catch (Exception ex)
            {
                this.Logger.WarnException("Couldn't open file " + file.Name, ex);

                MessageBox.Show(
                    string.Format(
                        DiagStrings.FileSystemView_DownloadFile_Error,
                        file.Name,
                        Environment.NewLine,
                        ex.Message),
                    DiagStrings.FileSystemView_DownloadFile_DialogTitle);
            }

            this.ClearDownloading();
        }

        /// <summary>
        /// Downloads a remote file, asking the user for the place to save.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        public void DownloadRemoteFile(FileViewModel file)
        {
            Action<SaveFileDialogInteraction> onSave = async interaction =>
            {
                if (!interaction.Confirmed)
                {
                    return;
                }

                this.SetDownloading();
                try
                {
                    await
                        this.DownloadRemoteFileAsync(
                            file,
                            interaction.FileName,
                            this.downloadCancellationToken.Token);
                }
                catch (Exception ex)
                {
                    this.Logger.WarnException("Couldn't download file " + file.Name, ex);

                    this.ClearDownloading();
                    MessageBox.Show(
                        string.Format(
                            DiagStrings.FileSystemView_DownloadFile_Error,
                            file.Name,
                            Environment.NewLine,
                            ex.Message),
                        DiagStrings.FileSystemView_DownloadFile_DialogTitle);
                    return;
                }

                this.ClearDownloading();
                MessageBox.Show(
                    string.Format(DiagStrings.FileSystemView_DownloadFile_Success, interaction.FileName),
                    DiagStrings.FileSystemView_DownloadFile_DialogTitle);
            };

            var saveDialogInteraction = new SaveFileDialogInteraction
            {
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                Title = DiagStrings.FileSystemView_DownloadFile_DialogTitle,
                FileName = file.Name
                ////DirectoryType = DialogDirectoryTypes.Project
            };

            var extension = this.GetExtension(file.Name);
            if (string.IsNullOrEmpty(extension))
            {
                saveDialogInteraction.DefaultExtension = string.Empty;
                saveDialogInteraction.Filter = "Any File|*.*";
            }
            else
            {
                saveDialogInteraction.DefaultExtension = extension.TrimStart('.');
                saveDialogInteraction.Filter = string.Format(
                    "{0} File|*{1}",
                    saveDialogInteraction.DefaultExtension.ToUpper(),
                    extension);
            }

            InteractionManager<SaveFileDialogInteraction>.Current.Raise(saveDialogInteraction, onSave);
        }

        /// <summary>
        /// Cancels a file download, started either through <see cref="IUnitController.OpenRemoteFile"/>
        /// or <see cref="IUnitController.DownloadRemoteFile"/>.
        /// </summary>
        public void CancelRemoteFileDownload()
        {
            this.downloadCancellationToken.Cancel();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Disconnect();
        }

        private async Task OpenRemoteFileAsync(FileViewModel file, CancellationToken token)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var tempFile = Path.Combine(tempDir, file.Name);
            await this.DownloadRemoteFileAsync(file, tempFile, token);
            var fileAttributes = File.GetAttributes(tempFile);
            File.SetAttributes(tempFile, fileAttributes | FileAttributes.ReadOnly);
            var process = new Process { StartInfo = new ProcessStartInfo(tempFile), EnableRaisingEvents = true };
            process.Exited += (s, e) =>
                {
                    File.SetAttributes(tempFile, fileAttributes);
                    File.Delete(tempFile);
                };
            process.Start();
        }

        private string GetExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext))
            {
                return null;
            }

            if (!ext.StartsWith("."))
            {
                ext = "." + ext;
            }

            return ext;
        }

        private async Task DownloadRemoteFileAsync(FileViewModel file, string fileName, CancellationToken token)
        {
            var fileSystem = file.File.FileSystem as RemoteFileSystem;
            if (fileSystem == null)
            {
                throw new ArgumentException("File isn't a remote file");
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var tcs = new TaskCompletionSource<object>();
            token.Register(() => tcs.TrySetCanceled());
            fileSystem.BeginDownloadFile(
                file.File,
                fileName,
                ar =>
                {
                    try
                    {
                        fileSystem.EndDownloadFile(ar);
                        tcs.SetResult(ar.AsyncState);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                },
                null);
            await tcs.Task;
        }

        private void SetDownloading()
        {
            this.downloadCancellationToken = new CancellationTokenSource();
            this.ViewModel.FileSystemIsDownloading = true;
        }

        private void ClearDownloading()
        {
            this.ViewModel.FileSystemIsDownloading = false;
            this.downloadCancellationToken = null;
        }
    }
}