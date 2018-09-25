// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionGraphicPrimitiveBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PositionGraphicPrimitiveBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Primitives
{
    /// <summary>
    /// Base class for all graphic primitives that have alignment attributes.
    /// </summary>
    public abstract class PositionGraphicPrimitiveBase : GraphicPrimitiveBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionGraphicPrimitiveBase"/> class.
        /// </summary>
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
        internal PositionGraphicPrimitiveBase(int position, byte displayParam, int offsetX, int offsetY)
            : base(displayParam, offsetX, offsetY)
        {
            this.Inverted = (position & 0x01) != 0;

            // TODO: 0x06 is missing, is it also a valid value?
            switch (position & 0x0E)
            {
                case 0x00:
                    this.HorizontalAlignment = HorizontalAlignment.Undefined;
                    this.VerticalAlignment = VerticalAlignment.Undefined;
                    break;
                case 0x02:
                    this.HorizontalAlignment = HorizontalAlignment.Center;
                    this.VerticalAlignment = VerticalAlignment.BottomOrTop;
                    break;
                case 0x04:
                    this.HorizontalAlignment = HorizontalAlignment.Left;
                    this.VerticalAlignment = VerticalAlignment.BottomOrTop;
                    break;
                case 0x06:
                    this.HorizontalAlignment = HorizontalAlignment.Right;
                    this.VerticalAlignment = VerticalAlignment.BottomOrTop;
                    break;
                case 0x08:
                    this.HorizontalAlignment = HorizontalAlignment.Undefined;
                    this.VerticalAlignment = VerticalAlignment.Middle;
                    break;
                case 0x0A:
                    this.HorizontalAlignment = HorizontalAlignment.Center;
                    this.VerticalAlignment = VerticalAlignment.Middle;
                    break;
                case 0x0C:
                    this.HorizontalAlignment = HorizontalAlignment.Left;
                    this.VerticalAlignment = VerticalAlignment.Middle;
                    break;
                default:
                    this.HorizontalAlignment = HorizontalAlignment.Undefined;
                    this.VerticalAlignment = VerticalAlignment.Undefined;
                    break;
            }
        }

        /// <summary>
        /// Gets the vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; private set; }

        /// <summary>
        /// Gets the horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this primitive is drawn inverted.
        /// </summary>
        public bool Inverted { get; private set; }
    }
}