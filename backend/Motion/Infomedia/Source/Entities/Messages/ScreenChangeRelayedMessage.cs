// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenChangeRelayedMessage.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>The screen change relay message use to share screen changes.</summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ScreenChangeRelayedMessage<T>
        where T : class
    {
        /// <summary>The from screen change.</summary>
        /// <param name="screenChange">The screen change.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public abstract T FromScreenChange(ScreenChange screenChange);
    }
}