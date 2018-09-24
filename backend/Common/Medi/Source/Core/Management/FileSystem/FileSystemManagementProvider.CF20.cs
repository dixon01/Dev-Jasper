// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemManagementProvider.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.FileSystem
{
    /// <summary>
    /// Management provider for file system information.
    /// </summary>
    public partial class FileSystemManagementProvider
    {
        /// <summary>
        /// Creates the root management provider that returns all drives of this machine as children.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// the root management provider.
        /// </returns>
        public static IManagementProvider CreateRoot(IManagementProvider parent)
        {
            return new FileSystemManagementProvider("\\", RootName, parent);
        }
    }
}
