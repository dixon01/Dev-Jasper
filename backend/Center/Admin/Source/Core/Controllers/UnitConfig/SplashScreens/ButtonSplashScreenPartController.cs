// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ButtonSplashScreenPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ButtonSplashScreenPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SplashScreens
{
    using System;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The button splash screen part controller.
    /// </summary>
    public class ButtonSplashScreenPartController : SplashScreenPartControllerBase
    {
        private IncomingPartController incoming;

        private OutgoingPartController outgoing;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonSplashScreenPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public ButtonSplashScreenPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.SplashScreens.Button, parent)
        {
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

            var pc = descriptor.Platform as PcPlatformDescriptorBase;
            if (pc == null)
            {
                this.ViewModel.IsVisible = false;
                return;
            }

            if (pc.HasGenericButton)
            {
                this.ViewModel.DisplayName = AdminStrings.UnitConfig_SplashScreens_LocalButton;
                this.ViewModel.Description = AdminStrings.UnitConfig_SplashScreens_LocalButton_Description;
                return;
            }

            this.ViewModel.DisplayName = AdminStrings.UnitConfig_SplashScreens_RemoteButton;
            this.ViewModel.Description = AdminStrings.UnitConfig_SplashScreens_RemoteButton_Description;

            this.incoming = this.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += this.SoftwareOnViewModelUpdated;

            this.outgoing = this.GetPart<OutgoingPartController>();
            this.outgoing.ViewModelUpdated += this.SoftwareOnViewModelUpdated;

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
            var viewModel = base.CreateViewModel();
            return viewModel;
        }

        private void UpdateVisibility()
        {
            // only show this splash screen if we have Medi enabled, otherwise there is no button in the system
            this.ViewModel.IsVisible = this.incoming.HasSelected(IncomingData.Medi)
                                       || this.outgoing.HasSelected(OutgoingData.Medi);
        }

        private void SoftwareOnViewModelUpdated(object sender, EventArgs e)
        {
            this.UpdateVisibility();
        }
    }
}