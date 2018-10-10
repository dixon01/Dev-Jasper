// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FramesEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FramesEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Providers;

    /// <summary>
    /// <see cref="EventArgs"/> subclass that has a list of frames.
    /// </summary>
    public class FramesEventArgs : EventArgs
    {
        private readonly IEnumerable<LongFrameBase> frames;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramesEventArgs"/> class
        /// with a single frame.
        /// </summary>
        /// <param name="frame">
        /// The frame to be returned by <see cref="GetFrames"/>.
        /// </param>
        public FramesEventArgs(LongFrameBase frame)
            : this(new[] { frame })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FramesEventArgs"/> class
        /// with all frames created by the given <paramref name="provider"/>.
        /// This constructor also adds a setup frame before and after the created frames
        /// from the <see cref="provider"/>.
        /// </summary>
        /// <param name="provider">
        /// The provider from which the setup and output commands are taken.
        /// </param>
        public FramesEventArgs(IFrameProvider provider)
            : this(EnumerateFrames(provider))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FramesEventArgs"/> class
        /// from an enumeration of frames.
        /// </summary>
        /// <param name="frames">
        /// The frames to be returned by <see cref="GetFrames"/>.
        /// </param>
        public FramesEventArgs(IEnumerable<LongFrameBase> frames)
        {
            this.frames = frames;
        }

        /// <summary>
        /// Gets the frames.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/> of frames.
        /// </returns>
        public IEnumerable<LongFrameBase> GetFrames()
        {
            return this.frames;
        }

        private static IEnumerable<LongFrameBase> EnumerateFrames(IFrameProvider provider)
        {
            yield return new StatusRequestFrame();

            yield return provider.SetupCommand;

            foreach (var command in provider.GetOutputCommands())
            {
                yield return command;
            }

            yield return new StatusRequestFrame();
        }
    }
}