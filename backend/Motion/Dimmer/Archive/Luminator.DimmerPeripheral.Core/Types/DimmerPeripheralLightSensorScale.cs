namespace Luminator.DimmerPeripheral.Core.Types
{
    using System.ComponentModel;

    public enum DimmerPeripheralLightSensorScale : byte
    {
        [Description("Range 1")]
        Range1 = 0x00,
        [Description("Range 2")]
        Range2 = 0x01,
        [Description("Range 3")]
        Range3 = 0x02,
        [Description("Range 4")]
        Range4 = 0x03
    }
}
