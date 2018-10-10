// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileReceivedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileReceivedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System;

    /// <summary>
    /// Base class for an event argument used by
    /// <see cref="IResourceService.FileReceived"/>.
    /// </summary>
    public abstract class FileReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name.
        /// </param>
        internal FileReceivedEventArgs(MediAddress source, string originalFileName)
        {
            this.Source = source;
            this.OriginalFileName = originalFileName;
        }

        /// <summary>
        /// Gets the address of the node from which the file was received.
        /// </summary>
        public MediAddress Source { get; private set; }

        /// <summary>
        /// Gets the original file name including its path as it was on
        /// the <see cref="Source"/> node.
        /// </summary>
        public string OriginalFileName { get; private set; }

        /// <summary>
        /// Copies the received (temporary) file to the given location.
        /// </summary>
        /// <param name="localFileName">
        /// The local file name to copy the file to.
        /// </param>
        public abstract void CopyTo(string localFileName);
    }
}