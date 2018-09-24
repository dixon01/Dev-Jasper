// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRecorder.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Recording
{
    using System;
    using System.IO;

    /// <summary>
    /// Interface for all Recorders used in IBIS.
    /// </summary>
    public interface IRecorder
    {
        /// <summary>
        /// Gets a value indicating whether
        /// this recorder is recording or not.
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        /// Starts the recorder.
        /// <exception cref="UnauthorizedAccessException">If the software doesn't have the grants
        /// to create a new file.</exception>
        /// <exception cref="ArgumentException">If the settings used to create the file are invalid.</exception>
        /// <exception cref="IOException">If some error occurs on creating and opening the file.</exception>
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the recorder.
        /// </summary>
        void Stop();

        /// <summary>
        /// Writea a buffer on the file.
        /// </summary>
        /// <param name="buffer">The buffer that has to be wrote.</param>
        void Write(byte[] buffer);
    }
}