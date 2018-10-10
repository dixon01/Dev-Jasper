// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediSlavePartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediSlavePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Software
{
    using System;
    using System.Net;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The Medi slave configuration part controller.
    /// </summary>
    public class MediSlavePartController : MultiEditorPartControllerBase
    {
        private const string MasterAddressKey = "MasterAddress";

        private TextEditorViewModel masterAddress;

        private IncomingPartController incoming;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediSlavePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public MediSlavePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Software.MediSlave, parent)
        {
        }

        /// <summary>
        /// Gets the master address.
        /// </summary>
        public string MasterAddress
        {
            get
            {
                return this.masterAddress.Text;
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
            this.masterAddress.Text = partData.GetValue(string.Empty, MasterAddressKey);
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
            partData.SetValue(this.masterAddress.Text, MasterAddressKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Software_MediSlave;
            viewModel.Description = AdminStrings.UnitConfig_Software_MediSlave_Description;

            this.masterAddress = new TextEditorViewModel();
            this.masterAddress.Label = AdminStrings.UnitConfig_Software_MediSlave_MasterAddress;
            viewModel.Editors.Add(this.masterAddress);

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
            base.Prepare(descriptor);
            this.incoming = this.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
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

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.incoming.HasSelected(IncomingData.Medi);
        }

        private void UpdateErrors()
        {
            IPAddress address;
            var valid = IPAddress.TryParse(this.masterAddress.Text, out address);
            this.masterAddress.SetError(
                "Text",
                valid ? ErrorState.Ok : (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing),
                AdminStrings.Errors_ValidIpAddress);
        }
    }
}