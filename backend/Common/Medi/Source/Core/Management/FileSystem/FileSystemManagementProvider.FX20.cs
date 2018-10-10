// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemManagementProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.FileSystem
{
    using System.IO;

    using Gorba.Common.Medi.Core.Management.Provider;

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
            var root = new ModifiableManagementProvider(RootName, parent);

            foreach (var drive in Directory.GetLogicalDrives())
            {
                root.AddChild(new FileSystemManagementProvider(drive, drive, root));
            }

            return root;
        }
    }
}
