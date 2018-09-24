// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HPW074TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HPW074TelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The HPW074 telegram part controller.
    /// </summary>
    public class HPW074TelegramPartController : AnswerWithDS120TelegramPartController<HPW074Config>
    {
        /// <summary>
        /// The special text file name.
        /// </summary>
        public static readonly string SpecialTextFile = "specialtext.csv";

        private const string SpecialTextKey = "SpecialText";

        private MulitLineTextEditorViewModel specialText;

        /// <summary>
        /// Initializes a new instance of the <see cref="HPW074TelegramPartController"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="defaultUsage">
        /// The default usage.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public HPW074TelegramPartController(
            string telegramName, GenericUsage defaultUsage, CategoryControllerBase parent)
            : base(telegramName, defaultUsage, parent)
        {
            this.TelegramType = TelegramType.Integer;
        }

        /// <summary>
        /// Gets the special text (CSV contents).
        /// </summary>
        public string SpecialText
        {
            get
            {
                return this.specialText.Text;
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
            base.Load(partData);

            // ReSharper disable StringLiteralTypo
            this.specialText.Text =
                partData.GetValue("1;Achtung Taschendiebe. Bitte achten;Sie auf Ihre Wertsachen!", SpecialTextKey);
            // ReSharper restore StringLiteralTypo
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            base.Save(partData);

            partData.SetValue(this.SpecialText, SpecialTextKey);
        }

        /// <summary>
        /// Prepares the given telegram with the data configured in this part.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be filled with data.
        /// </param>
        protected override void PrepareTelegram(HPW074Config telegram)
        {
            base.PrepareTelegram(telegram);

            telegram.SpecialTextFile = SpecialTextFile;
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
            viewModel.Description = AdminStrings.UnitConfig_Ibis_HPW074_Description;

            this.specialText = new MulitLineTextEditorViewModel();
            this.specialText.Label = AdminStrings.UnitConfig_Ibis_HPW074_SpecialText;
            this.specialText.MinLines = 6;
            this.specialText.MaxLines = 1000;
            this.specialText.Text = string.Empty;
            viewModel.Editors.Add(this.specialText);

            return viewModel;
        }

        /// <summary>
        /// Updates the errors on the editors.
        /// </summary>
        protected override void UpdateErrors()
        {
            base.UpdateErrors();

            this.specialText.SetError(
                "Text",
                this.specialText.Text.Contains(";") ? ErrorState.Ok : ErrorState.Warning,
                AdminStrings.Errors_NoSemicolon);
        }
    }
}