// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using Gorba.Common.Medi.Core.Peers.Transport;

    /// <summary>
    /// Base interface for <see cref="MediMessage"/> and <see cref="StreamMessage"/>.
    /// </summary>
    internal interface IMessage
    {
        /// <summary>
        /// Gets the source address from where this message comes.
        /// </summary>
        MediAddress Source { get; }

        /// <summary>
        /// Gets the destination address to where this message goes.
        /// </summary>
        MediAddress Destination { get; }
    }
}