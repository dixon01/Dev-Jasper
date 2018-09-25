// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO003TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO003TelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The GO003 telegram part controller.
    /// </summary>
    public class GO003TelegramPartController : SimpleTelegramPartController<SimpleTelegramConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO003TelegramPartController"/> class.
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
        public GO003TelegramPartController(
            string telegramName, GenericUsage defaultUsage, CategoryControllerBase parent)
            : base(telegramName, defaultUsage, parent)
        {
        }

        /// <summary>
        /// Prepares the given telegram with the data configured in this part.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be filled with data.
        /// </param>
        protected override void PrepareTelegram(SimpleTelegramConfig telegram)
        {
            base.PrepareTelegram(telegram);
            telegram.Answer = new Answer
                                  {
                                      Telegram =
                                          new DS120Config
                                              {
                                                  Name = "DS120",
                                                  Enabled = true,
                                                  DefaultResponse = 0,
                                                  Responses =
                                                      {
                                                          new Response
                                                              {
                                                                  Status = Status.IncorrectRecord,
                                                                  Value = 5
                                                              }
                                                      }
                                              }
                                  };
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
            this.GenericUsageEditor.ShouldShowRow = false;
            return viewModel;
        }
    }
}