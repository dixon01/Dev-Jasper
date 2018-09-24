
namespace Gorba.Motion.Infomedia.SharpDXRendererTest.DxExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Text;

    using SharpDX;
    using SharpDX.Direct3D9;

    public static class TextureFactory
    {
        public static Texture FromBitmap(Device device, Bitmap bitmap, Usage usage, Pool pool)
        {
            const Format DefaultTextureFormat = Format.A8R8G8B8;
            var size = bitmap.Size;
            var format = GetFormat(bitmap.PixelFormat);
            Texture texture;
            if (format == Format.Unknown)
            {
                format = DefaultTextureFormat;
            }
            else
            {
                try
                {
                    // we try to make it fast, if it fails we go into normal mode (see below)
                    texture = new Texture(device, size.Width, size.Height, 1, usage, format, pool);
                    CopyBitmapFast(texture, bitmap);
                    return texture;
                }
                catch (Exception)
                {
                    format = DefaultTextureFormat;
                }
            }

            texture = new Texture(device, size.Width, size.Height, 1, usage, format, pool);
            CopyBitmapSlow(texture, bitmap);
            return texture;
        }

        private static unsafe void CopyBitmapSlow(Texture texture, Bitmap bitmap)
        {
            var rect = texture.LockRectangle(0, LockFlags.None);
            var ptr = (byte*)rect.DataPointer.ToPointer();
            try
            {
                // Convert all pixels
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        *(int*)(ptr + (x * 4)) = pixel.ToArgb();
                    }

                    ptr += rect.Pitch;
                }
            }
            finally
            {
                texture.UnlockRectangle(0);
            }
        }

        private static void CopyBitmapFast(Texture texture, Bitmap bitmap)
        {
            //Marshal.Copy();
            throw new NotImplementedException();
        }

        private static Format GetFormat(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format16bppArgb1555:
                    return Format.A1R5G5B5;
                case PixelFormat.Format24bppRgb:
                    return Format.R8G8B8;
                case PixelFormat.Format32bppRgb:
                    return Format.X8R8G8B8;
                case PixelFormat.Format32bppArgb:
                    return Format.A8R8G8B8;
                case PixelFormat.Format64bppArgb:
                    return Format.A16B16G16R16;
                default:
                    return Format.Unknown;
            }
        }
    }
}
