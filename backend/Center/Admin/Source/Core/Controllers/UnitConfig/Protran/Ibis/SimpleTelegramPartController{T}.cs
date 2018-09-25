// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleTelegramPartController{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleTelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Telegram configuration part controller for simple telegrams.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="TelegramConfig"/> that is created by this class.
    /// </typeparam>
    public class SimpleTelegramPartController<T> : IbisTelegramPartControllerBase<T>
        where T : TelegramConfig, new()
    {
        private readonly GenericUsage defaultUsage;

        private SelectionEditorViewModel transformation;

        private TransformationSelectionController transformationSelectionController;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTelegramPartController{T}"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="defaultUsage">
        /// The default generic usage of the telegram.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public SimpleTelegramPartController(
            string telegramName, GenericUsage defaultUsage, CategoryControllerBase parent)
            : base(telegramName, parent)
        {
            this.defaultUsage = defaultUsage;
        }

        /// <summary>
        /// Gets the first generic usage editor that is available for all telegrams.
        /// </summary>
        protected GenericUsageEditorViewModel GenericUsageEditor { get; private set; }

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
            partData.SetXmlValue((T)this.CreateTelegramConfig());
        }

        /// <summary>
        /// Prepares the given telegram with the data configured in this part.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be filled with data.
        /// </param>
        protected override void PrepareTelegram(T telegram)
        {
            telegram.TransfRef = this.transformationSelectionController.SelectedChainId;
            telegram.UsedFor = this.GenericUsageEditor.GenericUsage;
        }

        /// <summary>
        /// Loads the configuration from the given telegram into this part's editors.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to load the data from.
        /// </param>
        protected virtual void LoadFrom(T telegram)
        {
            if (this.GenericUsageEditor.IsNullable && !string.IsNullOrEmpty(telegram.Name))
            {
                this.GenericUsageEditor.GenericUsage = telegram.UsedFor;
            }
            else
            {
                this.GenericUsageEditor.GenericUsage = telegram.UsedFor ?? this.defaultUsage;
            }

            this.transformationSelectionController.SelectedChainId = telegram.TransfRef;
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
            var viewModel = base.CreateViewModel();

            this.transformation = new SelectionEditorViewModel();
            this.transformation.Label = AdminStrings.UnitConfig_IbisProtocol_Telegram_Transformation;
            if (this.TelegramType != TelegramType.Empty)
            {
                viewModel.Editors.Add(this.transformation);
            }

            this.transformationSelectionController = new TransformationSelectionController(this.transformation);

            this.GenericUsageEditor = new GenericUsageEditorViewModel();
            this.GenericUsageEditor.ShouldShowRow = true;
            viewModel.Editors.Add(this.GenericUsageEditor);

            return viewModel;
        }
    }
}