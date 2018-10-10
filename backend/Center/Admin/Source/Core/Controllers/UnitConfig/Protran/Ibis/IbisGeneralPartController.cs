// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisGeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisGeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// The general IBIS protocol part controller.
    /// </summary>
    public class IbisGeneralPartController : MultiEditorPartControllerBase
    {
        private const string SourceTypeKey = "SourceType";
        private const string ConnectionTimeoutKey = "ConnectionTimeout";
        private const string RecordingKey = "Recording";
        private const string ByteTypeKey = "ByteType";
        private const string CheckCrcKey = "CheckCrc";
        private const string AddressesKey = "Addresses";

        private SelectionEditorViewModel sourceType;

        private TimeSpanEditorViewModel connectionTimeout;

        private SelectionEditorViewModel byteType;

        private CheckableEditorViewModel checkCrc;

        private MultiSelectEditorViewModel addresses;

        private bool isInform;

        private CheckableEditorViewModel recording;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisGeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IbisGeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.IbisProtocol.General, parent)
        {
        }

        /// <summary>
        /// Gets the IBIS source type.
        /// </summary>
        public IbisSourceType SourceType
        {
            get
            {
                return (IbisSourceType)(this.sourceType.SelectedValue ?? IbisSourceType.None);
            }
        }

        /// <summary>
        /// Gets the connection timeout.
        /// </summary>
        public TimeSpan ConnectionTimeout
        {
            get
            {
                // ReSharper disable once PossibleInvalidOperationException
                return this.connectionTimeout.Value.Value;
            }
        }

        /// <summary>
        /// Gets the byte type.
        /// </summary>
        public ByteType ByteType
        {
            get
            {
                return (ByteType)this.byteType.SelectedValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to check the CRC.
        /// </summary>
        public bool CheckCrc
        {
            get
            {
                return this.checkCrc.IsChecked.HasValue && this.checkCrc.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether recording should be enabled.
        /// </summary>
        public bool Recording
        {
            get
            {
                return this.recording.IsChecked.HasValue && this.recording.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets all configured IBIS addresses.
        /// </summary>
        /// <returns>
        /// The list of IBIS addresses.
        /// </returns>
        public IEnumerable<int> GetAddresses()
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
            this.sourceType.SelectValue(
                partData.GetEnumValue(this.isInform ? IbisSourceType.SerialPort : IbisSourceType.JSON, SourceTypeKey));
            this.connectionTimeout.Value = partData.GetValue(TimeSpan.FromSeconds(60), ConnectionTimeoutKey);
            this.recording.IsChecked = partData.GetValue(false, RecordingKey);
            this.byteType.SelectValue(
                this.isInform ? partData.GetEnumValue(ByteType.Ascii7, ByteTypeKey) : ByteType.Ascii7);
            this.checkCrc.IsChecked = !this.isInform || partData.GetValue(true, CheckCrcKey);

            var addressList = partData.GetValue(string.Empty, AddressesKey).Split(';');
            foreach (var option in this.addresses.Options)
            {
                option.IsChecked = addressList.Contains(option.Value.ToString());
            }

            this.UpdateEnabled();
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
            partData.SetEnumValue(this.SourceType, SourceTypeKey);
            partData.SetValue(this.ConnectionTimeout, ConnectionTimeoutKey);
            partData.SetValue(this.Recording, RecordingKey);
            partData.SetEnumValue(this.ByteType, ByteTypeKey);
            partData.SetValue(this.CheckCrc, CheckCrcKey);
            partData.SetValue(string.Join(";", this.GetAddresses()), AddressesKey);
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.isInform = descriptor.Platform is InformPlatformDescriptor;
            this.byteType.IsEnabled = this.isInform;
            this.checkCrc.IsEnabled = this.isInform;

            if (this.isInform)
            {
                this.sourceType.Options.Remove(
                    this.sourceType.Options.FirstOrDefault(o => o.Value.Equals(IbisSourceType.JSON)));
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Ibis_General;
            viewModel.Description = AdminStrings.UnitConfig_Ibis_General_Description;

            this.sourceType = new SelectionEditorViewModel();
            this.sourceType.Label = AdminStrings.UnitConfig_Ibis_General_SourceType;
            this.sourceType.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_General_SourceType_Json,
                    IbisSourceType.JSON));
            this.sourceType.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_General_SourceType_Serial,
                    IbisSourceType.SerialPort));
            this.sourceType.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_General_SourceType_UdpServer,
                    IbisSourceType.UDPServer));
            this.sourceType.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_General_SourceType_Simulation,
                    IbisSourceType.Simulation));
            this.sourceType.PropertyChanged += (s, e) => this.UpdateEnabled();
            viewModel.Editors.Add(this.sourceType);

            this.connectionTimeout = new TimeSpanEditorViewModel();
            this.connectionTimeout.Label = AdminStrings.UnitConfig_Ibis_General_ConnectionTimeout;
            this.connectionTimeout.IsNullable = false;
            viewModel.Editors.Add(this.connectionTimeout);

            this.recording = new CheckableEditorViewModel();
            this.recording.Label = AdminStrings.UnitConfig_Ibis_General_Recording;
            this.recording.IsThreeState = false;
            viewModel.Editors.Add(this.recording);

            this.byteType = new SelectionEditorViewModel();
            this.byteType.Label = AdminStrings.UnitConfig_Ibis_General_ByteType;
            this.byteType.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ibis_General_ByteType_Ascii7, ByteType.Ascii7));
            this.byteType.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_General_ByteType_Hengartner8,
                    ByteType.Hengartner8));
            this.byteType.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_General_ByteType_UnicodeBigEndian,
                    ByteType.UnicodeBigEndian));
            viewModel.Editors.Add(this.byteType);

            this.checkCrc = new CheckableEditorViewModel();
            this.checkCrc.Label = AdminStrings.UnitConfig_Ibis_General_CheckCrc;
            this.checkCrc.IsThreeState = false;
            viewModel.Editors.Add(this.checkCrc);

            this.addresses = new MultiSelectEditorViewModel();
            this.addresses.Label = AdminStrings.UnitConfig_Ibis_General_Addresses;
            viewModel.Editors.Add(this.addresses);
            for (int i = 1; i <= 15; i++)
            {
                this.addresses.Options.Add(
                    new CheckableOptionViewModel(i.ToString(CultureInfo.InvariantCulture), i));
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

        private void UpdateEnabled()
        {
            if (this.SourceType == IbisSourceType.Simulation)
            {
                this.recording.IsEnabled = false;
                this.recording.IsChecked = false;
            }
            else
            {
                this.recording.IsEnabled = true;
            }
        }

        private void UpdateErrors()
        {
            var errorState = this.addresses.GetCheckedOptions().Any()
                                 ? ErrorState.Ok
                                 : (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing);
            this.addresses.SetError("Options", errorState, AdminStrings.Errors_SelectOneAtLeast);
        }
    }
}