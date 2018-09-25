// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;

    using NLog;

    /// <summary>
    /// Extension methods use by update classes.
    /// </summary>
    public static class UpdateExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Includes the given <paramref name="addStructure"/> in the <paramref name="existingStructure"/>.
        /// </summary>
        /// <param name="existingStructure">
        /// The existing structure to which the items are added.
        /// </param>
        /// <param name="addStructure">
        /// The structure to add to the existing structure.
        /// </param>
        /// <returns>
        /// The <paramref name="existingStructure"/>.
        /// </returns>
        public static UpdateFolderStructure Include(
            this UpdateFolderStructure existingStructure, UpdateFolderStructure addStructure)
        {
            foreach (var folder in addStructure.Folders)
            {
                var commandFolder =
                    existingStructure.Folders.FirstOrDefault(
                        f => f.Name.Equals(folder.Name, StringComparison.InvariantCultureIgnoreCase));
                if (commandFolder != null)
                {
                    FillFolder(commandFolder.Items, folder.Items, folder.Name);
                    continue;
                }

                existingStructure.Folders.Add((FolderUpdate)folder.Clone());
            }

            return existingStructure;
        }

        /// <summary>
        /// Includes the given <paramref name="addInstructions"/> in the <paramref name="existingInstructions"/>.
        /// </summary>
        /// <param name="existingInstructions">
        /// The existing instructions to which the items are added.
        /// </param>
        /// <param name="addInstructions">
        /// The structure to add to the existing instructions.
        /// </param>
        /// <returns>
        /// The <paramref name="existingInstructions"/>.
        /// </returns>
        public static InstallationInstructions Include(
            this InstallationInstructions existingInstructions,
            InstallationInstructions addInstructions)
        {
            if (!existingInstructions.InstallAfterBoot.HasValue)
            {
                existingInstructions.InstallAfterBoot = addInstructions.InstallAfterBoot;
            }
            else if (existingInstructions.InstallAfterBoot.Value && addInstructions.InstallAfterBoot.HasValue)
            {
                // if existing has flag set to true, overwrite it with the added flag (which might be false)
                existingInstructions.InstallAfterBoot = addInstructions.InstallAfterBoot;
            }

            existingInstructions.PreInstallation = MergeRunCommands(
                existingInstructions.PreInstallation,
                addInstructions.PreInstallation);
            existingInstructions.PostInstallation = MergeRunCommands(
                existingInstructions.PostInstallation,
                addInstructions.PostInstallation);

            return existingInstructions;
        }

        private static RunCommands MergeRunCommands(RunCommands existing, RunCommands add)
        {
            if (add == null)
            {
                return existing;
            }

            if (existing == null)
            {
                existing = new RunCommands();
            }

            FillFolder(existing.Items, add.Items, "RunCommands");
            return existing;
        }

        private static void FillFolder(
            ICollection<FileSystemUpdate> parent, IEnumerable<FileSystemUpdate> adding, string addName)
        {
            foreach (var add in adding)
            {
                var existing =
                    parent.FirstOrDefault(
                        i => i.Name.Equals(add.Name, StringComparison.InvariantCultureIgnoreCase));
                if (existing == null)
                {
                    parent.Add((FileSystemUpdate)add.Clone());
                    continue;
                }

                if (existing.GetType() != add.GetType())
                {
                    Logger.Warn(
                        "Couldn't add {0} in {1} because it is a {2} instead of {3}",
                        add.Name,
                        addName,
                        add.GetType().Name,
                        existing.GetType().Name);
                    continue;
                }

                var existingFolder = existing as FolderUpdate;
                if (existingFolder == null)
                {
                    Logger.Warn("Couldn't add {0} because it already exists in {1}", add.Name, addName);
                    continue;
                }

                FillFolder(existingFolder.Items, ((FolderUpdate)add).Items, add.Name);
            }
        }
    }
}