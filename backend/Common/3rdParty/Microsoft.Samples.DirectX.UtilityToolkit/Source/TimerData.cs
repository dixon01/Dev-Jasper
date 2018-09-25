namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    /// <summary>Stores timer callback information</summary>
    public struct TimerData
    {
        public TimerCallback callback;
        public float TimeoutInSecs;
        public float Countdown;
        public bool IsEnabled;
    }
}