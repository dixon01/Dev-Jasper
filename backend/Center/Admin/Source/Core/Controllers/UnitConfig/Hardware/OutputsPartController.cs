// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputsPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OutputsPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The outputs part controller.
    /// </summary>
    public class OutputsPartController : MultiEditorPartControllerBase
    {
        private const string NameKeyFormat = "{0}.Name";
        private const string LedName = "LED";

        private readonly List<TextEditorViewModel> outputViewModels = new List<TextEditorViewModel>();

        private List<OutputDescriptor> outputs;

        private InputsPartController inputsController;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputsPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public OutputsPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.Outputs, parent)
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
            for (int i = 0; i < this.outputs.Count; i++)
            {
                var output = this.outputs[i];
                this.outputViewModels[i].Text = partData.GetValue(
                    this.outputViewModels[i].Text,
                    string.Format(NameKeyFormat, output != null ? (object)output.Index : LedName));
#if __UseLuminatorTftDisplay
                if (string.Compare(this.outputViewModels[i].Text, "Output1") == 0)
                {
                    this.outputViewModels[i].Text = "Interior";
                }

                else if (string.Compare(this.outputViewModels[i].Text, "Output2") == 0)
                {
                    this.outputViewModels[i].Text = "Exterior";
                }
#endif
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
            for (int i = 0; i < this.outputs.Count; i++)
            {
                var input = this.outputs[i];
                partData.SetValue(
                    this.outputViewModels[i].Text,
                    string.Format(NameKeyFormat, input != null ? (object)input.Index : LedName));
            }
        }

        /// <summary>
        /// Gets the output names.
        /// </summary>
        /// <returns>
        /// The enumeration over all output names.
        /// </returns>
        public IEnumerable<string> GetNames()
        {
            return this.outputViewModels.Select(vm => vm.Text);
        }

        /// <summary>
        /// Gets the list of all general outputs defined in this part.
        /// </summary>
        /// <returns>
        /// A dictionary containing the output index in the system and the text assigned to the given output.
        /// </returns>
        public IDictionary<int, string> GetOutputs()
        {
            return
                this.outputs.Where(i => i != null)
                    .Select((input, index) => new { input.Index, this.outputViewModels[index].Text })
                    .ToDictionary(i => i.Index, i => i.Text);
        }

        /// <summary>
        /// Gets the name of the generic LED defined in this part.
        /// </summary>
        /// <returns>
        /// The name or null if no LED is available.
        /// </returns>
        public string GetLedName()
        {
            return this.outputs.Last() != null ? null : this.outputViewModels.Last().Text;
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
                                    DisplayName = AdminStrings.UnitConfig_Hardware_Outputs,
                                    Description = AdminStrings.UnitConfig_Hardware_Outputs_Description,
                                    IsVisible = true
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
            this.inputsController = this.GetPart<InputsPartController>();
            this.inputsController.ViewModelUpdated += (s, e) => this.UpdateErrors();

            this.outputs = descriptor.Platform.Outputs.ToList();
            foreach (var output in this.outputs)
            {
                var outputEditor = new TextEditorViewModel
                                       {
                                           Label = output.Name,
                                           Text = string.Format("Output{0}", output.Index)
                                       };
                outputEditor.PropertyChanged += (s, e) => this.UpdateErrors();
                this.ViewModel.Editors.Add(outputEditor);
                this.outputViewModels.Add(outputEditor);
            }

            var platform = descriptor.Platform as PcPlatformDescriptorBase;
            if (platform == null || !platform.HasGenericLed)
            {
                return;
            }

            var ledEditor = new TextEditorViewModel
                                {
                                    Label = AdminStrings.UnitConfig_Hardware_Outputs_LED,
                                    Text = "LED"
                                };
            ledEditor.PropertyChanged += (s, e) => this.UpdateErrors();
            this.ViewModel.Editors.Add(ledEditor);
            this.outputViewModels.Add(ledEditor);
            this.outputs.Add(null);
        }

        private void UpdateErrors()
        {
            foreach (var editor in this.ViewModel.Editors.OfType<TextEditorViewModel>())
            {
                editor.SetError(
                    "Text",
                    string.IsNullOrWhiteSpace(editor.Text) ? ErrorState.Error : ErrorState.Ok,
                    AdminStrings.Errors_TextNotWhitespace);

                var current = editor;
                var hasDuplicate =
                    this.ViewModel.Editors.OfType<TextEditorViewModel>()
                        .Any(e => e != current && e.Text == current.Text)
                        || this.inputsController.GetNames().Contains(current.Text);
                editor.SetError(
                    "Text",
                    hasDuplicate ? ErrorState.Error : ErrorState.Ok,
                    AdminStrings.Errors_DuplicateName);
            }
        }
    }
}