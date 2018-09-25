// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationSelectionController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationSelectionController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;

    /// <summary>
    /// The controller that handles a <see cref="SelectionEditorViewModel"/>
    /// that allows to select a transformation.
    /// </summary>
    public class TransformationSelectionController
    {
        private readonly SelectionEditorViewModel viewModel;

        private MultiEditorPartControllerBase parent;

        private List<TransformationSelectionOptionViewModel> transformationOptions;

        private TransformationsGeneralPartController general;

        private TransformationsCategoryController transformations;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationSelectionController"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model to be controlled by this controller.
        /// </param>
        public TransformationSelectionController(SelectionEditorViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        /// <summary>
        /// Gets or sets the id of the selected transformation chain.
        /// A value <c>null</c> means that the user selected "No Transformation".
        /// </summary>
        public string SelectedChainId
        {
            get
            {
                return this.viewModel.SelectedValue != null  ? this.viewModel.SelectedOption.Label : null;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.viewModel.SelectValue(null);
                }
                else
                {
                    this.viewModel.SelectedOption = this.viewModel.Options.FirstOrDefault(o => o.Label == value);
                }
            }
        }

        /// <summary>
        /// Initializes this controller with the given part controller that owns this controller.
        /// Do not call this method before all controllers are initialized properly; call it from
        /// one of the <c>Prepare</c> methods.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public void Initialize(MultiEditorPartControllerBase parentController)
        {
            this.parent = parentController;
            this.viewModel.Options.Clear();

            this.viewModel.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_Protran_NoTransformation, null));

            this.general = this.parent.Parent.Parent.GetPart<TransformationsGeneralPartController>();
            this.transformations = (TransformationsCategoryController)this.general.Parent;

            this.transformationOptions =
                this.transformations.TransformationPartControllers.Select(
                    c => new TransformationSelectionOptionViewModel(c)).ToList();

            this.general.ViewModelUpdated += (s, e) => this.UpdateOptions();
            this.UpdateOptions();

            this.viewModel.PropertyChanged += this.ViewModelOnPropertyChanged;
        }

        private void UpdateOptions()
        {
            var expectedCount = this.general.TransformationsCount + 1;

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (expectedCount > this.viewModel.Options.Count)
            {
                this.viewModel.Options.Add(this.transformationOptions[this.viewModel.Options.Count - 1]);
            }

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (expectedCount < this.viewModel.Options.Count)
            {
                this.viewModel.Options.RemoveAt(this.viewModel.Options.Count - 1);
            }

            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            var errorState = this.viewModel.SelectedOption == null
                                 ? (this.parent.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing)
                                 : ErrorState.Ok;
            this.viewModel.SetError("SelectedOption", errorState, AdminStrings.Errors_NoItemSelected);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedOption")
            {
                this.UpdateErrors();
            }
        }

        private class TransformationSelectionOptionViewModel : SelectionOptionViewModel
        {
            private readonly TransformationPartController controller;

            public TransformationSelectionOptionViewModel(TransformationPartController controller)
            {
                this.controller = controller;
                this.controller.ViewModelUpdated += (s, e) => this.UpdateLabel();

                this.UpdateLabel();
                this.Value = controller;
            }

            private void UpdateLabel()
            {
                this.Label = this.controller.ViewModel.ChainId;
            }
        }
    }
}