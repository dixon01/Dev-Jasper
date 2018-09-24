// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetMessageBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The qnet message base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    using NLog;

    /// <summary>
    /// Defines the base qnet message.
    /// </summary>
    public abstract class QnetMessageBase
    {
        private static readonly Logger Logger = LogManager.GetLogger("GlobalLog");

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetMessageBase"/> class.
        /// </summary>
        /// <param name="sourceAddress">
        /// The source address.
        /// </param>
        /// <param name="destAddress">
        /// The destination address.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        protected QnetMessageBase(ushort sourceAddress, ushort destAddress, ushort gatewayAddress)
        {
            this.SourceAddress = sourceAddress;
            this.DestAddress = destAddress;
            this.GatewayAddress = gatewayAddress;

            this.DottedSourceAddress = new QnetAddress(sourceAddress).DottedAddress;
            this.DottedDestAddress = new QnetAddress(destAddress).DottedAddress;
            try
            {
                this.DottedGatewayAddress = new QnetAddress(gatewayAddress).DottedAddress;
            }
            catch (Exception exception)
            {
                Logger.DebugException(
                    string.Format("Value {0} is not a valid gateway address", gatewayAddress), exception);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets DataLength.
        /// </summary>
        public byte DataLength
        {
            get
            {
                return this.GetDataLenght();
            }
        }

        /// <summary>
        /// Gets or sets the destination address as short of the synchronizer server.
        /// </summary>
        public ushort DestAddress { get; protected set; }

        /// <summary>
        /// Gets or sets the address of the gateway.
        /// </summary>
        public ushort GatewayAddress { get; set; }

        /// <summary>
        /// Gets or sets the readable address of the gateway.
        /// </summary>
        public string DottedGatewayAddress { get; set; }

        /// <summary>
        /// Gets or sets the readable destination address (A:2.1.0 for example)
        /// </summary>
        public string DottedDestAddress { get; protected set; }

        /// <summary>
        /// Gets or sets qnet address as unsigned short.
        /// </summary>
        public ushort SourceAddress { get; protected set; }

        /// <summary>
        /// Gets or sets the readable unit source address (A:2.1.0 for example)
        /// </summary>
        public string DottedSourceAddress { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the length of data in bytes.
        /// </summary>
        /// <returns>
        /// Type : byte
        /// The number of bytes contained in data.
        /// </returns>
        protected virtual byte GetDataLenght()
        {
            return 0;
        }

        #endregion
    }
}