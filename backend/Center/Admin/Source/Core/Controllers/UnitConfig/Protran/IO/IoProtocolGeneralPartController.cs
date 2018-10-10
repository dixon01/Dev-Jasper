// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoProtocolGeneralPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IoProtocolGeneralPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.IO
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The I/O protocol general part controller.
    /// </summary>
    public class IoProtocolGeneralPartController : MultiEditorPartControllerBase
    {
        private NumberEditorViewModel inputsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="IoProtocolGeneralPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public IoProtocolGeneralPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.IoProtocol.General, parent)
        {
        }

        /// <summary>
        /// Gets the number of currently visible input parts.
        /// </summary>
        public int InputsCount
        {
            get
            {
                return (int)this.inputsCount.Value;
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
            this.inputsCount.Value = partData.GetValue(0);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.inputsCount.Value);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_IoProtocol_General;
            viewModel.Description = AdminStrings.UnitConfig_IoProtocol_General_Description;

            this.inputsCount = new NumberEditorViewModel();
            this.inputsCount.Label = AdminStrings.UnitConfig_IoProtocol_General_Count;
            this.inputsCount.IsInteger = true;
            this.inputsCount.MinValue = 0;
            this.inputsCount.MaxValue = IoProtocolCategoryController.MaxInputsCount;
            viewModel.Editors.Add(this.inputsCount);

            return viewModel;
        }
    }
}