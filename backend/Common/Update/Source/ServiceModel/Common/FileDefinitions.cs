// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDefinitions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileDefinitions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    /// <summary>
    /// File name and extension definitions.
    /// </summary>
    public static class FileDefinitions
    {
        /// <summary>
        /// The file extension for resource files including the leading dot.
        /// </summary>
        public static readonly string ResourceFileExtension = ".rx";

        /// <summary>
        /// The file extension for update commands including the leading dot.
        /// </summary>
        public static readonly string UpdateCommandExtension = ".guc";

        /// <summary>
        /// The file extension for update state information (feedback) including the leading dot.
        /// </summary>
        public static readonly string UpdateStateInfoExtension = ".guf";

        /// <summary>
        /// The file extension for log files including the leading dot.
        /// </summary>
        public static readonly string LogFileExtension = ".log";

        /// <summary>
        /// The file extension for temporary files including the leading dot.
        /// </summary>
        public static readonly string TempFileExtension = ".tmp";

        /// <summary>
        /// The file extension for backup files including the leading dot.
        /// </summary>
        public static readonly string BackupFileExtension = ".bak";
    }
}
