// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyTelegram.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyTelegram type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    /// <summary>
    /// Base class for telegrams that don't contain a payload.
    /// This is used to filter telegrams that don't need transformations.
    /// </summary>
    public abstract class EmptyTelegram : Telegram
    {
    }
}