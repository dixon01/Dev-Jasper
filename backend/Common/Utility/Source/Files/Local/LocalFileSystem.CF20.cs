// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileSystem.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// The local file system.
    /// Partial class for .NET Framework 2.0
    /// </summary>
    internal partial class LocalFileSystem
    {
        /// <summary>
        /// Gets all known drives in this file system.
        /// </summary>
        /// <returns>
        /// The all known drives.
        /// </returns>
        public IDriveInfo[] GetDrives()
        {
            return new IDriveInfo[] { new LocalDriveInfo(this) };
        }

        IWritableDriveInfo[] IWritableFileSystem.GetDrives()
        {
            return new IWritableDriveInfo[] { new LocalDriveInfo(this) };
        }
    }
}
