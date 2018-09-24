// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageField.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessageField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    ///   The interface for the message field
    /// </summary>
    public interface IMessageField
    {
        /// <summary>
        ///   If the message was confirmed from the driver
        /// </summary>
        event EventHandler<IndexEventArgs> Confirmed;

        /// <summary>
        /// This text will be shown if no other messages are active. Add here the destination
        /// </summary>
        /// <param name="txt">
        /// The text.
        /// </param>
        void SetDestinationText(string txt);

        /// <summary>
        ///   Shows a message
        /// </summary>
        /// <param name = "type">type of the message</param>
        /// <param name = "text">message text</param>
        /// <param name = "msgId">the ID of this message</param>
        void ShowMessage(MessageType type, string text, int msgId);
    }
}