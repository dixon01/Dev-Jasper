// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralBaseMessageType.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral base message type.</summary>
    /// <typeparam name="TMesssageType"></typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Constants.PeripheralHeaderSize + sizeof(byte))]
    public class PeripheralBaseMessageType<TMesssageType> : IPeripheralBaseMessageType<TMesssageType>
    {
        /// <summary>The size.</summary>
        public const int Size = Constants.PeripheralHeaderSize + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessageType{TMesssageType}"/> class.</summary>
        public PeripheralBaseMessageType()
        {
            this.Header = new PeripheralHeader<TMesssageType>();
            this.Checksum = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessageType{TMesssageType}"/> class.</summary>
        /// <param name="header">The header.</param>
        /// <param name="checksum">The checksum.</param>
        public PeripheralBaseMessageType(PeripheralHeader<TMesssageType> header, byte checksum)
        {
            this.Checksum = checksum;
            this.Header = header;
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader<TMesssageType> Header { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }

        public bool IsAck
        {
            get
            {
                return this.Header.IsAck;
            }
        }

        public bool IsNak
        {
            get
            {
                return this.Header.IsNak;
            }
        }
    }
}