// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TreeHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Helper class for finding folders in <see cref="UpdateSet"/> and <see cref="UpdateCommand"/>.
    /// </summary>
    internal static class TreeHelper
    {
        private delegate IEnumerable<TItem> ItemsGetter<TFolder, TItem>(TFolder folder);

        private delegate string NameGetter<TItem>(TItem item);

        /// <summary>
        /// Searches for the folder with the given path in the given update set.
        /// </summary>
        /// <param name="updateSet">
        /// The update set.
        /// </param>
        /// <param name="path">
        /// The path to search, delimited by '\'.
        /// </param>
        /// <returns>
        /// The <see cref="UpdateFolder"/> or null if not found.
        /// </returns>
        public static UpdateFolder FindFolder(UpdateSet updateSet, string path)
        {
            return FindFolder<UpdateSubNode, UpdateFolder>(
                updateSet.Folders.ConvertAll(f => (UpdateSubNode)f), path, f => f.Name, f => f.Items);
        }

        /// <summary>
        /// Searches for the folder with the given path in the given update command.
        /// </summary>
        /// <param name="command">
        /// The update command.
        /// </param>
        /// <param name="path">
        /// The path to search, delimited by '\'.
        /// </param>
        /// <returns>
        /// The <see cref="FolderUpdate"/> or null if not found.
        /// </returns>
        public static FolderUpdate FindFolder(UpdateCommand command, string path)
        {
            return FindFolder<FileSystemUpdate, FolderUpdate>(
                command.Folders.ConvertAll(f => (FileSystemUpdate)f), path, f => f.Name, f => f.Items);
        }

        private static TFolder FindFolder<TItem, TFolder>(
            IEnumerable<TItem> items, string path, NameGetter<TFolder> getName, ItemsGetter<TFolder, TItem> getItems)
            where TFolder : class, TItem
        {
            var parts = path.Split(Path.DirectorySeparatorChar);
            var index = 0;
            if (parts.Length > 0 && string.IsNullOrEmpty(parts[0]))
            {
                index++;
            }

            var endIndex = parts.Length - 1;
            if (endIndex < index)
            {
                return null;
            }

            if (string.IsNullOrEmpty(parts[endIndex]))
            {
                endIndex--;
            }

            TFolder folder = null;
            while (index <= endIndex)
            {
                var folderName = parts[index++];
                folder = FindFolder(items, folderName, getName);
                if (folder == null)
                {
                    return null;
                }

                items = getItems(folder);
            }

            return folder;
        }

        private static TFolder FindFolder<TItem, TFolder>(
            IEnumerable<TItem> items, string folderName, NameGetter<TFolder> getName)
            where TFolder : class, TItem
        {
            foreach (var item in items)
            {
                var folder = item as TFolder;
                if (folder != null && getName(folder).Equals(folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return folder;
                }
            }

            return null;
        }
    }
}
