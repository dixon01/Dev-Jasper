// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferedStream.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BufferedStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System.IO;

    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Dummy class to provide a buffered stream class (that doesn't do buffering).
    /// </summary>
    internal partial class BufferedStream : WrapperStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedStream"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public BufferedStream(Stream stream)
        {
            this.Open(stream);
        }
    }
}