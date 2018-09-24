// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcVisualizer
{
    using System;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Event arguments with a single <see cref="FrameBase"/>.
    /// </summary>
    public class FrameEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameEventArgs"/> class.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        public FrameEventArgs(FrameBase frame)
        {
            this.Frame = frame;
        }

        /// <summary>
        /// Gets the frame associated to this event.
        /// </summary>
        public FrameBase Frame { get; private set; }
    }
}