// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.Views;
    using Gorba.Center.Media.Core.Views.Editors;

    /// <summary>
    /// The text replacement controller.
    /// </summary>
    public class CsvMappingController : ICsvMappingController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvMappingController"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media Shell.
        /// </param>
        /// <param name="parentController">
        /// The parent Controller.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public CsvMappingController(
            IMediaShell mediaShell,
            IMediaShellController parentController,
            ICommandRegistry commandRegistry)
        {
            this.MediaShell = mediaShell;
            this.ParentController = parentController;
            this.CommandRegistry = commandRegistry;

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.CsvMapping.CreateCsv,
                new RelayCommand(this.CreateCsv, this.CanCreateCsv));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.CsvMapping.ImportCsv,
                new RelayCommand(this.ImportCsv, this.CanImportCsv));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.CsvMapping.DeleteCsv,
                new RelayCommand<CsvMappingDataViewModel>(this.DeleteCsv, this.CanDeleteCsv));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.CsvMapping.EditCsv,
                new RelayCommand<CsvMappingDataViewModel>(this.EditCsv, this.CanEditCsv));
        }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; private set; }

        /// <summary>
        /// Gets the media shell.
        /// </summary>
        /// <value>
        /// The media shell.
        /// </value>
        public IMediaShell MediaShell { get; private set; }

        private bool CanDeleteCsv(CsvMappingDataViewModel obj)
        {
            return this.HasWritePermission();
        }

        private bool CanImportCsv(object obj)
        {
            return this.HasWritePermission();
        }

        private bool CanCreateCsv(object obj)
        {
            return this.HasWritePermission();
        }

        private bool HasWritePermission()
        {
            return this.MediaShell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration);
        }

        private void CreateCsv()
        {
            var csv = new CsvMappingDataViewModel(this.MediaShell, this.CommandRegistry);

            var filename = MediaStrings.CsvMappingController_CsvMappingDefaultFilename;
            var isUnique = this.IsFilenameWithoutExtensionUnique(filename);

            if (!isUnique)
            {
                filename = this.CreateUniqueCsvFilenameWithoutExtension(filename);
            }

            csv.Filename.Value = filename;
            csv.RawContent.Value = MediaStrings.CsvMappingController_CsvMappingDefaultHeader;

            var historyEntry = new CreateCsvMappingHistoryEntry(
                csv,
                this.MediaShell.MediaApplicationState.CurrentProject,
                MediaStrings.ResourceManagerView_AddCsv);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void ImportCsv()
        {
            var filter = MediaStrings.OpenFileDialog_CsvFilter;
            var directoryType = DialogDirectoryTypes.Project;
            var title = MediaStrings.ResourceManagerView_ImportCsv;

            Action<OpenFileDialogInteraction> onFinished = interaction =>
                {
                    if (!interaction.Confirmed)
                    {
                        return;
                    }

                    foreach (var fileName in interaction.FileNames)
                    {
                        var uniqueFileName = Path.GetFileNameWithoutExtension(fileName);
                        var isUnique = this.IsFilenameWithoutExtensionUnique(uniqueFileName);

                        if (!isUnique)
                        {
                            uniqueFileName = this.CreateUniqueCsvFilenameWithoutExtension(uniqueFileName);
                        }

                        try
                        {
                            var newMapping = new CsvMappingDataViewModel(this.MediaShell, this.CommandRegistry);
                            newMapping.Filename.Value = Path.GetFileNameWithoutExtension(uniqueFileName);
                            newMapping.RawContent.Value = File.ReadAllText(fileName, Encoding.Default);
                            var historyEntry = new CreateCsvMappingHistoryEntry(
                            newMapping,
                            this.MediaShell.MediaApplicationState.CurrentProject,
                            MediaStrings.ResourceManagerView_AddCsv);
                            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
                        }
                        catch (IOException e)
                        {
                            var message = new ConnectionExceptionPrompt(e, e.Message, "File error");
                            InteractionManager<ConnectionExceptionPrompt>.Current.Raise(message);
                            return;
                        }
                    }
                };

            var openDialogInteraction = new OpenFileDialogInteraction
            {
                Filter = filter,
                Title = title,
                DirectoryType = directoryType,
                MultiSelect = true
            };
            InteractionManager<OpenFileDialogInteraction>.Current.Raise(openDialogInteraction, onFinished);
        }

        private void DeleteCsv(CsvMappingDataViewModel mapping)
        {
            var historyEntry = new DeleteCsvMappingHistoryEntry(
                mapping,
                this.MediaShell.MediaApplicationState.CurrentProject,
                MediaStrings.ResourceManagerView_RemoveCsv);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void EditCsv(CsvMappingDataViewModel mappings)
        {
            var editorViewModel = new CsvEditorViewModel(this.MediaShell, mappings);
            var ownerWindow = Application.Current.Windows.OfType<MediaShellWindow>().FirstOrDefault();

            var window = new CsvEditor
                             {
                                 DataContext = editorViewModel,
                                 Width = 600,
                                 Height = 500,
                                 Owner = ownerWindow,
                                 WindowStartupLocation = WindowStartupLocation.CenterOwner,
                             };
            window.ShowDialog();
        }

        private bool CanEditCsv(CsvMappingDataViewModel mappings)
        {
            return mappings != null;
        }

        private string CreateUniqueCsvFilenameWithoutExtension(string filename)
        {
            var index = 1;
            var newFileName = filename + string.Format(Settings.Default.DuplicatedMediaPostfix, index);

            while (!this.IsFilenameWithoutExtensionUnique(newFileName))
            {
                index++;
                newFileName = filename + string.Format(Settings.Default.DuplicatedMediaPostfix, index);
            }

            return newFileName;
        }

        private bool IsFilenameWithoutExtensionUnique(string name)
        {
            if (this.MediaShell.MediaApplicationState.CurrentProject.CsvMappings
                .Any(c => string.Equals(c.Filename.Value, name, StringComparison.InvariantCultureIgnoreCase))
                || name.Equals("codeconversion", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}