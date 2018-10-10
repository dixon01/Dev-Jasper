namespace AudioSwitch.Test
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;

    /// <summary>The bogus peripheral base message.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = PeripheralHeader.Size)]
    internal class BogusPeripheralBaseMessage : IPeripheralBaseMessage
    {
        /// <summary>Initializes a new instance of the <see cref="BogusPeripheralBaseMessage"/> class.</summary>
        public BogusPeripheralBaseMessage()
        {
            this.Header = new PeripheralHeader();
            this.Checksum = 0;
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }
    }
}