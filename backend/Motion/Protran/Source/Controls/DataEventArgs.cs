// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Controls
{
    using System;

    /// <summary>
    /// Event arguments for data events containing a byte array.
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the binary data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
