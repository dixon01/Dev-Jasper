// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpServerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdpServerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;

    /// <summary>
    /// Configuration for the IBIS over UDP server.
    /// </summary>
    [Serializable]
    public class UdpServerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpServerConfig"/> class.
        /// </summary>
        public UdpServerConfig()
        {
            this.LocalPort = 47555;
            this.ReceiveFormat = TelegramFormat.Full;
            this.SendFormat = TelegramFormat.Full;
        }

        /// <summary>
        /// Gets or sets the local UDP Port.
        /// </summary>
        public int LocalPort { get; set; }

        /// <summary>
        /// Gets or sets the expected format of the received telegrams.
        /// Default value: <see cref="TelegramFormat.NoChecksum"/>
        /// </summary>
        public TelegramFormat ReceiveFormat { get; set; }

        /// <summary>
        /// Gets or sets the format to be used to send answer telegrams.
        /// Default value: <see cref="TelegramFormat.NoFooter"/>
        /// </summary>
        public TelegramFormat SendFormat { get; set; }
    }
}
