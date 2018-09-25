// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdatePartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The base class for all update method part controllers (FTP and USB).
    /// </summary>
    public abstract class UpdatePartControllerBase : MultiEditorPartControllerBase
    {
        private const string ShowVisualizationsKey = "ShowVisualizations";

        private CheckableEditorViewModel showVisualizations;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected UpdatePartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether visualizations should be shown.
        /// </summary>
        public bool ShouldShowVisualizations
        {
            get
            {
                return this.showVisualizations.IsChecked.HasValue && this.showVisualizations.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="UpdateMethodsPartController"/>.
        /// </summary>
        protected UpdateMethodsPartController MethodsController { get; private set; }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.showVisualizations.IsChecked = partData.GetValue(true, ShowVisualizationsKey);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.ShouldShowVisualizations, ShowVisualizationsKey);
        }

        /// <summary>
        /// Updates the visibility of the view model of this part controller.
        /// </summary>
        protected abstract void UpdateVisibility();

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            base.Prepare(descriptor);
            this.MethodsController = this.GetPart<UpdateMethodsPartController>();
            this.MethodsController.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
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

            this.showVisualizations = new CheckableEditorViewModel();
            this.showVisualizations.Label = AdminStrings.UnitConfig_Update_ShowVisualizations;
            viewModel.Editors.Add(this.showVisualizations);

            return viewModel;
        }
    }
}