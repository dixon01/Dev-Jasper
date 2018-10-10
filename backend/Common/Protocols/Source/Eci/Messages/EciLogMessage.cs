// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciLogMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The log message.
    /// </summary>
    public class EciLogMessage : EciMessageBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.Log;
            }
        }

        /// <summary>
        /// Gets or sets the log text.
        /// </summary>
        public string LogText { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public EciLogCode LogLevel { get; set; }
    }
}
