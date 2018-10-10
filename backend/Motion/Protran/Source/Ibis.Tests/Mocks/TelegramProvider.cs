// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   An object that abstract the "Read" function of the
//   .NET System.IO.Ports.SerialPort object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Mocks
{
    using System.Collections.Generic;

    /// <summary>
    /// An object that abstract the "Read" function of the
    /// .NET System.IO.Ports.SerialPort object.
    /// </summary>
    public abstract class TelegramProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramProvider"/> class.
        /// </summary>
        protected TelegramProvider()
        {
            this.Telegrams = new List<byte[]>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// serial port "emulator" produces telegrams on 16 bits based.
        /// </summary>
        public bool Is16Bits { get; set; }

        /// <summary>
        /// Gets a value indicating whether there are some
        /// bytes available on the stream.
        /// </summary>
        public virtual bool IsSomethingAvailable
        {
            get
            {
                // I read only once the whole list.
                // then I consider closed the stream.
                return this.TelegramsRead != this.Telegrams.Count;
            }
        }

        /// <summary>
        /// Gets or sets Telegrams.
        /// </summary>
        protected List<byte[]> Telegrams { get; set; }

        /// <summary>
        /// Gets or sets the number of telegrams read from the stream.
        /// </summary>
        protected int TelegramsRead { get; set; }

        /// <summary>
        /// Gets or sets the index of the next telegram that
        /// will be read from the stream.
        /// </summary>
        protected int IndexNextTelegram { get; set; }

        /// <summary>
        /// Reads all the bytes currently available on the stream.
        /// </summary>
        /// <returns>
        /// All the bytes currently availbale on the stream.
        /// </returns>
        public virtual byte[] ReadAvailableBytes()
        {
            this.IndexNextTelegram = (this.IndexNextTelegram + 1) % this.Telegrams.Count;
            this.TelegramsRead++;
            return this.Telegrams[this.IndexNextTelegram];
        }
    }
}