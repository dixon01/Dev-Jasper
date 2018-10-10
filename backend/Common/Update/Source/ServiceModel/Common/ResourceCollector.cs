// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceCollector.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Class that collects all resource hashes from an <see cref="UpdateCommand"/>.
    /// </summary>
    public class ResourceCollector
    {
        /// <summary>
        /// Gets a list of all resource hashes included in the given command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The list of resource hashes.
        /// </returns>
        public List<string> GetAllResourceHashes(UpdateCommand command)
        {
            var root = new FolderUpdate();

            foreach (var folder in command.Folders)
            {
                root.Items.Add(folder);
            }

            this.AddRunCommands(root, command.PreInstallation);
            this.AddRunCommands(root, command.PostInstallation);

            var hashes = new List<string>();
            this.CollectResources(root, hashes);
            return hashes;
        }

        private void AddRunCommands(FolderUpdate root, RunCommands commands)
        {
            if (commands == null)
            {
                return;
            }

            root.Items.AddRange(commands.Items);
        }

        private void CollectResources(FolderUpdate folder, ICollection<string> resourceIds)
        {
            foreach (var item in folder.Items)
            {
                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    this.CollectResources(subFolder, resourceIds);
                    continue;
                }

                var file = item as FileUpdate;
                if (file != null)
                {
                    var id = file.Hash;
                    if (!resourceIds.Contains(id))
                    {
                        resourceIds.Add(id);
                    }
                }
            }
        }
    }
}
