// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramHandlerEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramHandlerEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using System;

    using Gorba.Motion.Protran.Ibis.Handlers;

    /// <summary>
    /// Event argument that carries an <see cref="ITelegramHandler"/>.
    /// </summary>
    public class TelegramHandlerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramHandlerEventArgs"/> class.
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public TelegramHandlerEventArgs(ITelegramHandler handler)
        {
            this.Handler = handler;
        }

        /// <summary>
        /// Gets the associated telegram handler.
        /// </summary>
        public ITelegramHandler Handler { get; private set; }
    }
}
