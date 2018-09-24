// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IXimpleSource.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IXimpleSource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Multiplexing
{
    using System;

    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Interface for classes that emit Ximple events.
    /// </summary>
    public interface IXimpleSource
    {
        /// <summary>
        /// Event that is fired whenever the this object creates
        /// a new ximple object.
        /// </summary>
        event EventHandler<XimpleEventArgs> XimpleCreated;
    }
}
