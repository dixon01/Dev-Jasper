// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFileInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update.Azure
{
    using System;

    /// <summary>
    /// Information about a log file to be downloaded from Azure Blob Storage.
    /// </summary>
    public class LogFileInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogFileInfo"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="uri">
        /// The uri of the file.
        /// </param>
        public LogFileInfo(string unitName, Uri uri)
        {
            this.UnitName = unitName;
            this.Uri = uri;
        }

        /// <summary>
        /// Gets the unit name.
        /// </summary>
        public string UnitName { get; private set; }

        /// <summary>
        /// Gets the entire URI of the log file.
        /// </summary>
        public Uri Uri { get; private set; }
    }
}