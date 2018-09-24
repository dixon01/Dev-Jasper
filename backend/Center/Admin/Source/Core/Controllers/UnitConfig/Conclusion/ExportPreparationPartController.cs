// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportPreparationPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ExportPreparationPartController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The ExportPreparationPartController.
    /// </summary>
    public class ExportPreparationPartController : PartControllerBase<ExportPreparationPartViewModel>
    {
        private UnitConfigData currentExportBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportPreparationPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public ExportPreparationPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Conclusion.ExportPreparation, parent)
        {
        }

        private ExportPreparationEditorViewModel Editor
        {
            get
            {
                return this.ViewModel.Editor;
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.Parent.Parent.ViewModel.PropertyChanged -= this.ViewModelOnPropertyChanged;
            this.Parent.Parent.ViewModel.Categories.ItemPropertyChanged -= this.CategoriesOnItemPropertyChanged;

            this.Parent.Parent.ViewModel.PropertyChanged += this.ViewModelOnPropertyChanged;
            this.Parent.Parent.ViewModel.Categories.ItemPropertyChanged += this.CategoriesOnItemPropertyChanged;
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="ExportPreparationPartViewModel"/>.
        /// </returns>
        protected override ExportPreparationPartViewModel CreateViewModel()
        {
            var viewModel = new ExportPreparationPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Conclusion_ExportPreparation;
            viewModel.Description = AdminStrings.UnitConfig_Conclusion_ExportPreparation_Description;
            viewModel.IsVisible = true;
            viewModel.Editor.ReloadCommand = new RelayCommand(this.ReloadExportTree);
            viewModel.Editor.CancelReloadCommand = new RelayCommand(() => this.Editor.ShouldReload = false);

            viewModel.Editor.PropertyChanged += (s, e) => this.RaiseViewModelUpdated(e);

            return viewModel;
        }

        private static int SortFolderItems(ExportItemBase x, ExportItemBase y)
        {
            var isFolderX = x is ExportFolder;
            var isFolderY = y is ExportFolder;
            if (isFolderX == isFolderY)
            {
                return string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
            }

            return isFolderX ? -1 : 1;
        }

        private void ReloadExportTree()
        {
            this.currentExportBase = this.Parent.Parent.CreateUnitConfigData();
            this.Editor.IsLoading = true;
            this.Editor.ShouldReload = false;
            this.Editor.ExportFolders.Clear();
            Task.Run(() => this.GenerateExportTree());
        }

        private async void GenerateExportTree()
        {
            try
            {
                var rootFolders = await this.Parent.Parent.CreateExportStructureAsync();
                rootFolders.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase));
                rootFolders.ForEach(this.SortFolder);
                this.StartNew(() => this.SetExportTree(rootFolders));
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't create export folder structure");
                var message = string.Format(
                    AdminStrings.UnitConfig_Conclusion_ExportPreparation_ErrorFormat, ex.Message);
                MessageBox.Show(
                    message,
                    AdminStrings.UnitConfig_Conclusion_ExportPreparation_ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SortFolder(ExportFolder folder)
        {
            var children = folder.Children.ToList();
            children.Sort(SortFolderItems);
            children.ForEach(c => folder.Children.Remove(c));
            children.ForEach(folder.Children.Add);

            foreach (var subFolder in children.OfType<ExportFolder>())
            {
                this.SortFolder(subFolder);
            }
        }

        private void SetExportTree(IEnumerable<ExportFolder> rootFolders)
        {
            foreach (var folder in rootFolders)
            {
                this.Editor.ExportFolders.Add(folder);
            }

            this.Editor.IsLoading = false;
        }

        private void CheckErrors()
        {
            this.Editor.ShouldFixCategoryErrors =
                this.Parent.Parent.ViewModel.Categories.Where(c => c != this.Parent.ViewModel)
                    .Any(category => category.ErrorState > ErrorState.Warning);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedItem")
            {
                return;
            }

            if (this.Parent.Parent.ViewModel.SelectedItem != this.ViewModel)
            {
                return;
            }

            if (this.Editor.IsLoading)
            {
                return;
            }

            if (this.Editor.ShouldFixCategoryErrors)
            {
                return;
            }

            var exportBase = this.Parent.Parent.CreateUnitConfigData();
            if (this.currentExportBase != null)
            {
                // data was already loaded, the user can choose if he wants to reload the data
                this.Editor.ShouldReload = !this.currentExportBase.Equals(exportBase);
                return;
            }

            this.ReloadExportTree();
        }

        private void CategoriesOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<CategoryViewModel> e)
        {
            if (e.PropertyName != "ErrorState" && e.PropertyName != "IsVisible")
            {
                return;
            }

            if (e.Item == this.Parent.ViewModel)
            {
                // don't check if we are changing ourselves
                return;
            }

            this.CheckErrors();
        }
    }
}