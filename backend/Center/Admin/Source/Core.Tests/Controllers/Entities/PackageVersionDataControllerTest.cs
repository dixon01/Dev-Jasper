// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageVersionDataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for the PackageVersion specific pre and post implementations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Software;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Center.Common.ServiceModel.Filters.Software;
    using Gorba.Center.Common.ServiceModel.Software;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests for the PackageVersion specific pre and post implementations.
    /// </summary>
    [TestClass]
    public class PackageVersionDataControllerTest
    {
        /// <summary>
        /// The initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// The cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Helpers.ResetServiceLocator();
        }

        /// <summary>
        /// Tests the creation and preparation of an editable package version entity.
        /// </summary>
        [TestMethod]
        public void EditPackageVersionTest()
        {
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, null);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.PackageVersionManagerMock.Setup(manager => manager.Create())
                .Returns(new PackageVersionWritableModel());

            var package = new Package { Id = 100, ProductName = "TestProduct" };
            var packageReadable = new PackageReadableModelMock(package);
            connectionControllerMockSetup.PackageManagerMock.Setup(
                manager => manager.QueryAsync(It.IsAny<PackageQuery>()))
                .Returns(Task.FromResult(new List<PackageReadableModel> { packageReadable }.AsEnumerable()));
            var initialStructure = new SoftwarePackageDescriptor
                                       {
                                           Description = "SoftwareDescriptor",
                                           Name = "PackageDescriptor",
                                           PackageId = "Package1",
                                           Version =
                                               new SoftwareVersion
                                                   {
                                                       Description = "Version",
                                                       VersionNumber = "1.0",
                                                       Structure = CreateInitialFolderStructure()
                                                   }
                                       };
            var packageToEdit = new PackageVersion
                                    {
                                        Id = 10,
                                        Package = package,
                                        Description = "Description",
                                        Structure = new XmlData(initialStructure)
                                    };
            var packageVersionReadable = new PackageVersionReadableModelMock(packageToEdit);
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);
            var packageReadableDvm = dataController.Factory.CreateReadOnly(packageVersionReadable);
            var editEntity = dataController.PackageVersion.EditEntityAsync(packageReadableDvm).Result;
            Assert.IsNotNull(editEntity);
            var editPackage = editEntity as PackageVersionDataViewModel;
            Assert.IsNotNull(editPackage);
            Assert.AreEqual(1, editPackage.RootFolders.Count);
            Assert.AreEqual("Progs", editPackage.RootFolders.First().Name);
            Assert.AreEqual(1, editPackage.RootFolders.First().Items.Count());
            var subFolder = editPackage.RootFolders.First().Items.First();
            var subFolderItem = subFolder as FolderItem;
            Assert.IsNotNull(subFolderItem);
            Assert.AreEqual("SubFolder", subFolderItem.Name);
            Assert.AreEqual(1, subFolderItem.Items.Count());
        }

        /// <summary>
        /// Tests saving a new package version.
        /// </summary>
        [TestMethod]
        public void SavePackageVersionTest()
        {
            var resourceServiceMock = new Mock<IResourceService>();
            var resourceChannel = ChannelScope<IResourceService>.Create(resourceServiceMock.Object);
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, null);
            var connectionControllerMockSetup = Helpers.CreateConnectionControllerMock();
            connectionControllerMockSetup.PackageVersionManagerMock.Setup(manager => manager.Create())
                .Returns(new PackageVersionWritableModel());
            connectionControllerMockSetup.ConnectionControllerMock.Setup(
                controller => controller.CreateChannelScope<IResourceService>()).Returns(resourceChannel);
            connectionControllerMockSetup.PackageVersionManagerMock.Setup(
                manager => manager.CommitAndVerifyAsync(It.IsAny<PackageVersionWritableModel>()))
                .Returns(
                    (PackageVersionWritableModel model) =>
                        {
                            var dto = model.ToDto();
                            dto.Id = 300;
                            return Task.FromResult(
                                (PackageVersionReadableModel)new PackageVersionReadableModelMock(dto));
                        });
            var folderStructure = CreateFolderStructureForSave();
            var dataController = Helpers.SetupDataController(connectionControllerMockSetup.ConnectionControllerMock);

            var packageToSave = new PackageVersion
                                    {
                                        Id = 10,
                                        Package =
                                            new Package { Id = 100, ProductName = "TestProduct", PackageId = "50" },
                                        Description = "Description",
                                    };
            var packageReadable = new PackageVersionReadableModelMock(packageToSave);
            var writable = packageReadable.ToChangeTrackingModel();
            writable.Description = "NewDescription";
            var dvm = dataController.Factory.Create(writable);
            dvm.RootFolders.Add(folderStructure);
            var savedModel = dataController.PackageVersion.SaveEntityAsync(dvm).Result;
            Assert.IsNotNull(savedModel);
            var packageVersion = savedModel as PackageVersionReadOnlyDataViewModel;
            Assert.IsNotNull(packageVersion);
            Assert.AreEqual(300, packageVersion.Id);
            var structure = packageVersion.ReadableModel.Structure;
            var softwareDescriptor = (SoftwarePackageDescriptor)structure.Deserialize();
            Assert.IsNotNull(softwareDescriptor);
            Assert.AreEqual("TestProduct", softwareDescriptor.Name);
            Assert.AreEqual("50", softwareDescriptor.PackageId);
            var softwareVersion = softwareDescriptor.Version;
            Assert.IsNotNull(softwareVersion);
            Assert.IsNotNull(softwareVersion.Structure);
            Assert.AreEqual(1, softwareVersion.Structure.Folders.Count);
        }

        private static UpdateFolderStructure CreateInitialFolderStructure()
        {
            var rootFolder = new FolderUpdate { Name = "Progs" };
            var updateFolder = new FolderUpdate
                                   {
                                       Items =
                                           new List<FileSystemUpdate>
                                               {
                                                   new FileUpdate
                                                       {
                                                           Hash =
                                                               "FileHash1",
                                                           Name = "ResourceFile.txt"
                                                       }
                                               },
                                       Name = "SubFolder"
                                   };
            rootFolder.Items.Add(updateFolder);
            var folderStructure = new UpdateFolderStructure { Folders = new List<FolderUpdate> { rootFolder },  };
            return folderStructure;
        }

        private static FolderItem CreateFolderStructureForSave()
        {
            var rootFolder = new FolderItem { Name = "Progs" };
            var updateFolder = new FolderItem { Name = "Subfolder" };
            var fileItem = new FileItem { Name = "File1", Hash = "FileHash" };
            updateFolder.Children.Add(fileItem);
            rootFolder.Children.Add(updateFolder);
            return rootFolder;
        }

        private class PackageReadableModelMock : PackageReadableModel
        {
            public PackageReadableModelMock(Package entity)
                : base(entity)
            {
                this.Populate();
            }
        }

        private sealed class PackageVersionReadableModelMock : PackageVersionReadableModel
        {
            public PackageVersionReadableModelMock(PackageVersion entity)
                : base(entity)
            {
                this.Populate();
                this.Package = new PackageReadableModelMock(entity.Package);
            }

            public override Task LoadReferencePropertiesAsync()
            {
                return Task.FromResult(0);
            }

            public override Task LoadNavigationPropertiesAsync()
            {
                return Task.FromResult(0);
            }

            public override Task LoadXmlPropertiesAsync()
            {
                return Task.FromResult(0);
            }
        }
    }
}
