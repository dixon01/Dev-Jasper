// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcSignPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcSignPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.AhdlcRenderer
{
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;

    /// <summary>
    /// The AHDLC Renderer sign part controller.
    /// This controller is responsible for a single sign with a given address.
    /// </summary>
    public class AhdlcSignPartController : FilteredPartControllerBase
    {
        private const string SignModeKey = "Mode";
        private const string WidthKey = "Width";
        private const string HeightKey = "Height";
        private const string BrightnessKey = "Brightness";

        private readonly int address;

        private AhdlcGeneralPartController general;

        private bool parentVisible;

        private SelectionEditorViewModel signMode;

        private NumberEditorViewModel width;

        private NumberEditorViewModel height;

        private SelectionEditorViewModel brightness;

        /// <summary>
        /// Initializes a new instance of the <see cref="AhdlcSignPartController"/> class.
        /// </summary>
        /// <param name="address">
        /// The AHDLC address.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public AhdlcSignPartController(int address, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.AhdlcRenderer.SignFormat, address), parent)
        {
            this.address = address;
        }

        /// <summary>
        /// Gets the sign mode.
        /// </summary>
        public SignMode SignMode
        {
            get
            {
                return (SignMode)this.signMode.SelectedValue;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)this.width.Value;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)this.height.Value;
            }
        }

        /// <summary>
        /// Gets the brightness.
        /// </summary>
        public SignBrightness Brightness
        {
            get
            {
                return (SignBrightness)this.brightness.SelectedValue;
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
            this.signMode.SelectValue(partData.GetEnumValue(SignMode.Monochrome, SignModeKey));
            this.width.Value = partData.GetValue(112, WidthKey);
            this.height.Value = partData.GetValue(16, HeightKey);
            this.brightness.SelectValue(partData.GetEnumValue(SignBrightness.Default, BrightnessKey));
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetEnumValue(this.SignMode, SignModeKey);
            partData.SetValue(this.Width, WidthKey);
            partData.SetValue(this.Height, HeightKey);
            partData.SetEnumValue(this.Brightness, BrightnessKey);
        }

        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        public override void UpdateVisibility(bool visible)
        {
            this.parentVisible = visible;
            this.UpdateVisibility();
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel();
            viewModel.DisplayName = string.Format(AdminStrings.UnitConfig_Ahdlc_Sign_Format, this.address);
            viewModel.Description = AdminStrings.UnitConfig_Ahdlc_Sign_Description;

            this.signMode = new SelectionEditorViewModel();
            this.signMode.Label = AdminStrings.UnitConfig_Ahdlc_Sign_Mode;
            this.signMode.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ahdlc_Sign_Mode_Monochrome, SignMode.Monochrome));
            this.signMode.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ahdlc_Sign_Mode_Color, SignMode.Color));
            this.signMode.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ahdlc_Sign_Mode_Text, SignMode.Text));
            viewModel.Editors.Add(this.signMode);

            this.width = new NumberEditorViewModel();
            this.width.Label = AdminStrings.UnitConfig_Ahdlc_Sign_Width;
            this.width.MinValue = 28;
            this.width.MaxValue = 224;
            this.width.IsInteger = true;
            viewModel.Editors.Add(this.width);

            this.height = new NumberEditorViewModel();
            this.height.Label = AdminStrings.UnitConfig_Ahdlc_Sign_Height;
            this.height.MinValue = 8;
            this.height.MaxValue = 32;
            this.height.IsInteger = true;
            viewModel.Editors.Add(this.height);

            this.brightness = new SelectionEditorViewModel();
            this.brightness.Label = AdminStrings.UnitConfig_Ahdlc_Sign_Brightness;
            this.brightness.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ahdlc_Sign_Brightness_Default,
                    SignBrightness.Default));
            this.brightness.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ahdlc_Sign_Brightness_1,
                    SignBrightness.Brightness1));
            this.brightness.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ahdlc_Sign_Brightness_2,
                    SignBrightness.Brightness2));
            this.brightness.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ahdlc_Sign_Brightness_3,
                    SignBrightness.Brightness3));
            viewModel.Editors.Add(this.brightness);

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
            this.general = this.GetPart<AhdlcGeneralPartController>();
            this.general.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.parentVisible && this.general.GetCheckedAddresses().Contains(this.address);
        }
    }
}