// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DevModeFieldsFlags.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    using System;

    /// <summary>
    /// Specifies whether certain members of the DEVMODE structure have been initialized.
    /// If a member is initialized, its corresponding bit is set, otherwise the bit is clear.
    /// </summary>
    [Flags]
    public enum DevModeFieldsFlags
    {
        /// <summary>
        /// The orientation.
        /// </summary>
        Orientation = 0x1,

        /// <summary>
        /// The paper size.
        /// </summary>
        PaperSize = 0x2,

        /// <summary>
        /// The paper length.
        /// </summary>
        PaperLength = 0x4,

        /// <summary>
        /// The paper width.
        /// </summary>
        PaperWidth = 0x8,

        /// <summary>
        /// The scale.
        /// </summary>
        Scale = 0x10,

        /// <summary>
        /// The position.
        /// </summary>
        Position = 0x20,

        /// <summary>
        /// The NUP.
        /// </summary>
        Nup = 0x40,

        /// <summary>
        /// The display orientation.
        /// </summary>
        DisplayOrientation = 0x80,

        /// <summary>
        /// The copies.
        /// </summary>
        Copies = 0x100,

        /// <summary>
        /// The default source.
        /// </summary>
        DefaultSource = 0x200,

        /// <summary>
        /// The print quality.
        /// </summary>
        PrintQuality = 0x400,

        /// <summary>
        /// The color.
        /// </summary>
        Color = 0x800,

        /// <summary>
        /// The duplex.
        /// </summary>
        Duplex = 0x1000,

        /// <summary>
        /// The y resolution.
        /// </summary>
        YResolution = 0x2000,

        /// <summary>
        /// The tt option.
        /// </summary>
        TtOption = 0x4000,

        /// <summary>
        /// The collate.
        /// </summary>
        Collate = 0x8000,

        /// <summary>
        /// The form name.
        /// </summary>
        FormName = 0x10000,

        /// <summary>
        /// The log pixels.
        /// </summary>
        LogPixels = 0x20000,

        /// <summary>
        /// The bits per pixel.
        /// </summary>
        BitsPerPixel = 0x40000,

        /// <summary>
        /// The width.
        /// </summary>
        PelsWidth = 0x80000,

        /// <summary>
        /// The height.
        /// </summary>
        PelsHeight = 0x100000,

        /// <summary>
        /// The display flags.
        /// </summary>
        DisplayFlags = 0x200000,

        /// <summary>
        /// The display frequency.
        /// </summary>
        DisplayFrequency = 0x400000,

        /// <summary>
        /// The ICM method.
        /// </summary>
        IcmMethod = 0x800000,

        /// <summary>
        /// The ICM intent.
        /// </summary>
        IcmIntent = 0x1000000,

        /// <summary>
        /// The media type.
        /// </summary>
        MediaType = 0x2000000,

        /// <summary>
        /// The dither type.
        /// </summary>
        DitherType = 0x4000000,

        /// <summary>
        /// The panning width.
        /// </summary>
        PanningWidth = 0x8000000,

        /// <summary>
        /// The panning height.
        /// </summary>
        PanningHeight = 0x10000000,

        /// <summary>
        /// The display fixed output.
        /// </summary>
        DisplayFixedOutput = 0x20000000
    }
}
