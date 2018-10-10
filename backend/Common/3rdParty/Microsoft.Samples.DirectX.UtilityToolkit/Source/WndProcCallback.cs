namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;

    public delegate IntPtr WndProcCallback(IntPtr hWnd, NativeMethods.WindowMessage msg, IntPtr wParam, IntPtr lParam, ref bool NoFurtherProcessing);
}