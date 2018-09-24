// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerFrameHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerFrameHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcVisualizer
{
    using System;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Protocols.Ahdlc;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Handlers;

    /// <summary>
    /// Special implementation of <see cref="FrameHandlerFactory"/> that raises events
    /// when frames are created or received.
    /// </summary>
    public class VisualizerFrameHandlerFactory : FrameHandlerFactory
    {
        /// <summary>
        /// Event that is risen whenever a new frame is created.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameCreated;

        /// <summary>
        /// Event that is risen whenever a new frame is received.
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameReceived;

        /// <summary>
        /// Creates a frame handler for the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IFrameHandler"/>.
        /// </returns>
        public override IFrameHandler CreateFrameHandler(ChannelConfig config)
        {
            var handler = new VisualizerFrameHandler(base.CreateFrameHandler(config));
            handler.FrameCreated += (s, e) => this.RaiseFrameCreated(e);
            handler.FrameReceived += (s, e) => this.RaiseFrameReceived(e);
            return handler;
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