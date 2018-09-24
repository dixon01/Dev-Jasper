// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPortForwarding.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPortForwarding type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports
{
    using System;

    /// <summary>
    /// A single port forwarding. This class can be used to close an existing forwarding.
    /// </summary>
    public interface IPortForwarding : IDisposable
    {
        /// <summary>
        /// Closes this forwarding.
        /// </summary>
        void Close();
    }
}