// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadDataEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoadDataEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Init
{
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Configurations;

    /// <summary>
    /// The editor view model for the load data part.
    /// </summary>
    public class LoadDataEditorViewModel : DataErrorViewModelBase
    {
        private bool isCreateEmptySelected;

        private bool isCopySelected;

        private bool isImportSelected;

        private string importFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadDataEditorViewModel"/> class.
        /// </summary>
        public LoadDataEditorViewModel()
        {
            this.CopyConfiguration = new ItemSelectionViewModel<UnitConfigurationReadOnlyDataViewModel>();
            this.CopyConfiguration.IsRequired = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether create empty configuration is selected.
        /// </summary>
        public bool IsCreateEmptySelected
        {
            get
            {
                return this.isCreateEmptySelected;
            }

            set
            {
                if (this.SetProperty(ref this.isCreateEmptySelected, value, () => this.IsCreateEmptySelected) && value)
                {
                    this.IsImportSelected = false;
                    this.IsCopySelected = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether copy existing configuration is selected.
        /// </summary>
        public bool IsCopySelected
        {
            get
            {
                return this.isCopySelected;
            }

            set
            {
                if (this.SetProperty(ref this.isCopySelected, value, () => this.IsCopySelected) && value)
                {
                    this.IsCreateEmptySelected = false;
                    this.IsImportSelected = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import configuration is selected.
        /// </summary>
        public bool IsImportSelected
        {
            get
            {
                return this.isImportSelected;
            }

            set
            {
                if (this.SetProperty(ref this.isImportSelected, value, () => this.IsImportSelected) && value)
                {
                    this.IsCreateEmptySelected = false;
                    this.IsCopySelected = false;
                }
            }
        }

        /// <summary>
        /// Gets the configuration from which to copy the data.
        /// </summary>
        public ItemSelectionViewModel<UnitConfigurationReadOnlyDataViewModel> CopyConfiguration { get; private set; }

        /// <summary>
        /// Gets or sets the file name of the file to be imported.
        /// </summary>
        public string ImportFileName
        {
            get
            {
                return this.importFileName;
            }

            set
            {
                this.SetProperty(ref this.importFileName, value, () => this.ImportFileName);
            }
        }

        /// <summary>
        /// Gets or sets the choose import file command.
        /// </summary>
        public ICommand ChooseImportFileCommand { get; set; }

        /// <summary>
        /// Gets or sets the load data command.
        /// </summary>
        public ICommand LoadCommand { get; set; }
    }
}