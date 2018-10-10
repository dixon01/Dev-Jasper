// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIsiSerializerHook.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIsiSerializerHook type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi
{
    using Gorba.Common.Protocols.Isi.Messages;

    /// <summary>
    /// Hook that allows to get notifications when ISI messages
    /// are serialized or deserialized in an <see cref="IsiSerializer"/>.
    /// </summary>
    public interface IIsiSerializerHook
    {
        /// <summary>
        /// Message has been serialized.
        /// </summary>
        /// <param name="message">
        /// The message the was serialized.
        /// </param>
        /// <param name="output">
        /// The serialized message.
        /// </param>
        void MessageSerialized(IsiMessageBase message, string output);

        /// <summary>
        /// Message has been deserialized.
        /// </summary>
        /// <param name="input">
        /// The serialized message input.
        /// </param>
        /// <param name="message">
        /// The deserialized message.
        /// </param>
        void MessageDeserialized(string input, IsiMessageBase message);
    }
}
