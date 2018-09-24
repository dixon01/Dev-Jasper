// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenResolutionsPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenResolutionsPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The screen resolutions part controller.
    /// </summary>
    public class ScreenResolutionsPartController : MultiEditorPartControllerBase
    {
        private const string WidthKeyFormat = "{0}.Width";
        private const string HeightKeyFormat = "{0}.Height";

        private static readonly ScreenResolution[] Resolutions =
            {
                new ScreenResolution("18.5\"", 1366, 768),
                new ScreenResolution("19\"", 1440, 900),
                new ScreenResolution("21.5\"", 1920, 1080),
                new ScreenResolution("24\"", 1920, 1080),
                new ScreenResolution("29\"", 1920, 630, 1920, 1080),
                new ScreenResolution("29\"", 1920, 540, 1920, 1080)
            };

        private SelectionEditorViewModel primaryScreenResolution;
        private SelectionEditorViewModel secondaryScreenResolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenResolutionsPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public ScreenResolutionsPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.ScreenResolutions, parent)
        {
        }

        /// <summary>
        /// Gets the primary screen resolution configuration or null if not configured.
        /// </summary>
        public ScreenResolution PrimaryResolution
        {
            get
            {
                return this.primaryScreenResolution.SelectedValue as ScreenResolution;
            }
        }

        /// <summary>
        /// Gets the secondary screen resolution configuration or null if not configured.
        /// </summary>
        public ScreenResolution SecondaryResolution
        {
            get
            {
                var resolution = this.secondaryScreenResolution.SelectedValue as ScreenResolution;
                return resolution != null && resolution.PhysicalWidth > 0 ? resolution : null;
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
            LoadResolution(this.primaryScreenResolution, partData, 0);
            LoadResolution(this.secondaryScreenResolution, partData, 1);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            SaveResolution(this.primaryScreenResolution, partData, 0);
            SaveResolution(this.secondaryScreenResolution, partData, 1);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel
                                {
                                    DisplayName = AdminStrings.UnitConfig_Hardware_ScreenResolutions,
                                    Description = AdminStrings.UnitConfig_Hardware_ScreenResolutions_Description,
                                    PartKey = UnitConfigKeys.Hardware.ScreenResolutions
                                };

            this.primaryScreenResolution = new SelectionEditorViewModel();
            this.primaryScreenResolution.Label = AdminStrings.UnitConfig_Hardware_ScreenResolutions_Primary;
            CreateResolutions(this.primaryScreenResolution);
            this.primaryScreenResolution.PropertyChanged += this.PrimaryScreenResolutionOnPropertyChanged;
            viewModel.Editors.Add(this.primaryScreenResolution);

            this.secondaryScreenResolution = new SelectionEditorViewModel();
            this.secondaryScreenResolution.Label = AdminStrings.UnitConfig_Hardware_ScreenResolutions_Secondary;
            this.secondaryScreenResolution.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Hardware_ScreenResolutions_None,
                    new ScreenResolution(null, 0, 0)));
            CreateResolutions(this.secondaryScreenResolution);
            viewModel.Editors.Add(this.secondaryScreenResolution);

            return viewModel;
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.ViewModel.IsVisible = descriptor.Platform is PcPlatformDescriptorBase;
            var platform = descriptor.Platform as PcPlatformDescriptorBase;
            this.secondaryScreenResolution.IsEnabled = platform != null && platform.DisplayAdapters.Count > 1;

            if (platform != null && platform.BuiltInScreen != null)
            {
                this.primaryScreenResolution.SelectedOption =
                    this.primaryScreenResolution.Options.FirstOrDefault(
                        o =>
                        ((ScreenResolution)o.Value).VisibleWidth == platform.BuiltInScreen.VisibleWidth
                        && ((ScreenResolution)o.Value).VisibleHeight == platform.BuiltInScreen.VisibleHeight);
            }

            this.UpdateErrors();
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateErrors();
        }

        private static void CreateResolutions(SelectionEditorViewModel selection)
        {
            foreach (var resolution in Resolutions)
            {
                selection.Options.Add(new SelectionOptionViewModel(resolution.ToString(), resolution));
            }
        }

        private static void LoadResolution(SelectionEditorViewModel viewModel, UnitConfigPart partData, int index)
        {
            var width = (int)partData.GetValue(0, string.Format(WidthKeyFormat, index));
            var height = (int)partData.GetValue(0, string.Format(HeightKeyFormat, index));

            var option =
                viewModel.Options.FirstOrDefault(
                    o =>
                    ((ScreenResolution)o.Value).VisibleWidth == width
                    && ((ScreenResolution)o.Value).VisibleHeight == height);
            if (option != null)
            {
                viewModel.SelectedOption = option;
            }
        }

        private static void SaveResolution(SelectionEditorViewModel viewModel, UnitConfigPart partData, int index)
        {
            var resolution = viewModel.SelectedValue as ScreenResolution;
            partData.SetValue(resolution == null ? 0 : resolution.VisibleWidth, string.Format(WidthKeyFormat, index));
            partData.SetValue(resolution == null ? 0 : resolution.VisibleHeight, string.Format(HeightKeyFormat, index));
        }

        private void PrimaryScreenResolutionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            ErrorState errorState;
            if (this.primaryScreenResolution.SelectedOption != null)
            {
                errorState = ErrorState.Ok;
            }
            else if (this.ViewModel.WasVisited)
            {
                errorState = ErrorState.Error;
            }
            else
            {
                errorState = ErrorState.Missing;
            }

            this.primaryScreenResolution.SetError("SelectedOption", errorState, AdminStrings.Errors_NoItemSelected);
        }
    }
}