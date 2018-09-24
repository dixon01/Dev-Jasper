// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXImagePartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXImagePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.DirectXRenderer
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;

    /// <summary>
    /// The DirectX image part controller.
    /// </summary>
    public class DirectXImagePartController : MultiEditorPartControllerBase
    {
        private const string CacheTimeoutKey = "CacheTimeout";
        private const string CacheMegaBytesKey = "CacheMegaBytes";
        private const string MegaBytesPerBitmapKey = "MegaBytesPerBitmap";
        private const string PreloadImagesKey = "PreloadImages";

        private const decimal MegaByte = 1024 * 1024;

        private TimeSpanEditorViewModel cacheTimeout;

        private NumberEditorViewModel cacheMegaBytes;

        private NumberEditorViewModel megaBytesPerBitmap;

        private CheckableEditorViewModel preloadImages;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXImagePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public DirectXImagePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.DirectXRenderer.Image, parent)
        {
        }

        /// <summary>
        /// Gets the bitmap cache timeout.
        /// </summary>
        public TimeSpan BitmapCacheTimeout
        {
            get
            {
                return this.cacheTimeout.Value.HasValue ? this.cacheTimeout.Value.Value : TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Gets the maximum bitmap cache size in bytes.
        /// </summary>
        public long MaxBitmapCacheBytes
        {
            get
            {
                return (long)(this.cacheMegaBytes.Value * MegaByte);
            }
        }

        /// <summary>
        /// Gets the maximum cache bytes per bitmap.
        /// </summary>
        public int MaxCacheBytesPerBitmap
        {
            get
            {
                return (int)(this.megaBytesPerBitmap.Value * MegaByte);
            }
        }

        /// <summary>
        /// Gets a value indicating whether images should be pre-loaded at start-up.
        /// </summary>
        public bool PreloadImages
        {
            get
            {
                return this.preloadImages.IsChecked.HasValue && this.preloadImages.IsChecked.Value;
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
            var defaultConfig = new ImageConfig();
            this.cacheTimeout.Value = partData.GetValue(defaultConfig.BitmapCacheTimeout, CacheTimeoutKey);

            var defaultCacheMb = defaultConfig.MaxBitmapCacheBytes / MegaByte;
            if (this.Parent.Parent.HardwareDescriptor.Platform is InformPlatformDescriptor)
            {
                // reduce the cache size to 120 MB for Inform hardware (limited RAM)
                defaultCacheMb = 120;
            }

            this.cacheMegaBytes.Value = partData.GetValue(defaultCacheMb, CacheMegaBytesKey);
            this.megaBytesPerBitmap.Value = partData.GetValue(
                defaultConfig.MaxCacheBytesPerBitmap / MegaByte,
                MegaBytesPerBitmapKey);

            this.preloadImages.IsChecked = partData.GetValue(false, PreloadImagesKey);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.BitmapCacheTimeout, CacheTimeoutKey);
            partData.SetValue(this.MaxBitmapCacheBytes / MegaByte, CacheMegaBytesKey);
            partData.SetValue(this.MaxCacheBytesPerBitmap / MegaByte, MegaBytesPerBitmapKey);
            partData.SetValue(this.PreloadImages, PreloadImagesKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_DirectX_Image;
            viewModel.Description = AdminStrings.UnitConfig_DirectX_Image_Description;

            this.cacheTimeout = new TimeSpanEditorViewModel();
            this.cacheTimeout.Label = AdminStrings.UnitConfig_DirectX_Image_CacheTimeout;
            this.cacheTimeout.IsNullable = false;
            viewModel.Editors.Add(this.cacheTimeout);

            this.cacheMegaBytes = new NumberEditorViewModel();
            this.cacheMegaBytes.Label = AdminStrings.UnitConfig_DirectX_Image_CacheMegaBytes;
            this.cacheMegaBytes.MinValue = 0;
            this.cacheMegaBytes.MaxValue = 4 * 1024; // 4 GB
            viewModel.Editors.Add(this.cacheMegaBytes);

            this.megaBytesPerBitmap = new NumberEditorViewModel();
            this.megaBytesPerBitmap.Label = AdminStrings.UnitConfig_DirectX_Image_MegaBytesPerBitmap;
            this.megaBytesPerBitmap.MinValue = 0;
            this.megaBytesPerBitmap.MaxValue = 1024; // 1 GB
            viewModel.Editors.Add(this.megaBytesPerBitmap);

            this.preloadImages = new CheckableEditorViewModel();
            this.preloadImages.Label = AdminStrings.UnitConfig_DirectX_Image_PreloadImages;
            this.preloadImages.IsThreeState = false;
            viewModel.Editors.Add(this.preloadImages);

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

            var validator = new VersionedSettingValidator(
                this.preloadImages,
                false,
                PackageIds.Motion.DirectXRenderer,
                SoftwareVersions.Infomedia.DirectXPreloadImages,
                this.Parent.Parent);
            validator.Start();
        }
    }
}