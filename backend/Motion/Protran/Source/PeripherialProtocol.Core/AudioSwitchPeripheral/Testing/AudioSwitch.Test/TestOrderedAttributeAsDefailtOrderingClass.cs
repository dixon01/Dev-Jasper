namespace AudioSwitch.Test
{
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Models;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = PeripheralHeader.Size)]
    class TestOrderedAttributeAsDefailtOrderingClass
    {
        [Order]
        public byte Field0 { get; set; }
        [Order]
        public byte Field1 { get; set; }

        [Order]
        public byte Field2 { get; set; }
        [Order]
        public byte Field3 { get; set; }
    }
}