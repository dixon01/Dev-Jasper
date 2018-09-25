namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Class describing device settings that contain a unique combination
    /// of adapter format, back buffer format, and windowed that is compatible
    /// with a particular Direct3D device and the application
    /// </summary>
    public class EnumDeviceSettingsCombo
    {
        public uint AdapterOrdinal;
        public DeviceType DeviceType;
        public Format AdapterFormat;
        public Format BackBufferFormat;
        public bool IsWindowed;

        // Array lists
        public ArrayList depthStencilFormatList = new ArrayList();
        public ArrayList multiSampleTypeList = new ArrayList();
        public ArrayList multiSampleQualityList = new ArrayList();
        public ArrayList presentIntervalList = new ArrayList();
        public ArrayList depthStencilConflictList = new ArrayList();

        public EnumAdapterInformation adapterInformation = null;
        public EnumDeviceInformation deviceInformation = null;
    }
}