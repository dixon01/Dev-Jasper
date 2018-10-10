namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// No compatible devices were found for this application.  
    /// </summary>
    public class NoCompatibleDevicesException : DirectXSampleException
    {
        public NoCompatibleDevicesException() : base("Could not find any compatible Direct3D devices.") { }
        public NoCompatibleDevicesException(Exception inner) : base("Could not find any compatible Direct3D devices.", inner) {}
    }
}