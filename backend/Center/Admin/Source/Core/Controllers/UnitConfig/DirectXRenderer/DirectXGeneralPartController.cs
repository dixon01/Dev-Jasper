// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXGeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXGeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.DirectXRenderer
{
    using System;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The DirectX general part controller.
    /// </summary>
    public class DirectXGeneralPartController : MultiEditorPartControllerBase
    {
        private const string FallbackTimeoutKey = "FallbackTimeout";
        private const string UseDefaultFallbackImageKey = "UseDefaultFallbackImage";
        private const string FallbackImageKey = "FallbackImage";
        private const string PrimaryScreenIdKey = "PrimaryScreenId";
        private const string SecondaryScreenIdKey = "SecondaryScreenId";

        private TimeSpanEditorViewModel fallbackTimeout;

        private CheckableEditorViewModel useDefaultFallbackImage;

        private ResourceUploadEditorViewModel fallbackImage;

        private TextEditorViewModel secondaryScreenId;

        private TextEditorViewModel primaryScreenId;

        private ScreenResolutionsPartController screenResolutions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXGeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public DirectXGeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.DirectXRenderer.General, parent)
        {
        }

        /// <summary>
        /// Gets the fallback timeout.
        /// </summary>
        public TimeSpan FallbackTimeout
        {
            get
            {
                return this.fallbackTimeout.Value.HasValue
                           ? this.fallbackTimeout.Value.Value
                           : TimeSpan.FromSeconds(30);
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use the default fallback image.
        /// </summary>
        public bool UseDefaultFallbackImage
        {
            get
            {
                return this.useDefaultFallbackImage.IsChecked.HasValue && this.useDefaultFallbackImage.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets the fallback image resource hash.
        /// </summary>
        public string FallbackResourceHash
        {
            get
            {
                return this.fallbackImage.ResourceHash;
            }
        }

        /// <summary>
        /// Gets the primary screen id.
        /// This can be empty if the user didn't specify an id, but it's never null.
        /// </summary>
        public string PrimaryScreenId
        {
            get
            {
                return this.primaryScreenId.Text;
            }
        }

        /// <summary>
        /// Gets the secondary screen id.
        /// This can be empty if the user didn't specify an id, but it's never null.
        /// </summary>
        public string SecondaryScreenId
        {
            get
            {
                return this.secondaryScreenId.Text;
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
            this.fallbackTimeout.Value = partData.GetValue(TimeSpan.FromSeconds(30), FallbackTimeoutKey);
            this.useDefaultFallbackImage.IsChecked = partData.GetValue(true, UseDefaultFallbackImageKey);
            this.fallbackImage.ResourceHash = partData.GetValue((string)null, FallbackImageKey);
            this.primaryScreenId.Text = partData.GetValue((string)null, PrimaryScreenIdKey);
            this.secondaryScreenId.Text = partData.GetValue((string)null, SecondaryScreenIdKey);

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
            partData.SetValue(this.FallbackTimeout, FallbackTimeoutKey);
            partData.SetValue(this.UseDefaultFallbackImage, UseDefaultFallbackImageKey);
            partData.SetValue(this.fallbackImage.ResourceHash, FallbackImageKey);
            partData.SetValue(this.PrimaryScreenId, PrimaryScreenIdKey);
            partData.SetValue(this.SecondaryScreenId, SecondaryScreenIdKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_DirectX_General;
            viewModel.Description = AdminStrings.UnitConfig_DirectX_General_Description;

            this.fallbackTimeout = new TimeSpanEditorViewModel();
            this.fallbackTimeout.IsNullable = false;
            this.fallbackTimeout.Label = AdminStrings.UnitConfig_DirectX_General_FallbackTimeout;
            viewModel.Editors.Add(this.fallbackTimeout);

            this.useDefaultFallbackImage = new CheckableEditorViewModel();
            this.useDefaultFallbackImage.Label = AdminStrings.UnitConfig_DirectX_General_UseDefaultFallbackImage;
            viewModel.Editors.Add(this.useDefaultFallbackImage);

            this.fallbackImage =
                new ResourceUploadEditorViewModel(
                    this.Parent.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>());
            this.fallbackImage.FileFilters = AdminStrings.ImageFileFilters;
            this.fallbackImage.Label = AdminStrings.UnitConfig_DirectX_General_FallbackImage;
            viewModel.Editors.Add(this.fallbackImage);

            this.primaryScreenId = new TextEditorViewModel();
            this.primaryScreenId.Label = AdminStrings.UnitConfig_DirectX_General_PrimaryScreenId;
            this.primaryScreenId.Watermark = AdminStrings.UnitConfig_DirectX_General_ScreenId_Watermark;
            viewModel.Editors.Add(this.primaryScreenId);

            this.secondaryScreenId = new TextEditorViewModel();
            this.secondaryScreenId.Label = AdminStrings.UnitConfig_DirectX_General_SecondaryScreenId;
            this.secondaryScreenId.Watermark = AdminStrings.UnitConfig_DirectX_General_ScreenId_Watermark;
            viewModel.Editors.Add(this.secondaryScreenId);

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
            this.screenResolutions = this.GetPart<ScreenResolutionsPartController>();
            this.screenResolutions.ViewModelUpdated += (s, e) => this.UpdateSecondaryScreenId();
            this.UpdateSecondaryScreenId();
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
            this.fallbackImage.IsEnabled = !this.UseDefaultFallbackImage;

            var errorState = !this.UseDefaultFallbackImage && string.IsNullOrEmpty(this.fallbackImage.ResourceHash)
                                 ? ErrorState.Error
                                 : ErrorState.Ok;
            this.fallbackImage.SetError("Filename", errorState, AdminStrings.Errors_NoResourceSelected);

            errorState = this.FallbackTimeout != TimeSpan.Zero && this.FallbackTimeout < TimeSpan.FromSeconds(20)
                             ? ErrorState.Error
                             : ErrorState.Ok;
            this.fallbackTimeout.SetError("Value", errorState, AdminStrings.UnitConfig_DirectX_General_BadTimeout);
        }

        private void UpdateSecondaryScreenId()
        {
            this.secondaryScreenId.IsEnabled = this.screenResolutions.SecondaryResolution != null;
            this.secondaryScreenId.Watermark = this.secondaryScreenId.IsEnabled
                                                   ? AdminStrings.UnitConfig_DirectX_General_ScreenId_Watermark
                                                   : string.Empty;
        }
    }
}