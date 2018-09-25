// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//  Export a main unit
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.MainUnit;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Utils;
    using Gorba.Common.Configuration.EPaper.MainUnit;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The export controller that exports everything related to a main unit.
    /// </summary>
    public class MainUnitExportController : ExportControllerBase
    {
        private MainUnitConfigPartController mainUnitConfigPart;

        private SoftwareVersionsPartController softwareVersionsPart;

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public override void Initialize(UnitConfiguratorController parentController)
        {
            base.Initialize(parentController);
            this.mainUnitConfigPart = this.Parent.GetPart<MainUnitConfigPartController>();
            this.mainUnitConfigPart.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();

            var displayUnitsCount =
                HardwareDescriptors.PowerUnit.GetDisplayUnitCount(parentController.UnitConfiguration.ProductType.Name);
            for (var i = 0; i < displayUnitsCount; i++)
            {
                var displayUnitPart =
                    this.Parent.GetPart<DisplayUnitPartController>(UnitConfigKeys.MainUnit.DisplayUnit + (i + 1));
                displayUnitPart.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();
            }

            this.softwareVersionsPart = this.Parent.GetPart<SoftwareVersionsPartController>();

            this.UpdateSoftwarePackages();
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
        public override async Task CreateExportStructureAsync(List<ExportFolder> rootFolders)
        {
            if (this.SoftwarePackageIds.Count == 0)
            {
                return;
            }

            var mainUnitConfig = this.UnitConfigXml();

            this.AddXmlConfigFile(mainUnitConfig, @"Config\MainUnit\MainUnit.xml", rootFolders);
            await Task.FromResult(1);
        }

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<ApplicationConfigBase> CreateApplicationConfigs()
        {
            return new List<ApplicationConfigBase>();
        }

        private static void InsertMainUnitFirmwareHash(XDocument doc, string hash)
        {
            foreach (var element in doc.Elements("MainUnit"))
            {
                element.Attribute("FirmwareHash").Value = hash;
            }
        }

        private static void InsertDisplayUnitFirmwareHash(XDocument doc, string hash)
        {
            foreach (var element in doc.Descendants().Elements("DisplayUnit"))
            {
                element.Attribute("FirmwareHash").Value = hash;
            }
        }

        private static string RemoveBom(string xml)
        {
            var byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (xml.StartsWith(byteOrderMarkUtf8))
            {
                xml = xml.Remove(0, byteOrderMarkUtf8.Length);
            }

            return xml;
        }

        private MainUnitConfig UnitConfigXml()
        {
            MainUnitConfig mainUnitConfig;
            var config = this.mainUnitConfigPart.ViewModel.Editor.Config;

            var filledDocument = this.AddFirmwareToConfig(config.Document);

            var serializer = new XmlSerializer(typeof(MainUnitConfig));
            using (var memoryStream = new MemoryStream(filledDocument.Length))
            {
                using (var sw = new StreamWriter(memoryStream))
                {
                    sw.Write(filledDocument);
                    sw.Flush();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    mainUnitConfig = (MainUnitConfig)serializer.Deserialize(memoryStream);
                }
            }

            return mainUnitConfig;
        }

        private string AddFirmwareToConfig(string config)
        {
            var mainUnitFirmwareHash = this.EnsureXxHashVersionIsUploaded(PackageIds.PowerUnit.MainUnitFirmware);
            var displayUnitFirmwareHash = this.EnsureXxHashVersionIsUploaded(PackageIds.PowerUnit.DisplayUnitFirmware);

            var xml = RemoveBom(config);
            var doc = XDocument.Parse(xml);
            InsertMainUnitFirmwareHash(doc, mainUnitFirmwareHash);
            InsertDisplayUnitFirmwareHash(doc, displayUnitFirmwareHash);

            var xmlWriterSetting = new XmlWriterSettings
                                       {
                                           Indent = true,
                                           OmitXmlDeclaration = false,
                                           Encoding = Encoding.UTF8
                                       };
            var memoryStream = new MemoryStream();
            using (var writer = XmlWriter.Create(memoryStream, xmlWriterSetting))
            {
                doc.WriteTo(writer);
            }

            memoryStream.Position = 0;
            var sr = new StreamReader(memoryStream);
            var fullXml = sr.ReadToEnd();
            return fullXml;
        }

        private string EnsureXxHashVersionIsUploaded(string packageName)
        {
            var mainUnitFirmwareResource = this.DownloadFirmwareResource(packageName);

            var firmware = new MemoryStream();
            mainUnitFirmwareResource.Content.CopyTo(firmware);
            firmware.Seek(0, SeekOrigin.Begin);

            var hash = ContentResourceHash.Create(firmware, HashAlgorithmTypes.xxHash64);

            using (var contentResourceService =
                    this.Parent.DataController.ConnectionController.CreateChannelScope<IContentResourceService>())
            {
                var contentResourceExists =
                    contentResourceService.Channel.TestContentResourceAsync(hash, HashAlgorithmTypes.xxHash64).Result;

                if (contentResourceExists)
                {
                    return hash;
                }

                firmware.Seek(0, SeekOrigin.Begin);
                var contentResource = new ContentResource
                                          {
                                              Hash = hash,
                                              HashAlgorithmType = HashAlgorithmTypes.xxHash64,
                                              OriginalFilename = mainUnitFirmwareResource.Resource.OriginalFilename,
                                              MimeType = mainUnitFirmwareResource.Resource.MimeType,
                                              UploadingUser = mainUnitFirmwareResource.Resource.UploadingUser
                                          };

                var uploadArguments = new ContentResourceUploadRequest
                                          {
                                              Resource = contentResource,
                                              Content = firmware
                                          };
                var result = contentResourceService.Channel.UploadAsync(uploadArguments).Result;
                return result.Resource.Hash;
            }
        }

        private ResourceDownloadResult DownloadFirmwareResource(
            string packageName)
        {
            var selectedPackageVersions = this.softwareVersionsPart.GetSelectedPackageVersions();
            var selectedFirmware = selectedPackageVersions[packageName];

            if (selectedFirmware.Structure == null)
            {
                throw new InvalidDataException("Could not deserialize a SoftwarePackageDescriptor, no XML structure.");
            }

            var descriptor = selectedFirmware.Structure.Deserialize() as SoftwarePackageDescriptor;
            if (descriptor == null)
            {
                throw new InvalidDataException("Could not deserialize a SoftwarePackageDescriptor.");
            }

            var folders = descriptor.Version.Structure.Folders;
            if (folders.Count != 1)
            {
                throw new InvalidDataException(
                    "SoftwarePackageDescriptor for firmware contains not exactly one folder.");
            }

            if (folders[0].Items.Count != 1)
            {
                throw new InvalidDataException("SoftwarePackageDescriptor for firmware contains not exactly one file.");
            }

            var file = folders[0].Items[0] as Gorba.Common.Update.ServiceModel.Messages.FileUpdate;
            if (file == null)
            {
                throw new InvalidDataException("Could not get FileUpdate from folder.");
            }

            using (var resourceChannel =
                    this.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>())
            {
                var request = new ResourceDownloadRequest { Hash = file.Hash };
                return resourceChannel.Channel.DownloadAsync(request).Result;
            }
        }

        private void UpdateSoftwarePackages()
        {
            if (this.mainUnitConfigPart.ViewModel.IsVisible)
            {
                this.AddSoftwarePackageId(PackageIds.PowerUnit.MainUnitFirmware);
                this.AddSoftwarePackageId(PackageIds.PowerUnit.DisplayUnitFirmware);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.PowerUnit.MainUnitFirmware);
                this.RemoveSoftwarePackageId(PackageIds.PowerUnit.DisplayUnitFirmware);
            }
        }
    }
}
