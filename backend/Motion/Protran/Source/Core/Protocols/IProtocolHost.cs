// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProtocolHost.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the functions that an "host" must implement
//   in order to be considered a valid "host".
//   An "host" is the object that will receive data from
//   the loaded protocols and that will send other data to the protocols.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Protocols
{
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Defines the functions that an "host" must implement
    /// in order to be considered a valid "host".
    /// An "host" is the object that will receive data from
    /// the loaded protocols and that will send other data to the protocols.
    /// </summary>
    public interface IProtocolHost
    {
        /// <summary>
        /// The "host"'s function that will be invoked whenever a protocol
        /// wants send some data to the "host" itself.
        /// </summary>
        /// <param name="sender">The protocol that sends the data to the "host".</param>
        /// <param name="data">The data sent from the protocol "sender" to the "host".</param>
        void OnDataFromProtocol(IProtocol sender, Ximple data);
    }
}
