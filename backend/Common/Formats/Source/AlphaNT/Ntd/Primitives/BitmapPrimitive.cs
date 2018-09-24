// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapPrimitive.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BitmapPrimitive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    using System;

    /// <summary>
    /// Primitive that draws a bitmap.
    /// </summary>
    public class BitmapPrimitive : PositionGraphicPrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapPrimitive"/> class.
        /// </summary>
        /// <param name="address">
        /// The address of the bitmap.
        /// </param>
        /// <param name="position">
        /// The position data.
        /// </param>
        /// <param name="displayParam">
        /// The display parameter.
        /// </param>
        /// <param name="offsetX">
        /// The horizontal offset.
        /// </param>
        /// <param name="offsetY">
        /// The vertical offset.
        /// </param>
        internal BitmapPrimitive(IntPtr address, int position, byte displayParam, int offsetX, int offsetY)
            : base(position, displayParam, offsetX, offsetY)
        {
            this.BitmapAddress = address;
        }

        /// <summary>
        /// Gets the address of the bitmap in the file. See also <see cref="NtdFile.GetBitmap"/>.
        /// </summary>
        public IntPtr BitmapAddress { get; private set; }
    }
}