// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringArrayTelegram.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Base class for telegrams that contain an array
    /// of strings as its data (e.g. stop list)
    /// </summary>
    public abstract class StringArrayTelegram : Telegram
    {
        /// <summary>
        /// Gets or sets the data of this telegram.
        /// </summary>
        public string[] Data { get; set; }
    }
}
