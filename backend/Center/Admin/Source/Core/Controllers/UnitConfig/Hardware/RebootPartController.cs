// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RebootPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RebootPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The part controller responsible for system reboots (SystemManager.xml).
    /// </summary>
    public class RebootPartController : MultiEditorPartControllerBase
    {
        private const string RebootAtKey = "RebootAt";
        private const string RebootAfterKey = "RebootAfter";

        private TimeSpanEditorViewModel rebootAt;

        private TimeSpanEditorViewModel rebootAfter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RebootPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public RebootPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.Reboot, parent)
        {
        }

        /// <summary>
        /// Gets the time at which the system should reboot or null if disabled.
        /// </summary>
        public TimeSpan? RebootAt
        {
            get
            {
                return this.rebootAt.Value;
            }
        }

        /// <summary>
        /// Gets the time after which the system should reboot or null if disabled.
        /// </summary>
        public TimeSpan? RebootAfter
        {
            get
            {
                return this.rebootAfter.Value;
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
            this.rebootAt.Value = partData.GetValue((TimeSpan?)null, RebootAtKey);
            this.rebootAfter.Value = partData.GetValue((TimeSpan?)null, RebootAfterKey);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.rebootAt.Value, RebootAtKey);
            partData.SetValue(this.rebootAfter.Value, RebootAfterKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Hardware_Reboot;
            viewModel.Description = AdminStrings.UnitConfig_Hardware_Reboot_Description;
            viewModel.IsVisible = true;

            this.rebootAt = new TimeSpanEditorViewModel();
            this.rebootAt.IsNullable = true;
            this.rebootAt.Label = AdminStrings.UnitConfig_Hardware_Reboot_RebootAt;
            viewModel.Editors.Add(this.rebootAt);

            this.rebootAfter = new TimeSpanEditorViewModel();
            this.rebootAfter.IsNullable = true;
            this.rebootAfter.Label = AdminStrings.UnitConfig_Hardware_Reboot_RebootAfter;
            viewModel.Editors.Add(this.rebootAfter);

            return viewModel;
        }
    }
}