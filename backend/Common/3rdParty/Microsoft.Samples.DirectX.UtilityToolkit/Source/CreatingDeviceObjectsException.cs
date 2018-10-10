namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// Creating the device objects failed
    /// </summary>
    public class CreatingDeviceObjectsException : DirectXSampleException
    {
        public CreatingDeviceObjectsException() : base("Failed creating Direct3D device objects.") { }
        public CreatingDeviceObjectsException(Exception inner) : base("Failed creating Direct3D device objects.", inner) {}
    }
}