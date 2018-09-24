// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramHandlerXimpleEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramHandlerXimpleEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Ibis.Handlers;

    /// <summary>
    /// Event arguments containing an <see cref="ITelegramHandler"/> and a <see cref="Ximple"/> object.
    /// </summary>
    public class TelegramHandlerXimpleEventArgs : TelegramHandlerEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramHandlerXimpleEventArgs"/> class.
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        public TelegramHandlerXimpleEventArgs(ITelegramHandler handler, Ximple ximple)
            : base(handler)
        {
            this.Ximple = ximple;
        }

        /// <summary>
        /// Gets the Ximple object.
        /// </summary>
        public Ximple Ximple { get; private set; }
    }
}