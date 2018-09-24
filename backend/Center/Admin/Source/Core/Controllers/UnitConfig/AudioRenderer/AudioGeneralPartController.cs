// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioGeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioGeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.AudioRenderer
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The audio general part controller.
    /// </summary>
    public class AudioGeneralPartController : FilteredPartControllerBase
    {
        private const string NumberOfChannelsKey = "NumberOfChannels";
        private const string UseAcapelaKey = "UseAcapela";

        private NumberEditorViewModel numberOfChannels;

        private CheckableEditorViewModel useAcapela;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioGeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public AudioGeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.AudioRenderer.General, parent)
        {
        }

        /// <summary>
        /// Gets the number of channels.
        /// </summary>
        public int NumberOfChannels
        {
            get
            {
                return (int)this.numberOfChannels.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use acapela.
        /// </summary>
        public bool UseAcapela
        {
            get
            {
                return this.useAcapela.IsChecked.HasValue && this.useAcapela.IsChecked.Value;
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
            this.numberOfChannels.Value = partData.GetValue(1, NumberOfChannelsKey);
            this.useAcapela.IsChecked = partData.GetValue(false, UseAcapelaKey);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.NumberOfChannels, NumberOfChannelsKey);
            partData.SetValue(this.UseAcapela, UseAcapelaKey);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Audio_General;
            viewModel.Description = AdminStrings.UnitConfig_Audio_General_Description;

            this.numberOfChannels = new NumberEditorViewModel();
            this.numberOfChannels.Label = AdminStrings.UnitConfig_Audio_General_NumChannels;
            this.numberOfChannels.IsInteger = true;
            this.numberOfChannels.MinValue = 1;
            this.numberOfChannels.MaxValue = AudioRendererCategoryController.MaxChannelCount;
            viewModel.Editors.Add(this.numberOfChannels);

            this.useAcapela = new CheckableEditorViewModel();
            this.useAcapela.Label = AdminStrings.UnitConfig_Audio_General_UseAcapela;
            this.useAcapela.IsThreeState = false;
            viewModel.Editors.Add(this.useAcapela);

            return viewModel;
        }
    }
}