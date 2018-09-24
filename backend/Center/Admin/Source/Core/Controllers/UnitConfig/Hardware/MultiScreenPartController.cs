// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiScreenPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiScreenPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.HardwareManager;

    /// <summary>
    /// The multi screen part controller.
    /// </summary>
    public class MultiScreenPartController : MultiEditorPartControllerBase
    {
        private const string MainScreenKey = "MainScreen";
        private const string ScreenModeKey = "ScreenMode";

        private SelectionEditorViewModel mainScreen;
        private SelectionEditorViewModel screenMode;
        private SelectionOptionViewModel cloneModeOption;

        private ScreenResolutionsPartController resolutionsController;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiScreenPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public MultiScreenPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.MultiScreenMode, parent)
        {
        }

        /// <summary>
        /// Gets the display mode.
        /// </summary>
        /// <returns>
        /// The <see cref="DisplayMode"/>.
        /// </returns>
        public DisplayMode GetDisplayMode()
        {
            return (DisplayMode)this.screenMode.SelectedValue;
        }

        /// <summary>
        /// Gets the index of the primary screen (indexed from 1!).
        /// </summary>
        /// <returns>
        /// The index of the primary screen (indexed from 1!).
        /// </returns>
        public int GetPrimaryScreen()
        {
            return (int)this.mainScreen.SelectedValue;
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.mainScreen.SelectValue((int)partData.GetValue(1, MainScreenKey));
            this.screenMode.SelectValue(partData.GetEnumValue(DisplayMode.Extend, ScreenModeKey));
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.GetPrimaryScreen(), MainScreenKey);
            partData.SetEnumValue(this.GetDisplayMode(), ScreenModeKey);
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.resolutionsController = this.GetPart<ScreenResolutionsPartController>();
            this.resolutionsController.ViewModelUpdated += (s, e) => this.UpdateViewModel();

            this.UpdateViewModel();
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
                                    IsVisible = false,
                                    DisplayName = AdminStrings.UnitConfig_Hardware_MultiScreen,
                                    Description = AdminStrings.UnitConfig_Hardware_MultiScreen_Description
                                };

            this.mainScreen = new SelectionEditorViewModel();
            this.mainScreen.Label = AdminStrings.UnitConfig_Hardware_MultiScreen_Main;
            this.mainScreen.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Hardware_ScreenResolutions_Primary,
                    1));
            this.mainScreen.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Hardware_ScreenResolutions_Secondary,
                    2));
            viewModel.Editors.Add(this.mainScreen);

            this.screenMode = new SelectionEditorViewModel();
            this.screenMode.Label = AdminStrings.UnitConfig_Hardware_MultiScreen_Mode;
            this.screenMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Hardware_MultiScreen_Mode_Extend,
                    DisplayMode.Extend));
            viewModel.Editors.Add(this.screenMode);

            this.cloneModeOption = new SelectionOptionViewModel(
                AdminStrings.UnitConfig_Hardware_MultiScreen_Mode_Clone,
                DisplayMode.Clone);

            return viewModel;
        }

        private void UpdateViewModel()
        {
            var primaryResolution = this.resolutionsController.PrimaryResolution;
            var secondaryResolution = this.resolutionsController.SecondaryResolution;

            this.ViewModel.IsVisible = secondaryResolution != null;

            var canClone = false;
            if (primaryResolution != null && secondaryResolution != null)
            {
                canClone = primaryResolution.VisibleWidth == secondaryResolution.VisibleWidth
                           && primaryResolution.VisibleHeight == secondaryResolution.VisibleHeight;
            }

            if (this.screenMode.Options.Contains(this.cloneModeOption) == canClone)
            {
                return;
            }

            if (canClone)
            {
                this.screenMode.Options.Add(this.cloneModeOption);
                return;
            }

            if (this.screenMode.SelectedOption == this.cloneModeOption)
            {
                this.screenMode.SelectedOption = this.screenMode.Options[0];
            }

            this.screenMode.Options.Remove(this.cloneModeOption);
        }
    }
}