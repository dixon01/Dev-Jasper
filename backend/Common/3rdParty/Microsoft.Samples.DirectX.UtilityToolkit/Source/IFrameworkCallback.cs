namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using Microsoft.DirectX.Direct3D;

    /// <summary>Interface that the framework will use to call into samples</summary>
    public interface IFrameworkCallback
    {
        void OnFrameMove(Device device, double totalTime, float elapsedTime);
        void OnFrameRender(Device device, double totalTime, float elapsedTime);
    }
}