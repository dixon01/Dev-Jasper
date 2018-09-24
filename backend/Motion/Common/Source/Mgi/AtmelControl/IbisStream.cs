// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisStream.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    using System.Collections.Generic;

    /// <summary>
    /// The Atmel Controller IBIS stream.
    /// </summary>
    public class IbisStream : AtmelControlObject
    {
        /// <summary>
        /// Gets or sets the received IBIS telegram payloads (no CR, no checksum).
        /// </summary>
        public List<string> Data { get; set; }
    }
}