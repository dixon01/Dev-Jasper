namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// A depth/stencil buffer format that is incompatible
    /// with a multisample type
    /// </summary>
    public struct EnumDepthStencilMultisampleConflict
    {
        public DepthFormat DepthStencilFormat;
        public MultiSampleType MultisampleType;
    }
}