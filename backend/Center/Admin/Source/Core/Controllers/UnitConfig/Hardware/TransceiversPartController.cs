// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransceiversPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransceiversPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.HardwareManager.Mgi;

    /// <summary>
    /// The transceivers controller.
    /// </summary>
    public class TransceiversPartController : MultiEditorPartControllerBase
    {
        private const string TypeKeyFormat = "{0}.Type";
        private const string ModeKeyFormat = "{0}.Mode";
        private const string TermKeyFormat = "{0}.Term";

        private readonly List<SelectionEditorViewModel> typeViewModels = new List<SelectionEditorViewModel>();
        private readonly List<SelectionEditorViewModel> modeViewModels = new List<SelectionEditorViewModel>();
        private readonly List<SelectionEditorViewModel> termViewModels = new List<SelectionEditorViewModel>();

        private List<MultiProtocolTransceiverDescriptor> transceivers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransceiversPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public TransceiversPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.Transceivers, parent)
        {
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            if (this.transceivers == null)
            {
                return;
            }

            for (int i = 0; i < this.transceivers.Count; i++)
            {
                var transceiver = this.transceivers[i];
                var trim = partData.GetEnumValue(
                    TransceiverType.RS485,
                    string.Format(TypeKeyFormat, transceiver.Index));
                this.typeViewModels[i].SelectValue(trim);

                var mode = partData.GetEnumValue(
                    TransceiverMode.FullDuplex,
                    string.Format(ModeKeyFormat, transceiver.Index));
                this.modeViewModels[i].SelectValue(mode);

                var term = partData.GetValue(false, string.Format(TermKeyFormat, transceiver.Index));
                this.termViewModels[i].SelectValue(term);
            }
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            if (this.transceivers == null)
            {
                return;
            }

            for (int i = 0; i < this.transceivers.Count; i++)
            {
                var transceiver = this.transceivers[i];
                partData.SetEnumValue(
                    (TransceiverType)this.typeViewModels[i].SelectedValue,
                    string.Format(TypeKeyFormat, transceiver.Index));

                partData.SetEnumValue(
                    (TransceiverMode)this.modeViewModels[i].SelectedValue,
                    string.Format(ModeKeyFormat, transceiver.Index));

                partData.SetValue(
                    (bool)this.termViewModels[i].SelectedValue,
                    string.Format(TermKeyFormat, transceiver.Index));
            }
        }

        /// <summary>
        /// Gets all transceiver configurations configured in this part.
        /// </summary>
        /// <returns>
        /// The the list of configurations.
        /// </returns>
        public IEnumerable<TransceiverConfig> GetTransceiverConfigs()
        {
            return
                this.transceivers.Select(
                    (t, i) =>
                    new TransceiverConfig
                        {
                            Index = t.Index + 1,
                            Type =
                                (TransceiverType)
                                (this.typeViewModels[i].SelectedValue ?? TransceiverType.RS232),
                            Mode =
                                (TransceiverMode)
                                (this.modeViewModels[i].SelectedValue ?? TransceiverMode.FullDuplex),
                            Termination = (bool)(this.termViewModels[i].SelectedValue ?? true)
                        });
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel
                                {
                                    DisplayName = AdminStrings.UnitConfig_Hardware_Transceivers,
                                    Description = AdminStrings.UnitConfig_Hardware_Transceivers_Description,
                                    IsVisible = false
                                };
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
            var infoVision = descriptor.Platform as InfoVisionPlatformDescriptor;
            if (infoVision == null)
            {
                return;
            }

            this.transceivers = infoVision.Transceivers.ToList();
            foreach (var transceiver in infoVision.Transceivers)
            {
                this.ViewModel.IsVisible = true;
                var title = new TitleEditorViewModel();
                title.Label = string.Format(
                    AdminStrings.UnitConfig_Hardware_Transceivers_LabelFormat, transceiver.Index + 1);
                this.ViewModel.Editors.Add(title);
                var type = new SelectionEditorViewModel
                               {
                                   Label = AdminStrings.UnitConfig_Hardware_Transceivers_Type,
                                   Options =
                                       {
                                           new SelectionOptionViewModel("RS-485", TransceiverType.RS485),
                                           new SelectionOptionViewModel("RS-232", TransceiverType.RS232)
                                       }
                               };
                this.ViewModel.Editors.Add(type);
                this.typeViewModels.Add(type);

                var mode = new SelectionEditorViewModel
                               {
                                   Label = AdminStrings.UnitConfig_Hardware_Transceivers_Mode,
                                   Options =
                                       {
                                           new SelectionOptionViewModel(
                                               AdminStrings.UnitConfig_Hardware_Transceivers_Mode_FullDuplex,
                                               TransceiverMode.FullDuplex),
                                           new SelectionOptionViewModel(
                                               AdminStrings.UnitConfig_Hardware_Transceivers_Mode_HalfDuplex,
                                               TransceiverMode.HalfDuplex)
                                       }
                               };
                this.ViewModel.Editors.Add(mode);
                this.modeViewModels.Add(mode);

                var term = new SelectionEditorViewModel
                               {
                                   Label = AdminStrings.UnitConfig_Hardware_Transceivers_Term,
                                   Options =
                                       {
                                           new SelectionOptionViewModel(
                                               AdminStrings.UnitConfig_Hardware_Transceivers_Term_Off,
                                               false),
                                           new SelectionOptionViewModel(
                                               AdminStrings.UnitConfig_Hardware_Transceivers_Term_On,
                                               true)
                                       }
                               };
                this.ViewModel.Editors.Add(term);
                this.termViewModels.Add(term);
            }
        }
    }
}