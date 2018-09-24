// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS020TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS020TelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Special part controller for DS020.
    /// </summary>
    public class DS020TelegramPartController : IbisTelegramPartControllerBase<SimpleTelegramConfig>
    {
        private const string AnswerKey = "Answer";

        private NumberEditorViewModel defaultAnswer;

        private NumberEditorViewModel noDataAnswer;

        private NumberEditorViewModel missingDataAnswer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS020TelegramPartController"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DS020TelegramPartController(string telegramName, CategoryControllerBase parent)
            : base(telegramName, parent)
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
            var answer = partData.GetXmlValue<DS120Config>(AnswerKey);

            this.defaultAnswer.Value = answer.DefaultResponse;
            this.noDataAnswer.Value = this.GetResponse(answer, Status.NoData, 3);
            this.missingDataAnswer.Value = this.GetResponse(answer, Status.MissingData, 4);
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetXmlValue(this.CreateAnswer(), AnswerKey);
        }

        /// <summary>
        /// Prepares the given telegram with the data configured in this part.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be filled with data.
        /// </param>
        protected override void PrepareTelegram(SimpleTelegramConfig telegram)
        {
            telegram.Answer = new Answer { Telegram = this.CreateAnswer() };
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

            this.defaultAnswer = this.CreateAnswerEditor();
            this.defaultAnswer.Label = AdminStrings.UnitConfig_Ibis_DS020_DefaultAnswer;
            viewModel.Editors.Add(this.defaultAnswer);

            this.noDataAnswer = this.CreateAnswerEditor();
            this.noDataAnswer.Label = AdminStrings.UnitConfig_Ibis_DS020_NoDataAnswer;
            viewModel.Editors.Add(this.noDataAnswer);

            this.missingDataAnswer = this.CreateAnswerEditor();
            this.missingDataAnswer.Label = AdminStrings.UnitConfig_Ibis_DS020_MissingDataAnswer;
            viewModel.Editors.Add(this.missingDataAnswer);

            return viewModel;
        }

        private NumberEditorViewModel CreateAnswerEditor()
        {
            return new NumberEditorViewModel { IsInteger = true, MinValue = 0, MaxValue = 15 };
        }

        private DS120Config CreateAnswer()
        {
            var answer = new DS120Config { Enabled = true, Name = "DS120" };
            answer.DefaultResponse = (int)this.defaultAnswer.Value;

            answer.Responses.Add(new Response { Status = Status.NoData, Value = (int)this.noDataAnswer.Value });
            answer.Responses.Add(
                new Response { Status = Status.MissingData, Value = (int)this.missingDataAnswer.Value });

            return answer;
        }

        private int GetResponse(DS120Config answer, Status status, int defaultValue)
        {
            var response = answer.Responses.FirstOrDefault(r => r.Status == status);
            return response != null ? response.Value : defaultValue;
        }
    }
}