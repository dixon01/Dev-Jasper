// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOSystemConfigPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOSystemConfigPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The I/O specific system config part controller.
    /// </summary>
    public class IOSystemConfigPartController : SystemConfigPartControllerBase
    {
        private const string InputKeyFormat = "Input.{0}";

        private readonly int index;

        private List<InputSelectionEditorViewModel> inputSelections;

        private GlobalSystemConfigPartController globalPart;

        private InputsPartController inputsPart;

        /// <summary>
        /// Initializes a new instance of the <see cref="IOSystemConfigPartController"/> class.
        /// </summary>
        /// <param name="index">
        /// The index of this controller (indexed from zero).
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IOSystemConfigPartController(int index, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.SystemConfig.IoFormat, index), parent)
        {
            this.index = index;
        }

        /// <summary>
        /// Gets the input conditions.
        /// </summary>
        /// <returns>
        /// A dictionary mapping input names to their value (true = 1, false = 0).
        /// </returns>
        public IDictionary<string, bool> GetInputConditions()
        {
            var inputs = this.inputsPart.GetInputs();
            return this.inputSelections.Where(s => s.SelectedValue != null)
                .ToDictionary(s => inputs[s.Input.Index], s => (bool)s.SelectedValue);
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            base.Load(partData);

            for (int i = 0; i < this.inputSelections.Count; i++)
            {
                this.inputSelections[i].SelectValue(partData.GetValue((bool?)null, string.Format(InputKeyFormat, i)));
            }
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            base.Save(partData);

            for (int i = 0; i < this.inputSelections.Count; i++)
            {
                partData.SetValue(this.inputSelections[i].SelectedValue as bool?, string.Format(InputKeyFormat, i));
            }
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            base.Prepare(descriptor);

            this.globalPart = this.GetPart<GlobalSystemConfigPartController>();
            this.globalPart.ViewModelUpdated += (s, e) => this.UpdateDisabledFields();

            this.inputsPart = this.GetPart<InputsPartController>();
            this.inputsPart.ViewModelUpdated += (s, e) => this.UpdateInputNames();

            this.inputSelections =
                descriptor.Platform.Inputs.Select(input => new InputSelectionEditorViewModel(input)).ToList();

            var insertAt = 1;
            foreach (var inputSelection in this.inputSelections)
            {
                this.ViewModel.Editors.Insert(insertAt++, inputSelection);
            }

            this.UpdateInputNames();
        }

        /// <summary>
        /// Updates this controller with the values from
        /// <see cref="SystemConfigPartControllerBase.ConfigModeController"/>.
        /// </summary>
        protected override void UpdateFromConfigMode()
        {
            this.IpAddressEnabled = this.ConfigModeController.GetIpAddressUsage() == SettingItemMode.UseSpecific;
            this.NetworkMaskEnabled = this.ConfigModeController.GetNetworkMaskUsage() == SettingItemMode.UseSpecific;
            this.IpGatewayEnabled = this.ConfigModeController.GetIpGatewayUsage() == SettingItemMode.UseSpecific;
            this.DnsServersEnabled = this.ConfigModeController.GetDnsServersUsage() == SettingItemMode.UseSpecific;
            this.TimeZoneEnabled = this.ConfigModeController.GetTimeZoneUsage() == SettingItemMode.UseSpecific;

            this.ViewModel.IsVisible = this.ConfigModeController.ShouldUseMultiConfig()
                                       && this.ConfigModeController.GetIoConfigCount() > this.index
                                       && (this.IpAddressEnabled || this.NetworkMaskEnabled || this.IpGatewayEnabled
                                           || this.DnsServersEnabled || this.TimeZoneEnabled);

            this.UpdateDisabledFields();
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = base.CreateViewModel();
            viewModel.DisplayName = string.Format(AdminStrings.UnitConfig_SystemConfig_IO_Format, this.index);
            viewModel.Description = AdminStrings.UnitConfig_SystemConfig_IO_Description;

            viewModel.Editors.Insert(
                0,
                new TitleEditorViewModel { Label = AdminStrings.UnitConfig_SystemConfig_IO_IOTitle });
            viewModel.Editors.Insert(
                1,
                new TitleEditorViewModel { Label = AdminStrings.UnitConfig_SystemConfig_IO_SettingsTitle });
            return viewModel;
        }

        private void UpdateInputNames()
        {
            var inputs = this.inputsPart.GetInputs();
            foreach (var editor in this.ViewModel.Editors.OfType<InputSelectionEditorViewModel>())
            {
                string name;
                if (inputs.TryGetValue(editor.Input.Index, out name))
                {
                    editor.Label = name;
                }
            }
        }

        private void UpdateDisabledFields()
        {
            if (!this.IpAddressEnabled)
            {
                this.IpAddress = this.globalPart.IpAddress;
            }

            if (!this.NetworkMaskEnabled)
            {
                this.NetworkMask = this.globalPart.NetworkMask;
            }

            if (!this.IpGatewayEnabled)
            {
                this.IpGateway = this.globalPart.IpGateway;
            }

            if (!this.DnsServersEnabled)
            {
                this.DnsServers = this.globalPart.DnsServers;
            }

            if (!this.TimeZoneEnabled)
            {
                this.TimeZone = this.globalPart.TimeZone;
            }
        }

        private class InputSelectionEditorViewModel : SelectionEditorViewModel
        {
            public InputSelectionEditorViewModel(InputDescriptor input)
            {
                this.Input = input;
                this.Options.Add(new SelectionOptionViewModel(AdminStrings.UnitConfig_SystemConfig_IO_Ignore, null));
                this.Options.Add(new SelectionOptionViewModel(AdminStrings.UnitConfig_SystemConfig_IO_On, true));
                this.Options.Add(new SelectionOptionViewModel(AdminStrings.UnitConfig_SystemConfig_IO_Off, false));
            }

            public InputDescriptor Input { get; private set; }
        }
    }
}