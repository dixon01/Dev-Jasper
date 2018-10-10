// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistencePartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PersistencePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The persistence part controller.
    /// </summary>
    public class PersistencePartController : FilteredPartControllerBase
    {
        private const string IsEnabledKey = "IsEnabled";
        private const string DefaultValidityKey = "DefaultValidity";

        private CheckableEditorViewModel enabled;

        private TimeSpanEditorViewModel defaultValidity;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistencePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public PersistencePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Protran.Persistence, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether persistence is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.enabled.IsChecked.HasValue && this.enabled.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets the default validity.
        /// </summary>
        public TimeSpan DefaultValidity
        {
            get
            {
                // ReSharper disable once PossibleInvalidOperationException
                return this.defaultValidity.Value.Value;
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
            this.enabled.IsChecked = partData.GetValue(false, IsEnabledKey);
            this.defaultValidity.Value = partData.GetValue(TimeSpan.FromMinutes(10), DefaultValidityKey);
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
            partData.SetValue(this.IsEnabled, IsEnabledKey);
            partData.SetValue(this.DefaultValidity, DefaultValidityKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Protran_Persistence;
            viewModel.Description = AdminStrings.UnitConfig_Protran_Persistence_Description;

            this.enabled = new CheckableEditorViewModel();
            this.enabled.Label = AdminStrings.UnitConfig_Protran_Persistence_Enabled;
            this.enabled.IsThreeState = false;
            viewModel.Editors.Add(this.enabled);

            this.defaultValidity = new TimeSpanEditorViewModel();
            this.defaultValidity.Label = AdminStrings.UnitConfig_Protran_Persistence_DefaultValidity;
            this.defaultValidity.IsNullable = false;
            viewModel.Editors.Add(this.defaultValidity);

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
            this.UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            this.defaultValidity.IsEnabled = this.IsEnabled;
        }
    }
}