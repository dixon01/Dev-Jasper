// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDxDeviceRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDxDeviceRenderContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// A render context that contains device specific information.
    /// </summary>
    public interface IDxDeviceRenderContext : IDxRenderContext
    {
        /// <summary>
        /// Gets the DirectX device.
        /// </summary>
        Device Device { get; }

        /// <summary>
        /// Creates a new image texture or takes it from the cache.
        /// It is important to release the returned texture again using
        /// <see cref="ReleaseImageTexture"/> once you are not displaying
        /// the image anymore.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="IImageTexture"/>.
        /// </returns>
        IImageTexture GetImageTexture(string filename);

        /// <summary>
        /// Creates a new image texture or takes it from the cache.
        /// It is important to release the returned texture again using
        /// <see cref="ReleaseImageTexture"/> once you are not displaying
        /// the image anymore.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap for which the texture is created.
        /// </param>
        /// <returns>
        /// The <see cref="IImageTexture"/>.
        /// </returns>
        IImageTexture GetImageTexture(Bitmap bitmap);

        /// <summary>
        /// Releases the given image texture.
        /// This will release the underlying resources if the image
        /// is no longer used and shouldn't be kept in a cache.
        /// </summary>
        /// <param name="texture">
        /// The texture.
        /// </param>
        void ReleaseImageTexture(IImageTexture texture);

        /// <summary>
        /// Creates a new font or takes one from the cache.
        /// </summary>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="weight">
        /// The weight.
        /// </param>
        /// <param name="mipLevels">
        /// The number of MIP levels.
        /// </param>
        /// <param name="italic">
        /// The italic.
        /// </param>
        /// <param name="charSet">
        /// The char set.
        /// </param>
        /// <param name="outputPrecision">
        /// The output precision.
        /// </param>
        /// <param name="quality">
        /// The quality.
        /// </param>
        /// <param name="pitchAndFamily">
        /// The pitch and family.
        /// </param>
        /// <param name="fontName">
        /// The font name.
        /// </param>
        /// <returns>
        /// The <see cref="FontInfo"/>.
        /// </returns>
        IFontInfo GetFontInfo(
            int height,
            int width,
            FontWeight weight,
            int mipLevels,
            bool italic,
            CharacterSet charSet,
            Precision outputPrecision,
            FontQuality quality,
            PitchAndFamily pitchAndFamily,
            string fontName);

        /// <summary>
        /// Creates a new font or takes one from the cache.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The <see cref="FontInfo"/>.
        /// </returns>
        IFontInfo GetFontInfo(FontDescription description);
    }
}