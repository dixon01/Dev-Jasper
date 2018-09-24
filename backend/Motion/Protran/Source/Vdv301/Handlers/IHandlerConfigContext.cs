// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHandlerConfigContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IHandlerConfigContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Configuration context for <see cref="ServiceHandlerBase"/>.
    /// </summary>
    public interface IHandlerConfigContext
    {
        /// <summary>
        /// Gets the VDV 301 protocol configuration.
        /// </summary>
        Vdv301ProtocolConfig Config { get; }

        /// <summary>
        /// Gets the generic dictionary.
        /// </summary>
        Dictionary Dictionary { get; }
    }
}