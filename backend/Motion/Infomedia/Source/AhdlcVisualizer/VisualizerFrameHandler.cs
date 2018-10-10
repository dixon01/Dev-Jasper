// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerFrameHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerFrameHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcVisualizer
{
    using System;

    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Implementation of <see cref="IFrameHandler"/> that wraps another frame handler and
    /// raises events whenever a new frame is created locally or a new frame is received.
    /// </summary>
    public class VisualizerFrameHandler : IFrameHandler
    {
        private readonly IFrameHandler frameHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerFrameHandler"/> class.
        /// </summary>
        /// <param name="frameHandler">
        /// The frame handler that is doing the actual frame reception and sending.
        /// </param>
        public VisualizerFrameHandler(IFrameHandler frameHandler)
        {
            this.frameHandler = frameHandler;
        }

        /// <summary>
        /// Event that is risen whenever a new frame is created.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameCreated;

        /// <summary>
        /// Event that is risen whenever a new frame is received.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameReceived;

        /// <summary>
        /// Reads the next frame from the underlying stream.
        /// This method blocks until an entire frame is available or the
        /// end of the stream was reached (EOS).
        /// </summary>
        /// <returns>
        /// The decoded <see cref="FrameBase"/> or null if the end of the stream was reached (EOS).
        /// </returns>
        public FrameBase ReadNextFrame()
        {
            var frame = this.frameHandler.ReadNextFrame();
            if (frame != null)
            {
                this.RaiseFrameReceived(new FrameEventArgs(frame));
            }

            return frame;
        }

        /// <summary>
        /// Writes an entire frame to the underlying stream.
        /// </summary>
        /// <param name="frame">
        /// The frame to write.
        /// </param>
        public void WriteFrame(FrameBase frame)
        {
            this.frameHandler.WriteFrame(frame);
            this.RaiseFrameCreated(new FrameEventArgs(frame));
        }

        /// <summary>
        /// Raises the <see cref="FrameCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseFrameCreated(FrameEventArgs e)
        {
            var handler = this.FrameCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="FrameReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseFrameReceived(FrameEventArgs e)
        {
            var handler = this.FrameReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}