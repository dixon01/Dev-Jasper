// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerGeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerGeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Composer
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The composer part controller.
    /// </summary>
    public class ComposerGeneralPartController : MultiEditorPartControllerBase
    {
        private const string XimpleInactivityEnabledKey = "XimpleInactivityEnabled";
        private const string AtStartupKey = "AtStartup";
        private const string XimpleInactivityTimeoutKey = "XimpleInactivityTimeout";
        private const string PresentationLoggingEnabledKey = "PresentationLoggingEnabled";

        private CheckableEditorViewModel ximpleInactivityEnabled;

        private CheckableEditorViewModel atStartup;

        private TimeSpanEditorViewModel ximpleInactivityTimeout;

        private CheckableEditorViewModel presentationLoggingEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerGeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public ComposerGeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Composer.General, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether Ximple inactivity is enabled.
        /// </summary>
        public bool XimpleInactivityEnabled
        {
            get
            {
                return this.ximpleInactivityEnabled.IsChecked.HasValue && this.ximpleInactivityEnabled.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Ximple inactivity should be set at startup.
        /// </summary>
        public bool AtStartup
        {
            get
            {
                return this.atStartup.IsChecked.HasValue && this.atStartup.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets the Ximple inactivity timeout.
        /// </summary>
        public TimeSpan XimpleInactivityTimeout
        {
            get
            {
                return this.ximpleInactivityTimeout.Value.HasValue
                           ? this.ximpleInactivityTimeout.Value.Value
                           : TimeSpan.Zero;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether or not Presentation Play Logging is enabled
        /// </summary>
        public bool PresentationLoggingEnabled
        {
            get
            {
                return this.presentationLoggingEnabled.IsChecked.HasValue && this.presentationLoggingEnabled.IsChecked.Value;
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
            this.ximpleInactivityEnabled.IsChecked = partData.GetValue(true, XimpleInactivityEnabledKey);
            this.atStartup.IsChecked = partData.GetValue(false, AtStartupKey);
            this.ximpleInactivityTimeout.Value = partData.GetValue(
                TimeSpan.FromSeconds(60),
                XimpleInactivityTimeoutKey);
            this.presentationLoggingEnabled.IsChecked = partData.GetValue(true, PresentationLoggingEnabledKey);

            this.UpdateEditors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.XimpleInactivityEnabled, XimpleInactivityEnabledKey);
            partData.SetValue(this.AtStartup, AtStartupKey);
            partData.SetValue(this.XimpleInactivityTimeout, XimpleInactivityTimeoutKey);
            partData.SetValue(this.PresentationLoggingEnabled, PresentationLoggingEnabledKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Composer_General;
            viewModel.Description = AdminStrings.UnitConfig_Composer_General_Description;

            this.ximpleInactivityEnabled = new CheckableEditorViewModel();
            this.ximpleInactivityEnabled.IsThreeState = false;
            this.ximpleInactivityEnabled.Label = AdminStrings.UnitConfig_Composer_XimpleInactivityEnabled;
            viewModel.Editors.Add(this.ximpleInactivityEnabled);

            this.atStartup = new CheckableEditorViewModel();
            this.atStartup.IsThreeState = false;
            this.atStartup.Label = AdminStrings.UnitConfig_Composer_XimpleInactivityAtStartup;
            viewModel.Editors.Add(this.atStartup);

            this.ximpleInactivityTimeout = new TimeSpanEditorViewModel();
            this.ximpleInactivityTimeout.IsNullable = false;
            this.ximpleInactivityTimeout.Label = AdminStrings.UnitConfig_Composer_XimpleInactivityTimeout;
            viewModel.Editors.Add(this.ximpleInactivityTimeout);
            
            this.presentationLoggingEnabled = new CheckableEditorViewModel();
            this.presentationLoggingEnabled.IsThreeState = false;
            this.presentationLoggingEnabled.Label = AdminStrings.UnitConfig_Composer_PresentationLoggingEnabled;
            viewModel.Editors.Add(this.presentationLoggingEnabled);

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
            this.UpdateEditors();
        }

        private void UpdateEditors()
        {
            this.atStartup.IsEnabled = this.XimpleInactivityEnabled;
            this.ximpleInactivityTimeout.IsEnabled = this.XimpleInactivityEnabled;
            this.presentationLoggingEnabled.IsChecked = this.PresentationLoggingEnabled;
            
            if (!this.XimpleInactivityEnabled
                || (this.ximpleInactivityTimeout.Value.HasValue
                    && this.ximpleInactivityTimeout.Value.Value > TimeSpan.Zero))
            {
                this.ximpleInactivityTimeout.RemoveError("Value", AdminStrings.Errors_PositiveValue);
            }
            else
            {
                this.ximpleInactivityTimeout.SetError(
                    "Value",
                    this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing,
                    AdminStrings.Errors_PositiveValue);
            }
        }
    }
}