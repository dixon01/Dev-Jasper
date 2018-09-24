namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Class describing a Direct3D device that contains a 
    /// unique supported device type
    /// </summary>
    public class EnumDeviceInformation
    {
        public uint AdapterOrdinal; // Ordinal for this adapter
        public DeviceType DeviceType; // Type of the device
        public Caps Caps; // Capabilities of the device
        public ArrayList deviceSettingsList = new ArrayList(); // Array with unique set of adapter format, back buffer format, and windowed
    }
}