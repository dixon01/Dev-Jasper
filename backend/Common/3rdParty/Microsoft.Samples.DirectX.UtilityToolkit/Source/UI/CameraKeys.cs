namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    /// <summary>
    /// Used to map keys to the camera
    /// </summary>
    public enum CameraKeys : byte
    {
        StrafeLeft,
        StrafeRight,
        MoveForward,
        MoveBackward,
        MoveUp,
        MoveDown,
        Reset,
        ControlDown,
        MaxKeys,
        Unknown=0xff
    }
}