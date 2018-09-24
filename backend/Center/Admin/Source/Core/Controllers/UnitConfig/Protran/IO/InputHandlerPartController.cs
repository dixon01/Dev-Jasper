// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputHandlerPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputHandlerPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.IO
{
    using System;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// The part controller for the input handler configuration of the Protran I/O protocol.
    /// </summary>
    public class InputHandlerPartController : MultiEditorPartControllerBase
    {
        private const string IsManualKey = "IsManual";
        private const string UnitNameKey = "UnitName";
        private const string ApplicationNameKey = "ApplicationName";
        private const string InputNameKey = "InputName";
        private const string GenericUsageKey = "GenericUsage";
        private const string TransformationKey = "Transformation";

        private readonly int index;

        private SelectionEditorViewModel inputSelection;

        private InputsPartController inputsPart;

        private TextEditorViewModel unitName;

        private TextEditorViewModel applicationName;

        private TextEditorViewModel inputName;

        private GenericUsageEditorViewModel genericUsage;

        private SelectionEditorViewModel transformation;

        private TransformationSelectionController transformationSelectionController;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputHandlerPartController"/> class.
        /// </summary>
        /// <param name="index">
        /// The index (from 1).
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public InputHandlerPartController(int index, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.IoProtocol.InputHandlerFormat, index), parent)
        {
            this.index = index;
        }

        /// <summary>
        /// Gets the unit name of the I/O configured in this editor.
        /// </summary>
        public string UnitName
        {
            get
            {
                var input = this.inputSelection.SelectedValue as InputDescriptor;
                return input != null ? null : this.unitName.Text;
            }
        }

        /// <summary>
        /// Gets the application name of the I/O configured in this editor.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                var input = this.inputSelection.SelectedValue as InputDescriptor;
                return input != null ? null : this.applicationName.Text;
            }
        }

        /// <summary>
        /// Gets the name of the I/O configured in this editor.
        /// </summary>
        public string InputName
        {
            get
            {
                var input = this.inputSelection.SelectedValue as InputDescriptor;
                if (input == null)
                {
                    return this.inputName.Text;
                }

                string name;
                this.inputsPart.GetInputs().TryGetValue(input.Index, out name);
                return name;
            }
        }

        /// <summary>
        /// Gets the generic usage of the I/O configured in this editor.
        /// </summary>
        public GenericUsage GenericUsage
        {
            get
            {
                return this.genericUsage.GenericUsage;
            }
        }

        /// <summary>
        /// Gets the id of the transformation chain chosen by the user.
        /// This can be null if the user doesn't need a transformation (i.e. default transformation).
        /// </summary>
        public string TransformationChainId
        {
            get
            {
                return this.transformationSelectionController.SelectedChainId;
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
            this.genericUsage.GenericUsage = partData.GetXmlValue<GenericUsage>(GenericUsageKey);
            this.transformationSelectionController.SelectedChainId = partData.GetValue(string.Empty, TransformationKey);

            var isManual = partData.GetValue(false, IsManualKey);
            if (isManual)
            {
                this.inputSelection.SelectValue(null);
                this.unitName.Text = partData.GetValue(string.Empty, UnitNameKey);
                this.applicationName.Text = partData.GetValue(string.Empty, ApplicationNameKey);
                this.inputName.Text = partData.GetValue(string.Empty, InputNameKey);
            }
            else
            {
                var name = partData.GetValue(string.Empty, InputNameKey);
                var inputs = this.inputsPart.GetInputs();
                var kvp = inputs.FirstOrDefault(i => i.Value == name);
                if (kvp.Value == null)
                {
                    this.inputSelection.SelectValue(null);
                }
                else
                {
                    this.inputSelection.SelectValue(
                        this.inputSelection.Options.Select(o => (InputDescriptor)o.Value)
                            .FirstOrDefault(i => i != null && i.Index == kvp.Key));
                }

                this.unitName.Text = string.Empty;
                this.applicationName.Text = string.Empty;
                this.inputName.Text = string.Empty;
            }

            this.UpdateEnabled();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetXmlValue(this.GenericUsage, GenericUsageKey);
            partData.SetValue(this.TransformationChainId, TransformationKey);
            var input = this.inputSelection.SelectedValue as InputDescriptor;
            if (input != null)
            {
                partData.SetValue(false, IsManualKey);
                partData.SetValue(this.InputName, InputNameKey);
                return;
            }

            partData.SetValue(true, IsManualKey);
            partData.SetValue(this.UnitName, UnitNameKey);
            partData.SetValue(this.ApplicationName, ApplicationNameKey);
            partData.SetValue(this.InputName, InputNameKey);
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

            this.transformationSelectionController.Initialize(this);

            this.inputsPart = this.GetPart<InputsPartController>();
            this.inputsPart.ViewModelUpdated += (s, e) => this.UpdateInputNames();

            foreach (var input in descriptor.Platform.Inputs)
            {
                this.inputSelection.Options.Add(new SelectionOptionViewModel(input.Name, input));
            }

            this.UpdateInputNames();
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
            viewModel.DisplayName = string.Format(AdminStrings.UnitConfig_IoProtocol_Input_Format, this.index);
            viewModel.Description = AdminStrings.UnitConfig_IoProtocol_Input_Description;

            this.inputSelection = new SelectionEditorViewModel();
            this.inputSelection.Label = AdminStrings.UnitConfig_IoProtocol_Input_Select;
            this.inputSelection.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_IoProtocol_Input_Select_Manual, null));
            this.inputSelection.PropertyChanged += (s, e) => this.UpdateEnabled();
            viewModel.Editors.Add(this.inputSelection);

            this.unitName = new TextEditorViewModel();
            this.unitName.Label = AdminStrings.UnitConfig_IoProtocol_Input_Unit;
            viewModel.Editors.Add(this.unitName);

            this.applicationName = new TextEditorViewModel();
            this.applicationName.Label = AdminStrings.UnitConfig_IoProtocol_Input_Application;
            viewModel.Editors.Add(this.applicationName);

            this.inputName = new TextEditorViewModel();
            this.inputName.Label = AdminStrings.UnitConfig_IoProtocol_Input_InputName;
            viewModel.Editors.Add(this.inputName);

            this.genericUsage = new GenericUsageEditorViewModel();
            this.genericUsage.ShouldShowRow = true;
            viewModel.Editors.Add(this.genericUsage);

            this.transformation = new SelectionEditorViewModel();
            this.transformation.Label = AdminStrings.UnitConfig_IoProtocol_Input_Transformation;
            viewModel.Editors.Add(this.transformation);

            this.transformationSelectionController = new TransformationSelectionController(this.transformation);

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

        private void UpdateInputNames()
        {
            var inputs = this.inputsPart.GetInputs();
            foreach (var option in this.inputSelection.Options.Where(o => o.Value is InputDescriptor))
            {
                string name;
                if (inputs.TryGetValue(((InputDescriptor)option.Value).Index, out name))
                {
                    option.Label = name;
                }
            }
        }

        private void UpdateEnabled()
        {
            var manualEnabled = this.inputSelection.SelectedOption != null && this.inputSelection.SelectedValue == null;
            this.unitName.IsEnabled = manualEnabled;
            this.applicationName.IsEnabled = manualEnabled;
            this.inputName.IsEnabled = manualEnabled;

            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            var errorState = this.inputName.IsEnabled && string.IsNullOrWhiteSpace(this.inputName.Text)
                                 ? ErrorState.Error
                                 : ErrorState.Ok;
            this.inputName.SetError("Text", errorState, AdminStrings.Errors_TextNotWhitespace);

            errorState = this.inputSelection.SelectedOption == null
                             ? (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing)
                             : ErrorState.Ok;
            this.inputSelection.SetError("SelectedOption", errorState, AdminStrings.Errors_NoItemSelected);
        }
    }
}