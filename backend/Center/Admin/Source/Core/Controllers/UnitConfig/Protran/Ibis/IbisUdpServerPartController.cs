// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisUdpServerPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisUdpServerPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Ibis;

    /// <summary>
    /// The IBIS UDP server part controller.
    /// </summary>
    public class IbisUdpServerPartController : IbisSourcePartControllerBase
    {
        private const string LocalPortKey = "LocalPort";
        private const string ReceiveFormatKey = "ReceiveFormat";
        private const string SendFormatKey = "SendFormat";

        private NumberEditorViewModel localPort;

        private SelectionEditorViewModel receiveFormat;

        private SelectionEditorViewModel sendFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisUdpServerPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public IbisUdpServerPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.IbisProtocol.UdpServer, IbisSourceType.UDPServer, parent)
        {
        }

        /// <summary>
        /// Gets the local UDP port.
        /// </summary>
        public int LocalPort
        {
            get
            {
                return (int)this.localPort.Value;
            }
        }

        /// <summary>
        /// Gets the receive format.
        /// </summary>
        public TelegramFormat ReceiveFormat
        {
            get
            {
                return (TelegramFormat)this.receiveFormat.SelectedValue;
            }
        }

        /// <summary>
        /// Gets the send format.
        /// </summary>
        public TelegramFormat SendFormat
        {
            get
            {
                return (TelegramFormat)this.sendFormat.SelectedValue;
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
            var defaultConfig = new UdpServerConfig();
            this.localPort.Value = partData.GetValue(defaultConfig.LocalPort, LocalPortKey);
            this.receiveFormat.SelectValue(partData.GetEnumValue(defaultConfig.ReceiveFormat, ReceiveFormatKey));
            this.sendFormat.SelectValue(partData.GetEnumValue(defaultConfig.SendFormat, SendFormatKey));
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.LocalPort, LocalPortKey);
            partData.SetEnumValue(this.ReceiveFormat, ReceiveFormatKey);
            partData.SetEnumValue(this.SendFormat, SendFormatKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Ibis_UdpServer;
            viewModel.Description = AdminStrings.UnitConfig_Ibis_UdpServer_Description;

            this.localPort = new NumberEditorViewModel();
            this.localPort.Label = AdminStrings.UnitConfig_Ibis_UdpServer_LocalPort;
            this.localPort.IsInteger = true;
            this.localPort.MinValue = 1;
            this.localPort.MaxValue = 0xFFFF;
            viewModel.Editors.Add(this.localPort);

            this.receiveFormat = this.CreateFormatChooser();
            this.receiveFormat.Label = AdminStrings.UnitConfig_Ibis_UdpServer_ReceiveFormat;
            viewModel.Editors.Add(this.receiveFormat);

            this.sendFormat = this.CreateFormatChooser();
            this.sendFormat.Label = AdminStrings.UnitConfig_Ibis_UdpServer_SendFormat;
            viewModel.Editors.Add(this.sendFormat);

            return viewModel;
        }

        private SelectionEditorViewModel CreateFormatChooser()
        {
            var editor = new SelectionEditorViewModel();
            editor.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Ibis_UdpServer_Format_Full, TelegramFormat.Full));
            editor.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_UdpServer_Format_NoChecksum,
                    TelegramFormat.NoChecksum));
            editor.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_Ibis_UdpServer_Format_NoFooter,
                    TelegramFormat.NoFooter));

            return editor;
        }
    }
}