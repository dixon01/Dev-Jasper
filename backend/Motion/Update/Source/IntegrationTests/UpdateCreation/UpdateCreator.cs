// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCreator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.IntegrationTests.UpdateCreation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Class to create an update package
    /// </summary>
    public class UpdateCreator
    {
        private readonly string backgroundSystemGuid;

        private readonly IWritableFileSystem fileSystem;

        private string commandName;

        private string tempResourceDirectoryPath;

        private FolderUpdate rootfolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCreator"/> class.
        /// </summary>
        public UpdateCreator()
        {
            this.backgroundSystemGuid = Guid.NewGuid().ToString();
            this.fileSystem = (IWritableFileSystem)FileSystemManager.Local;
        }

        /// <summary>
        /// Creates the update package
        /// </summary>
        /// <param name="rootDirectoryPath">
        /// The root directory Path.
        /// </param>
        /// <param name="updateSetPath">
        /// The update Set Path.
        /// </param>
        public void CreateUpdatepackage(
            string rootDirectoryPath, string updateSetPath)
        {
            this.rootfolder = new FolderUpdate();

            this.tempResourceDirectoryPath = Path.Combine(updateSetPath, "Resources");
            Directory.CreateDirectory(this.tempResourceDirectoryPath);

            this.CreateUpdateTree(rootDirectoryPath, this.rootfolder);

            Directory.CreateDirectory(Path.Combine(updateSetPath, "Feedback"));
            this.CreateRepositoryConfig(updateSetPath);
        }

        /// <summary>
        /// Creates an update command for all the units.
        /// </summary>
        /// <param name="computerName">
        /// The name of the unit.
        /// </param>
        /// <param name="validFrom">
        /// The time the update command is valid from.
        /// </param>
        /// <param name="updateSetPath">
        /// The path for the update set
        /// </param>
        /// <param name="updateIndex">
        /// The update index for the update command.
        /// </param>
        /// <param name="installAfterBoot">
        /// The installAfterBoot flag indicates the update command be installed after boot if set to true.
        /// </param>
        public void CreateUpdateCommand(
            string computerName, DateTime validFrom, string updateSetPath, int updateIndex, bool installAfterBoot)
        {
            var commandsDir = Path.Combine(updateSetPath, "Commands");
            Directory.CreateDirectory(commandsDir);

            var units = new List<string>();
            units.Add(computerName);

            foreach (var unit in units)
            {
                var command = this.CreateUpdateCommand(
                    unit, validFrom.ToUniversalTime(), this.rootfolder, updateIndex, installAfterBoot);
                var commandForUnitpath = Path.Combine(commandsDir, unit);
                this.commandName = string.Format(
                    "{0}-{1:####0000}{2}",
                    this.backgroundSystemGuid,
                    updateIndex,
                    FileDefinitions.UpdateCommandExtension);
                this.CreateUpdateCommandFile(commandForUnitpath, this.commandName, command);
            }
        }

        /// <summary>
        /// Verifies the time stamp validity of the set time versus the actual timestamp from the installed
        /// feedback file
        /// </summary>
        /// <param name="setValidfrom">
        /// timestamp set in the command.
        /// </param>
        /// <param name="feedbackpath">
        /// The path of the feedback file.
        /// </param>
        /// <returns>
        /// returns true if the timestamp of the feedback is greater than or equal to the timestamp
        /// on the command file <see cref="bool"/>.
        /// </returns>
        public bool VerifyTimeValidity(DateTime setValidfrom, string feedbackpath)
        {
            var files = Directory.GetFiles(feedbackpath, "*Installed.guf", SearchOption.AllDirectories);
            return
                files.Select(file => new Configurator(file))
                     .Select(configurator => configurator.Deserialize<UpdateStateInfo>())
                     .Any(updateStateInfo => updateStateInfo.TimeStamp >= setValidfrom.ToUniversalTime());
        }

        /// <summary>
        /// Creates the update tree based on the folders and files in the given path
        ///  </summary>
        /// <param name="path">
        /// the path to get the folders or files from
        /// </param>
        /// <param name="folder">
        /// The root folder.
        /// </param>
        private void CreateUpdateTree(string path, FolderUpdate folder)
        {
            var directory = new DirectoryInfo(path);
            foreach (var subDir in directory.GetDirectories())
            {
                var subFolder = new FolderUpdate { Name = subDir.Name };
                folder.Items.Add(subFolder);
                this.CreateUpdateTree(subDir.FullName, subFolder);
            }

            foreach (var file in directory.GetFiles())
            {
                var hash = ResourceHash.Create(file.FullName);
                var item = new FileUpdate { Name = file.Name, Hash = hash };
                folder.Items.Add(item);

                this.CopyResource(file.FullName, hash);
            }
        }

        /// <summary>
        /// Creates the update command for unit
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="validFrom">
        /// The date and time from which the update command becomes valid
        /// </param>
        /// <param name="root">
        /// The root folder of the update folders and files.
        /// </param>
        /// <param name="updateIndex">
        /// the update index
        /// </param>
        /// <param name="installAfterBoot">
        /// The installAfterBoot flag indicates the update command be installed after boot if set to true.
        /// </param>
        /// <returns>
        /// The <see cref="UpdateCommand"/>.
        /// </returns>
        private UpdateCommand CreateUpdateCommand(
            string unitName, DateTime validFrom, FolderUpdate root, int updateIndex, bool installAfterBoot)
        {
            var unitId = new UnitId(unitName);
            var updateId = new UpdateId(this.backgroundSystemGuid, updateIndex);
            var command = new UpdateCommand
            {
                UpdateId = updateId,
                UnitId = unitId,
                ActivateTime = validFrom,
                InstallAfterBoot = installAfterBoot,
                PreInstallation = new RunCommands(),
                PostInstallation = new RunCommands()
            };

            foreach (var item in root.Items)
            {
                var folder = item as FolderUpdate;
                if (folder != null)
                {
                    command.Folders.Add(folder);
                }
            }

            return command;
        }

        /// <summary>
        /// Creates an update command file based on the update command and places it into the correct location
        /// </summary>
        /// <param name="directory">
        /// Directory to place the update command file
        /// </param>
        /// <param name="fileName">
        /// The name of the update command file
        /// </param>
        /// <param name="command">
        /// The update command.
        /// </param>
        private void CreateUpdateCommandFile(string directory, string fileName, UpdateCommand command)
        {
            Directory.CreateDirectory(directory);
            var commandPath = Path.Combine(directory, fileName);

            var tempCommandPath = Path.ChangeExtension(commandPath, FileDefinitions.TempFileExtension);
            var configurator = new Configurator(tempCommandPath);
            configurator.Serialize(command);
            this.MoveFile(tempCommandPath, commandPath);
        }

        /// <summary>
        /// Creates the repository config file
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        private void CreateRepositoryConfig(string path)
        {
            var respositoryPath = Path.Combine(path, RepositoryConfig.RepositoryXmlFileName);
            var repoConfig = new RepositoryConfig
                       {
                           Versions =
                               {
                                   new RepositoryVersionConfig
                                       {
                                           ValidFrom = new Version(1, 0),
                                           Compression = CompressionAlgorithm.None,
                                           ResourceDirectory = "Resources",
                                           CommandsDirectory = "Commands",
                                           FeedbackDirectory = "Feedback"
                                       }
                               }
                       };
            var configurator = new Configurator(respositoryPath, RepositoryConfig.Schema);
            configurator.Serialize(repoConfig);
        }

        private void MoveFile(string sourcePath, string destinationPath)
        {
            var file = this.fileSystem.GetFile(sourcePath).MoveTo(destinationPath);
            file.Attributes = FileAttributes.Normal;
        }

        private void CopyResource(string sourcePath, string hash)
        {
            var resourceName = string.Format("{0}{1}", hash, FileDefinitions.ResourceFileExtension);
            var resourcePath = Path.Combine(this.tempResourceDirectoryPath, resourceName);

            if (File.Exists(resourcePath))
            {
                var calculatedHash = ResourceHash.Create(resourcePath);
                if (calculatedHash.Equals(hash, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                File.Delete(resourcePath);
            }

            var file = this.fileSystem.GetFile(sourcePath).CopyTo(resourcePath);
            file.Attributes = FileAttributes.Normal;
        }
    }
}
