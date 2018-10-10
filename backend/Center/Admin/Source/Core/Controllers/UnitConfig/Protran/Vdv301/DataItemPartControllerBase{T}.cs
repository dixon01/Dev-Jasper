// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemPartControllerBase{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// Generic base class for all part controllers that handle a data item configuration.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="DataItemConfig"/> created by this controller.
    /// </typeparam>
    public abstract class DataItemPartControllerBase<T> : DataItemPartControllerBase
        where T : DataItemConfig, new()
    {
        private readonly string[] path;

        private readonly GenericUsageEditorViewModel genericUsage;

        private SelectionEditorViewModel transformation;

        private TransformationSelectionController transformationSelectionController;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemPartControllerBase{T}"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to the data item.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected DataItemPartControllerBase(string[] path, CategoryControllerBase parent)
            : base(string.Format(UnitConfigKeys.Vdv301Protocol.DataItemFormat, string.Join(".", path)), parent)
        {
            this.path = path;

            // we need to create this one here for the two flag properties to work
            this.genericUsage = new GenericUsageEditorViewModel();
            this.genericUsage.ShouldShowLanguage = true;
            this.genericUsage.ShouldShowRow = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to should show the row editor.
        /// </summary>
        public sealed override bool ShouldShowRow
        {
            get
            {
                return this.genericUsage.ShouldShowRow;
            }

            set
            {
                this.genericUsage.ShouldShowRow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to should show the language editor.
        /// </summary>
        public sealed override bool ShouldShowLanguage
        {
            get
            {
                return this.genericUsage.ShouldShowLanguage;
            }

            set
            {
                this.genericUsage.ShouldShowLanguage = value;
            }
        }

        /// <summary>
        /// Creates a <see cref="DataItemConfig"/> (or a subclass) for this data item.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="DataItemConfig"/>.
        /// </returns>
        public sealed override DataItemConfig CreateConfig()
        {
            return this.CreateDataItemConfig();
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.LoadFrom(partData.GetXmlValue<T>());
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetXmlValue(this.CreateDataItemConfig());
        }

        /// <summary>
        /// Loads the data from the given config into the editors.
        /// </summary>
        /// <param name="dataItemConfig">
        /// The data item config.
        /// </param>
        protected virtual void LoadFrom(T dataItemConfig)
        {
            this.genericUsage.GenericUsage = dataItemConfig;
            this.transformationSelectionController.SelectedChainId = dataItemConfig.TransfRef;
        }

        /// <summary>
        /// Creates the data item config for this part.
        /// </summary>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        protected virtual T CreateDataItemConfig()
        {
            var usage = this.genericUsage.GenericUsage;
            return new T
                       {
                           Enabled = this.ViewModel.IsVisible,
                           Language = usage.Language,
                           Table = usage.Table,
                           Column = usage.Column,
                           Row = usage.Row,
                           TransfRef = this.transformationSelectionController.SelectedChainId
                       };
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
            viewModel.DisplayName = this.path.Last();
            viewModel.Description = string.Format(
                AdminStrings.UnitConfig_Vdv301_DataItem_DescriptionFormat,
                string.Join(" > ", this.path));

            // the editor was already created in the constructor
            ////this.genericUsage = new GenericUsageEditorViewModel();
            viewModel.Editors.Add(this.genericUsage);

            this.transformation = new SelectionEditorViewModel();
            this.transformation.Label = AdminStrings.UnitConfig_Vdv301_DataItem_Transformation;
            viewModel.Editors.Add(this.transformation);

            this.transformationSelectionController = new TransformationSelectionController(this.transformation);

            return viewModel;
        }
    }
}