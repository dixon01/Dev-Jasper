// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationActionPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationActionPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Collections;

    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

    /// <summary>
    /// <see cref="PartViewModelBase"/> that contains one <see cref="NamedListEditorViewModel"/>.
    /// </summary>
    public class InstallationActionPartViewModel : PartViewModelBase
    {
        private readonly ICollection<ErrorItem> errors;

        private InstallationActionViewModelBase selectedActionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationActionPartViewModel"/> class.
        /// </summary>
        public InstallationActionPartViewModel()
        {
            this.Actions = new ObservableItemCollection<InstallationActionViewModelBase>();
            this.Files = new ObservableCollection<InstallationActionAdditionalFileViewModel>();

            this.AddLocalExecutableCommand = new RelayCommand(this.AddLocalExecutable);
            this.AddUnitExecutableCommand = new RelayCommand(this.AddUnitExecutable);
            this.DeleteInstallationActionCommand =
                new RelayCommand<InstallationActionViewModelBase>(this.DeleteInstallationAction);

            this.AddFileCommand = new RelayCommand(this.AddFile);
            this.RemoveAdditionalFileCommand =
                new RelayCommand<InstallationActionAdditionalFileViewModel>(this.RemoveFile);

            this.errors = new Collection<ErrorItem>();

            this.Actions.ItemPropertyChanged += this.ActionChanged;
            this.Actions.CollectionChanged += this.ActionsChanged;
        }

        /// <summary>
        /// Gets or sets the add local executable command.
        /// </summary>
        public ICommand AddLocalExecutableCommand { get; set; }

        /// <summary>
        /// Gets or sets the add unit executable command.
        /// </summary>
        public ICommand AddUnitExecutableCommand { get; set; }

        /// <summary>
        /// Gets or sets the add file command.
        /// </summary>
        public ICommand AddFileCommand { get; set; }

        /// <summary>
        /// Gets or sets the delete installation action command.
        /// </summary>
        public ICommand DeleteInstallationActionCommand { get; set; }

        /// <summary>
        /// Gets the remove additional file command.
        /// </summary>
        public ICommand RemoveAdditionalFileCommand { get; private set; }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        public ObservableItemCollection<InstallationActionViewModelBase> Actions { get; private set; }

        /// <summary>
        /// Gets the files.
        /// </summary>
        public ObservableCollection<InstallationActionAdditionalFileViewModel> Files { get; private set; }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public override ICollection<ErrorItem> Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Gets or sets the selected action.
        /// </summary>
        public InstallationActionViewModelBase SelectedActionViewModel
        {
            get
            {
                return this.selectedActionViewModel;
            }

            set
            {
                this.SetProperty(ref this.selectedActionViewModel, value, () => this.SelectedActionViewModel);
            }
        }

        /// <summary>
        /// The has errors.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasErrors()
        {
            return this.errors.Count > 0;
        }

        private void ActionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ValidateActions();
        }

        private void ActionChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ValidateActions();
        }

        private void ValidateActions()
        {
            this.errors.Clear();

            foreach (var action in this.Actions)
            {
                var unitAction = action as UnitExecutableInstallationActionViewModel;
                if (unitAction == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(unitAction.ExecutablePathBase))
                {
                    this.errors.Add(
                        new ErrorItem(
                            ErrorState.Error,
                            AdminStrings.Editors_InstallationAction_Error_EmptyUnitExecutable));
                    break;
                }
            }

            var errorState = ErrorState.Ok;
            if (this.Errors.Count > 0)
            {
                errorState = ErrorState.Error;
            }

            this.SetErrorState(errorState);
            this.RaisePropertyChanged(() => this.Errors);
        }

        private void RemoveFile(InstallationActionAdditionalFileViewModel file)
        {
            this.Files.Remove(file);
        }

        private void DeleteInstallationAction(InstallationActionViewModelBase item)
        {
            if (item == null)
            {
                return;
            }

            this.Actions.Remove(item);
        }

        private void AddUnitExecutable()
        {
            var newAction = new UnitExecutableInstallationActionViewModel();
            this.Actions.Add(newAction);
            this.SelectedActionViewModel = newAction;
        }

        private void AddLocalExecutable()
        {
            var openDialog = new OpenFileDialog
                                 {
                                     InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                                     Filter = AdminStrings.Editors_InstallationAction_SelectFileDialog_ExecutableFilter,
                                     FilterIndex = 0,
                                     RestoreDirectory = true
                                 };

            if (openDialog.ShowDialog() == true)
            {
                var newAction = new LocalExecutableInstallationActionViewModel
                                {
                                    ExecutablePathBase = openDialog.FileName
                                };
                this.Actions.Add(newAction);
                this.SelectedActionViewModel = newAction;
            }
        }

        private void AddFile()
        {
            var openDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Filter = AdminStrings.Editors_InstallationAction_SelectFileDialog_FileFilter,
                FilterIndex = 0,
                Multiselect = true,
                RestoreDirectory = true
            };

            if (openDialog.ShowDialog() != true)
            {
                return;
            }

            var duplicates = new List<string>();
            foreach (var fileName in openDialog.FileNames)
            {
                if (this.Files.Any(s => s.FileName.Equals(fileName)))
                {
                    duplicates.Add(fileName);
                    continue;
                }

                var viewModel = new InstallationActionAdditionalFileViewModel { FileName = fileName };
                this.Files.Add(viewModel);
            }

            if (duplicates.Count > 0)
            {
                var duplicateInfo = string.Join(Environment.NewLine, duplicates.ToArray());
                var info = string.Format(
                                  AdminStrings.Editors_InstallationAction_FileAlreadyPressent,
                                  duplicates.Count > 1 ? "s" : string.Empty,
                                  Environment.NewLine,
                                  duplicateInfo);

                    MessageBox.Show(
                        info,
                        AdminStrings.Editors_InstallationAction_FileAlreadyPressentTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }
    }
}
