// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateClientTests.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateClientTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Gorba.Common.Update.ServiceModel.Clients;

namespace Gorba.Motion.Update.Core.Tests.UpdateClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.Usb;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The update client tests.
    /// </summary>
    [TestClass]
    public class UpdateClientTests
    {
        private TestingFileSystem fileSystem;

        /// <summary>
        /// Tests the reception of update commands in the correct order.
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestReceptionOfCommands()
        {
            this.fileSystem = new TestingFileSystem();
            var root = this.fileSystem.CreateDirectory(@"D:\Root\Update");
            this.CreateFileSystemForUsb(root);
            var config = new UsbUpdateClientConfig
                             {
                                 Name = "USB",
                                 RepositoryBasePath = @"D:\Root\Update",
                                 PollInterval = TimeSpan.FromSeconds(20),
                                 UsbDetectionTimeOut = TimeSpan.FromSeconds(20)
                             };

            FileSystemManager.ChangeLocalFileSystem(this.fileSystem);
            var usbUpdateClient = new UsbUpdateClient();
            usbUpdateClient.Configure(config, null);
            usbUpdateClient.Start();
        }

        private void CreateFileSystemForUsb(IWritableDirectoryInfo root)
        {
            var commandspath = Path.Combine(root.FullName, @"Commands");
            this.fileSystem.CreateDirectory(commandspath);
            this.fileSystem.CreateDirectory(Path.Combine(root.FullName, @"Feedback"));
            this.fileSystem.CreateDirectory(Path.Combine(root.FullName, @"Resources"));
            var unitPath = Path.Combine(commandspath, @"PC1236");
            this.fileSystem.CreateDirectory(unitPath);
            var guid = Guid.NewGuid().ToString();
            var fileName = string.Concat(guid, "-0001.guc");
            this.fileSystem.CreateFile(Path.Combine(unitPath, fileName));
            fileName = string.Concat(guid, "-0003.guc");
            this.fileSystem.CreateFile(Path.Combine(unitPath, fileName));
            fileName = string.Concat(guid, "-0002.guc");
            this.fileSystem.CreateFile(Path.Combine(unitPath, fileName));
            fileName = string.Concat(guid, "-0004.guc");
            this.fileSystem.CreateFile(Path.Combine(unitPath, fileName));
            var repositoryConfig = new RepositoryConfig
                                       {
                                           Versions =
                                               new List<RepositoryVersionConfig>
                                                   {
                                                       new RepositoryVersionConfig
                                                           {
                                                               CommandsDirectory
                                                                   =
                                                                   "Commands",
                                                               FeedbackDirectory
                                                                   =
                                                                   "Feedback",
                                                               ResourceDirectory
                                                                   =
                                                                   "Resources"
                                                           }
                                                   }
                                       };
            var repositoryXml = Path.Combine(root.FullName, @"repository.xml");
            using (var writer = new StreamWriter(this.fileSystem.CreateFile(repositoryXml).OpenWrite()))
            {
                writer.Write(@"repository.xml");
            }

            var fileInfo = this.fileSystem.GetFile(repositoryXml);
            var configurator = new Configurator(fileInfo.FullName);
            configurator.Serialize(repositoryConfig);
       }
    }
}
