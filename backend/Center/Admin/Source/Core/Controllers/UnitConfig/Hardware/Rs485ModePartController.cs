// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rs485ModePartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Rs485ModePartController type.
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
    using Gorba.Common.Configuration.HardwareManager.Mgi;

    /// <summary>
    /// The RS-485 mode part controller.
    /// </summary>
    public class Rs485ModePartController : MultiEditorPartControllerBase
    {
        private SelectionEditorViewModel rs485Mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rs485ModePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public Rs485ModePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.Rs485Mode, parent)
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
            this.rs485Mode.SelectValue(partData.GetEnumValue(CompactRs485Switch.At91));
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetEnumValue(this.GetCompactRs485Switch());
        }

        /// <summary>
        /// Gets the configured RS-485 setting.
        /// </summary>
        /// <returns>
        /// The <see cref="CompactRs485Switch"/> as configured by the user.
        /// </returns>
        public CompactRs485Switch GetCompactRs485Switch()
        {
            return (CompactRs485Switch)(this.rs485Mode.SelectedValue ?? CompactRs485Switch.At91);
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            var infoVision = descriptor.Platform as InfoVisionPlatformDescriptor;
            this.ViewModel.IsVisible = infoVision != null && infoVision.HasSharedRs485Port;
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
                                    DisplayName = AdminStrings.UnitConfig_Hardware_Rs485Mode,
                                    Description = AdminStrings.UnitConfig_Hardware_Rs485Mode_Description,
                                    IsVisible = false
                                };

            this.rs485Mode = new SelectionEditorViewModel
                                 {
                                     Label = AdminStrings.UnitConfig_Hardware_Rs485Mode_Label,
                                     Options =
                                         {
                                             new SelectionOptionViewModel(
                                                 AdminStrings.UnitConfig_Hardware_Rs485Mode_AT91,
                                                 CompactRs485Switch.At91),
                                             new SelectionOptionViewModel(
                                                 AdminStrings.UnitConfig_Hardware_Rs485Mode_CPU,
                                                 CompactRs485Switch.Cpu)
                                         }
                                 };

            viewModel.Editors.Add(this.rs485Mode);
            return viewModel;
        }
    }
}