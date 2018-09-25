// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputsPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputsPartController type.
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
    /// The inputs part controller.
    /// </summary>
    public class InputsPartController : MultiEditorPartControllerBase
    {
        private const string NameKeyFormat = "{0}.Name";
        private const string ButtonName = "Button";

        private readonly List<TextEditorViewModel> inputViewModels = new List<TextEditorViewModel>();

        private List<InputDescriptor> inputs;

        private OutputsPartController outputsController;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputsPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public InputsPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.Inputs, parent)
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
            for (int i = 0; i < this.inputs.Count; i++)
            {
                var input = this.inputs[i];
                this.inputViewModels[i].Text = partData.GetValue(
                    this.inputViewModels[i].Text,
                    string.Format(NameKeyFormat, input != null ? (object)input.Index : ButtonName));
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
            for (int i = 0; i < this.inputs.Count; i++)
            {
                var input = this.inputs[i];
                partData.SetValue(
                    this.inputViewModels[i].Text,
                    string.Format(NameKeyFormat, input != null ? (object)input.Index : ButtonName));
            }
        }

        /// <summary>
        /// Gets the input names.
        /// </summary>
        /// <returns>
        /// The enumeration over all input names.
        /// </returns>
        public IEnumerable<string> GetNames()
        {
            return this.inputViewModels.Select(vm => vm.Text);
        }

        /// <summary>
        /// Gets the list of all general inputs defined in this part.
        /// </summary>
        /// <returns>
        /// A dictionary containing the input index in the system and the text assigned to the given input.
        /// </returns>
        public IDictionary<int, string> GetInputs()
        {
            return
                this.inputs.Where(i => i != null)
                    .Select((input, index) => new { input.Index, this.inputViewModels[index].Text })
                    .ToDictionary(i => i.Index, i => i.Text);
        }

        /// <summary>
        /// Gets the name of the generic button defined in this part.
        /// </summary>
        /// <returns>
        /// The name or null if no button is available.
        /// </returns>
        public string GetButtonName()
        {
            return this.inputs.Last() != null ? null : this.inputViewModels.Last().Text;
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
                                    DisplayName = AdminStrings.UnitConfig_Hardware_Inputs,
                                    Description = AdminStrings.UnitConfig_Hardware_Inputs_Description,
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
            this.outputsController = this.GetPart<OutputsPartController>();
            this.outputsController.ViewModelUpdated += (s, e) => this.UpdateErrors();

            this.inputs = descriptor.Platform.Inputs.ToList();

            foreach (var input in this.inputs)
            {
                var inputEditor = new TextEditorViewModel
                                      {
                                          Label = input.Name,
                                          Text = string.Format("Input{0}", input.Index)
                                      };
                inputEditor.PropertyChanged += (s, e) => this.UpdateErrors();
                this.ViewModel.Editors.Add(inputEditor);
                this.inputViewModels.Add(inputEditor);
            }

            var platform = descriptor.Platform as PcPlatformDescriptorBase;
            if (platform == null || !platform.HasGenericButton)
            {
                return;
            }

            var buttonEditor = new TextEditorViewModel
                                   {
                                       Label = AdminStrings.UnitConfig_Hardware_Inputs_Button,
                                       Text = ButtonName
                                   };
            buttonEditor.PropertyChanged += (s, e) => this.UpdateErrors();
            this.ViewModel.Editors.Add(buttonEditor);
            this.inputViewModels.Add(buttonEditor);
            this.inputs.Add(null);
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
                        || this.outputsController.GetNames().Contains(current.Text);
                editor.SetError(
                    "Text",
                    hasDuplicate ? ErrorState.Error : ErrorState.Ok,
                    AdminStrings.Errors_DuplicateName);
            }
        }
    }
}