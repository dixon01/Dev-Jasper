// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Handlers
{
    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Protocols.Ahdlc;

    /// <summary>
    /// Factory creating <see cref="IFrameHandler"/> implementations for a channel.
    /// </summary>
    public class FrameHandlerFactory
    {
        /// <summary>
        /// Creates a frame handler for the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IFrameHandler"/>.
        /// </returns>
        public virtual IFrameHandler CreateFrameHandler(ChannelConfig config)
        {
            return new SerialPortFrameHandler(config.SerialPort);
        }
    }
}
