// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioChannelPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioChannelPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.AudioRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The audio channel part controller.
    /// </summary>
    public class AudioChannelPartController : FilteredPartControllerBase
    {
        private const string SpeakerPortsKey = "SpeakerPorts";
        private readonly int index;

        private OutputsPartController outputsPart;
        private AudioGeneralPartController generalPart;

        private MultiSelectEditorViewModel speakerPorts;

        private bool parentVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioChannelPartController"/> class.
        /// </summary>
        /// <param name="index">
        /// The index of the audio channel (indexed from 1).
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public AudioChannelPartController(int index, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.AudioRenderer.ChannelFormat, index), parent)
        {
            this.index = index;
        }

        /// <summary>
        /// Gets speaker ports.
        /// </summary>
        /// <returns>
        /// The list of speaker ports to be enabled for this channel.
        /// </returns>
        public IEnumerable<string> GetSpeakerPorts()
        {
            var outputs = this.outputsPart.GetOutputs();
            return this.speakerPorts.GetCheckedValues().Cast<int>().Select(i => outputs[i]);
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            var ports = partData.GetValue(string.Empty, SpeakerPortsKey).Split(';');
            foreach (var option in this.speakerPorts.Options)
            {
                option.IsChecked = ports.Contains(option.Value.ToString());
            }

            this.UpdateErrors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(string.Join(";", this.speakerPorts.GetCheckedValues().Cast<int>()), SpeakerPortsKey);
        }

        /// <summary>
        /// Update the visibility of this part.
        /// </summary>
        /// <param name="visible">
        /// A flag indicating if the view model of this part should be visible.
        /// </param>
        public override void UpdateVisibility(bool visible)
        {
            this.parentVisible = visible;
            this.UpdateVisibility();
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.outputsPart = this.GetPart<OutputsPartController>();
            this.outputsPart.ViewModelUpdated += (s, e) => this.UpdateOutputNames();

            this.generalPart = this.GetPart<AudioGeneralPartController>();
            this.generalPart.ViewModelUpdated += (s, e) => this.UpdateVisibility();

            foreach (var output in descriptor.Platform.Outputs)
            {
                this.speakerPorts.Options.Add(new CheckableOptionViewModel(output.Name, output.Index));
            }

            this.UpdateOutputNames();
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
            viewModel.DisplayName = string.Format(AdminStrings.UnitConfig_Audio_Channel_Format, this.index);
            viewModel.Description = AdminStrings.UnitConfig_Audio_Channel_Description;

            this.speakerPorts = new MultiSelectEditorViewModel();
            this.speakerPorts.Label = AdminStrings.UnitConfig_Audio_Channel_SpeakerPorts;
            viewModel.Editors.Add(this.speakerPorts);

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
            this.UpdateErrors();
        }

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.parentVisible && this.generalPart.NumberOfChannels >= this.index;
        }

        private void UpdateOutputNames()
        {
            var outputs = this.outputsPart.GetOutputs();
            foreach (var option in this.speakerPorts.Options)
            {
                string name;
                if (outputs.TryGetValue((int)option.Value, out name))
                {
                    option.Label = name;
                }
            }
        }

        private void UpdateErrors()
        {
#if __UseLuminatorTftDisplay
            var count = this.speakerPorts.CheckedOptionsCount;
            var errorState = ErrorState.Ok;

            if (this.Key.Contains("3"))
            {
                if (count != 2)
                {
                    errorState = ErrorState.Error;
                    this.speakerPorts.SetError("Options", errorState, AdminStrings.Errors_SelectBoth);
                }
                else
                {
                    this.speakerPorts.SetError("Options", errorState, AdminStrings.Errors_SelectBoth);
                }
            }
            else
            {
                switch (count)
                {
                    case 0:
                        errorState = this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing;
                        this.speakerPorts.SetError("Options", errorState, AdminStrings.Errors_SelectOneAtLeast);
                        break;

                    case 1:
                        this.speakerPorts.SetError("Options", errorState, AdminStrings.Errors_SelectOneAtLeast);
                        this.speakerPorts.SetError("Options", errorState, AdminStrings.Errors_SelectOnlyOne);
                        break;

                    case 2:
                        errorState = ErrorState.Error;
                        this.speakerPorts.SetError("Options", errorState, AdminStrings.Errors_SelectOnlyOne);
                        break;

                }
            }
#else
            var errorState = this.speakerPorts.CheckedOptionsCount > 0
                                 ? ErrorState.Ok
                                 : (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing);
            this.speakerPorts.SetError("Options", errorState, AdminStrings.Errors_SelectOneAtLeast);
#endif
        }
    }
}