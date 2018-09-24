// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Protran.GorbaProtocol
{
    using System;

    using Gorba.Common.Protocols.GorbaProtocol;
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// The MessageHandler interface.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    public interface IMessageHandler<T>
        where T : GorbaMessage
    {
        /// <summary>
        /// The ximple created.
        /// </summary>
        event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// The process message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void ProcessMessage(T message);
    }
}