namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// The No Direct3D exception.  Something really had to go wrong for this to occur.
    /// </summary>
    public class NoDirect3DException : DirectXSampleException
    {
        public NoDirect3DException() : base("Could not initialize Direct3D. You may want to check that the latest version of DirectX is correctly installed on your system.") {}
        public NoDirect3DException(Exception inner) : base("Could not initialize Direct3D. You may want to check that the latest version of DirectX is correctly installed on your system.", inner) {}
    }
}