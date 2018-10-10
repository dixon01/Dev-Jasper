namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// Media couldn't be found
    /// </summary>
    public class MediaNotFoundException : DirectXSampleException
    {
        public MediaNotFoundException() : base("Could not find required media. Ensure that the DirectX SDK is correctly installed.") { }
        public MediaNotFoundException(Exception inner) : base("Could not find required media. Ensure that the DirectX SDK is correctly installed.", inner) {}
    }
}