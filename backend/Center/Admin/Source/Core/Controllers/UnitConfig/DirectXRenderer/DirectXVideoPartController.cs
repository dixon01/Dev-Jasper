// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectXVideoPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectXVideoPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.DirectXRenderer
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;

    /// <summary>
    /// The DirectX video part controller.
    /// </summary>
    public class DirectXVideoPartController : MultiEditorPartControllerBase
    {
        private const string VideoModeKey = "VideoMode";
        private const VideoMode NoVideoMode = (VideoMode)0xFFFF;

        private SelectionEditorViewModel videoMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXVideoPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public DirectXVideoPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.DirectXRenderer.Video, parent)
        {
        }

        /// <summary>
        /// Gets the video rendering mode.
        /// </summary>
        public VideoMode? VideoRenderingMode
        {
            get
            {
                return this.videoMode.SelectedValue.Equals(NoVideoMode)
                           ? (VideoMode?)null
                           : (VideoMode)this.videoMode.SelectedValue;
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
            this.videoMode.SelectValue(partData.GetEnumValue(NoVideoMode, VideoModeKey));
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetEnumValue(
                this.videoMode.SelectedValue.Equals(NoVideoMode)
                    ? NoVideoMode
                    : (VideoMode)this.videoMode.SelectedValue,
                VideoModeKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_DirectX_Video;
            viewModel.Description = AdminStrings.UnitConfig_DirectX_Video_Description;

            this.videoMode = new SelectionEditorViewModel();
            this.videoMode.Label = AdminStrings.UnitConfig_DirectX_Video_VideoMode;
            this.videoMode.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_DirectX_Video_VideoMode_None, NoVideoMode));
            this.videoMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Video_VideoMode_DirectShow,
                    VideoMode.DirectShow));
            this.videoMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Video_VideoMode_DirectXWindow,
                    VideoMode.DirectXWindow));
            this.videoMode.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_DirectX_Video_VideoMode_VlcWindow,
                    VideoMode.VlcWindow));
            viewModel.Editors.Add(this.videoMode);

            return viewModel;
        }
    }
}