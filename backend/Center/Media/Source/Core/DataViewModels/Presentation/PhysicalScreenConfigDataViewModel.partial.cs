// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The physical screen config data view model.
    /// </summary>
    public partial class PhysicalScreenConfigDataViewModel : IReusableEntity
    {
        private bool isInEditMode;

        private int referencesCount;

        private string isUsedToolTip;

        private MasterLayout selectedMasterLayout;

        private string description;

        private bool isMonochromeScreen;

        /// <summary>
        /// Gets or sets a value indicating whether the layout is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return this.isInEditMode;
            }

            set
            {
                this.SetProperty(ref this.isInEditMode, value, () => this.IsInEditMode);
            }
        }

        /// <summary>
        /// Gets the tool tip shown for the IsUsed flag.
        /// </summary>
        public string IsUsedToolTip
        {
            get
            {
                return this.isUsedToolTip;
            }

            private set
            {
                this.SetProperty(ref this.isUsedToolTip, value, () => this.IsUsedToolTip);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the screen is monochrome (LED).
        /// </summary>
        public bool IsMonochromeScreen
        {
            get
            {
                return this.isMonochromeScreen;
            }

            set
            {
                this.SetProperty(ref this.isMonochromeScreen, value, () => this.IsMonochromeScreen);
            }
        }

        /// <summary>
        /// Gets the number of sections where this layout is used.
        /// </summary>
        public int ReferencesCount
        {
            get
            {
                return this.referencesCount;
            }

            private set
            {
                this.SetProperty(ref this.referencesCount, value, () => this.ReferencesCount);
            }
        }

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        public string Resolution
        {
            get
            {
                return string.Format("{0}x{1}", this.Width.Value, this.Height.Value);
            }

            set
            {
                var resolution = value.Split('x');
                var resolutionWidth = int.Parse(resolution[0]);
                var resolutionHeight = int.Parse(resolution[1]);
                this.Width.Value = resolutionWidth;
                this.Height.Value = resolutionHeight;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [UserVisibleProperty("Content", OrderIndex = 3, GroupOrderIndex = 0)]
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets or sets the selected master layout.
        /// </summary>
        public MasterLayout SelectedMasterLayout
        {
            get
            {
                return this.selectedMasterLayout;
            }

            set
            {
                this.SetProperty(ref this.selectedMasterLayout, value, () => this.SelectedMasterLayout);
            }
        }

        /// <summary>
        /// Sets the name of the physical screen.
        /// </summary>
        /// <param name="screenName">
        /// The name.
        /// </param>
        public void SetName(string screenName)
        {
            this.Name.Value = screenName;
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        /// <returns>the name</returns>
        public string GetName()
        {
            return this.Name.Value;
        }

        /// <summary>
        /// Validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object value)
        {
            if (propertyName == "Name")
            {
                var valueString = value as string;
                if (string.IsNullOrEmpty(valueString))
                {
                    return MediaStrings.ResolutionNavigationDialog_NameMissing;
                }

                var screens =
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.Where(
                        screen => screen.Name.Value == valueString);
                if (screens.Count() > 1)
                {
                    return MediaStrings.ResolutionNavigationDialog_UniqueNameError;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The list of error messages for the given properties. Empty enumeration if no error was found.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<string> Validate(string propertyName)
        {
            if (propertyName == "Name")
            {
                var screens =
                    this.mediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.Where(
                        screen => screen.Name.Value == this.Name.Value);
                if (screens.Count() > 1)
                {
                    return new[] { MediaStrings.ResolutionNavigationDialog_UniqueNameError };
                }
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Checks if a compatibility conversion for CSV mapping is required
        /// </summary>
        /// <param name="exportParameters">
        /// The export parameters.
        /// </param>
        /// <returns>
        /// True if is required.
        /// </returns>
        protected bool CsvMappingCompatibilityRequired(object exportParameters)
        {
            var parameters = exportParameters as ExportCompatibilityParameters;
            return parameters != null && parameters.CsvMappingCompatibilityRequired;
        }

        partial void Initialize(Models.Presentation.PhysicalScreenConfigDataModel dataModel)
        {
            if (dataModel != null)
            {
                this.SelectedMasterLayout = dataModel.SelectedMasterLayout;
                this.Description = dataModel.Description;
                this.IsMonochromeScreen = dataModel.IsMonochromeScreen;
            }

            this.PropertyChanged += this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visible")
            {
                if (this.mediaShell.MediaApplicationState.CurrentProject != null)
                {
                    foreach (var resource in this.mediaShell.MediaApplicationState.CurrentProject.Resources)
                    {
                        resource.UpdateIsUsedVisible();
                    }
                }
            }
        }

        partial void Initialize(PhysicalScreenConfigDataViewModel dataViewModel)
        {
            this.SelectedMasterLayout = dataViewModel.SelectedMasterLayout;
            this.Description = dataViewModel.Description;
            this.IsMonochromeScreen = dataViewModel.IsMonochromeScreen;

            this.PropertyChanged += this.OnPropertyChanged;
        }

        partial void ConvertNotGeneratedToDataModel(ref Models.Presentation.PhysicalScreenConfigDataModel dataModel)
        {
            dataModel.SelectedMasterLayout = this.SelectedMasterLayout;
            dataModel.Description = this.Description;
            dataModel.IsMonochromeScreen = this.IsMonochromeScreen;
        }

        partial void ExportNotGeneratedValues(
            Gorba.Common.Configuration.Infomedia.Presentation.PhysicalScreenConfig model, object exportParameters)
        {
            if (model.Identifier == string.Empty)
            {
                model.Identifier = null;
            }
        }
    }
}
