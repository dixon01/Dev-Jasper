// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedTelegramInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColorTelegramInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Telegrams
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Formats.AlphaNT.Ntd.Primitives;

    /// <summary>
    /// Implementation of <see cref="ITelegramInfo"/> for
    /// chapter 3.4.2. <c>"Definition Darstellung Texte und Grafiken erweitert"</c>.
    /// </summary>
    internal class ExtendedTelegramInfo : TelegramInfoBase
    {
        private readonly BinaryFileReader reader;

        private readonly List<GraphicPrimitiveBase> primitives;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedTelegramInfo"/> class.
        /// </summary>
        /// <param name="mode">
        /// The mode byte.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public ExtendedTelegramInfo(byte mode, BinaryFileReader reader)
            : base(mode)
        {
            this.reader = reader;
            if (!reader.ExtendedFormat || reader.HasColors)
            {
                throw new NotSupportedException("Only extended (non-color) format is supported");
            }

            // 1
            if ((mode & 0x0F) != 0)
            {
                throw new FileFormatException("Found unsupported mode 0x" + mode.ToString("X2"));
            }

            this.TelegramNumber = reader.ReadMsbUInt16(); // 2, 3

            this.PrimitiveCount = reader.ReadByte(); // 4
            this.primitives = new List<GraphicPrimitiveBase>(this.PrimitiveCount);

            for (int i = 0; i < this.PrimitiveCount; i++)
            {
                this.primitives.Add(this.ReadPrimitive());
            }
        }

        /// <summary>
        /// Gets all primitives.
        /// </summary>
        /// <returns>
        /// The list of <see cref="GraphicPrimitiveBase"/>-subclass objects.
        /// </returns>
        public override IEnumerable<GraphicPrimitiveBase> GetPrimitives()
        {
            return this.primitives;
        }

        private GraphicPrimitiveBase ReadPrimitive()
        {
            var displayParam = this.reader.ReadByte(); // 5
            var byte6 = this.reader.ReadByte(); // 6
            var byte7 = this.reader.ReadByte(); // 7
            var byte8 = this.reader.ReadByte(); // 8
            var byte9 = this.reader.ReadByte(); // 9
            var modeAndSpacing = this.reader.ReadByte(); // 10
            var fontAndPosition = this.reader.ReadByte(); // 11
            var offsetX = this.reader.ReadMsbUInt16(); // 12, 13
            var offsetY = this.reader.ReadMsbUInt16(); // 14, 15

            var address = new IntPtr(byte6 | (byte7 << 8) | (byte8 << 16));
            var height = (byte6 << 8) | byte7;
            var width = (byte8 << 8) | byte9;

            var font = (fontAndPosition >> 4) & 0x0F;
            var position = fontAndPosition & 0x0F;

            var mode = modeAndSpacing & 0xE0;
            var spacing = modeAndSpacing & 0x1F;
            switch (mode)
            {
                case 0x00:
                    return new TextPrimitive(
                        address,
                        font,
                        spacing,
                        Colors.White,
                        TextOutline.None,
                        position,
                        displayParam,
                        offsetX,
                        offsetY);
                case 0x20:
                    return new BitmapPrimitive(address, position, displayParam, offsetX, offsetY);
                case 0x40:
                    return new DeleteAreaPrimitive(width, height, displayParam, offsetX, offsetY);
                case 0x80:
                    return new InvertAreaPrimitive(width, height, displayParam, offsetX, offsetY);
                default:
                    throw new FileFormatException("Unknown mode: 0x" + mode.ToString("X2"));
            }
        }
    }
}