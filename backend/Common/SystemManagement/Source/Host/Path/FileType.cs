// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host.Path
{
    /// <summary>
    /// The type of file.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// An application file, in most cases either an EXE or a DLL.
        /// These files are never user-editable.
        /// </summary>
        Application,

        /// <summary>
        /// A config file.
        /// These files can be changed by the user,
        /// but should only be read by the application.
        /// </summary>
        Config,

        /// <summary>
        /// A log file.
        /// These files should only be written by the application,
        /// but can be read and deleted by the user.
        /// </summary>
        Log,

        /// <summary>
        /// A data file.
        /// These files should only be read and written by the application,
        /// they have no significance to the user.
        /// </summary>
        Data,

        /// <summary>
        /// A file used for presentation (Infomedia).
        /// These files can be changed by the user,
        /// but should only be read by the application.
        /// </summary>
        Presentation,

        /// <summary>
        /// A file used for persistent database information (Bus.exe).
        /// These files can be changed by the user,
        /// but should (for now) only be read by the application.
        /// </summary>
        Database
    }
}