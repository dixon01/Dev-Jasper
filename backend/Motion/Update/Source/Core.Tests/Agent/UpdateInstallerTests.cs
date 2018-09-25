// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateInstallerTests.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateInstallerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using CommandLineParser;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;
    using Gorba.Common.Utility.FilesTesting.Wrapper;
    using Gorba.Motion.Update.Core.Agent;
    using Gorba.Motion.Update.Core.Tests.Utility;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// Tests that the <see cref="UpdateInstaller"/> behaves as defined.
    /// </summary>
    [TestClass]
    public class UpdateInstallerTests
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            // initialize something from the Gorba.Common.Medi.Resources.dll to load it
            Assert.IsNotNull(new StoredResource());
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.BeginVerifyResources"/> and
        /// <see cref="UpdateInstaller.EndVerifyResources"/> when all resources are available.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestVerifyResources_Available()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);
            Assert.AreEqual(24, resources.Count);
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.BeginVerifyResources"/> and
        /// <see cref="UpdateInstaller.EndVerifyResources"/> when a resource is missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [ExpectedException(typeof(UpdateException))]
        public void TestVerifyResources_Missing()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            fileSystem.GetFile(@"D:\Root\Presentation\Images\logo.jpg").Delete();
            AddResources(root);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));

            target.EndVerifyResources(result);
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for the update config (only).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_UpdateConfigOnly()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string UpdateXml = @"Config\Update\Update.xml";
            FileSystemMock.ChangeFile(root, UpdateXml, "This is an old file");

            var hostMock = new Mock<IInstallationHost>();

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(SelfConfigInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            Assert.IsFalse(engine.Install(hostMock.Object));

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            // "Relaunch" should have been called, meaning we should re-launch Update.exe
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("Update.xml", FileSystemMock.GetFile(root, UpdateXml));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for the update config (only) that fails.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_UpdateConfigOnly_Failed()
        {
            var fileSystem = new FailingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string UpdateXml = @"Config\Update\Update.xml";
            FileSystemMock.ChangeFile(root, UpdateXml, "This is an old file");

            const string MediConfig = @"Config\Update\medi.config";
            FileSystemMock.ChangeFile(root, MediConfig, "This is another old file");

            fileSystem.FailingMoves.Add(Path.Combine(root.FullName, UpdateXml));

            var hostMock = new Mock<IInstallationHost>();

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(SelfConfigInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            try
            {
                engine.Install(hostMock.Object);
                Assert.Fail("Expected IOException");
            }
            catch (IOException)
            {
                // expected this exception
            }

            engine.Rollback(hostMock.Object);

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            // "Relaunch" shouldn't have been called
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // check that the file contents has not changed
            Assert.AreEqual("This is an old file", FileSystemMock.GetFile(root, UpdateXml));
            Assert.AreEqual("This is another old file", FileSystemMock.GetFile(root, MediConfig));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for the update config (with additional files
        /// that shouldn't be touched before restarting Update.exe).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_UpdateConfigAndOthers()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string UpdateXml = @"Config\Update\Update.xml";
            const string LogoJpg = @"Presentation\Images\logo.jpg";

            const string OldImageContents = "This is an old image";

            FileSystemMock.ChangeFile(root, UpdateXml, "This is an old file");
            FileSystemMock.ChangeFile(root, LogoJpg, OldImageContents);

            var hostMock = new Mock<IInstallationHost>();

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(SelfConfigInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            Assert.IsFalse(engine.Install(hostMock.Object));

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            // "Relaunch" should have been called, meaning we should re-launch Update.exe
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("Update.xml", FileSystemMock.GetFile(root, UpdateXml));
            Assert.AreEqual(OldImageContents, FileSystemMock.GetFile(root, LogoJpg));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for Update.exe binaries.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateInstallationEngine_UpdateUpdate()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string UpdateDll = @"Progs\Update\Gorba.Common.Update.ServiceModel.dll";
            const string OldText = "This is an old file";
            FileSystemMock.ChangeFile(root, UpdateDll, OldText);

            ProcessStartInfo startProcess = null;

            var hostMock = new Mock<IInstallationHost>();
            hostMock.Setup(h => h.ExecutablePath).Returns(@"D:\Root\Progs\Update\Update.exe");
            hostMock.Setup(h => h.StartProcess(It.IsAny<ProcessStartInfo>()))
                    .Callback<ProcessStartInfo>(i => startProcess = i);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(SelfInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (s, e) => states.Add(engine.State);

            Assert.IsFalse(engine.Install(hostMock.Object));

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            // "Exit" should have been called, meaning we should exit Update.exe
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Once());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // let's verify the process that should get started
            Assert.IsNotNull(startProcess);
            Assert.IsNotNull(startProcess.Arguments);

            var args = startProcess.Arguments.Split(' ');
            Assert.AreEqual(6, args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim('"');
            }

            var parser = new CommandLineParser();
            var options = new CommandLineOptions();
            parser.ExtractArgumentAttributes(options);
            parser.ParseCommandLine(args);

            Assert.IsNotNull(options.InstallFile);
            Assert.IsNotNull(options.TargetPath);
            Assert.AreEqual(Process.GetCurrentProcess().Id, options.WaitForExit);

            Assert.IsNotNull(fileSystem.GetFile(options.InstallFile));
            Assert.IsNotNull(fileSystem.GetDirectory(options.TargetPath));

            // the process to be started is the new Update.exe
            Assert.AreEqual("Update.exe", FileSystemMock.GetFile(root, startProcess.FileName));

            // verify the contents of the temp directory
            var tempDir = fileSystem.GetDirectory(Path.GetDirectoryName(startProcess.FileName));
            Assert.IsNotNull(tempDir);
            Assert.AreEqual(0, tempDir.GetDirectories().Length);
            var files = tempDir.GetFiles().ToList();
            Assert.AreEqual(6, files.Count);

            // verify there is the <guid>-Update.exe file with the contents of the original Update.exe
            var tempExe = files.FirstOrDefault(f => f.Name.EndsWith("-Update.exe"));
            Assert.IsNotNull(tempExe);
            Assert.AreEqual("Update.exe", FileSystemMock.GetFile(root, tempExe.FullName));
            files.Remove(tempExe);

            foreach (var file in files)
            {
                Assert.AreEqual(file.Name, FileSystemMock.GetFile(root, file.FullName));
            }

            // check that the old file contents has not changed
            Assert.AreEqual(OldText, FileSystemMock.GetFile(root, UpdateDll));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for one other application (not SM or Update).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_SingleApplication()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string IbisXml = @"Config\Protran\ibis.xml";
            FileSystemMock.ChangeFile(root, IbisXml, "This is an old file");
            FileSystemMock.CreateDirectory(root, @"Presentation\Test");

            var hostMock = new Mock<IInstallationHost>();
            var apps = SetupMockApplications(root, hostMock);
            Assert.AreEqual(5, apps.Count);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            Assert.IsTrue(engine.Install(hostMock.Object));

            Assert.AreEqual(2, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);
            Assert.AreEqual(UpdateState.Installed, states[1]);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called exactly once for Protran.exe
            hostMock.Verify(h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is called exactly once for Protran.exe
            hostMock.Verify(h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("ibis.xml", FileSystemMock.GetFile(root, IbisXml));

            IDirectoryInfo testDir;
            Assert.IsFalse(fileSystem.TryGetDirectory(@"D:\Root\Presentation\Test", out testDir));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for one other application (not SM or Update)
        /// that fails because of an <see cref="IOException"/>.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateInstallationEngine_SingleApplication_Failed()
        {
            var fileSystem = new FailingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string ProtranXml = @"Config\Protran\Protran.xml";
            FileSystemMock.ChangeFile(root, ProtranXml, "This is an old file");

            const string IbisXml = @"Config\Protran\ibis.xml";
            FileSystemMock.ChangeFile(root, IbisXml, "This is another old file");

            const string Im2File = @"Presentation\Test\file.im2";
            FileSystemMock.CreateFile(root, Im2File, "Presentation file");
            FileSystemMock.CreateDirectory(root, @"Presentation\Other\");

            const string PresentationLogo = @"D:\Root\Presentation\Images\logo.jpg";
            fileSystem.GetFile(PresentationLogo).Delete();

            const string MainIm2 = @"D:\Root\Presentation\main.im2";
            fileSystem.GetFile(MainIm2).Delete();

            fileSystem.FailingMoves.Add(MainIm2);

            var hostMock = new Mock<IInstallationHost>();
            var apps = SetupMockApplications(root, hostMock);
            Assert.AreEqual(5, apps.Count);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            try
            {
                engine.Install(hostMock.Object);
                Assert.Fail("Expected IOException");
            }
            catch (IOException)
            {
                // expected this exception
            }

            // ExitApplication is called exactly once for Protran.exe
            hostMock.Verify(h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is not yet called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            engine.Rollback(hostMock.Object);

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called exactly once for Protran.exe
            hostMock.Verify(h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is called exactly once for Protran.exe
            hostMock.Verify(h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // check that the file contents has not changed
            Assert.AreEqual("This is an old file", FileSystemMock.GetFile(root, ProtranXml));
            Assert.AreEqual("This is another old file", FileSystemMock.GetFile(root, IbisXml));

            Assert.AreEqual("Presentation file", FileSystemMock.GetFile(root, Im2File));

            IDirectoryInfo testDir;
            Assert.IsTrue(fileSystem.TryGetDirectory(@"D:\Root\Presentation\Other", out testDir));
            Assert.AreEqual(0, testDir.GetDirectories().Length);
            Assert.AreEqual(0, testDir.GetFiles().Length);

            IFileInfo testFile;
            Assert.IsFalse(fileSystem.TryGetFile(MainIm2, out testFile));
            Assert.IsFalse(fileSystem.TryGetFile(PresentationLogo, out testFile));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is nothing to be updated.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_UpToDate()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            var hostMock = new Mock<IInstallationHost>();
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(NullInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            Assert.IsTrue(engine.Install(hostMock.Object));

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installed, states[0]);

            // nothing is exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for multiple application (without SM or Update).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_MultipleApplication()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string IbisXml = @"Config\Protran\ibis.xml";
            const string ComposerDll = @"Progs\Composer\Gorba.Motion.Infomedia.Core.dll";
            FileSystemMock.ChangeFile(root, IbisXml, "This is an old file");
            FileSystemMock.ChangeFile(root, ComposerDll, "This is an old DLL");

            var hostMock = new Mock<IInstallationHost>();
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            Assert.IsTrue(engine.Install(hostMock.Object));

            Assert.AreEqual(2, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);
            Assert.AreEqual(UpdateState.Installed, states[1]);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for Protran.exe and Composer.exe
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(2));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is called for Protran.exe and Composer.exe
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(2));
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("ibis.xml", FileSystemMock.GetFile(root, IbisXml));
            Assert.AreEqual("Gorba.Motion.Infomedia.Core.dll", FileSystemMock.GetFile(root, ComposerDll));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for one dependant application which is defined in the config.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateInstallationEngine_Dependencies_One()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string MainIm2 = @"D:\Root\Presentation\main.im2";
            FileSystemMock.ChangeFile(root, MainIm2, "This is an old file");

            const string VideoMpg = @"D:\Root\Presentation\Videos\presentation.mpg";
            FileSystemMock.ChangeFile(root, VideoMpg, "This is another old file");

            var hostMock = new Mock<IInstallationHost>();
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var restart = new RestartApplicationsConfig
                              {
                                  Dependencies =
                                      {
                                          new DependencyConfig
                                              {
                                                  Path = @"D:\Root\Presentation\main.im2",
                                                  ExecutablePaths =
                                                      {
                                                          @"D:\Root\Progs\Composer\Composer.exe"
                                                      }
                                              },
                                          new DependencyConfig
                                              {
                                                  Path = @"D:\Root\Presentation\Images",
                                                  ExecutablePaths =
                                                      {
                                                          @"D:\Root\Progs\Protran\Protran.exe"
                                                      }
                                              }
                                      }
                              };

            var engine = target.CreateInstallationEngine(resources, restart);
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            Assert.IsTrue(engine.Install(hostMock.Object));

            Assert.AreEqual(2, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);
            Assert.AreEqual(UpdateState.Installed, states[1]);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for Protran.exe and Composer.exe
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is called for Protran.exe and Composer.exe
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("main.im2", FileSystemMock.GetFile(root, MainIm2));
            Assert.AreEqual("presentation.mpg", FileSystemMock.GetFile(root, VideoMpg));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for multiple dependant applications which are defined in the config.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateInstallationEngine_Dependencies_Multiple()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string MainIm2 = @"D:\Root\Presentation\main.im2";
            FileSystemMock.ChangeFile(root, MainIm2, "This is an old file");

            const string LogoJpg = @"D:\Root\Presentation\Images\logo.jpg";
            FileSystemMock.ChangeFile(root, LogoJpg, "This is another old file");

            var hostMock = new Mock<IInstallationHost>();
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var restart = new RestartApplicationsConfig
                              {
                                  Dependencies =
                                      {
                                          new DependencyConfig
                                              {
                                                  Path = @"D:\Root\Presentation\main.im2",
                                                  ExecutablePaths =
                                                      {
                                                          @"D:\Root\Progs\Composer\Composer.exe"
                                                      }
                                              },
                                          new DependencyConfig
                                              {
                                                  Path = @"D:\Root\Presentation\Images",
                                                  ExecutablePaths =
                                                      {
                                                          @"D:\Root\Progs\Protran\Protran.exe"
                                                      }
                                              }
                                      }
                              };

            var engine = target.CreateInstallationEngine(resources, restart);
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (sender, args) => states.Add(engine.State);

            Assert.IsTrue(engine.Install(hostMock.Object));

            Assert.AreEqual(2, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);
            Assert.AreEqual(UpdateState.Installed, states[1]);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for Protran.exe and Composer.exe
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(2));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is called for Protran.exe and Composer.exe
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(2));
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("main.im2", FileSystemMock.GetFile(root, MainIm2));
            Assert.AreEqual("logo.jpg", FileSystemMock.GetFile(root, LogoJpg));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for System Manager only.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_SystemManager()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string SystemManagerXml = @"Config\SystemManager\SystemManager.xml";
            FileSystemMock.ChangeFile(root, SystemManagerXml, "This is an old file");

            ProcessStartInfo startProcess = null;

            var hostMock = new Mock<IInstallationHost>();
            hostMock.Setup(h => h.ExecutablePath).Returns(@"D:\Root\Progs\Update\Update.exe");
            hostMock.Setup(h => h.StartProcess(It.IsAny<ProcessStartInfo>()))
                    .Callback<ProcessStartInfo>(i => startProcess = i);
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (s, e) => states.Add(engine.State);

            Assert.IsFalse(engine.Install(hostMock.Object));

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            Assert.IsNotNull(startProcess);
            Assert.AreEqual(@"D:\Root\Progs\SystemManager\SystemManager.exe", startProcess.FileName);
            Assert.IsNotNull(startProcess.Arguments);
            var args = startProcess.Arguments.Split(' ');
            Assert.AreEqual(2, args.Length);
            Assert.AreEqual("/waitforexit", args[0]);
            int processId;
            Assert.IsTrue(int.TryParse(args[1], out processId));
            Assert.IsTrue(processId > 0);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for every application but us
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(3));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "System Manager"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is never called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            // ForceExit is called once
            hostMock.Verify(h => h.ForceExit(), Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("SystemManager.xml", FileSystemMock.GetFile(root, SystemManagerXml));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for System Manager only that fails.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateInstallationEngine_SystemManager_Failed()
        {
            var fileSystem = new FailingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string SystemManagerXml = @"Config\SystemManager\SystemManager.xml";
            FileSystemMock.ChangeFile(root, SystemManagerXml, "This is an old file");

            const string SystemManagerMedi = @"D:\Root\Config\SystemManager\Medi.config";
            fileSystem.GetFile(SystemManagerMedi).Delete();

            const string SystemManagerDll = @"D:\Root\Progs\SystemManager\Gorba.Common.Medi.Core.dll";
            fileSystem.GetFile(SystemManagerDll).Delete();

            fileSystem.FailingMoves.Add(Path.Combine(root.FullName, SystemManagerXml));

            ProcessStartInfo startProcess = null;

            var hostMock = new Mock<IInstallationHost>();
            hostMock.Setup(h => h.ExecutablePath).Returns(@"D:\Root\Progs\Update\Update.exe");
            hostMock.Setup(h => h.StartProcess(It.IsAny<ProcessStartInfo>()))
                    .Callback<ProcessStartInfo>(i => startProcess = i);
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (s, e) => states.Add(engine.State);

            try
            {
                engine.Install(hostMock.Object);
                Assert.Fail("Expected IOException");
            }
            catch (IOException)
            {
                // expected this exception
            }

            Assert.IsNull(startProcess);

            // ExitApplication is called for every application but us
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(3));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "System Manager"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is never called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            // ForceExit is not yet called
            hostMock.Verify(h => h.ForceExit(), Times.Never());

            engine.Rollback(hostMock.Object);

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            Assert.IsNotNull(startProcess);
            Assert.AreEqual(@"D:\Root\Progs\SystemManager\SystemManager.exe", startProcess.FileName);
            Assert.IsNotNull(startProcess.Arguments);
            var args = startProcess.Arguments.Split(' ');
            Assert.AreEqual(2, args.Length);
            Assert.AreEqual("/waitforexit", args[0]);
            int processId;
            Assert.IsTrue(int.TryParse(args[1], out processId));
            Assert.IsTrue(processId > 0);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for every application but us
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(3));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "System Manager"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is never called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            // ForceExit is called once
            hostMock.Verify(h => h.ForceExit(), Times.Once());

            // check that the file contents has not changed
            Assert.AreEqual("This is an old file", FileSystemMock.GetFile(root, SystemManagerXml));

            IFileInfo testFile;
            Assert.IsFalse(fileSystem.TryGetFile(SystemManagerMedi, out testFile));
            Assert.IsFalse(fileSystem.TryGetFile(SystemManagerDll, out testFile));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for System Manager and other components.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        public void TestCreateInstallationEngine_SystemManagerMixed()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string SystemManagerXml = @"Config\SystemManager\SystemManager.xml";
            const string IbisXml = @"Config\Protran\ibis.xml";
            FileSystemMock.ChangeFile(root, SystemManagerXml, "This is an old file");
            FileSystemMock.ChangeFile(root, IbisXml, "This is an old file");

            ProcessStartInfo startProcess = null;

            var hostMock = new Mock<IInstallationHost>();
            hostMock.Setup(h => h.ExecutablePath).Returns(@"D:\Root\Progs\Update\Update.exe");
            hostMock.Setup(h => h.StartProcess(It.IsAny<ProcessStartInfo>()))
                    .Callback<ProcessStartInfo>(i => startProcess = i);
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (s, e) => states.Add(engine.State);

            Assert.IsFalse(engine.Install(hostMock.Object));

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            Assert.IsNotNull(startProcess);
            Assert.AreEqual(@"D:\Root\Progs\SystemManager\SystemManager.exe", startProcess.FileName);
            Assert.IsNotNull(startProcess.Arguments);
            var args = startProcess.Arguments.Split(' ');
            Assert.AreEqual(2, args.Length);
            Assert.AreEqual("/waitforexit", args[0]);
            int processId;
            Assert.IsTrue(int.TryParse(args[1], out processId));
            Assert.IsTrue(processId > 0);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for every application but us
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(3));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "System Manager"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is never called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            // ForceExit is called once
            hostMock.Verify(h => h.ForceExit(), Times.Once());

            // check that the file contents has actually changed
            Assert.AreEqual("SystemManager.xml", FileSystemMock.GetFile(root, SystemManagerXml));
            Assert.AreEqual("ibis.xml", FileSystemMock.GetFile(root, IbisXml));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for System Manager and other components,
        /// but it fails when updating System Manager
        /// (the other components will be installed properly).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateInstallationEngine_SystemManagerMixed_FailedSystemManager()
        {
            var fileSystem = new FailingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string SystemManagerXml = @"Config\SystemManager\SystemManager.xml";
            const string IbisXml = @"Config\Protran\ibis.xml";
            FileSystemMock.ChangeFile(root, SystemManagerXml, "This is an old file");
            FileSystemMock.ChangeFile(root, IbisXml, "This is an old file");

            const string SystemManagerDll = @"D:\Root\Progs\SystemManager\Gorba.Common.Medi.Core.dll";
            fileSystem.GetFile(SystemManagerDll).Delete();

            fileSystem.FailingMoves.Add(Path.Combine(root.FullName, SystemManagerXml));

            ProcessStartInfo startProcess = null;

            var hostMock = new Mock<IInstallationHost>();
            hostMock.Setup(h => h.ExecutablePath).Returns(@"D:\Root\Progs\Update\Update.exe");
            hostMock.Setup(h => h.StartProcess(It.IsAny<ProcessStartInfo>()))
                    .Callback<ProcessStartInfo>(i => startProcess = i);
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (s, e) => states.Add(engine.State);

            try
            {
                engine.Install(hostMock.Object);
                Assert.Fail("Expected IOException");
            }
            catch (IOException)
            {
                // expected this exception
            }

            Assert.IsNull(startProcess);

            // ExitApplication is called for every application but us
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(3));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "System Manager"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is never called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            // ForceExit is not yet called
            hostMock.Verify(h => h.ForceExit(), Times.Never());

            engine.Rollback(hostMock.Object);

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            Assert.IsNotNull(startProcess);
            Assert.AreEqual(@"D:\Root\Progs\SystemManager\SystemManager.exe", startProcess.FileName);
            Assert.IsNotNull(startProcess.Arguments);
            var args = startProcess.Arguments.Split(' ');
            Assert.AreEqual(2, args.Length);
            Assert.AreEqual("/waitforexit", args[0]);
            int processId;
            Assert.IsTrue(int.TryParse(args[1], out processId));
            Assert.IsTrue(processId > 0);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for every application but us
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Exactly(3));
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Composer"), It.IsAny<string>()),
                Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "System Manager"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is never called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            // ForceExit is called once
            hostMock.Verify(h => h.ForceExit(), Times.Once());

            // check that the file contents of the other application has actually changed
            Assert.AreEqual("ibis.xml", FileSystemMock.GetFile(root, IbisXml));

            // check that the file contents of System Manager has not changed
            Assert.AreEqual("This is an old file", FileSystemMock.GetFile(root, SystemManagerXml));

            IFileInfo testFile;
            Assert.IsFalse(fileSystem.TryGetFile(SystemManagerDll, out testFile));
        }

        /// <summary>
        /// Tests <see cref="UpdateInstaller.CreateInstallationEngine"/>
        /// when there is an update for System Manager and other components,
        /// but it fails when updating the other components (nothing will be installed).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Agent\UpdateCommand.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateInstallationEngine_SystemManagerMixed_FailedOther()
        {
            var fileSystem = new FailingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");

            SetupServices(root);

            var configurator = new Configurator("UpdateCommand.xml");
            var command = configurator.Deserialize<UpdateCommand>();
            CreateFileSystem(root, command);
            AddResources(root);

            const string SystemManagerXml = @"Config\SystemManager\SystemManager.xml";
            const string IbisXml = @"Config\Protran\ibis.xml";
            FileSystemMock.ChangeFile(root, SystemManagerXml, "This is an old file");
            FileSystemMock.ChangeFile(root, IbisXml, "This is an old file");

            const string AbuDhabiXml = @"Config\Protran\AbuDhabi.xml";
            FileSystemMock.CreateFile(root, AbuDhabiXml, AbuDhabiXml);

            const string SystemManagerDll = @"D:\Root\Progs\SystemManager\Gorba.Common.Medi.Core.dll";
            fileSystem.GetFile(SystemManagerDll).Delete();

            const string ProtranDll = @"D:\Root\Progs\Protran\Gorba.Motion.Protran.Ibis.dll";
            fileSystem.GetFile(ProtranDll).Delete();

            fileSystem.FailingMoves.Add(Path.Combine(root.FullName, IbisXml));

            ProcessStartInfo startProcess = null;

            var hostMock = new Mock<IInstallationHost>();
            hostMock.Setup(h => h.ExecutablePath).Returns(@"D:\Root\Progs\Update\Update.exe");
            hostMock.Setup(h => h.StartProcess(It.IsAny<ProcessStartInfo>()))
                    .Callback<ProcessStartInfo>(i => startProcess = i);
            SetupMockApplications(root, hostMock);

            var target = new UpdateInstaller(command, root, new NullProgressMonitor());
            var result = target.BeginVerifyResources(null, null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
            var resources = target.EndVerifyResources(result);

            var engine = target.CreateInstallationEngine(resources, new RestartApplicationsConfig());
            Assert.IsInstanceOfType(engine, typeof(DefaultInstaller));

            var states = new List<UpdateState>();
            engine.StateChanged += (s, e) => states.Add(engine.State);

            try
            {
                engine.Install(hostMock.Object);
                Assert.Fail("Expected IOException");
            }
            catch (IOException)
            {
                // expected this exception
            }

            Assert.IsNull(startProcess);

            // ExitApplication is called for the touched application only
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication has not yet been called
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Never());

            // ForceExit is never called
            hostMock.Verify(h => h.ForceExit(), Times.Never());

            engine.Rollback(hostMock.Object);

            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(UpdateState.Installing, states[0]);

            Assert.IsNull(startProcess);

            // Update.exe is not exited or relaunched
            hostMock.Verify(h => h.Exit(It.IsAny<string>()), Times.Never());
            hostMock.Verify(h => h.Relaunch(It.IsAny<string>()), Times.Never());

            // ExitApplication is called for the touched application only
            hostMock.Verify(
                h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.ExitApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // RelaunchApplication is called for the touched application
            hostMock.Verify(
                h => h.RelaunchApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()), Times.Once());
            hostMock.Verify(
                h => h.RelaunchApplication(It.Is<ApplicationInfo>(i => i.Name == "Protran"), It.IsAny<string>()),
                Times.Once());

            // ForceExit is never called
            hostMock.Verify(h => h.ForceExit(), Times.Never());

            // check that the file contents has actually changed
            Assert.AreEqual("This is an old file", FileSystemMock.GetFile(root, SystemManagerXml));
            Assert.AreEqual("This is an old file", FileSystemMock.GetFile(root, IbisXml));

            IFileInfo testFile;
            Assert.IsFalse(fileSystem.TryGetFile(SystemManagerDll, out testFile));
            Assert.IsFalse(fileSystem.TryGetFile(ProtranDll, out testFile));

            Assert.AreEqual(AbuDhabiXml, FileSystemMock.GetFile(root, AbuDhabiXml));
        }

        private static List<ApplicationInfo> SetupMockApplications(
            IWritableDirectoryInfo root, Mock<IInstallationHost> hostMock)
        {
            var apps = new List<ApplicationInfo>();
            CreateApps(root, apps);

            apps.Add(
                new ApplicationInfo("0")
                    {
                        Name = "Another Application",
                        Path = @"D:\Progs\Other\AnotherApp.exe",
                        State = ApplicationState.Unknown
                    });

            hostMock.Setup(h => h.GetRunningApplications()).Returns(apps);
            hostMock.Setup(h => h.CreateApplicationStateObserver(It.IsAny<ApplicationInfo>()))
                    .Returns<ApplicationInfo>(
                        i =>
                            {
                                var observerMock = new Mock<IApplicationStateObserver>();
                                observerMock.Setup(o => o.ApplicationName).Returns(i.Name);
                                observerMock.Setup(o => o.State).Returns(() => i.State);
                                return observerMock.Object;
                            });
            hostMock.Setup(h => h.ExitApplication(It.IsAny<ApplicationInfo>(), It.IsAny<string>()))
                    .Callback<ApplicationInfo, string>(
                        (app, reason) => app.State = ApplicationState.Exited);
            return apps;
        }

        private static void CreateApps(IDirectoryInfo directory, List<ApplicationInfo> apps)
        {
            foreach (var subDir in directory.GetDirectories())
            {
                CreateApps(subDir, apps);
            }

            foreach (var file in directory.GetFiles())
            {
                if (file.Extension == ".exe")
                {
                    var name = Path.GetFileNameWithoutExtension(file.Name);
                    if (name == null || name.Equals("SystemManager", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name = "System Manager";
                    }

                    apps.Add(new ApplicationInfo("0")
                    {
                        Name = name,
                        Path = file.FullName,
                        State = ApplicationState.Running
                    });
                }
            }
        }

        private static void AddResources(IWritableDirectoryInfo directory)
        {
            var resourceService = MessageDispatcher.Instance.GetService<IResourceService>();
            foreach (var file in directory.GetFiles())
            {
                resourceService.RegisterResource(file.FullName, false);
            }

            foreach (var subDir in directory.GetDirectories())
            {
                AddResources(subDir);
            }
        }

        private static void SetupServices(IWritableDirectoryInfo root)
        {
            var serviceContainer = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => new ServiceContainerLocator(serviceContainer));

            FileSystemManager.ChangeLocalFileSystem(root.FileSystem);
            PathManager.ChangeInstance(new TestingPathManager(root, "Update"));
            var mediConfig = new MediConfig
                                 {
                                     Services =
                                         {
                                             new LocalResourceServiceConfig
                                                 {
                                                     ResourceDirectory = @"D:\Resources\"
                                                 }
                                         }
                                 };
            var configurator = new ObjectConfigurator(mediConfig, "Unit", "Update");
            MessageDispatcher.Instance.Configure(configurator);
        }

        private static void CreateFileSystem(IWritableDirectoryInfo root, UpdateCommand command)
        {
            var rootFolder = new FolderUpdate();
            foreach (var folder in command.Folders)
            {
                rootFolder.Items.Add(folder);
            }

            CreateFileSystem(root, rootFolder);
        }

        private static void CreateFileSystem(IWritableDirectoryInfo directory, FolderUpdate folder)
        {
            var fileSystem = directory.FileSystem;
            foreach (var item in folder.Items)
            {
                var path = Path.Combine(directory.FullName, item.Name);
                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    var subDir = fileSystem.CreateDirectory(path);
                    CreateFileSystem(subDir, subFolder);
                    continue;
                }

                var file = item as FileUpdate;
                if (file != null)
                {
                    using (var writer = new StreamWriter(fileSystem.CreateFile(path).OpenWrite()))
                    {
                        writer.Write(file.Name);
                    }
                }
            }
        }

        private RestartApplicationsConfig GetRestartAppConfig(string configUpdateXml)
        {
            var configManager = new ConfigManager<UpdateConfig>();
            configManager.FileName = configUpdateXml;
            configManager.EnableCaching = true;
            return configManager.Config.Agent.RestartApplications;
        }

        private class FailingFileSystem : WrapperFileSystem
        {
            public FailingFileSystem()
                : base(new TestingFileSystem())
            {
                this.FailingMoves = new List<string>();
            }

            public List<string> FailingMoves { get; private set; }

            public override WrapperFileInfo CreateFileInfo(IWritableFileInfo file)
            {
                return new FailingFileInfo(file, this);
            }

            private class FailingFileInfo : WrapperFileInfo
            {
                private readonly FailingFileSystem fileSystem;

                public FailingFileInfo(IWritableFileInfo file, FailingFileSystem fileSystem)
                    : base(file, fileSystem)
                {
                    this.fileSystem = fileSystem;
                }

                public override IWritableFileInfo MoveTo(string newFileName)
                {
                    if (this.fileSystem.FailingMoves.Remove(newFileName))
                    {
                        throw new IOException("Can't move file to " + newFileName);
                    }

                    return base.MoveTo(newFileName);
                }
            }
        }

        // ReSharper restore InconsistentNaming
    }
}
