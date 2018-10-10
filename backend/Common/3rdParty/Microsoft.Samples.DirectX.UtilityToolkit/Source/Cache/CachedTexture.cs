namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using Microsoft.DirectX.Direct3D;

    /// <summary>Information about a cached texture</summary>
    struct CachedTexture
    {
        public string Source; // Data source
        public int Width;
        public int Height;
        public int Depth;
        public int MipLevels;
        public Usage Usage;
        public Format Format;
        public Pool Pool;
        public ResourceType Type;
    }
}