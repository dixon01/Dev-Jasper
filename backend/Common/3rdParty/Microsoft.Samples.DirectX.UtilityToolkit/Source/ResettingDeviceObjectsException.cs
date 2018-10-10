namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// Resetting the device failed
    /// </summary>
    public class ResettingDeviceObjectsException : DirectXSampleException
    {
        public ResettingDeviceObjectsException() : base("Failed resetting Direct3D device objects.") { }
        public ResettingDeviceObjectsException(Exception inner) : base("Failed resetting Direct3D device objects.", inner) {}
    }
}