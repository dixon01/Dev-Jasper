// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301GeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301GeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The VDV 301 general part controller.
    /// </summary>
    public class Vdv301GeneralPartController : FilteredPartControllerBase
    {
        private const string SubscriptionTimeoutKey = "SubscriptionTimeout";
        private const string ValidateHttpRequestsKey = "ValidateHttpRequests";
        private const string ValidateHttpResponsesKey = "ValidateHttpResponses";
        private const string VerifyVersionKey = "VerifyVersion";

        private TimeSpanEditorViewModel subscriptionTimeout;

        private CheckableEditorViewModel validateHttpRequests;

        private CheckableEditorViewModel validateHttpResponses;

        private CheckableEditorViewModel verifyVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301GeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public Vdv301GeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Vdv301Protocol.General, parent)
        {
        }

        /// <summary>
        /// Gets the subscription timeout.
        /// </summary>
        public TimeSpan? SubscriptionTimeout
        {
            get
            {
                return this.subscriptionTimeout.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to validate HTTP requests.
        /// </summary>
        public bool ValidateHttpRequests
        {
            get
            {
                return this.validateHttpRequests.IsChecked.HasValue && this.validateHttpRequests.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to validate HTTP responses.
        /// </summary>
        public bool ValidateHttpResponses
        {
            get
            {
                return this.validateHttpResponses.IsChecked.HasValue && this.validateHttpResponses.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to verify the protocol version.
        /// </summary>
        public bool VerifyVersion
        {
            get
            {
                return this.verifyVersion.IsChecked.HasValue && this.verifyVersion.IsChecked.Value;
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
            this.subscriptionTimeout.Value = partData.GetValue((TimeSpan?)null, SubscriptionTimeoutKey);
            this.validateHttpRequests.IsChecked = partData.GetValue(false, ValidateHttpRequestsKey);
            this.validateHttpResponses.IsChecked = partData.GetValue(false, ValidateHttpResponsesKey);
            this.verifyVersion.IsChecked = partData.GetValue(false, VerifyVersionKey);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.SubscriptionTimeout, SubscriptionTimeoutKey);
            partData.SetValue(this.ValidateHttpRequests, ValidateHttpRequestsKey);
            partData.SetValue(this.ValidateHttpResponses, ValidateHttpResponsesKey);
            partData.SetValue(this.VerifyVersion, VerifyVersionKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Vdv301_General;
            viewModel.Description = AdminStrings.UnitConfig_Vdv301_General_Description;

            this.subscriptionTimeout = new TimeSpanEditorViewModel();
            this.subscriptionTimeout.Label = AdminStrings.UnitConfig_Vdv301_General_SubscriptionTimeout;
            this.subscriptionTimeout.IsNullable = true;
            viewModel.Editors.Add(this.subscriptionTimeout);

            // TODO: add the editors when we are allowed again to edit those properties (i.e. valid XSD)
            this.validateHttpRequests = new CheckableEditorViewModel();
            this.validateHttpRequests.Label = AdminStrings.UnitConfig_Vdv301_General_ValidateHttpRequests;
            this.validateHttpRequests.IsThreeState = false;
            ////viewModel.Editors.Add(this.validateHttpRequests);

            this.validateHttpResponses = new CheckableEditorViewModel();
            this.validateHttpResponses.Label = AdminStrings.UnitConfig_Vdv301_General_ValidateHttpResponses;
            this.validateHttpResponses.IsThreeState = false;
            ////viewModel.Editors.Add(this.validateHttpResponses);

            this.verifyVersion = new CheckableEditorViewModel();
            this.verifyVersion.Label = AdminStrings.UnitConfig_Vdv301_General_VerifyVersion;
            this.verifyVersion.IsThreeState = false;
            ////viewModel.Editors.Add(this.verifyVersion);

            return viewModel;
        }
    }
}