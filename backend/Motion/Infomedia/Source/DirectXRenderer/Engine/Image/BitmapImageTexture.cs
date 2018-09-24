// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapImageTexture.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BitmapImageTexture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image
{
    using System.Drawing;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Image texture that simply takes a bitmap and displays it.
    /// </summary>
    public class BitmapImageTexture : ImageTextureBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapImageTexture"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap this bitmap is not taken over, it won't be disposed.
        /// </param>
        /// <param name="device">
        /// The device.
        /// </param>
        public BitmapImageTexture(Bitmap bitmap, Device device)
            : base(device)
        {
            this.Initialize(bitmap);
        }
    }
}
