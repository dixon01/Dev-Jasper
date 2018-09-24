// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemConnectionPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemConnectionPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Software
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The background system connection part controller.
    /// </summary>
    public class BackgroundSystemConnectionPartController : MultiEditorPartControllerBase
    {
        private const string ConnectKey = "Connect";

        private CheckableEditorViewModel connect;

        private IncomingPartController incoming;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSystemConnectionPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public BackgroundSystemConnectionPartController(SoftwareCategoryController parent)
            : base(UnitConfigKeys.Software.BackgroundSystemConnection, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the Unit should connect to the BGS.
        /// </summary>
        public bool ShouldConnect
        {
            get
            {
                return this.connect.IsChecked.HasValue && this.connect.IsChecked.Value;
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
            this.connect.IsChecked = partData.GetValue(false, ConnectKey);
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
            partData.SetValue(this.ShouldConnect, ConnectKey);
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.incoming = this.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateErrors();

            var validator = new VersionedSettingValidator(
                this.connect,
                false,
                PackageIds.Motion.SystemManager,
                SoftwareVersions.SystemManager.MediSupportsGateways,
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
            var viewModel = new MultiEditorPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Software_BGSConnection;
            viewModel.Description = AdminStrings.UnitConfig_Software_BGSConnection_Description;
            viewModel.IsVisible = true;

            this.connect = new CheckableEditorViewModel();
            this.connect.Label = AdminStrings.UnitConfig_Software_BGSConnection_Connect;
            this.connect.IsThreeState = false;
            this.connect.PropertyChanged += (s, e) => this.UpdateErrors();
            viewModel.Editors.Add(this.connect);

            return viewModel;
        }

        private void UpdateErrors()
        {
            if (this.incoming == null)
            {
                return;
            }

            var state = this.incoming.HasSelected(IncomingData.Medi) && this.ShouldConnect
                            ? ErrorState.Warning
                            : ErrorState.Ok;
            this.connect.SetError("IsChecked", state, AdminStrings.UnitConfig_Software_BGSConnection_Connect_Slave);
        }
    }
}