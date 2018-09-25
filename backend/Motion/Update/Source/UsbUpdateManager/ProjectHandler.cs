// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Update.UsbUpdateManager.Data;

    /// <summary>
    /// Class handling a single project (file).
    /// </summary>
    internal class ProjectHandler
    {
        private readonly ConfigManager<UpdateProject> configManager;

        private ProjectHandler(ConfigManager<UpdateProject> configManager)
        {
            this.configManager = configManager;
            this.Config = configManager.Config;
            this.ReadOnly = !this.CanWrite();
            this.ConfigureResourceService();
        }

        /// <summary>
        /// Gets the update project configuration of this project.
        /// </summary>
        public UpdateProject Config { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this project is read-only.
        /// </summary>
        public bool ReadOnly { get; private set; }

        /// <summary>
        /// Gets the resource service.
        /// </summary>
        public IResourceService ResourceService { get; private set; }

        private string DataDirectory
        {
            get
            {
                return Path.ChangeExtension(this.configManager.FullConfigFileName, string.Empty);
            }
        }

        /// <summary>
        /// Creates a new project at the given location.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="ProjectHandler"/> for the newly created project.
        /// </returns>
        public static ProjectHandler Create(string filename)
        {
            var configMgr = new ConfigManager<UpdateProject> { FileName = filename };
            configMgr.CreateConfig();

            configMgr.Config.Guid = Guid.NewGuid().ToString();

            configMgr.SaveConfig();
            return new ProjectHandler(configMgr);
        }

        /// <summary>
        /// Loads a project from the given location.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="ProjectHandler"/> for the loaded project.
        /// </returns>
        public static ProjectHandler Load(string filename)
        {
            return new ProjectHandler(new ConfigManager<UpdateProject> { FileName = filename });
        }

        /// <summary>
        /// Saves this project.
        /// </summary>
        public void Save()
        {
            var backupFile = this.configManager.FullConfigFileName + ".bak";
            File.Copy(this.configManager.FullConfigFileName, backupFile, true);
            try
            {
                this.configManager.SaveConfig();
            }
            catch
            {
                File.Copy(backupFile, this.configManager.FullConfigFileName, true);
                throw;
            }
        }

        /// <summary>
        /// Saves a stripped-down copy of the current project to the given file.
        /// This will only retain all unit groups and units with their
        /// current directory structures without updates, feedback and log files.
        /// </summary>
        /// <param name="filename">
        /// The filename of the project file to create.
        /// </param>
        public void SaveAs(string filename)
        {
            var configMgr = new ConfigManager<UpdateProject> { FileName = filename };
            configMgr.CreateConfig();
            var tempDir = Path.Combine(Path.ChangeExtension(configMgr.FullConfigFileName, string.Empty), "Temp");
            Directory.CreateDirectory(tempDir);

            foreach (var unitGroup in this.Config.UnitGroups)
            {
                this.ExportAllResources(unitGroup.CurrentDirectoryStructure, tempDir);
            }

            try
            {
                var handler = new ProjectHandler(configMgr);
                var config = handler.Config;
                config.Guid = Guid.NewGuid().ToString();
                config.FtpServers = new List<FtpUpdateProviderConfig>(this.Config.FtpServers);

                foreach (var unitGroup in this.Config.UnitGroups)
                {
                    config.UnitGroups.Add(this.CopyUnitGroup(unitGroup));
                }

                foreach (var resource in Directory.GetFiles(tempDir, "*" + FileDefinitions.ResourceFileExtension))
                {
                    handler.ResourceService.RegisterResource(resource, true);
                }

                Directory.Delete(tempDir, true);

                // we can't call handle.Save() because the file doesn't exist yet and the backup would be missing
                handler.configManager.SaveConfig();
            }
            finally
            {
                // we need to revert back to our original resource service
                this.ConfigureResourceService();
            }
        }

        /// <summary>
        /// Creates an export preview from the current state of this project.
        /// </summary>
        /// <returns>
        /// The new <see cref="UpdateExportPreview"/>. You can add pre- and post-
        /// installation commands as well as resources to it.
        /// </returns>
        public UpdateExportPreview CreateExportPreview()
        {
            var preview = new UpdateExportPreview();
            foreach (var unitGroup in this.Config.UnitGroups)
            {
                var root = new FolderUpdate();
                this.CreateUpdateTree(unitGroup.CurrentDirectoryStructure, root, preview.Resources);
                preview.UnitGroups.Add(new UnitGroupExportPreview(unitGroup, root));
            }

            return preview;
        }

        /// <summary>
        /// Creates an export of the given <see cref="UpdateExportPreview"/>.
        /// The export contains all resources and commands needed to update
        /// all units in the project to match the current project structure.
        /// Pre- and post-installation commands are added from the preview as well.
        /// </summary>
        /// <param name="preview">
        /// The preview created with <see cref="IProjectManager.CreateExportPreview"/>.
        /// </param>
        /// <param name="name">
        /// The name of the update.
        /// </param>
        /// <param name="validFromDateTime">
        /// The date and time in UTC from which the update becomes valid
        /// </param>
        /// <param name="installAfterBoot"> The installAfterBoot flag indicate to install
        ///  the Update command after application restart if true </param>
        /// <returns>
        /// The <see cref="UpdateExport"/>.
        /// </returns>
        public UpdateExport CreateExport(
            UpdateExportPreview preview, string name, DateTime validFromDateTime, bool installAfterBoot)
        {
            var updateIndex = ++this.Config.LastUpdateIndex;

            var updateId = new UpdateId(this.Config.Guid, updateIndex);

            var now = TimeProvider.Current.UtcNow;

            var export = new UpdateExport();
            export.Resources.AddRange(preview.Resources);

            foreach (var unitGroup in preview.UnitGroups)
            {
                foreach (var unit in unitGroup.UnitGroup.Units)
                {
                    var unitId = new UnitId(unit.Name);
                    var command = new UpdateCommand
                    {
                        UpdateId = updateId,
                        UnitId = unitId,
                        ActivateTime = validFromDateTime,
                        InstallAfterBoot = installAfterBoot,
                        PreInstallation = unitGroup.PreInstallation,
                        PostInstallation = unitGroup.PostInstallation
                    };

                    foreach (var item in unitGroup.UpdateRoot.Items)
                    {
                        var folder = item as FolderUpdate;
                        if (folder != null)
                        {
                            command.Folders.Add(folder);
                        }
                    }

                    unit.Updates.Add(
                        new UpdateInfo
                        {
                            Name = name,
                            Command = command,
                            States =
                                    {
                                        new UpdateStateInfo
                                            {
                                                UnitId = unitId,
                                                TimeStamp = now,
                                                State = UpdateState.Created
                                            }
                                    }
                        });

                    export.Commands.Add(command);
                }
            }

            return export;
        }

        /// <summary>
        /// Imports a log file received from an <see cref="IUpdateProvider"/> to the current project.
        /// </summary>
        /// <param name="logFile">
        /// The log file.
        /// </param>
        public void ImportLogFile(IReceivedLogFile logFile)
        {
            var dir = this.GetFeedbackDirectory(logFile.UnitName);
            Directory.CreateDirectory(dir);
            var fileName = logFile.FileName;
            string filePath;
            var index = 0;
            while (File.Exists(filePath = Path.Combine(dir, fileName)))
            {
                fileName = Path.ChangeExtension(
                    logFile.FileName,
                    string.Format(".{0}{1}", ++index, FileDefinitions.LogFileExtension));
            }

            logFile.CopyTo(filePath);
        }

        /// <summary>
        /// Gets all stored log files for a given unit.
        /// </summary>
        /// <param name="unit">
        /// The unit for which to get the log files.
        /// </param>
        /// <returns>
        /// The list of full paths to the log files.
        /// </returns>
        public string[] GetLogFilesFor(Unit unit)
        {
            var dir = this.GetFeedbackDirectory(unit.Name);
            if (!Directory.Exists(dir))
            {
                return new string[0];
            }

            return Directory.GetFiles(dir, "*" + FileDefinitions.LogFileExtension);
        }

        private void ConfigureResourceService()
        {
            var resourceDirectory = Path.Combine(this.DataDirectory, "Resources");
            if (!Directory.Exists(resourceDirectory))
            {
                Directory.CreateDirectory(resourceDirectory);
            }

            var config = new MediConfig
            {
                Services =
                                     {
                                         new LocalResourceServiceConfig
                                             {
                                                 ResourceDirectory = resourceDirectory
                                             }
                                     }
            };
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(config));
            this.ResourceService = MessageDispatcher.Instance.GetService<IResourceService>();
        }

        private void ExportAllResources(DirectoryNode dir, string destination)
        {
            foreach (var subDir in dir.Directories)
            {
                this.ExportAllResources(subDir, destination);
            }

            foreach (var file in dir.Files)
            {
                var tempFile = Path.Combine(destination, file.ResourceId.Hash + FileDefinitions.ResourceFileExtension);
                if (!File.Exists(tempFile))
                {
                    this.ResourceService.ExportResource(this.ResourceService.GetResource(file.ResourceId), tempFile);
                }
            }
        }

        private UnitGroup CopyUnitGroup(UnitGroup unitGroup)
        {
            var newGroup = new UnitGroup
            {
                Name = unitGroup.Name,
                Description = unitGroup.Description,
                CurrentDirectoryStructure = unitGroup.CurrentDirectoryStructure
            };
            foreach (var unit in unitGroup.Units)
            {
                newGroup.Units.Add(new Unit { Name = unit.Name, Description = unit.Description });
            }

            return newGroup;
        }

        private void CreateUpdateTree(DirectoryNode directoryNode, FolderUpdate folder, List<ResourceInfo> resources)
        {
            foreach (var subDir in directoryNode.Directories)
            {
                var subFolder = new FolderUpdate { Name = subDir.Name };
                folder.Items.Add(subFolder);
                this.CreateUpdateTree(subDir, subFolder, resources);
            }

            foreach (var file in directoryNode.Files)
            {
                if (resources.Find(r => r.Id.Equals(file.ResourceId)) == null)
                {
                    resources.Add(this.ResourceService.GetResource(file.ResourceId));
                }

                var item = new FileUpdate { Name = file.Name, Hash = file.ResourceId.Hash };
                folder.Items.Add(item);
            }
        }

        private string GetFeedbackDirectory(string unitName)
        {
            return Path.Combine(Path.Combine(this.DataDirectory, "Feedback"), unitName);
        }

        private bool CanWrite()
        {
            try
            {
                File.OpenWrite(this.configManager.FullConfigFileName).Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}