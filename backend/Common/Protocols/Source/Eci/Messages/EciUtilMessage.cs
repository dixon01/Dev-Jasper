// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciUtilMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EciUtilMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The ECI utility message.
    /// </summary>
    public class EciUtilMessage : EciMessageBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.Util;
            }
        }

        /// <summary>
        /// Gets or sets the sub-type.
        /// </summary>
        public EciRequestCode SubType { get; set; }
    }
}