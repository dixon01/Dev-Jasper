// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    /// <summary>
    /// The text mode.
    /// </summary>
    public enum TextMode
    {
        /// <summary>
        /// The text is rendered using <see cref="Microsoft.DirectX.Direct3D.Font"/>
        /// but without a sprite caching the rendered triangles.
        /// </summary>
        Font,

        /// <summary>
        /// The text is rendered using <see cref="Microsoft.DirectX.Direct3D.Font"/>
        /// using a sprite to cache the rendered triangles.
        /// </summary>
        FontSprite,

        /// <summary>
        /// The text is rendered using GDI to a bitmap which is then rendered using
        /// a texture.
        /// </summary>
        Gdi
    }
}