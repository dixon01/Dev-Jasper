namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// Resetting the device failed
    /// </summary>
    public class ResettingDeviceException : DirectXSampleException
    {
        public ResettingDeviceException() : base("Failed resetting the Direct3D device.") { }
        public ResettingDeviceException(Exception inner) : base("Failed resetting the Direct3D device.", inner) {}
    }
}