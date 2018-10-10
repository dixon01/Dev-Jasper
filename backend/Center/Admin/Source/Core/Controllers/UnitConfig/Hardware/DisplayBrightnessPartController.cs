// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayBrightnessPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayBrightnessPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.HardwareManager.Mgi;

    /// <summary>
    /// The display brightness part controller.
    /// </summary>
    public class DisplayBrightnessPartController : MultiEditorPartControllerBase
    {
        private const int AutoBrightness = -1;
        private const string ManualModeKey = "ManualMode";
        private const string MinimumBrightnessKey = "MinimumBrightness";
        private const string MaximumBrightnessKey = "MaximumBrightness";
        private const string SpeedKey = "Speed";

        private NumberEditorViewModel displayBrightness;

        private NumberEditorViewModel minimumBrightness;

        private NumberEditorViewModel maximumBrightness;

        private NumberEditorViewModel speed;

        private SelectionEditorViewModel manualMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayBrightnessPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DisplayBrightnessPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.DisplayBrightness, parent)
        {
        }

        /// <summary>
        /// Gets the display brightness.
        /// </summary>
        public int DisplayBrightness
        {
            get
            {
                return (bool)this.manualMode.SelectedValue ? (int)this.displayBrightness.Value : AutoBrightness;
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
            var config = new BacklightControlRateConfig();
            var brightness = partData.GetValue(AutoBrightness);
            this.manualMode.SelectValue(partData.GetValue(brightness != AutoBrightness, ManualModeKey));
            this.displayBrightness.Value = brightness == AutoBrightness ? 255 : brightness;
            this.minimumBrightness.Value = partData.GetValue(config.Minimum, MinimumBrightnessKey);
            this.maximumBrightness.Value = partData.GetValue(config.Maximum, MaximumBrightnessKey);
            this.speed.Value = partData.GetValue(config.Speed, SpeedKey);

            this.UpdateEditors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue((bool)this.manualMode.SelectedValue, ManualModeKey);
            partData.SetValue(this.displayBrightness.Value);
            partData.SetValue(this.minimumBrightness.Value, MinimumBrightnessKey);
            partData.SetValue(this.maximumBrightness.Value, MaximumBrightnessKey);
            partData.SetValue(this.speed.Value, SpeedKey);
        }

        /// <summary>
        /// Gets the backlight control rate configurations configured in this part.
        /// </summary>
        /// <returns>
        /// The <see cref="BacklightControlRateConfig"/>.
        /// </returns>
        public BacklightControlRateConfig GetBacklightControlRate()
        {
            return new BacklightControlRateConfig
                       {
                           Minimum = (int)this.minimumBrightness.Value,
                           Maximum = (int)this.maximumBrightness.Value,
                           Speed = (int)this.speed.Value
                       };
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        ///     The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.ViewModel.IsVisible = descriptor.Platform is InfoVisionPlatformDescriptor;

            var config = new BacklightControlRateConfig();
            var validator = new VersionedSettingValidator(
                this.minimumBrightness,
                config.Minimum,
                PackageIds.Motion.HardwareManager,
                SoftwareVersions.HardwareManager.AutoBrightnessParameters,
                this.Parent.Parent);
            validator.Start();

            validator = new VersionedSettingValidator(
                this.maximumBrightness,
                config.Maximum,
                PackageIds.Motion.HardwareManager,
                SoftwareVersions.HardwareManager.AutoBrightnessParameters,
                this.Parent.Parent);
            validator.Start();

            validator = new VersionedSettingValidator(
                this.speed,
                config.Speed,
                PackageIds.Motion.HardwareManager,
                SoftwareVersions.HardwareManager.AutoBrightnessParameters,
                this.Parent.Parent);
            validator.Start();
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
                                    DisplayName = AdminStrings.UnitConfig_Hardware_Brightness,
                                    Description = AdminStrings.UnitConfig_Hardware_Brightness_Description
                                };

            this.manualMode = new SelectionEditorViewModel();
            this.manualMode.Label = AdminStrings.UnitConfig_Hardware_Brightness_Options;
            this.manualMode.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Hardware_Brightness_Options_Manual, true));
            this.manualMode.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Hardware_Brightness_Options_Automatic, false));
            this.manualMode.PropertyChanged += (s, e) => this.UpdateEditors();
            viewModel.Editors.Add(this.manualMode);

            this.displayBrightness = new NumberEditorViewModel
                                         {
                                             Label = AdminStrings.UnitConfig_Hardware_Brightness,
                                             IsInteger = true,
                                             MinValue = 90,
                                             MaxValue = 255
                                         };
            viewModel.Editors.Add(this.displayBrightness);

            this.minimumBrightness = new NumberEditorViewModel
                                         {
                                             Label = AdminStrings.UnitConfig_Hardware_Brightness_Minimum,
                                             IsInteger = true,
                                             MinValue = 90,
                                             MaxValue = 255
                                         };
            viewModel.Editors.Add(this.minimumBrightness);

            this.maximumBrightness = new NumberEditorViewModel
                                         {
                                             Label = AdminStrings.UnitConfig_Hardware_Brightness_Maximum,
                                             IsInteger = true,
                                             MinValue = 90,
                                             MaxValue = 255
                                         };
            viewModel.Editors.Add(this.maximumBrightness);

            this.speed = new NumberEditorViewModel
                             {
                                 Label = AdminStrings.UnitConfig_Hardware_Brightness_Speed,
                                 IsInteger = true,
                                 MinValue = 1,
                                 MaxValue = 10
                             };
            viewModel.Editors.Add(this.speed);

            return viewModel;
        }

        private void UpdateEditors()
        {
            if (this.manualMode.SelectedValue == null)
            {
                return;
            }

            var manual = (bool)this.manualMode.SelectedValue;

            this.displayBrightness.IsEnabled = manual;
            this.minimumBrightness.IsEnabled = !manual;
            this.maximumBrightness.IsEnabled = !manual;
            this.speed.IsEnabled = !manual;
         }
    }
}