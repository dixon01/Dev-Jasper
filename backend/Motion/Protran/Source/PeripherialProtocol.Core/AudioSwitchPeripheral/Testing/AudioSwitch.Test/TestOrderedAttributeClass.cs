namespace AudioSwitch.Test
{
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Models;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = PeripheralHeader.Size)]
    class TestOrderedAttributeClass
    {
        [Order(0)]
        public byte Field0 { get; set; }

        [Order(1)]
        public byte Field1 { get; set; }

        [Order(2)]
        public byte Field2 { get; set; }

        [Order(3)]
        public byte Field3 { get; set; }
    }
}