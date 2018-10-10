// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    /// <summary>
    /// The display mode used in the <see cref="SetupCommandFrame"/>.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// The static bitmap mode (0x00).
        /// </summary>
        StaticBitmap = 0x00,

        /// <summary>
        /// The scrolling bitmap mode (0x01).
        /// </summary>
        ScrollingBitmap = 0x01,

        /// <summary>
        /// The block scroll bitmap mode (0x04).
        /// </summary>
        BlockScrollBitmap = 0x04,

        /// <summary>
        /// The auto text mode (0x10)
        /// </summary>
        AutoText = 0x10,

        /// <summary>
        /// The scroll text mode (0x11)
        /// </summary>
        ScrollText = 0x11,

        /// <summary>
        /// The static text mode (0x12)
        /// </summary>
        StaticText = 0x12,

        /// <summary>
        /// The block scroll bitmap mode for large signs (0x44).
        /// </summary>
        BlockScrollLargeBitmap = 0x44,

        /// <summary>
        /// The block scroll bitmap mode with speed (0x45).
        /// </summary>
        BlockScrollSpeedBitmap = 0x45,

        /// <summary>
        /// Special value for color display mode since this mode is actually not transmitted.
        /// </summary>
        Color = -1
    }
}