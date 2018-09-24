// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTelegramPartControllerBase{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisTelegramPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// The generic base class for all part controllers responsible for IBIS telegrams.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <see cref="TelegramConfig"/> that is created by this class.
    /// </typeparam>
    public abstract class IbisTelegramPartControllerBase<T> : IbisTelegramPartControllerBase
        where T : TelegramConfig, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTelegramPartControllerBase{T}"/> class.
        /// </summary>
        /// <param name="telegramName">
        /// The telegram name.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected IbisTelegramPartControllerBase(string telegramName, CategoryControllerBase parent)
            : base(telegramName, parent)
        {
        }

        /// <summary>
        /// Creates the telegram config for this part.
        /// </summary>
        /// <returns>
        /// The <see cref="TelegramConfig"/>.
        /// </returns>
        public sealed override TelegramConfig CreateTelegramConfig()
        {
            var telegram = new T { Enabled = this.ViewModel.IsVisible, Name = this.TelegramName };
            this.PrepareTelegram(telegram);
            return telegram;
        }

        /// <summary>
        /// Prepares the given telegram with the data configured in this part.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be filled with data.
        /// </param>
        protected abstract void PrepareTelegram(T telegram);
    }
}