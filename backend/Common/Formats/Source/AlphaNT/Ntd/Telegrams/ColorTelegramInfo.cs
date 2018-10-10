// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorTelegramInfo.cs" company="Gorba AG">
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
    /// chapter 3.4.4. <c>"Definition Darstellung Texte und Grafiken farbig"</c>.
    /// </summary>
    internal class ColorTelegramInfo : TelegramInfoBase
    {
        private readonly BinaryFileReader reader;

        private readonly List<GraphicPrimitiveBase> primitives;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTelegramInfo"/> class.
        /// </summary>
        /// <param name="mode">
        /// The mode byte.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public ColorTelegramInfo(byte mode, BinaryFileReader reader)
            : base(mode)
        {
            this.reader = reader;
            if (!reader.ExtendedFormat || !reader.HasColors)
            {
                throw new NotSupportedException("Only extended (color) format is supported");
            }

            // 1
            if ((mode & 0x0F) != 0)
            {
                throw new FileFormatException("Found unsupported mode 0x" + mode.ToString("X2"));
            }

            this.TelegramNumber = reader.ReadMsbUInt16(); // 2, 3

            this.PrimitiveCount = reader.ReadByte(); // 4
            this.BackgroundColor = new TwoByteColor(reader.ReadMsbUInt16()); // 5, 6
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
            var displayParam = this.reader.ReadByte(); // 7
            var color = new TwoByteColor(this.reader.ReadMsbUInt16()); // 8, 9
            var byte10 = this.reader.ReadByte(); // 10
            var byte11 = this.reader.ReadByte(); // 11
            var byte12 = this.reader.ReadByte(); // 12
            var byte13 = this.reader.ReadByte(); // 13
            var modeAndSpacing = this.reader.ReadByte(); // 14
            var fontAndPosition = this.reader.ReadByte(); // 15
            var offsetX = this.reader.ReadMsbUInt16(); // 16, 17
            var offsetY = this.reader.ReadMsbUInt16(); // 18, 19
            var border = this.reader.ReadByte(); // 20

            this.reader.ReadByte(); // 21, skip lower grey threshold
            this.reader.ReadByte(); // 22, skip upper grey threshold
            this.reader.ReadByte(); // 23, skip spare byte

            var address = new IntPtr(byte10 | (byte11 << 8) | (byte12 << 16));
            var height = (byte10 << 8) | byte11;
            var width = (byte12 << 8) | byte13;

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
                        color,
                        (TextOutline)border,
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