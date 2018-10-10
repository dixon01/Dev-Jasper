// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelType.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using Gorba.Common.Medi.Core.Peers.Streams;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// Type of the channel (socket connection).
    /// </summary>
    internal enum ChannelType
    {
        /// <summary>
        /// Channel that sends and receives <see cref="MessageBuffer"/>s.
        /// </summary>
        Message,

        /// <summary>
        /// Channel that sends and receives <see cref="StreamMessageBuffer"/>s.
        /// </summary>
        Stream
    }
}