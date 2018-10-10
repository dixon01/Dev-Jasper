namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    /// <summary>
    /// Mouse button mask values
    /// </summary>
    [Flags]
    public enum MouseButtonMask : byte
    {
        None = 0,
        Left = 0x01,
        Middle = 0x02,
        Right = 0x04,
        Wheel = 0x08,
    }
}