// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301TimeSyncPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301TimeSyncPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync
{
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareManager.Vdv301;

    /// <summary>
    /// The part controller for time synchronization over VDV 301 (SNTP).
    /// </summary>
    public class Vdv301TimeSyncPartController : SntpTimeSyncPartControllerBase<TimeSyncConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301TimeSyncPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public Vdv301TimeSyncPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.TimeSync.Vdv301, IncomingData.Vdv301, parent)
        {
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
            viewModel.DisplayName = AdminStrings.UnitConfig_TimeSync_Vdv301;
            viewModel.Description = AdminStrings.UnitConfig_TimeSync_Vdv301_Description;

            return viewModel;
        }
    }
}