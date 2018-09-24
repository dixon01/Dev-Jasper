// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpServerDataControllerTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpServerDataControllerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Tests.Controllers.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.Controllers.Entities.Meta;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Meta;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.Wpf.Client.Tests.Mocks;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Configuration.Update.Providers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Unit tests for <see cref="FtpServerDataController"/>.
    /// </summary>
    [TestClass]
    public class FtpServerDataControllerTest
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
        /// Unit test for <see cref="FtpServerDataController.get_All"/>.
        /// </summary>
        [TestMethod]
        public void TestGetAll()
        {
            var dataController = PrepareDataController();

            var target = new FtpServerDataController(dataController.SystemConfig);
            target.AwaitAllDataAsync().Wait();

            var all = target.All;
            Assert.AreEqual(2, all.Count);

            var ftp = all.FirstOrDefault(f => f.Host == "ftp.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser", ftp.Username);
            Assert.AreEqual("ftppassword", ftp.Password);
            Assert.AreEqual("/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(15), ftp.PollInterval);

            ftp = all.FirstOrDefault(f => f.Host == "ftp2.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.GZIP, ftp.Compression);
            Assert.AreEqual("ftpuser2", ftp.Username);
            Assert.AreEqual("ftppassword2", ftp.Password);
            Assert.AreEqual("/sub/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromSeconds(15), ftp.PollInterval);
        }

        /// <summary>
        /// Unit test for <see cref="FtpServerDataController.CreateEntityAsync"/>
        /// and <see cref="FtpServerDataController.SaveEntityAsync"/>.
        /// </summary>
        [TestMethod]
        public void TestCreateAndSaveEntity()
        {
            var dataController = PrepareDataController();

            var target = new FtpServerDataController(dataController.SystemConfig);
            target.AwaitAllDataAsync().Wait();

            var writable = target.CreateEntityAsync().Result as FtpServerDataViewModel;
            Assert.IsNotNull(writable);

            writable.Host = "new.example.com";
            writable.Port = 23;
            writable.Compression = CompressionAlgorithm.None;
            writable.Username = "ftpuser3";
            writable.Password = "ftppassword3";
            writable.RepositoryBasePath = "/new/";
            writable.PollInterval = TimeSpan.FromMinutes(5);

            var ftp = target.SaveEntityAsync(writable).Result as FtpServerReadOnlyDataViewModel;
            Assert.IsNotNull(ftp);
            Assert.AreEqual("new.example.com", ftp.Host);
            Assert.AreEqual(23, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser3", ftp.Username);
            Assert.AreEqual("ftppassword3", ftp.Password);
            Assert.AreEqual("/new/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(5), ftp.PollInterval);

            var all = target.All;
            Assert.AreEqual(3, all.Count);

            ftp = all.FirstOrDefault(f => f.Host == "ftp.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser", ftp.Username);
            Assert.AreEqual("ftppassword", ftp.Password);
            Assert.AreEqual("/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(15), ftp.PollInterval);

            ftp = all.FirstOrDefault(f => f.Host == "new.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual("new.example.com", ftp.Host);
            Assert.AreEqual(23, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser3", ftp.Username);
            Assert.AreEqual("ftppassword3", ftp.Password);
            Assert.AreEqual("/new/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(5), ftp.PollInterval);

            ftp = all.FirstOrDefault(f => f.Host == "ftp2.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.GZIP, ftp.Compression);
            Assert.AreEqual("ftpuser2", ftp.Username);
            Assert.AreEqual("ftppassword2", ftp.Password);
            Assert.AreEqual("/sub/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromSeconds(15), ftp.PollInterval);
        }

        /// <summary>
        /// Unit test for <see cref="FtpServerDataController.EditEntityAsync"/>
        /// and <see cref="FtpServerDataController.SaveEntityAsync"/>.
        /// </summary>
        [TestMethod]
        public void TestEditAndSaveEntity()
        {
            var dataController = PrepareDataController();

            var target = new FtpServerDataController(dataController.SystemConfig);
            target.AwaitAllDataAsync().Wait();

            var ftp = target.All.FirstOrDefault(f => f.Host == "ftp.example.com");
            Assert.IsNotNull(ftp);

            var writable = target.EditEntityAsync(ftp).Result as FtpServerDataViewModel;
            Assert.IsNotNull(writable);
            Assert.AreEqual(21, writable.Port);
            Assert.AreEqual(CompressionAlgorithm.None, writable.Compression);
            Assert.AreEqual("ftpuser", writable.Username);
            Assert.AreEqual("ftppassword", writable.Password);
            Assert.AreEqual("/", writable.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(15), writable.PollInterval);

            writable.RepositoryBasePath = "/new/";

            ftp = target.SaveEntityAsync(writable).Result as FtpServerReadOnlyDataViewModel;
            Assert.IsNotNull(ftp);
            Assert.AreEqual("ftp.example.com", ftp.Host);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser", ftp.Username);
            Assert.AreEqual("ftppassword", ftp.Password);
            Assert.AreEqual("/new/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(15), ftp.PollInterval);

            var all = target.All;
            Assert.AreEqual(2, all.Count);

            ftp = all.FirstOrDefault(f => f.Host == "ftp.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser", ftp.Username);
            Assert.AreEqual("ftppassword", ftp.Password);
            Assert.AreEqual("/new/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(15), ftp.PollInterval);

            ftp = all.FirstOrDefault(f => f.Host == "ftp2.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.GZIP, ftp.Compression);
            Assert.AreEqual("ftpuser2", ftp.Username);
            Assert.AreEqual("ftppassword2", ftp.Password);
            Assert.AreEqual("/sub/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromSeconds(15), ftp.PollInterval);
        }

        /// <summary>
        /// Unit test for <see cref="FtpServerDataController.CopyEntityAsync"/>
        /// and <see cref="FtpServerDataController.SaveEntityAsync"/>.
        /// </summary>
        [TestMethod]
        public void TestCopyAndSaveEntity()
        {
            var dataController = PrepareDataController();

            var target = new FtpServerDataController(dataController.SystemConfig);
            target.AwaitAllDataAsync().Wait();

            var ftp = target.All.FirstOrDefault(f => f.Host == "ftp.example.com");
            Assert.IsNotNull(ftp);

            var writable = target.CopyEntityAsync(ftp).Result as FtpServerDataViewModel;
            Assert.IsNotNull(writable);
            Assert.AreEqual(21, writable.Port);
            Assert.AreEqual(CompressionAlgorithm.None, writable.Compression);
            Assert.AreEqual("ftpuser", writable.Username);
            Assert.AreEqual("ftppassword", writable.Password);
            Assert.AreEqual("/", writable.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(15), writable.PollInterval);

            writable.Host = "new.example.com";
            writable.PollInterval = TimeSpan.FromMinutes(5);

            ftp = target.SaveEntityAsync(writable).Result as FtpServerReadOnlyDataViewModel;
            Assert.IsNotNull(ftp);
            Assert.AreEqual("new.example.com", ftp.Host);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser", ftp.Username);
            Assert.AreEqual("ftppassword", ftp.Password);
            Assert.AreEqual("/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(5), ftp.PollInterval);

            var all = target.All;
            Assert.AreEqual(3, all.Count);

            ftp = all.FirstOrDefault(f => f.Host == "ftp.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser", ftp.Username);
            Assert.AreEqual("ftppassword", ftp.Password);
            Assert.AreEqual("/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(15), ftp.PollInterval);

            ftp = all.FirstOrDefault(f => f.Host == "new.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.None, ftp.Compression);
            Assert.AreEqual("ftpuser", ftp.Username);
            Assert.AreEqual("ftppassword", ftp.Password);
            Assert.AreEqual("/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromMinutes(5), ftp.PollInterval);

            ftp = all.FirstOrDefault(f => f.Host == "ftp2.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.GZIP, ftp.Compression);
            Assert.AreEqual("ftpuser2", ftp.Username);
            Assert.AreEqual("ftppassword2", ftp.Password);
            Assert.AreEqual("/sub/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromSeconds(15), ftp.PollInterval);
        }

        /// <summary>
        /// Unit test for <see cref="FtpServerDataController.DeleteEntityAsync"/>.
        /// </summary>
        [TestMethod]
        public void TestDeleteEntity()
        {
            var dataController = PrepareDataController();

            var target = new FtpServerDataController(dataController.SystemConfig);
            target.AwaitAllDataAsync().Wait();

            var ftp = target.All.FirstOrDefault(f => f.Host == "ftp.example.com");
            Assert.IsNotNull(ftp);

            target.DeleteEntityAsync(ftp).Wait();

            var all = target.All;
            Assert.AreEqual(1, all.Count);

            ftp = all.FirstOrDefault(f => f.Host == "ftp2.example.com");
            Assert.IsNotNull(ftp);
            Assert.AreEqual(21, ftp.Port);
            Assert.AreEqual(CompressionAlgorithm.GZIP, ftp.Compression);
            Assert.AreEqual("ftpuser2", ftp.Username);
            Assert.AreEqual("ftppassword2", ftp.Password);
            Assert.AreEqual("/sub/", ftp.RepositoryBasePath);
            Assert.AreEqual(TimeSpan.FromSeconds(15), ftp.PollInterval);
        }

        private static DataController PrepareDataController()
        {
            var container = Helpers.InitializeServiceLocator();
            Helpers.CreateApplicationStateMock(container, new Tenant { Id = 1, Name = "TestTenant1" });
            var dataViewModelFactory = new DataViewModelFactory(new Mock<ICommandRegistry>().Object);
            var dataController = new DataController(dataViewModelFactory);
            var connectionController = new ConnectionControllerMock();
            connectionController.Configure();
            dataController.Initialize(connectionController);

            var settings = new BackgroundSystemSettings
                {
                    MaintenanceMode = new MaintenanceModeSettings { IsEnabled = false },
                    FtpUpdateProviders = new List<FtpUpdateProviderConfig>
                        {
                            new FtpUpdateProviderConfig
                                {
                                    Host = "ftp.example.com",
                                    Port = 21,
                                    Compression = CompressionAlgorithm.None,
                                    Username = "ftpuser",
                                    Password = "ftppassword",
                                    RepositoryBasePath = "/",
                                    PollInterval = TimeSpan.FromMinutes(15)
                                },
                            new FtpUpdateProviderConfig
                                {
                                    Host = "ftp2.example.com",
                                    Port = 21,
                                    Compression = CompressionAlgorithm.GZIP,
                                    Username = "ftpuser2",
                                    Password = "ftppassword2",
                                    RepositoryBasePath = "/sub/",
                                    PollInterval = TimeSpan.FromSeconds(15)
                                },
                        }
                };

            var systemConfig = connectionController.SystemConfigChangeTrackingManager.Create();
            systemConfig.SystemId = Guid.NewGuid();
            systemConfig.Settings = new XmlData(settings);
            systemConfig.Commit();
            return dataController;
        }
    }
}
