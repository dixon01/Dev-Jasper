namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using Microsoft.DirectX.Direct3D;

    /// <summary>Interface that the framework will use to determine if a device is acceptable</summary>
    public interface IDeviceCreation
    {
        bool IsDeviceAcceptable(Caps caps, Format adapterFormat, Format backBufferFormat, bool isWindowed);
        void ModifyDeviceSettings(DeviceSettings settings, Caps caps);
    }
}