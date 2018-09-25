// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFrameProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFrameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Interface for setup and output command frame providers.
    /// </summary>
    public interface IFrameProvider
    {
        /// <summary>
        /// Gets the setup command that has to be sent first.
        /// </summary>
        SetupCommandFrame SetupCommand { get; }

        /// <summary>
        /// Gets the output commands to be sent after the setup command.
        /// </summary>
        /// <returns>
        /// An enumeration over all output commands in the order they need to be sent.
        /// </returns>
        IEnumerable<OutputCommandFrame> GetOutputCommands();
    }
}