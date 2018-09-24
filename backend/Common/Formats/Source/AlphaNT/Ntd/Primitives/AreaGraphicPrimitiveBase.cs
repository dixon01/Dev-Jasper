// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AreaGraphicPrimitiveBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AreaGraphicPrimitiveBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    /// <summary>
    /// Base class for all graphic primitives that have a width and a height.
    /// </summary>
    public abstract class AreaGraphicPrimitiveBase : GraphicPrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AreaGraphicPrimitiveBase"/> class.
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
        internal AreaGraphicPrimitiveBase(int width, int height, byte displayParam, int offsetX, int offsetY)
            : base(displayParam, offsetX, offsetY)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }
    }
}