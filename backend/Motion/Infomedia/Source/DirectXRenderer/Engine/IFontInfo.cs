// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFontInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFontInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine
{
    using System;

    using Gorba.Motion.Infomedia.DirectXRenderer.DxExtensions;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Information about a DirectX font.
    /// </summary>
    public interface IFontInfo
    {
        /// <summary>
        /// Gets the underlying DirectX font.
        /// </summary>
        Font Font { get; }

        /// <summary>
        /// Gets the width of a single space.
        /// </summary>
        int SpaceWidth { get; }

        /// <summary>
        /// Gets the text metrics.
        /// </summary>
        TextMetric Metrics { get; }
    }
}