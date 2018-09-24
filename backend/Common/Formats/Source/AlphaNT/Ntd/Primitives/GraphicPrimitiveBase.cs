// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicPrimitiveBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicPrimitiveBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    using Gorba.Common.Formats.AlphaNT.Ntd.Telegrams;

    /// <summary>
    /// Base class for graphic primitives that can be defined in a <see cref="ITelegramInfo"/>.
    /// </summary>
    public abstract class GraphicPrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicPrimitiveBase"/> class.
        /// </summary>
        /// <param name="displayParam">
        /// The display parameter.
        /// </param>
        /// <param name="offsetX">
        /// The horizontal offset.
        /// </param>
        /// <param name="offsetY">
        /// The vertical offset.
        /// </param>
        internal GraphicPrimitiveBase(byte displayParam, int offsetX, int offsetY)
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;

            this.DisplayDuration = displayParam & 0x3F;

            var status = displayParam & 0xC0;
            this.DeleteAfterDisplay = status == 0x40;
            this.Scrolling = status == 0x80;
        }

        /// <summary>
        /// Gets the horizontal offset of this primitive.
        /// </summary>
        public int OffsetX { get; private set; }

        /// <summary>
        /// Gets the vertical offset of this primitive.
        /// </summary>
        public int OffsetY { get; private set; }

        /// <summary>
        /// Gets the display duration in seconds, 0 means "info" (i.e. show forever).
        /// </summary>
        public int DisplayDuration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to delete the contents after displaying it.
        /// </summary>
        public bool DeleteAfterDisplay { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to scroll the primitive.
        /// </summary>
        public bool Scrolling { get; private set; }
    }
}