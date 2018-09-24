// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DviLevelShiftersPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DviLevelShiftersPartController type.
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
    /// The dvi level shifters part controller.
    /// </summary>
    public class DviLevelShiftersPartController : MultiEditorPartControllerBase
    {
        private const string TrimKeyFormat = "{0}.Trim";
        private const string LevelKeyFormat = "{0}.Level";

        private static readonly int[] DviLevels = { 0, 2, 4, 6 };

        private readonly List<SelectionEditorViewModel> trimViewModels = new List<SelectionEditorViewModel>();
        private readonly List<SelectionEditorViewModel> levelViewModels = new List<SelectionEditorViewModel>();

        private List<DisplayAdapterDescriptor> dviPorts;

        /// <summary>
        /// Initializes a new instance of the <see cref="DviLevelShiftersPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DviLevelShiftersPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.DviLevelShifters, parent)
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
            if (this.dviPorts == null)
            {
                return;
            }

            for (int i = 0; i < this.dviPorts.Count; i++)
            {
                var dviPort = this.dviPorts[i];
                var trim = partData.GetEnumValue(
                    TrimOptions.StandardCurrent,
                    string.Format(TrimKeyFormat, dviPort.Index));
                this.trimViewModels[i].SelectValue(trim);

                var level = (int)partData.GetValue(DviLevels[0], string.Format(LevelKeyFormat, dviPort.Index));
                this.levelViewModels[i].SelectValue(level);
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
            if (this.dviPorts == null)
            {
                return;
            }

            for (int i = 0; i < this.dviPorts.Count; i++)
            {
                var dviPort = this.dviPorts[i];
                partData.SetEnumValue(
                    (TrimOptions)this.trimViewModels[i].SelectedValue,
                    string.Format(TrimKeyFormat, dviPort.Index));

                partData.SetEnumValue(
                    (int)this.levelViewModels[i].SelectedValue,
                    string.Format(LevelKeyFormat, dviPort.Index));
            }
        }

        /// <summary>
        /// Gets all level shifter configurations configured in this part.
        /// </summary>
        /// <returns>
        /// The the list of configurations.
        /// </returns>
        public IEnumerable<DviLevelShifterConfig> GetLevelShifterConfigs()
        {
            return
                this.dviPorts.Select(
                    (d, i) =>
                    new DviLevelShifterConfig
                        {
                            Index = d.Index + 1,
                            Trim = (TrimOptions)this.trimViewModels[i].SelectedValue,
                            OutputLevel = (int)this.levelViewModels[i].SelectedValue
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
                                    DisplayName = AdminStrings.UnitConfig_Hardware_DviLevel,
                                    Description = AdminStrings.UnitConfig_Hardware_DviLevel_Description,
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

            this.dviPorts = infoVision.DisplayAdapters.Where(a => a.Connection == DisplayConnectionType.Dvi).ToList();
            foreach (var dviPort in this.dviPorts)
            {
                this.ViewModel.IsVisible = true;
                var title = new TitleEditorViewModel();
                title.Label = string.Format(AdminStrings.UnitConfig_Hardware_DviLevel_LabelFormat, dviPort.Index + 1);
                this.ViewModel.Editors.Add(title);
                var trim = new SelectionEditorViewModel
                               {
                                   Label = AdminStrings.UnitConfig_Hardware_DviLevel_Trim,
                                   Options =
                                       {
                                           new SelectionOptionViewModel(
                                               AdminStrings
                                               .UnitConfig_Hardware_DviLevel_Trim_Standard,
                                               TrimOptions.StandardCurrent),
                                           new SelectionOptionViewModel(
                                               AdminStrings
                                               .UnitConfig_Hardware_DviLevel_Trim_Increased,
                                               TrimOptions.IncreasedCurrent),
                                       }
                               };
                this.ViewModel.Editors.Add(trim);
                this.trimViewModels.Add(trim);

                var level = new SelectionEditorViewModel { Label = AdminStrings.UnitConfig_Hardware_DviLevel_Level };
                foreach (var dviLevel in DviLevels)
                {
                    level.Options.Add(
                        new SelectionOptionViewModel(
                            string.Format(AdminStrings.UnitConfig_Hardware_DviLevel_Level_Format, dviLevel),
                            dviLevel));
                }

                this.ViewModel.Editors.Add(level);
                this.levelViewModels.Add(level);
            }
        }
    }
}