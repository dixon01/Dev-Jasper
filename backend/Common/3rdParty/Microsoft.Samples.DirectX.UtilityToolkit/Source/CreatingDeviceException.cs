namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// Creating the device failed
    /// </summary>
    public class CreatingDeviceException : DirectXSampleException
    {
        public CreatingDeviceException() : base("Failed creating the Direct3D device.") { }
        public CreatingDeviceException(Exception inner) : base("Failed creating the Direct3D device.", inner) {}
    }
}