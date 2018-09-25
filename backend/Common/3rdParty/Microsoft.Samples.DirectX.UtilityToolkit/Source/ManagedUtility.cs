namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Helper methods
    /// </summary>
    internal static class ManagedUtility
    {
        /// <summary>
        /// Gets the number of ColorChanelBits from a format
        /// </summary>
        public static uint GetColorChannelBits(Format format)
        {
            switch (format)
            {
                case Format.R8G8B8:
                case Format.A8R8G8B8:
                case Format.X8R8G8B8:
                    return 8;
                case Format.R5G6B5:
                case Format.X1R5G5B5:
                case Format.A1R5G5B5:
                    return 5;
                case Format.A4R4G4B4:
                case Format.X4R4G4B4:
                    return 4;
                case Format.R3G3B2:
                case Format.A8R3G3B2:
                    return 2;
                case Format.A2B10G10R10:
                case Format.A2R10G10B10:
                    return 10;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the number of alpha channel bits 
        /// </summary>
        public static uint GetAlphaChannelBits(Format format)
        {
            switch (format)
            {
                case Format.X8R8G8B8:
                case Format.R8G8B8:
                case Format.R5G6B5:
                case Format.X1R5G5B5:
                case Format.R3G3B2:
                case Format.X4R4G4B4:
                    return 0;
                case Format.A8R3G3B2:
                case Format.A8R8G8B8:
                    return 8;
                case Format.A1R5G5B5:
                    return 1;
                case Format.A4R4G4B4:
                    return 4;
                case Format.A2B10G10R10:
                case Format.A2R10G10B10:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the number of depth bits
        /// </summary>
        public static uint GetDepthBits(DepthFormat format)
        {
            switch (format)
            {
                case DepthFormat.D16:
                case DepthFormat.D16Lockable:
                    return 16;

                case DepthFormat.D15S1:
                    return 15;

                case DepthFormat.D24X8:
                case DepthFormat.D24S8:
                case DepthFormat.D24X4S4:
                case DepthFormat.D24SingleS8:
                    return 24;

                case DepthFormat.D32:
                case DepthFormat.D32SingleLockable:
                    return 32;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the number of stencil bits
        /// </summary>
        public static uint GetStencilBits(DepthFormat format)
        {
            switch (format)
            {
                case DepthFormat.D16:
                case DepthFormat.D16Lockable:
                case DepthFormat.D24X8:
                case DepthFormat.D32:
                case DepthFormat.D32SingleLockable:
                    return 0;

                case DepthFormat.D15S1:
                    return 1;

                case DepthFormat.D24X4S4:
                    return 4;

                case DepthFormat.D24SingleS8:
                case DepthFormat.D24S8:
                    return 8;

                default:
                    return 0;
            }
        }
    }
}