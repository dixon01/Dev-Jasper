// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockChannel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MockChannel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Mocks
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Ibis.Channels;
    using Gorba.Motion.Protran.Ibis.Parsers;

    /// <summary>
    /// Extends the IbisSerialChannel object in order to really test
    /// the "HandleData" method.
    /// </summary>
    public class MockChannel : IbisChannel
    {
        /// <summary>
        /// The parser with 7 bit logic.
        /// </summary>
        private readonly Parser7Bit parser7;

        /// <summary>
        /// The parser with unicode big endian logic.
        /// </summary>
        private readonly Parser16Bit parser16;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public MockChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            var tlgNames = new[]
            {
                "DS001", "DS001A", "DS005", "DS006", "DS010B", "DS020", "DS021A", "GO003", "HPW074", "GO002"
            };

            var telegramsList = new List<TelegramConfig>();
            foreach (string name in tlgNames)
            {
                var typeName = string.Format("{0}.{1}Config", typeof(TelegramConfig).Namespace, name);
                var type = typeof(TelegramConfig).Assembly.GetType(typeName, false, true);
                TelegramConfig tlgConfig;
                if (type != null)
                {
                    tlgConfig = (TelegramConfig)Activator.CreateInstance(type);
                }
                else
                {
                    tlgConfig = new SimpleTelegramConfig();
                }

                tlgConfig.Name = name;
                tlgConfig.Enabled = true;
                tlgConfig.TransfRef = name;
                telegramsList.Add(tlgConfig);
            }

            this.parser7 = new Parser7Bit(true, telegramsList);
            this.parser16 = new Parser16Bit(true, telegramsList);
        }

        /// <summary>
        /// Handles the telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to handle.
        /// </param>
        /// <returns>
        /// True if the handle has succeeded, else false.
        /// </returns>
        public bool HandleData(byte[] telegram)
        {
            if (this.Config.Behaviour.ByteType == ByteType.Ascii7)
            {
                return base.HandleData(telegram, this.parser7.GetTelegramParser(telegram));
            }

            return base.HandleData(telegram, this.parser16.GetTelegramParser(telegram));
        }

        /// <summary>
        /// Subclasses implement this method to send an answer to the IBIS master.
        /// </summary>
        /// <param name="bytes">
        /// The buffer to send.
        /// </param>
        /// <param name="offset">
        /// The offset inside the buffer.
        /// </param>
        /// <param name="length">
        /// The number of bytes to send starting from <see cref="offset"/>.
        /// </param>
        protected override void SendAnswer(byte[] bytes, int offset, int length)
        {
        }
    }
}
