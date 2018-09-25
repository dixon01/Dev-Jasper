// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Base class for <see cref="IFrameProvider"/> implementations.
    /// </summary>
    public abstract class FrameProviderBase : IFrameProvider
    {
        /// <summary>
        /// Gets or sets the setup command that has to be sent first.
        /// </summary>
        public SetupCommandFrame SetupCommand { get; protected set; }

        /// <summary>
        /// Gets the output commands to be sent after the setup command.
        /// </summary>
        /// <returns>
        /// An enumeration over all output commands in the order they need to be sent.
        /// </returns>
        public abstract IEnumerable<OutputCommandFrame> GetOutputCommands();
    }
}