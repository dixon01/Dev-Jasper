namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    /// <summary>
    /// Options on how to match items
    /// </summary>
    public struct MatchOptions
    {
        public MatchType AdapterOrdinal;
        public MatchType DeviceType;
        public MatchType Windowed;
        public MatchType AdapterFormat;
        public MatchType VertexProcessing;
        public MatchType Resolution;
        public MatchType BackBufferFormat;
        public MatchType BackBufferCount;
        public MatchType MultiSample;
        public MatchType SwapEffect;
        public MatchType DepthFormat;
        public MatchType StencilFormat;
        public MatchType PresentFlags;
        public MatchType RefreshRate;
        public MatchType PresentInterval;
    };
}