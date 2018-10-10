// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceivedMediLogFile.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReceivedMediLogFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi
{
    using System.IO;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Implementation of <see cref="IReceivedLogFile"/> that wraps a
    /// <see cref="FileReceivedEventArgs"/> from Medi.
    /// This log file needs to be handled synchronously, otherwise we loose
    /// the context of the <see cref="FileReceivedEventArgs"/>.
    /// </summary>
    internal class ReceivedMediLogFile : IReceivedLogFile
    {
        private readonly FileReceivedEventArgs eventArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMediLogFile"/> class.
        /// </summary>
        /// <param name="eventArgs">
        /// The <see cref="FileReceivedEventArgs"/> that has to be wrapped.
        /// </param>
        public ReceivedMediLogFile(FileReceivedEventArgs eventArgs)
        {
            this.eventArgs = eventArgs;
        }

        /// <summary>
        /// Gets the unit name from which this log file comes.
        /// </summary>
        public string UnitName
        {
            get
            {
                return this.eventArgs.Source.Unit;
            }
        }

        /// <summary>
        /// Gets the file name (without path) of the log file.
        /// </summary>
        public string FileName
        {
            get
            {
                return Path.GetFileName(this.eventArgs.OriginalFileName);
            }
        }

        /// <summary>
        /// Copies this log file to the given path.
        /// </summary>
        /// <param name="filePath">
        /// The full file path where to copy the log file to.
        /// </param>
        public void CopyTo(string filePath)
        {
            this.eventArgs.CopyTo(filePath);
        }

        /// <summary>
        /// Opens this log file for reading.
        /// </summary>
        /// <returns>
        /// A stream that allows reading the log file.
        /// </returns>
        public Stream OpenRead()
        {
            var tempFile = Path.GetTempFileName();
            this.CopyTo(tempFile);
            return new TemporaryFileStream(tempFile);
        }
    }
}