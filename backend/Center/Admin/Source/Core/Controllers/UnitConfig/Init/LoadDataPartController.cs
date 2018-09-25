// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadDataPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoadDataPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Init
{
    using System;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Init;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Common.Configuration.Core;

    /// <summary>
    /// The part controller for loading the initial data.
    /// </summary>
    internal class LoadDataPartController : PartControllerBase<LoadDataPartViewModel>
    {
        /// <summary>
        /// The key which should be set to true if no data is stored in a Unit Configuration.
        /// </summary>
        public const string HasNoDataKey = "HasNoData";

        private UnitConfigContainer importedContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadDataPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public LoadDataPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Initialization.LoadData, parent)
        {
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.ViewModel.IsVisible = partData.GetValue(false, HasNoDataKey);
            this.UpdateErrors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(false, HasNoDataKey);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="LoadDataPartViewModel"/>.
        /// </returns>
        protected override LoadDataPartViewModel CreateViewModel()
        {
            var viewModel = new LoadDataPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Init_LoadData;
            viewModel.Description = AdminStrings.UnitConfig_Init_LoadData_Description;

            var currentId = this.Parent.Parent.UnitConfiguration.Id;
            viewModel.Editor.IsCreateEmptySelected = true;
            viewModel.Editor.CopyConfiguration.Objects =
                this.Parent.Parent.DataController.UnitConfiguration.All.Where(c => c.Id != currentId).ToArray();

            viewModel.Editor.ChooseImportFileCommand = new RelayCommand(
                this.ChooseImportFile,
                this.CanChooseImportFile);
            viewModel.Editor.LoadCommand = new RelayCommand(this.LoadData, this.CanLoadData);

            viewModel.Editor.PropertyChanged += (s, e) => this.UpdateErrors();
            viewModel.Editor.CopyConfiguration.PropertyChanged += (s, e) => this.UpdateErrors();

            return viewModel;
        }

        private void UpdateErrors()
        {
            var productType = this.Parent.Parent.UnitConfiguration.ProductType.Name;
            var wrongProductTypeMessage = string.Format(
                AdminStrings.UnitConfig_Init_LoadData_WrongProductTypeFormat,
                productType);

            var editor = this.ViewModel.Editor;
            var errorState = editor.IsCopySelected && editor.CopyConfiguration.SelectedObject == null
                                 ? ErrorState.Missing
                                 : ErrorState.Ok;
            editor.SetError("CopyConfiguration", errorState, AdminStrings.Errors_NoItemSelected);

            errorState = editor.IsCopySelected && editor.CopyConfiguration.SelectedObject != null
                         && editor.CopyConfiguration.SelectedObject.ProductType.Name != productType
                             ? ErrorState.Warning
                             : ErrorState.Ok;
            editor.SetError("CopyConfiguration", errorState, wrongProductTypeMessage);

            errorState = editor.IsImportSelected && string.IsNullOrEmpty(editor.ImportFileName)
                             ? ErrorState.Missing
                             : ErrorState.Ok;
            editor.SetError("ImportFileName", errorState, AdminStrings.Errors_NoResourceSelected);

            errorState = editor.IsImportSelected && this.importedContainer != null
                         && this.importedContainer.ProductType != productType
                             ? ErrorState.Warning
                             : ErrorState.Ok;
            editor.SetError("ImportFileName", errorState, wrongProductTypeMessage);
        }

        private bool CanChooseImportFile(object obj)
        {
            return this.ViewModel.Editor.IsImportSelected;
        }

        private void ChooseImportFile()
        {
            var interaction = new OpenFileDialogInteraction
                {
                    DefaultExtension = ".ucg",
                    AddExtension = true,
                    FileName = this.ViewModel.Editor.ImportFileName,
                    Filter = AdminStrings.UnitConfiguration_Export_FileFilters,
                    Title = AdminStrings.UnitConfig_Init_LoadData_ImportTitle
                };

            InteractionManager<OpenFileDialogInteraction>.Current.Raise(interaction, this.ImportSelected);
        }

        private void ImportSelected(OpenFileDialogInteraction interaction)
        {
            if (!interaction.Confirmed)
            {
                return;
            }

            try
            {
                var configurator = new Configurator(interaction.FileName);
                this.importedContainer = configurator.Deserialize<UnitConfigContainer>();
                this.ViewModel.Editor.ImportFileName = interaction.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Logger.Error(ex, "Couldn't import unit configuration from " + interaction.FileName);
            }

            this.UpdateErrors();
        }

        private bool CanLoadData(object obj)
        {
            return this.ViewModel.ErrorState <= ErrorState.Warning;
        }

        private async void LoadData()
        {
            this.Parent.Parent.ViewModel.IsLoading = true;
            try
            {
                var editor = this.ViewModel.Editor;
                if (editor.IsCopySelected && editor.CopyConfiguration.SelectedObject != null)
                {
                    var unitConfig = editor.CopyConfiguration.SelectedObject.ReadableModel;
                    await unitConfig.LoadReferencePropertiesAsync();
                    var document = unitConfig.Document;
                    await document.LoadNavigationPropertiesAsync();
                    var version = document.Versions.OrderByDescending(v => v.CreatedOn).First();
                    await version.LoadXmlPropertiesAsync();
                    this.Parent.Parent.LoadData((UnitConfigData)version.Content.Deserialize());
                }
                else if (editor.IsImportSelected && this.importedContainer != null)
                {
                    this.Parent.Parent.LoadData(this.importedContainer.UnitConfig);
                }
                else
                {
                    // reload the (empty) config, this will show all other categories
                    this.Parent.Parent.LoadData(this.Parent.Parent.CreateUnitConfigData());
                }

                this.ViewModel.IsVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Logger.Error(ex, "Couldn't load data");
            }

            this.Parent.Parent.ViewModel.IsLoading = false;

            this.Parent.Parent.ViewModel.SelectedItem =
                this.Parent.Parent.ViewModel.FilteredCategories.OfType<UnitConfigTreeNodeViewModelBase>()
                    .FirstOrDefault();
        }
    }
}