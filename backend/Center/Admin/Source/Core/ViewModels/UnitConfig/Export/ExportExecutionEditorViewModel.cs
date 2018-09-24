// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportExecutionEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportExecutionEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The editor view model for the actual exporting part.
    /// </summary>
    public class ExportExecutionEditorViewModel : DataErrorViewModelBase
    {
        private bool isExporting;

        private int exportProgress;

        private string exportItemName;

        private bool wasExported;

        private bool hasSelectedUpdateGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportExecutionEditorViewModel"/> class.
        /// </summary>
        public ExportExecutionEditorViewModel()
        {
            this.UpdateGroups = new MultiSelectEditorViewModel();
            this.UpdateGroups.PropertyChanged += this.UpdateGroupsOnPropertyChanged;

            this.DateTimeEditors = new MultiEditorPartViewModel();

            this.StartTime = new DateTimeEditorViewModel { UseUtcToUiTimeConverter = true };
            this.DateTimeEditors.Editors.Add(this.StartTime);
            this.StartTime.IsNullable = true;
            this.StartTime.Label = AdminStrings.UnitConfig_Conclusion_ExportExecution_StartTime;

            this.EndTime = new DateTimeEditorViewModel { UseUtcToUiTimeConverter = true };
            this.DateTimeEditors.Editors.Add(this.EndTime);
            this.EndTime.IsNullable = true;
            this.EndTime.Label = AdminStrings.UnitConfig_Conclusion_ExportExecution_EndTime;
        }

        /// <summary>
        /// Gets or sets the export command.
        /// This property is set from the controller.
        /// </summary>
        public ICommand ExportCommand { get; set; }

        /// <summary>
        /// Gets the update groups selection.
        /// </summary>
        public MultiSelectEditorViewModel UpdateGroups { get; private set; }

        /// <summary>
        /// Gets the date time editors.
        /// This property is required for data binding.
        /// It actually contains the <see cref="StartTime"/> and <see cref="EndTime"/> editors.
        /// </summary>
        public MultiEditorPartViewModel DateTimeEditors { get; private set; }

        /// <summary>
        /// Gets the start time editor.
        /// </summary>
        public DateTimeEditorViewModel StartTime { get; private set; }

        /// <summary>
        /// Gets the end time editor.
        /// </summary>
        public DateTimeEditorViewModel EndTime { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the controller is currently exporting.
        /// </summary>
        public bool IsExporting
        {
            get
            {
                return this.isExporting;
            }

            set
            {
                this.SetProperty(ref this.isExporting, value, () => this.IsExporting);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data was successfully exported.
        /// </summary>
        public bool WasExported
        {
            get
            {
                return this.wasExported;
            }

            set
            {
                this.SetProperty(ref this.wasExported, value, () => this.WasExported);
            }
        }

        /// <summary>
        /// Gets a value indicating whether any update groups are selected.
        /// </summary>
        public bool HasSelectedUpdateGroups
        {
            get
            {
                return this.hasSelectedUpdateGroups;
            }

            private set
            {
                if (this.SetProperty(ref this.hasSelectedUpdateGroups, value, () => this.HasSelectedUpdateGroups))
                {
                    this.SetError(
                        "UpdateGroups",
                        value ? ErrorState.Ok : ErrorState.Warning,
                        AdminStrings.UnitConfig_Conclusion_ExportExecution_NoUpdateGroupSelected);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the item currently being exported.
        /// This is used in the busy indicator to show which item is being exported.
        /// </summary>
        public string ExportItemName
        {
            get
            {
                return this.exportItemName;
            }

            set
            {
                this.SetProperty(ref this.exportItemName, value, () => this.ExportItemName);
            }
        }

        /// <summary>
        /// Gets or sets the export progress (a value between 0 and 100).
        /// This is used in the busy indicator to show the export progress.
        /// </summary>
        public int ExportProgress
        {
            get
            {
                return this.exportProgress;
            }

            set
            {
                this.SetProperty(ref this.exportProgress, value, () => this.ExportProgress);
            }
        }

        private void UpdateGroupsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CheckedOptionsCount")
            {
                this.HasSelectedUpdateGroups = this.UpdateGroups.GetCheckedOptions().Any();
            }
        }
    }
}