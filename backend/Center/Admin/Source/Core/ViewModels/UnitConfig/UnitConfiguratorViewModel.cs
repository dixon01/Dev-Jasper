// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfiguratorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfiguratorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.Collections;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The view model for the unit configurator window.
    /// </summary>
    public class UnitConfiguratorViewModel : WindowViewModelCloseStrategyBase, IDirty
    {
        private readonly ICommandRegistry commandRegistry;

        private readonly CollectionViewSource filteredCategoriesSource = new CollectionViewSource();

        private string title = "Document";

        private string documentName;

        private UnitConfigTreeNodeViewModelBase selectedItem;

        private bool isDirty;

        private bool isLoading;

        private bool isSaving;

        private bool isLatestVersion = true;

        private IObservableReadOnlyCollection<DocumentVersionReadableModel> documentVersions;

        private DocumentVersionReadableModel currentVersion;

        private bool restoreCurrentVersion;

        private DocumentVersionReadableModel previousCurrentVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitConfiguratorViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public UnitConfiguratorViewModel(ICommandRegistry commandRegistry)
            : base(new UnitConfiguratorWindowFactory())
        {
            this.commandRegistry = commandRegistry;

            this.Title = AdminStrings.UnitConfig_Title;
            this.Categories = new ObservableItemCollection<CategoryViewModel>();
            this.Categories.ItemPropertyChanged += this.CategoriesOnItemPropertyChanged;

            this.filteredCategoriesSource.Source = this.Categories;
            this.filteredCategoriesSource.Filter += this.FilteredCategoriesSourceOnFilter;

            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.SetProperty(ref this.title, value, () => this.Title);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current version is the latest version.
        /// </summary>
        public bool IsLatestVersion
        {
            get
            {
                return this.isLatestVersion;
            }

            set
            {
                this.SetProperty(ref this.isLatestVersion, value, () => this.IsLatestVersion);
            }
        }

        /// <summary>
        /// Gets or sets the document versions for the current configuration.
        /// </summary>
        public IObservableReadOnlyCollection<DocumentVersionReadableModel> DocumentVersions
        {
            get
            {
                return this.documentVersions;
            }

            set
            {
                this.SetProperty(ref this.documentVersions, value, () => this.DocumentVersions);
            }
        }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        public DocumentVersionReadableModel CurrentVersion
        {
            get
            {
                return this.currentVersion;
            }

            set
            {
                this.previousCurrentVersion = this.currentVersion;
                this.SetProperty(ref this.currentVersion, value, () => this.CurrentVersion);
            }
        }

        /// <summary>
        /// Gets or sets the Descriptor Name.
        /// </summary>
        public string DocumentName
        {
            get
            {
                return this.documentName;
            }

            set
            {
                this.SetProperty(ref this.documentName, value, () => this.DocumentName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changes, making it <c>dirty</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }

            private set
            {
                this.SetProperty(ref this.isDirty, value, () => this.IsDirty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the editor is loading data.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                if (this.SetProperty(ref this.isLoading, value, () => this.IsLoading))
                {
                    this.filteredCategoriesSource.View.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the editor is saving.
        /// </summary>
        public bool IsSaving
        {
            get
            {
                return this.isSaving;
            }

            set
            {
                this.SetProperty(ref this.isSaving, value, () => this.IsSaving);
            }
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        public ObservableItemCollection<CategoryViewModel> Categories { get; private set; }

        /// <summary>
        /// Gets the list of categories filtered by their visibility.
        /// Only categories are shown that contain filtered parts (i.e. "empty" categories are hidden).
        /// </summary>
        public ICollectionView FilteredCategories
        {
            get
            {
                return this.filteredCategoriesSource.View;
            }
        }

        /// <summary>
        /// Gets or sets the selected item (category or part).
        /// </summary>
        public UnitConfigTreeNodeViewModelBase SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                var previousPart = this.selectedItem as PartViewModelBase;
                if (!this.SetProperty(ref this.selectedItem, value, () => this.SelectedItem))
                {
                    return;
                }

                if (previousPart != null)
                {
                    previousPart.WasVisited = true;
                }

                var part = value as PartViewModelBase;
                if (part != null)
                {
                    part.Category.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// Gets the last visible tab. SelectedItem will not be available on loading.
        /// </summary>
        public UnitConfigTreeNodeViewModelBase LastVisibleTab { get; private set; }

        /// <summary>
        /// Gets the command to navigate to the previous node in the tree.
        /// </summary>
        public ICommand NavigateToPreviousCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToPrevious);
            }
        }

        /// <summary>
        /// Gets the command to navigate to the next node in the tree.
        /// </summary>
        public ICommand NavigateToNextCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.NavigateToNext);
            }
        }

        /// <summary>
        /// Gets the commit unit configuration changes.
        /// </summary>
        public ICommand CommitUnitConfigurationChanges
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.Commit);
            }
        }

        /// <summary>
        /// Gets the cancel unit configuration changes.
        /// </summary>
        public ICommand CancelUnitConfigurationChanges
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.Cancel);
            }
        }

        /// <summary>
        /// Gets the export unit configuration command.
        /// </summary>
        public ICommand ExportUnitConfiguration
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.Export);
            }
        }

        /// <summary>
        /// Gets the show check in dialog interaction request.
        /// </summary>
        public IInteractionRequest ShowCheckInDialogInteractionRequest
        {
            get
            {
                return InteractionManager<CheckInPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Brings this window to the front.
        /// </summary>
        public void BringToFront()
        {
            this.WindowView.Activate();
        }

        /// <summary>
        /// Sets the <see cref="IDirty.IsDirty"/> flag. The default behavior only sets the flag on the current object.
        /// </summary>
        public void MakeDirty()
        {
            this.IsDirty = true;
        }

        /// <summary>
        /// Clears the <see cref="IDirty.IsDirty"/> flag.
        /// The default behavior clears the flag on the current object and all its children.
        /// </summary>
        public void ClearDirty()
        {
            this.IsDirty = false;
            foreach (var category in this.Categories)
            {
                category.ClearDirty();
            }
        }

        /// <summary>
        /// Reverts the current selected version to the previous one without loading.
        /// </summary>
        /// <remarks>
        /// This method is needed to be able to get back to the dirty version if the user cancels the check-in.
        /// </remarks>
        internal void RevertCurrentVersion()
        {
            this.restoreCurrentVersion = true;
            this.CurrentVersion = this.previousCurrentVersion;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsLoading)
            {
                return;
            }

            if (e.PropertyName.Equals("CurrentVersion"))
            {
                if (this.restoreCurrentVersion)
                {
                    this.restoreCurrentVersion = false;
                    return;
                }

                // deleting config will trigger a change
                if (this.CurrentVersion == null)
                {
                    return;
                }

                var decision = this.UnitConfigurationSaveTrap();

                switch (decision)
                {
                    case SaveUserDecision.Cancel:
                        // restore previous selection
                        this.restoreCurrentVersion = true;
                        this.CurrentVersion = this.previousCurrentVersion;
                        break;

                    case SaveUserDecision.Save:
                        this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.CommitAndLoad)
                            .Execute(this.CurrentVersion);
                        break;

                    case SaveUserDecision.NoSaveRequired:
                    case SaveUserDecision.Discard:
                        this.commandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.LoadVersion)
                            .Execute(this.CurrentVersion);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (e.PropertyName.Equals("SelectedItem"))
            {
                if (this.SelectedItem != null)
                {
                    this.LastVisibleTab = this.SelectedItem;
                }
            }
        }

        private SaveUserDecision UnitConfigurationSaveTrap()
        {
            if (!this.IsDirty)
            {
                return SaveUserDecision.NoSaveRequired;
            }

            var message = string.Format(
                AdminStrings.UnitConfiguration_UnsavedConfiguration_Content,
                Environment.NewLine);

            var messageBoxResult = MessageBox.Show(
                message,
                AdminStrings.UnitConfiguration_UnsavedConfiguration_Title,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            switch (messageBoxResult)
            {
                case MessageBoxResult.Cancel:
                    return SaveUserDecision.Cancel;

                case MessageBoxResult.Yes:
                    return SaveUserDecision.Save;

                case MessageBoxResult.No:
                    return SaveUserDecision.Discard;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FilteredCategoriesSourceOnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = this.Filter(e.Item);
        }

        private bool Filter(object item)
        {
            var category = item as CategoryViewModel;
            return !this.isLoading && category != null && category.IsVisible;
        }

        private void CategoriesOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<CategoryViewModel> e)
        {
            switch (e.PropertyName)
            {
                case "IsVisible":
                    this.filteredCategoriesSource.View.Refresh();
                    break;
                case "IsDirty":
                    this.IsDirty = this.Categories.Any(c => c.IsDirty);
                    break;
            }
        }
    }
}
