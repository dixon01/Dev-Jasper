// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteAreaPrimitive.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeleteAreaPrimitive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    /// <summary>
    /// Primitive that deletes the contents of a given area.
    /// </summary>
    public class DeleteAreaPrimitive : AreaGraphicPrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteAreaPrimitive"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
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
        public DeleteAreaPrimitive(int width, int height, byte displayParam, int offsetX, int offsetY)
            : base(width, height, displayParam, offsetX, offsetY)
        {
        }
    }
}