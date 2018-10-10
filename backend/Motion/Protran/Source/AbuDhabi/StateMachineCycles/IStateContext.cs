// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interface for properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.StateMachineCycles
{
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.AbuDhabi.Ibis;

    /// <summary>
    /// Interface for properties.
    /// </summary>
    internal interface IStateContext
    {
        /// <summary>
        /// Gets Dictionary.
        /// </summary>
        Dictionary Dictionary { get; }

        /// <summary>
        /// Gets protocol.
        /// </summary>
        IbisProtocolHost CurrentProtocol { get; }
    }
}