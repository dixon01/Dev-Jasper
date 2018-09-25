// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReceivedLogFile.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IReceivedLogFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System.IO;

    /// <summary>
    /// Interface to access a received log file.
    /// </summary>
    public interface IReceivedLogFile
    {
        /// <summary>
        /// Gets the unit name from which this log file comes.
        /// </summary>
        string UnitName { get; }

        /// <summary>
        /// Gets the file name (without path) of the log file.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Copies this log file to the given path.
        /// </summary>
        /// <param name="filePath">
        /// The full file path where to copy the log file to.
        /// </param>
        void CopyTo(string filePath);

        /// <summary>
        /// Opens this log file for reading.
        /// </summary>
        /// <returns>
        /// A stream that allows reading the log file.
        /// </returns>
        Stream OpenRead();
    }
}