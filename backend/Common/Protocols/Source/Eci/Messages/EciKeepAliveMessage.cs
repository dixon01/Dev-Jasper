// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciKeepAliveMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The eci keep alive message.
    /// </summary>
    public class EciKeepAliveMessage : EciMessageBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.KeepAlive;
            }
        }
    }
}
