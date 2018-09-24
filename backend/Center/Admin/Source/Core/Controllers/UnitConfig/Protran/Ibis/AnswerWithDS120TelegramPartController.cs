// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnswerWithDS120TelegramPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnswerWithDS120TelegramPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Telegram configuration part controller for telegrams that are answered with DS120.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="TelegramConfig"/> that is created by this class.
    /// </typeparam>
    public class AnswerWithDS120TelegramPartController<T> : SimpleTelegramPartController<T>
        where T : TelegramConfig, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerWithDS120TelegramPartController{T}"/> class.
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
        public AnswerWithDS120TelegramPartController(
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
        protected override void PrepareTelegram(T telegram)
        {
            base.PrepareTelegram(telegram);

            var answer = new DS120Config { Enabled = true, Name = "DS120", DefaultResponse = 0 };
            telegram.Answer = new Answer { Telegram = answer };
        }
    }
}