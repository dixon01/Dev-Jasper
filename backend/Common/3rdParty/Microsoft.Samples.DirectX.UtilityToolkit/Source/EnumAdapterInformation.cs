namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Class describing an adapter which contains a unique adapter ordinal that
    /// is installed on the system.
    /// </summary>
    public class EnumAdapterInformation
    {
        public uint AdapterOrdinal; // Ordinal for this adapter
        public AdapterDetails AdapterInformation; // Information about this adapter
        public ArrayList displayModeList = new ArrayList(); // Array of display modes
        public ArrayList deviceInfoList = new ArrayList(); // Array of device information
        public string UniqueDescription; // Unique description of this device
    }
}