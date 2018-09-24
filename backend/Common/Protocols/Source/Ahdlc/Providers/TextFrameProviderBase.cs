// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFrameProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextFrameProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Base class for text frame providers.
    /// </summary>
    public abstract class TextFrameProviderBase : FrameProviderBase
    {
        private const int MaxOutputSize = 30;

        private readonly string[] texts;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFrameProviderBase"/> class.
        /// </summary>
        /// <param name="mode">
        /// The text display mode.
        /// </param>
        /// <param name="blockGap">
        /// The gap between text blocks.
        /// </param>
        /// <param name="padding">
        /// The padding after the last text block.
        /// </param>
        /// <param name="texts">
        /// The texts (maximum: 3).
        /// </param>
        protected TextFrameProviderBase(DisplayMode mode, int blockGap, int padding, params string[] texts)
        {
            if (texts == null || texts.Length < 1 || texts.Length > 3)
            {
                throw new ArgumentException("1 to 3 texts allowed");
            }

            this.texts = texts;
            var charCount = 0;
            foreach (var text in texts)
            {
                charCount += text.Length + 1;
            }

            this.SetupCommand = new SetupCommandFrame(mode)
                                    {
                                        DataBlockCount = (charCount + MaxOutputSize - 1) / MaxOutputSize
                                    };
            this.SetupCommand.Data[3] = 3; // change time (not documented)
            this.SetupCommand.Data[4] = (byte)blockGap;
            this.SetupCommand.Data[5] = (byte)padding;
            this.SetupCommand.Data[6] = 1; // unknown default 1
        }

        /// <summary>
        /// Gets the output commands to be sent after the setup command.
        /// </summary>
        /// <returns>
        /// An enumeration over all output commands in the order they need to be sent.
        /// </returns>
        public override IEnumerable<OutputCommandFrame> GetOutputCommands()
        {
            var data = new List<byte>();
            var first = true;
            foreach (var text in this.texts)
            {
                if (!first)
                {
                    data.Add((byte)'\n'); // <LF>
                }

                first = false;
                foreach (var c in text)
                {
                    data.Add((byte)c);
                }
            }

            data.Add((byte)'\r'); // <CR>

            var offset = 0;
            byte blockNumber = 0;
            while (data.Count - offset > 0)
            {
                var frame = new byte[Math.Min(MaxOutputSize, data.Count - offset)];
                data.CopyTo(offset, frame, 0, frame.Length);
                offset += frame.Length;

                yield return new OutputCommandFrame { BlockNumber = blockNumber++, Data = frame };
            }
        }
    }
}