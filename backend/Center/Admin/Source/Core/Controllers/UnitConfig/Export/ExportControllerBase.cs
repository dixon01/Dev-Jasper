// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The base class for export controllers.
    /// Export controllers are responsible for creating the folder structure required for the export.
    /// </summary>
    public abstract class ExportControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly ObservableCollection<string> softwarePackageIds = new ObservableCollection<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportControllerBase"/> class.
        /// </summary>
        protected ExportControllerBase()
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
            this.SoftwarePackageIds = new ReadOnlyObservableCollection<string>(this.softwarePackageIds);
        }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        public UnitConfiguratorController Parent { get; private set; }

        /// <summary>
        /// Gets the software package IDs provided by this controller.
        /// This collection can change (from within this controller) when options are chosen
        /// in the configurator.
        /// </summary>
        public ReadOnlyObservableCollection<string> SoftwarePackageIds { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this application controller is a renderer.
        /// </summary>
        public bool IsRenderer { get; protected set; }

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public virtual void Initialize(UnitConfiguratorController parentController)
        {
            this.Parent = parentController;
        }

        /// <summary>
        /// Asynchronously creates the part of the folder structure for which this controller is responsible.
        /// </summary>
        /// <param name="rootFolders">
        /// The root folders to fill with the structure.
        /// Implementers should only add files and folder, but never remove anything created by another controller.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait for the completion of this method.
        /// </returns>
        public abstract Task CreateExportStructureAsync(List<ExportFolder> rootFolders);

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public abstract IEnumerable<ApplicationConfigBase> CreateApplicationConfigs();

        /// <summary>
        /// Creates the restart dependencies required for Update.
        /// </summary>
        /// <returns>
        /// The list of dependencies. The list can be empty, but never null.
        /// </returns>
        public virtual IEnumerable<DependencyConfig> CreateRestartDependencies()
        {
            yield break;
        }

        /// <summary>
        /// Adds a software package ID to the <see cref="SoftwarePackageIds"/> list.
        /// This method must be called every time an export controller
        /// sees a change in the Unit Configurator that adds a new software component.
        /// </summary>
        /// <param name="packageId">
        /// The package ID. This should be an ID from <see cref="PackageIds"/>.
        /// </param>
        /// <returns>
        /// True if the ID was added, false if it was already in the list.
        /// </returns>
        protected bool AddSoftwarePackageId(string packageId)
        {
            if (this.softwarePackageIds.Contains(packageId))
            {
                return false;
            }

            this.softwarePackageIds.Add(packageId);
            return true;
        }

        /// <summary>
        /// Removes a software package ID from the <see cref="SoftwarePackageIds"/> list.
        /// This method must be called every time an export controller
        /// sees a change in the Unit Configurator that removes a software component.
        /// </summary>
        /// <param name="packageId">
        /// The package ID. This should be an ID from <see cref="PackageIds"/>.
        /// </param>
        /// <returns>
        /// True if the ID was removed, false if it was not in the list.
        /// </returns>
        protected bool RemoveSoftwarePackageId(string packageId)
        {
            return this.softwarePackageIds.Remove(packageId);
        }

        /// <summary>
        /// Gets the selected software version for the given package id.
        /// </summary>
        /// <param name="packageId">
        /// The package id (must match <see cref="PackageReadableModel.PackageId"/>).
        /// </param>
        /// <returns>
        /// The package version for the given package or null if no package version was found.
        /// </returns>
        protected PackageVersionReadableModel GetSoftwareVersion(string packageId)
        {
            PackageVersionReadableModel version;
            this.Parent.GetPart<SoftwareVersionsPartController>()
                .GetSelectedPackageVersions()
                .TryGetValue(packageId, out version);
            return version;
        }

        /// <summary>
        /// Loads the content of the given resource into a string.
        /// </summary>
        /// <param name="resourceName">
        /// The name of the resource.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> containing the data from the resource or null if not found.
        /// </returns>
        protected string LoadFileResource(string resourceName)
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), resourceName);
            return stream == null ? null : new StreamReader(stream, Encoding.UTF8).ReadToEnd();
        }

        /// <summary>
        /// Creates a default Medi client configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="MediConfig"/> with a BEC over TCP peer connecting locally.
        /// </returns>
        protected MediConfig CreateMediClientConfig()
        {
            var mediConfig = new MediConfig
                                 {
                                     InterceptLocalLogs = true,
                                     Peers = { this.CreateMediClientPeerConfig(IPAddress.Loopback.ToString()) },
                                 };
            return mediConfig;
        }

        /// <summary>
        /// Creates a Medi BEC/TCP client peer connecting to the given remote host.
        /// </summary>
        /// <param name="remoteHost">
        /// The remote host name (or IP address).
        /// </param>
        /// <returns>
        /// The <see cref="ClientPeerConfig"/>.
        /// </returns>
        protected ClientPeerConfig CreateMediClientPeerConfig(string remoteHost)
        {
            return new ClientPeerConfig
                       {
                           Codec =
                               new BecCodecConfig
                                   {
                                       Serializer = BecCodecConfig.SerializerType.Default
                                   },
                           Transport =
                               new TcpTransportClientConfig
                                   {
                                       RemoteHost = remoteHost,
                                       RemotePort = TcpTransportServerConfig.DefaultPort
                                   }
                       };
        }

        /// <summary>
        /// Serializes and adds an XML config structure to the given list of root folders.
        /// </summary>
        /// <param name="config">
        /// The config object to serialize.
        /// </param>
        /// <param name="filePath">
        /// The file path in the export structure where the file should be placed.
        /// </param>
        /// <param name="rootFolders">
        /// The root folders to which to add the file.
        /// </param>
        /// <param name="schema">
        /// The XML schema against which the file should be checked.
        /// </param>
        /// <returns>
        /// The <see cref="ExportXmlConfigFile"/> containing the XML data.
        /// </returns>
        protected ExportXmlConfigFile AddXmlConfigFile(
            object config, string filePath, List<ExportFolder> rootFolders, XmlSchema schema = null)
        {
            var memory = new MemoryStream();
            var serializer = new XmlSerializer(config.GetType());
            serializer.Serialize(new StreamWriter(memory, Encoding.UTF8), config);
            var xml = Encoding.UTF8.GetString(memory.ToArray());
            return this.AddFile(filePath, rootFolders, name => new ExportXmlConfigFile(name, xml) { Schema = schema });
        }

        /// <summary>
        /// Serializes and adds an XML config structure to the given list of root folders.
        /// The XML config is serialized using the <paramref name="softwareVersion"/> provided
        /// and verified against the XML schema from that same assembly.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the <paramref name="configObject"/> to be serialized.
        /// This type must exist in the provided <paramref name="softwareVersion"/>.
        /// </typeparam>
        /// <param name="configObject">
        /// The config object to serialize.
        /// </param>
        /// <param name="softwareVersion">
        /// The software version to use for the serialization of the <paramref name="configObject"/>
        /// and for getting the XML schema.
        /// </param>
        /// <param name="filePath">
        /// The file path in the export structure where the file should be placed.
        /// </param>
        /// <param name="rootFolders">
        /// The root folders to which to add the file.
        /// </param>
        /// <returns>
        /// The <see cref="ExportXmlConfigFile"/> containing the XML data.
        /// </returns>
        protected async Task<ExportXmlConfigFile> AddVersionedXmlConfigFileAsync<T>(
            T configObject,
            PackageVersionReadableModel softwareVersion,
            string filePath,
            List<ExportFolder> rootFolders)
        {
            using (
                var adjuster =
                    await
                    ConfigAdjuster<T>.CreateAsync(softwareVersion, this.Parent.DataController.ConnectionController))
            {
                XmlSchema schema = null;
                string xml = null;

                await Task.Run(
                    () =>
                        {
                            // ReSharper disable AccessToDisposedClosure
                            schema = adjuster.GetSchema();
                            xml = adjuster.SerializeToString(configObject);
                            // ReSharper restore AccessToDisposedClosure
                        });

                return this.AddFile(
                    filePath,
                    rootFolders,
                    name => new ExportXmlConfigFile(name, xml) { Schema = schema });
            }
        }

        /// <summary>
        /// Adds a file to the given list of root folders.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="ExportItemBase"/> to create.
        /// </typeparam>
        /// <param name="filePath">
        /// The file path in the export structure where the file should be placed.
        /// </param>
        /// <param name="rootFolders">
        /// The root folders to which to add the file.
        /// </param>
        /// <param name="fileCreator">
        /// The method that creates the <see cref="ExportItemBase"/>
        /// with the given file name (argument of the function).
        /// </param>
        /// <returns>
        /// The newly created file.
        /// </returns>
        protected T AddFile<T>(string filePath, List<ExportFolder> rootFolders, Func<string, T> fileCreator)
            where T : ExportItemBase
        {
            var parts = filePath.Split('\\', '/');
            ExportFolder parent = null;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var folder = this.GetFolder(parent == null ? rootFolders : (IList)parent.Children, parts[i]);
                parent = folder;
            }

            if (parent == null)
            {
                throw new ArgumentException("File path needs to have at least one directory: " + filePath);
            }

            var file = fileCreator(parts.Last());
            lock (parent)
            {
                parent.Children.Add(file);
            }

            return file;
        }

        /// <summary>
        /// Asynchronously adds all files from the given package version to the root folders.
        /// </summary>
        /// <param name="rootFolders">
        /// The root folders to which to add the files.
        /// </param>
        /// <param name="packageVersion">
        /// The package version.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await the completion of this method.
        /// </returns>
        protected async Task AddPackageVersionFilesAsync(
            List<ExportFolder> rootFolders, PackageVersionReadableModel packageVersion)
        {
            if (packageVersion == null)
            {
                return;
            }

            var structure = packageVersion.Structure.Deserialize() as SoftwarePackageDescriptor;
            if (structure == null || structure.Version == null)
            {
                return;
            }

            await this.AddFolderStructureFilesAsync(rootFolders, structure.Version.Structure, packageVersion);
        }

        /// <summary>
        /// Asynchronously adds all files from the given structure to the root folders.
        /// </summary>
        /// <param name="rootFolders">
        /// The root folders to which to add the files.
        /// </param>
        /// <param name="structure">
        /// The structure to add.
        /// </param>
        /// <param name="packageVersion">
        /// The package version.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await the completion of this method.
        /// </returns>
        protected async Task AddFolderStructureFilesAsync(
            List<ExportFolder> rootFolders,
            UpdateFolderStructure structure,
            PackageVersionReadableModel packageVersion = null)
        {
            using (
                var resourceService =
                    this.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>())
            {
                foreach (var folderUpdate in structure.Folders)
                {
                    ExportFolder rootFolder;
                    lock (rootFolders)
                    {
                        rootFolder = rootFolders.FirstOrDefault(
                            f => f.Name.Equals(folderUpdate.Name, StringComparison.InvariantCultureIgnoreCase));
                        if (rootFolder == null)
                        {
                            rootFolder = new ExportFolder(folderUpdate.Name);
                            rootFolders.Add(rootFolder);
                        }
                    }

                    await this.AddFolderStructureFilesAsync(rootFolder, folderUpdate, resourceService, packageVersion);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="ProcessConfig"/> for the given parameters.
        /// </summary>
        /// <param name="exePath">
        /// The executable path from the installation root (e.g. <c>Progs\Update\Update.exe</c>).
        /// </param>
        /// <param name="name">
        /// The name of the application as shown on the splash screen.
        /// </param>
        /// <param name="maxRamMb">
        /// The maximum amount of RAM to be used by the application in megabytes.
        /// </param>
        /// <param name="maxCpu">
        /// The maximum CPU usage in percent (100=100%).
        /// </param>
        /// <returns>
        /// The newly created <see cref="ProcessConfig"/>.
        /// </returns>
        protected ProcessConfig CreateProcessConfig(string exePath, string name, int maxRamMb = 0, int maxCpu = 0)
        {
            var isInform = this.Parent.HardwareDescriptor.Platform is InformPlatformDescriptor;
            var config = new ProcessConfig
                          {
                              Name = name,
                              Enabled = true,
                              UseWatchdog = true,
                              ExecutablePath = @"..\..\" + exePath,
                              WindowMode = ProcessWindowStyle.Minimized,
                          };
            if (maxRamMb > 0)
            {
                config.RamLimit = new ApplicationRamLimitConfig
                                      {
                                          Enabled = !isInform,
                                          MaxRamMb = maxRamMb,
                                          Actions =
                                              {
                                                  new RelaunchLimitActionConfig(),
                                                  new RebootLimitActionConfig()
                                              }
                                      };
            }

            if (maxCpu > 0)
            {
                config.CpuLimit = new CpuLimitConfig
                                   {
                                       Enabled = !isInform,
                                       MaxCpuPercentage = maxCpu,
                                       Actions = { new RelaunchLimitActionConfig() }
                                   };
            }

            return config;
        }

        private ExportFolder GetFolder(IList parentFolders, string name)
        {
            lock (parentFolders)
            {
                var folder =
                    parentFolders.OfType<ExportFolder>()
                        .FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (folder != null)
                {
                    return folder;
                }

                folder = new ExportFolder(name);
                parentFolders.Add(folder);

                return folder;
            }
        }

        private async Task AddFolderStructureFilesAsync(
            ExportFolder exportFolder,
            FolderUpdate folderUpdate,
            ChannelScope<IResourceService> resourceService,
            PackageVersionReadableModel packageVersion = null)
        {
            foreach (var fileSystemUpdate in folderUpdate.Items)
            {
                var fileUpdate = fileSystemUpdate as FileUpdate;
                if (fileUpdate != null)
                {
                    var resource = await resourceService.Channel.GetAsync(fileUpdate.Hash);
                    if (resource == null)
                    {
                        this.Logger.Warn(
                            "Couldn't find resource {0}, not adding file {1} in {2}",
                            fileUpdate.Hash,
                            fileUpdate.Name,
                            exportFolder.Name);
                        continue;
                    }

                    var exportFile = new ExportResourceFile(fileUpdate.Name, resource) { Source = packageVersion };
                    this.AddFile(exportFolder, exportFile);

                    continue;
                }

                var subFolderUpdate = fileSystemUpdate as FolderUpdate;
                if (subFolderUpdate == null)
                {
                    continue;
                }

                ExportFolder exportSubFolder;
                lock (exportFolder)
                {
                    exportSubFolder =
                        exportFolder.Children.OfType<ExportFolder>()
                            .FirstOrDefault(
                                f => f.Name.Equals(subFolderUpdate.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (exportSubFolder == null)
                    {
                        exportSubFolder = new ExportFolder(subFolderUpdate.Name);
                        exportFolder.Children.Add(exportSubFolder);
                    }
                }

                await this.AddFolderStructureFilesAsync(
                    exportSubFolder, subFolderUpdate, resourceService, packageVersion);
            }
        }

        private void AddFile(ExportFolder exportFolder, ExportResourceFile exportFile)
        {
            lock (exportFolder)
            {
                var existing =
                    exportFolder.Children.FirstOrDefault(
                        f => f.Name.Equals(exportFile.Name, StringComparison.InvariantCultureIgnoreCase));
                if (existing == null)
                {
                    exportFolder.Children.Add(exportFile);
                    return;
                }

                var existingFile = existing as ExportResourceFile;
                if (existingFile == null)
                {
                    this.Logger.Error(
                        "Can't add file {0} with hash {1} since there is an item with the same name {2}",
                        exportFile.Name,
                        exportFile.Resource.Hash,
                        existing.Name);
                    return;
                }

                if (existingFile.Resource.Hash == exportFile.Resource.Hash)
                {
                    this.Logger.Debug(
                        "Didn't add file {0} with hash {1} since it already exists",
                        existingFile.Name,
                        exportFile.Resource.Hash);
                    return;
                }

                var message = existingFile.Resource.Hash;
                if (exportFile.Source != null)
                {
                    message += "; the new file comes from " + exportFile.Source.Package.PackageId;
                }

                if (existingFile.Source != null)
                {
                    message += "; the existing file comes from " + existingFile.Source.Package.PackageId;
                }

                this.Logger.Error(
                    "Can't add file {0} with hash {1} since there is a file with the same name but different hash: {2}",
                    exportFile.Name,
                    exportFile.Resource.Hash,
                    message);
            }
        }
    }
}
