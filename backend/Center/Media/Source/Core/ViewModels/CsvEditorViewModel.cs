// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.DataViewModels;

    /// <summary>
    /// The csv editor view model.
    /// </summary>
    public class CsvEditorViewModel : ViewModelBase
    {
        private readonly CsvMappingDataViewModel dataViewModel;

        private string content;
        private bool isDirty;

        private IMediaShell shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvEditorViewModel"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        public CsvEditorViewModel(IMediaShell shell, CsvMappingDataViewModel dataViewModel)
        {
            this.dataViewModel = dataViewModel;
            this.Content = this.dataViewModel.RawContent.Value;
            this.shell = shell;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.SetProperty(ref this.content, value, () => this.Content);
            }
        }

        /// <summary>
        /// Gets the title of the window.
        /// </summary>
        public string Title
        {
            get
            {
                return this.dataViewModel.Filename.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the content is dirty.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }

            set
            {
                this.SetProperty(ref this.isDirty, value, () => this.IsDirty);
            }
        }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(this.SaveChanges, this.CanSave);
            }
        }

        private bool CanSave(object obj)
        {
            return this.IsDirty && this.HasWritePermission();
        }

        private bool HasWritePermission()
        {
            return this.shell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration);
        }

        private void SaveChanges()
        {
            this.dataViewModel.RawContent.Value = this.Content;
            this.IsDirty = false;

            this.shell.MediaApplicationState.MakeDirty();
            this.shell.MediaApplicationState.CurrentProject.IsCheckedIn = false;
        }
    }
}
