// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSendContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDataSendContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.Drawing;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Interface used by all controls that need to send data to a sign.
    /// </summary>
    public interface IDataSendContext
    {
        /// <summary>
        /// Event that is fired whenever <see cref="HasColor"/> or <see cref="SignSize"/> updated.
        /// </summary>
        event EventHandler Updated;

        /// <summary>
        /// Gets a value indicating whether the sign has color.
        /// </summary>
        bool HasColor { get; }

        /// <summary>
        /// Gets the configured sign size.
        /// </summary>
        Size SignSize { get; }

        /// <summary>
        /// Sends a frame to the sign.
        /// </summary>
        /// <param name="frame">
        /// The frame; its <see cref="FrameBase.Address"/> will be set automatically by this method.
        /// </param>
        void SendFrame(LongFrameBase frame);
    }
}
