// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInputHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IInputHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Handler for <see cref="Telegram"/>s.
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Event that is fired if <see cref="HandleInput"/> created
        /// some data. In rare cases like DS021a, this event might also be
        /// fired asynchronously through a timer that elapsed.
        /// </summary>
        event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired whenever an handler changes its status
        /// depending on the content of the events that it has to parse.
        /// </summary>
        event EventHandler StatusChanged;

        /// <summary>
        /// Gets the priority, the lower the number the higher the priority.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets the status of this handler. By default this should return
        /// <see cref="Gorba.Common.Configuration.Protran.Ibis.Telegrams.Status.Ok"/>.
        /// </summary>
        Status Status { get; }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// Implementations of this method usually check the type of the event.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given event.
        /// </returns>
        bool Accept(Telegram telegram);

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        void HandleInput(Telegram telegram);

        /// <summary>
        /// To start the handler and check for persistence. Update status of handler
        /// based on persistence.
        /// </summary>
        void StartCheck();
    }
}