// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Factory class for <see cref="ITelegramHandler"/>s.
    /// </summary>
    public class TelegramHandlerFactory
    {
        /// <summary>
        /// Creates an <see cref="ITelegramHandler"/> implementation for a given
        /// telegram configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        /// <returns>
        /// An <see cref="ITelegramHandler"/> implementation or null if non can be found.
        /// </returns>
        public virtual ITelegramHandler CreateHandler(
            TelegramConfig config, IIbisConfigContext configContext)
        {
            var telegramType = typeof(Telegram).Assembly.GetType(
                typeof(Telegram).Namespace + "." + config.Name,
                false,
                true);
            if (telegramType == null)
            {
                return null;
            }

            for (var t = telegramType; t != null && t != typeof(Telegram); t = t.BaseType)
            {
                var handler = this.CreateHandler(t.Name, telegramType);
                if (handler != null)
                {
                    handler.Configure(config, configContext);
                    return handler;
                }
            }

            return null;
        }

        private ITelegramHandler CreateHandler(string telegramName, Type telegramType)
        {
            var @namespace = typeof(TelegramHandlerFactory).Namespace;
            var handlerType = Type.GetType(string.Format("{0}.{1}Handler", @namespace, telegramName), false, true);

            if (handlerType == null)
            {
                handlerType = Type.GetType(string.Format("{0}.{1}Handler`1", @namespace, telegramName), false, true);
                if (handlerType == null)
                {
                    return null;
                }

                handlerType = handlerType.MakeGenericType(telegramType);
            }

            return Activator.CreateInstance(handlerType) as ITelegramHandler;
        }
    }
}
