// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigAdjuster.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigAdjuster type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class that takes a config object and adjusts it to match the given software version.
    /// This is done using a temporary <see cref="AppDomain"/> in which the config object is
    /// serialized and returned back to this class as an XML string.
    /// </summary>
    /// <typeparam name="T">
    /// The type of config adjusted by this class.
    /// </typeparam>
    internal class ConfigAdjuster<T> : MarshalByRefObject, IDisposable, IAssemblyResolver
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ConfigAdjuster<T>>();

        private readonly IConnectionController connectionController;

        private readonly List<string> tempFiles = new List<string>();

        private UpdateFolderStructure structure;

        private bool useLegacyAssemblies;

        private AppDomain appDomain;

        private ConfigAdjusterProxy proxy;

        private ConfigAdjuster(IConnectionController connectionController)
        {
            this.connectionController = connectionController;
        }

        /// <summary>
        /// Asynchronously creates an instance of this class.
        /// </summary>
        /// <param name="packageVersion">
        /// The package version.
        /// </param>
        /// <param name="connectionController">
        /// The connection controller.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> with the newly created <see cref="ConfigAdjuster{T}"/>.
        /// </returns>
        public static async Task<ConfigAdjuster<T>> CreateAsync(
            PackageVersionReadableModel packageVersion, IConnectionController connectionController)
        {
            var adjuster = new ConfigAdjuster<T>(connectionController);
            await adjuster.LoadAsync(packageVersion);
            return adjuster;
        }

        /// <summary>
        /// Gets the XML schema for the type <see cref="T"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="XmlSchema"/> or null if it is not defined.
        /// </returns>
        public XmlSchema GetSchema()
        {
            var xmlSchema = this.proxy.GetXmlSchema(typeof(T).FullName);
            if (xmlSchema == null)
            {
                return null;
            }

            return XmlSchema.Read(new StringReader(xmlSchema), null);
        }

        /// <summary>
        /// Serializes the given <paramref name="configObject"/> to an XML string.
        /// </summary>
        /// <param name="configObject">
        /// The config object.
        /// </param>
        /// <returns>
        /// The XML serialized object.
        /// </returns>
        public string SerializeToString(T configObject)
        {
            var serializer = new XmlSerializer(configObject.GetType());
            var writer = new StringWriter();
            serializer.Serialize(writer, configObject);

            return this.proxy.ConvertConfig(typeof(T).FullName, writer.ToString());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            AppDomain.Unload(this.appDomain);
            foreach (var tempFile in this.tempFiles)
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't delete temporary file");
                }
            }
        }

        string IAssemblyResolver.Resolve(string assemblyName)
        {
            var index = assemblyName.IndexOf(',');
            if (index > 0)
            {
                assemblyName = assemblyName.Substring(0, index);
            }

            var tempFilename = this.DownloadAssemblyAsync(assemblyName).Result;
            this.tempFiles.Add(tempFilename);
            return tempFilename;
        }

        private static async Task<string> CopyToTempFileAsync(Stream input)
        {
            string tempFile;
            using (input)
            {
                tempFile = Path.GetTempFileName();
                using (var output = File.Create(tempFile))
                {
                    await input.CopyToAsync(output);
                }
            }

            return tempFile;
        }

        private async Task LoadAsync(PackageVersionReadableModel packageVersion)
        {
            var info = new AppDomainSetup { DisallowApplicationBaseProbing = true };
            this.appDomain =
                AppDomain.CreateDomain(
                    packageVersion.Package.PackageId + "-" + packageVersion.SoftwareVersion,
                    null,
                    info);

            var type = typeof(ConfigAdjusterProxy);
            this.proxy =
                (ConfigAdjusterProxy)this.appDomain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);

            Version version;
            if (ParserUtil.TryParse(packageVersion.SoftwareVersion, out version) && version.Build <= 1525)
            {
                // see DownloadAssemblyAsync() for an explanation
                this.useLegacyAssemblies = true;
            }

            var descriptor = (SoftwarePackageDescriptor)packageVersion.Structure.Deserialize();
            this.structure = descriptor.Version.Structure;

            var tempFilename = await this.DownloadAssemblyAsync(typeof(T).Assembly.GetName().Name);
            this.tempFiles.Add(tempFilename);
            this.proxy.LoadAssembly(tempFilename);

            this.proxy.RegisterResolver(this);
        }

        private async Task<string> DownloadAssemblyAsync(string assemblyName)
        {
            if (this.useLegacyAssemblies)
            {
                // special case for pre-1.0 config files: we need to use the version that originally
                // shipped with icenter.admin (the XML serialization was fixed with that version)
                var input = this.GetType()
                    .Assembly.GetManifestResourceStream(this.GetType(), "Bin." + assemblyName + ".dll");
                if (input != null)
                {
                    return await CopyToTempFileAsync(input);
                }

                Logger.Debug("Couldn't find legacy DLL {0}, trying with file from structure", assemblyName);
            }

            using (var resourceService = this.connectionController.CreateChannelScope<IResourceService>())
            {
                foreach (var folder in this.structure.Folders)
                {
                    var tempFilename = await this.DownloadAssemblyAsync(folder, assemblyName, resourceService);
                    if (tempFilename != null)
                    {
                        return tempFilename;
                    }
                }
            }

            throw new FileNotFoundException("Couldn't find assembly " + assemblyName);
        }

        private async Task<string> DownloadAssemblyAsync(
            FolderUpdate folder, string assemblyName, ChannelScope<IResourceService> resourceService)
        {
            foreach (var item in folder.Items)
            {
                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    var tempFilename = await this.DownloadAssemblyAsync(subFolder, assemblyName, resourceService);
                    if (tempFilename != null)
                    {
                        return tempFilename;
                    }

                    continue;
                }

                var file = item as FileUpdate;
                if (file == null
                    || !assemblyName.Equals(
                        Path.GetFileNameWithoutExtension(file.Name),
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var request = new ResourceDownloadRequest { Hash = file.Hash };
                var result = await resourceService.Channel.DownloadAsync(request);

                return await CopyToTempFileAsync(result.Content);
            }

            return null;
        }
    }
}