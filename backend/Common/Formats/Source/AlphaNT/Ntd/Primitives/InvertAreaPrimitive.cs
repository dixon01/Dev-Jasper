// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvertAreaPrimitive.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InvertAreaPrimitive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    using Gorba.Common.Formats.AlphaNT.Ntd.Telegrams;

    /// <summary>
    /// Primitive that inverts the contents of a given area.
    /// This primitive should always come as the last one in the list of primitives of a telegram
    /// (see <see cref="ITelegramInfo.GetPrimitives"/>).
    /// </summary>
    public class InvertAreaPrimitive : AreaGraphicPrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvertAreaPrimitive"/> class.
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
        public InvertAreaPrimitive(int width, int height, byte displayParam, int offsetX, int offsetY)
            : base(width, height, displayParam, offsetX, offsetY)
        {
        }
    }
}