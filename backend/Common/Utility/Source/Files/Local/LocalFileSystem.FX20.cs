// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileSystem.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalFileSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Files.Local
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Compatibility;
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
            DriveInfo[] array = DriveInfo.GetDrives();
            Converter<DriveInfo, IDriveInfo> converter = i => (IDriveInfo)new LocalDriveInfo(i, this);
            return ArrayUtil.ConvertAll(array, converter);
        }

        IWritableDriveInfo[] IWritableFileSystem.GetDrives()
        {
            DriveInfo[] array = DriveInfo.GetDrives();
            Converter<DriveInfo, IWritableDriveInfo> converter = i => (IWritableDriveInfo)new LocalDriveInfo(i, this);
            return ArrayUtil.ConvertAll(array, converter);
        }
    }
}
