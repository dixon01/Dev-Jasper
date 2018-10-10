// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportPreparationEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportPreparationEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.Wpf.Core.Collections;

    /// <summary>
    /// The view model for the export editor.
    /// </summary>
    public class ExportPreparationEditorViewModel : DataErrorViewModelBase
    {
        private List<ErrorItem> folderErrors;

        private bool isLoading;

        private bool shouldReload;

        private bool shouldFixCategoryErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportPreparationEditorViewModel"/> class.
        /// </summary>
        public ExportPreparationEditorViewModel()
        {
            this.ExportFolders = new ObservableItemCollection<ExportFolder>();
            this.ExportFolders.CollectionChanged += this.ExportFoldersOnCollectionChanged;
            this.ExportFolders.ItemPropertyChanged += this.ExportFoldersOnItemPropertyChanged;
        }

        /// <summary>
        /// Gets the folders.
        /// </summary>
        public ObservableItemCollection<ExportFolder> ExportFolders { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this part is being loaded.
        /// Loading means that the contents of the export is being calculated and created.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.SetProperty(ref this.isLoading, value, () => this.IsLoading);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user should reload the configuration
        /// since it has been changed.
        /// </summary>
        public bool ShouldReload
        {
            get
            {
                return this.shouldReload;
            }

            set
            {
                this.SetProperty(ref this.shouldReload, value, () => this.ShouldReload);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user should fix first errors in other categories.
        /// </summary>
        public bool ShouldFixCategoryErrors
        {
            get
            {
                return this.shouldFixCategoryErrors;
            }

            set
            {
                if (this.SetProperty(ref this.shouldFixCategoryErrors, value, () => this.ShouldFixCategoryErrors))
                {
                    this.SetError(
                        string.Empty,
                        value ? ErrorState.Error : ErrorState.Ok,
                        AdminStrings.Errors_NoExportDueToError);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether any <see cref="ExportFolders"/> has changes.
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return this.ExportFolders.Any(f => f.HasChanges);
            }
        }

        /// <summary>
        /// Gets or sets the reload configuration command. This command is set by the controller.
        /// </summary>
        public ICommand ReloadCommand { get; set; }

        /// <summary>
        /// Gets or sets the cancel reload command. This command is set by the controller.
        /// </summary>
        public ICommand CancelReloadCommand { get; set; }

        /// <summary>
        /// Clears the <see cref="HasChanges"/> flag of this item and all export folders.
        /// </summary>
        public void ClearHasChanges()
        {
            this.ExportFolders.ForEach(f => f.ClearHasChanges());
        }

        private void UpdateErrors()
        {
            var hasRemoved = false;
            if (this.folderErrors != null && this.folderErrors.Count > 0)
            {
                hasRemoved = true;
                this.folderErrors.ForEach(e => this.RemoveError("ExportFolders", e));
            }

            this.folderErrors =
                this.ExportFolders.SelectMany(f => f.GetErrorMessages(null)).ToList();
            this.folderErrors.ForEach(e => this.SetError("ExportFolders", e));

            if (hasRemoved || this.folderErrors.Count > 0)
            {
                this.RaiseErrorsChanged("ExportFolders");
            }
        }

        private void ExportFoldersOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<ExportFolder> e)
        {
            switch (e.PropertyName)
            {
                case "HasErrors":
                    this.RaisePropertyChanged(() => this.HasErrors);
                    break;
                case "HasChanges":
                    this.RaisePropertyChanged(() => this.HasChanges);
                    break;
            }
        }

        private void ExportFoldersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ExportItemBase child in e.NewItems)
                {
                    child.ErrorsChanged += this.FolderOnErrorsChanged;
                    if (child.HasErrors)
                    {
                        this.UpdateErrors();
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ExportItemBase child in e.OldItems)
                {
                    child.ErrorsChanged -= this.FolderOnErrorsChanged;
                    if (child.HasErrors)
                    {
                        this.UpdateErrors();
                    }
                }
            }
        }

        private void FolderOnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.UpdateErrors();
        }
    }
}
