// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramDataEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramDataEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Parsers
{
    using System;

    /// <summary>
    /// Event that contains a binary telegram.
    /// </summary>
    public class TelegramDataEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramDataEventArgs"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public TelegramDataEventArgs(byte[] data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets the binary telegram.
        /// </summary>
        public byte[] Data { get; private set; }
    }
}