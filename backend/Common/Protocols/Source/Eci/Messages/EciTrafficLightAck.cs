// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciTrafficLightAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The traffic light acknowledgment.
    /// </summary>
    public class EciTrafficLightAck : EciTrafficLightBase
    {
        /// <summary>
        /// Gets the sub type.
        /// </summary>
        public override EciTrafficLightCode SubType
        {
            get
            {
                return EciTrafficLightCode.Ack;
            }
        }

        /// <summary>
        /// Gets or sets the value related to the type.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets the acknowledge.
        /// </summary>
        public int Acknowledge { get; set; }
    }
}
