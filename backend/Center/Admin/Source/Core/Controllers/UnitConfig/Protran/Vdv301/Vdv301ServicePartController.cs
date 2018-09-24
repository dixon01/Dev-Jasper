// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301ServicePartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301ServicePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The part controller for the client configuration for a single VDV 301 service.
    /// </summary>
    public class Vdv301ServicePartController : Vdv301PartControllerBase
    {
        private const string UseDnsSdKey = "UseDnsSd";
        private const string HostKey = "Host";
        private const string PortKey = "Port";
        private const string PathKey = "Path";

        private CheckableEditorViewModel useDnsSd;

        private TextEditorViewModel host;

        private NumberEditorViewModel port;

        private TextEditorViewModel path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301ServicePartController"/> class.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public Vdv301ServicePartController(string serviceName, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.Vdv301Protocol.ServiceFormat, serviceName), parent)
        {
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the VDV 301 service name.
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to use DNS-SD for this service.
        /// </summary>
        public bool UseDnsSd
        {
            get
            {
                return this.useDnsSd.IsChecked.HasValue && this.useDnsSd.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets the host name of the service.
        /// </summary>
        public string Host
        {
            get
            {
                return this.host.Text;
            }
        }

        /// <summary>
        /// Gets the port number of the service.
        /// </summary>
        public int Port
        {
            get
            {
                return (int)this.port.Value;
            }
        }

        /// <summary>
        /// Gets the path of the service.
        /// </summary>
        public string Path
        {
            get
            {
                return this.path.Text;
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
            this.useDnsSd.IsChecked = partData.GetValue(true, UseDnsSdKey);
            this.host.Text = partData.GetValue(string.Empty, HostKey);
            this.port.Value = partData.GetValue(0, PortKey);
            this.path.Text = partData.GetValue(string.Empty, PathKey);

            this.UpdateEnabled();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.UseDnsSd, UseDnsSdKey);
            partData.SetValue(this.Host, HostKey);
            partData.SetValue(this.Port, PortKey);
            partData.SetValue(this.Path, PathKey);
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
            viewModel.DisplayName = this.ServiceName;
            viewModel.Description = string.Format(
                AdminStrings.UnitConfig_Vdv301_Service_DescriptionFormat,
                this.ServiceName);

            this.useDnsSd = new CheckableEditorViewModel();
            this.useDnsSd.Label = AdminStrings.UnitConfig_Vdv301_Service_UseDnsSd;
            this.useDnsSd.IsThreeState = false;
            this.useDnsSd.PropertyChanged += this.UseDnsSdOnPropertyChanged;
            viewModel.Editors.Add(this.useDnsSd);

            this.host = new TextEditorViewModel();
            this.host.Label = AdminStrings.UnitConfig_Vdv301_Service_Host;
            viewModel.Editors.Add(this.host);

            this.port = new NumberEditorViewModel();
            this.port.Label = AdminStrings.UnitConfig_Vdv301_Service_Port;
            this.port.IsInteger = true;
            this.port.MinValue = 0;
            this.port.MaxValue = 0xFFFF;
            viewModel.Editors.Add(this.port);

            this.path = new TextEditorViewModel();
            this.path.Label = AdminStrings.UnitConfig_Vdv301_Service_Path;
            viewModel.Editors.Add(this.path);

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

        private void UseDnsSdOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                this.UpdateEnabled();
            }
        }

        private void UpdateEnabled()
        {
            var enabled = !this.UseDnsSd;
            this.host.IsEnabled = enabled;
            this.port.IsEnabled = enabled;
            this.path.IsEnabled = enabled;
            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            this.host.SetError(
                "Text",
                !this.UseDnsSd && string.IsNullOrWhiteSpace(this.host.Text) ? ErrorState.Error : ErrorState.Ok,
                AdminStrings.Errors_TextNotWhitespace);

            this.port.SetError(
                "Value",
                !this.UseDnsSd && this.port.Value < 1 ? ErrorState.Error : ErrorState.Ok,
                AdminStrings.Errors_PositiveValue);
        }
    }
}