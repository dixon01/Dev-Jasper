// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayOrientationPartController.cs" company="Luminator Technology Group">
//   Copyright © 2016 Luminator Technology Group. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayOrientationPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Gorba.Common.Configuration.HardwareManager;

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The display orientation part controller.
    /// </summary>
    public class DisplayOrientationPartController : MultiEditorPartControllerBase
    {
        private const string OrientationFormat = "{0}";
        private static readonly ScreenOrientation[] Orientations =
            {
                new ScreenOrientation("Landscape", OrientationMode.Landscape),
                new ScreenOrientation("LandscapeFlipped", OrientationMode.LandscapeFlipped),
            };

        private SelectionEditorViewModel screenOrientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware.DisplayOrientationPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DisplayOrientationPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Hardware.ScreenOrientation, parent)
        {
        }

        /// <summary>
        /// Gets the screen orientation configuration or null if not configured.
        /// </summary>
        public ScreenOrientation ScreenOrientation => screenOrientation.SelectedValue as ScreenOrientation;

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            LoadOrientation(screenOrientation, partData, 0);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            SaveOrientation(screenOrientation, partData, 0);
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
                DisplayName = AdminStrings.UnitConfig_Hardware_ScreenOrientation,
                Description = AdminStrings.UnitConfig_Hardware_ScreenOrientation_Description,
                PartKey = UnitConfigKeys.Hardware.ScreenOrientation
            };

            screenOrientation = new SelectionEditorViewModel();
            screenOrientation.Label = AdminStrings.UnitConfig_Hardware_ScreenOrientation;
            CreateOrientations(screenOrientation);
            screenOrientation.PropertyChanged += ScreenOrientationOnPropertyChanged;
            viewModel.Editors.Add(screenOrientation);
            viewModel.IsVisible = true;
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
            // This part controller currently has no information in the hardware descriptor.
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

        private static void CreateOrientations(SelectionEditorViewModel selection)
        {
            foreach (var orientation in Orientations)
            {
                selection.Options.Add(new SelectionOptionViewModel(orientation.ToString(), orientation));
            }
        }

        private static void LoadOrientation(SelectionEditorViewModel viewModel, UnitConfigPart partData, int index)
        {
            var orientation = (int)partData.GetValue(0, string.Format(OrientationFormat, index));

            var option = viewModel.Options.FirstOrDefault(o => ((ScreenOrientation)o.Value).Mode == (OrientationMode)orientation);
            if (option != null)
            {
                viewModel.SelectedOption = option;
            }
        }

        private static void SaveOrientation(SelectionEditorViewModel viewModel, UnitConfigPart partData, int index)
        {
            var orientation = viewModel.SelectedValue as ScreenOrientation;
            partData.SetValue(orientation == null ? 0 : (int)orientation.Mode, string.Format(OrientationFormat, index));
        }

        private void ScreenOrientationOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            ErrorState errorState;
            if (screenOrientation.SelectedOption != null)
            {
                errorState = ErrorState.Ok;
            }
            else if (this.ViewModel.WasVisited)
            {
                errorState = ErrorState.Error;
            }
            else
            {
                errorState = ErrorState.Missing;
            }

            screenOrientation.SetError("SelectedOption", errorState, AdminStrings.Errors_NoItemSelected);
        }
    }
}