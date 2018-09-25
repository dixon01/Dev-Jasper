// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcGeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcGeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.AhdlcRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.HardwareManager.Mgi;

    /// <summary>
    /// The AHDLC Renderer general part controller.
    /// </summary>
    public class AhdlcGeneralPartController : FilteredPartControllerBase
    {
        private const string ComPortKey = "ComPort";
        private const string AddressesKey = "Addresses";

        private EditableSelectionEditorViewModel comPort;

        private HardwareDescriptor hardwareDescriptor;

        private MultiSelectEditorViewModel addresses;

        private Rs485ModePartController rs485Mode;

        private TransceiversPartController transceivers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AhdlcGeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public AhdlcGeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.AhdlcRenderer.General, parent)
        {
        }

        /// <summary>
        /// Gets the COM port name.
        /// </summary>
        public string ComPort
        {
            get
            {
                return this.comPort.Value;
            }
        }

        /// <summary>
        /// Gets the list of addresses selected by the user.
        /// </summary>
        /// <returns>
        /// The list of addresses.
        /// </returns>
        public IEnumerable<int> GetCheckedAddresses()
        {
            return this.addresses.GetCheckedValues().Cast<int>();
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            var serialPorts = this.hardwareDescriptor.Platform.SerialPorts;
            var port = serialPorts.FirstOrDefault(p => p.IsDefaultRs485) ?? serialPorts.FirstOrDefault();
            this.comPort.Value = partData.GetValue(port != null ? port.Name : string.Empty, ComPortKey);

            var allAddresses = partData.GetValue(string.Empty, AddressesKey).Split(';');
            foreach (var option in this.addresses.Options)
            {
                option.IsChecked = allAddresses.Contains(option.Label);
            }

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
            partData.SetValue(this.ComPort, ComPortKey);
            partData.SetValue(string.Join(";", this.addresses.GetCheckedOptions().Select(o => o.Label)), AddressesKey);
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.hardwareDescriptor = descriptor;

            this.rs485Mode = this.GetPart<Rs485ModePartController>();
            this.rs485Mode.ViewModelUpdated += (s, e) => this.UpdateErrors();

            this.transceivers = this.GetPart<TransceiversPartController>();
            this.transceivers.ViewModelUpdated += (s, e) => this.UpdateErrors();

            foreach (var serialPort in descriptor.Platform.SerialPorts)
            {
                this.comPort.Options.Add(serialPort.Name);
            }
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Ahdlc_General;
            viewModel.Description = AdminStrings.UnitConfig_Ahdlc_General_Description;

            this.comPort = new EditableSelectionEditorViewModel();
            this.comPort.Label = AdminStrings.UnitConfig_Ahdlc_General_ComPort;
            viewModel.Editors.Add(this.comPort);

            this.addresses = new MultiSelectEditorViewModel();
            this.addresses.Label = AdminStrings.UnitConfig_Ahdlc_General_Addresses;
            viewModel.Editors.Add(this.addresses);
            for (int i = 1; i <= AhdlcRendererCategoryController.MaxAddress; i++)
            {
                this.addresses.Options.Add(new CheckableOptionViewModel(i.ToString(CultureInfo.InvariantCulture), i));
            }

            return viewModel;
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

        private void UpdateErrors()
        {
            if (this.hardwareDescriptor == null)
            {
                return;
            }

            this.comPort.SetError(
                "Value",
                string.IsNullOrWhiteSpace(this.comPort.Value) ? ErrorState.Error : ErrorState.Ok,
                AdminStrings.Errors_TextNotWhitespace);

            ErrorState errorState;
            var infoVision = this.hardwareDescriptor.Platform as InfoVisionPlatformDescriptor;
            if (infoVision != null)
            {
                MultiEditorPartControllerBase part;
                if (infoVision.HasSharedRs485Port)
                {
                    part = this.rs485Mode;
                    errorState = this.rs485Mode.GetCompactRs485Switch() == CompactRs485Switch.Cpu
                                     ? ErrorState.Ok
                                     : ErrorState.Warning;
                }
                else
                {
                    part = this.transceivers;
                    errorState = this.transceivers.GetTransceiverConfigs().Any(t => t.Type == TransceiverType.RS485)
                                     ? ErrorState.Ok
                                     : ErrorState.Warning;
                }

                var message = string.Format(
                    AdminStrings.Errors_NoRS485Configured_Format,
                    part.Parent.ViewModel.DisplayName,
                    part.ViewModel.DisplayName);
                this.comPort.SetError("Value", errorState, message);
            }

            errorState = this.addresses.CheckedOptionsCount == 0
                             ? (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing)
                             : ErrorState.Ok;
            this.addresses.SetError("Options", errorState, AdminStrings.Errors_SelectOneAtLeast);
        }
    }
}