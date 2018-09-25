// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerCompatibilityTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerCompatibilityTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager.Tests.Compatibility
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The System Manager configuration file compatibility test.
    /// </summary>
    [TestClass]
    public class SystemManagerCompatibilityTest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests <c>SystemManager.xml</c> version 2.2.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\SystemManager_v2.2.xml")]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestV2_2()
        {
            var configManager = new ConfigManager<SystemManagerConfig>();
            configManager.FileName = "SystemManager_v2.2.xml";
            configManager.XmlSchema = SystemManagerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Defaults);
            Assert.IsNull(config.Defaults.Component);
            Assert.IsNotNull(config.Defaults.Process);
            Assert.AreEqual(TimeSpan.Zero, config.Defaults.Process.LaunchDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.Defaults.Process.RelaunchDelay);
            Assert.AreEqual(ProcessWindowStyle.Normal, config.Defaults.Process.WindowMode);
            Assert.IsTrue(config.Defaults.Process.KillIfRunning);
            Assert.IsNull(config.Defaults.Process.Arguments);
            Assert.IsNull(config.Defaults.Process.CpuLimit);
            Assert.IsNull(config.Defaults.Process.ExecutablePath);
            Assert.IsNull(config.Defaults.Process.LaunchWaitFor);
            Assert.IsNull(config.Defaults.Process.WorkingDirectory);
            Assert.IsFalse(config.Defaults.Process.UseWatchdog);

            Assert.AreEqual(0, config.SplashScreens.X);
            Assert.AreEqual(0, config.SplashScreens.Y);
            Assert.AreEqual(-1, config.SplashScreens.Width);
            Assert.AreEqual(-1, config.SplashScreens.Height);
            Assert.AreEqual(1, config.SplashScreens.Items.Count);

            var splashScreen = config.SplashScreens.Items[0];
            Assert.AreEqual("Boot", splashScreen.Name);
            Assert.IsTrue(splashScreen.Enabled);
            Assert.AreEqual("Black", splashScreen.Foreground);
            Assert.AreEqual("#E6ECF0", splashScreen.Background);
            Assert.AreEqual(1, splashScreen.ShowOn.Count);
            Assert.IsInstanceOfType(splashScreen.ShowOn[0], typeof(SystemBootTriggerConfig));
            Assert.AreEqual(1, splashScreen.HideOn.Count);
            Assert.IsInstanceOfType(splashScreen.HideOn[0], typeof(SystemShutdownTriggerConfig));
            Assert.AreEqual(4, splashScreen.Items.Count);
            Assert.IsInstanceOfType(splashScreen.Items[0], typeof(LogoSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[1], typeof(SystemSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[2], typeof(NetworkSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[3], typeof(ApplicationsSplashScreenItem));

            Assert.IsNotNull(config.System);
            Assert.IsTrue(config.System.KickWatchdog);

            Assert.IsNotNull(config.System.RamLimit);
            Assert.IsTrue(config.System.RamLimit.Enabled);
            Assert.AreEqual(100, config.System.RamLimit.FreeRamMb);
            Assert.IsNull(config.System.RamLimit.FreeRamPercentage);
            Assert.AreEqual(1, config.System.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(config.System.RamLimit.Actions[0], typeof(RebootLimitActionConfig));

            Assert.IsNotNull(config.System.CpuLimit);
            Assert.IsTrue(config.System.CpuLimit.Enabled);
            Assert.AreEqual(98, config.System.CpuLimit.MaxCpuPercentage);
            Assert.AreEqual(1, config.System.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(config.System.RamLimit.Actions[0], typeof(RebootLimitActionConfig));

            Assert.IsNotNull(config.System.DiskLimits);
            Assert.IsTrue(config.System.DiskLimits.Enabled);
            Assert.AreEqual(2, config.System.DiskLimits.Disks.Count);

            var disk = config.System.DiskLimits.Disks[0];
            Assert.AreEqual("C:\\", disk.Path);
            Assert.IsTrue(disk.Enabled);
            Assert.AreEqual(5, disk.FreeSpaceMb);
            Assert.IsNull(disk.FreeSpacePercentage);
            Assert.AreEqual(1, disk.Actions.Count);
            Assert.IsInstanceOfType(disk.Actions[0], typeof(RebootLimitActionConfig));

            disk = config.System.DiskLimits.Disks[1];
            Assert.AreEqual("D:\\", disk.Path);
            Assert.IsTrue(disk.Enabled);
            Assert.AreEqual(10, disk.FreeSpaceMb);
            Assert.AreEqual(5, disk.FreeSpacePercentage);
            Assert.AreEqual(2, disk.Actions.Count);
            Assert.IsInstanceOfType(disk.Actions[0], typeof(PurgeLimitActionConfig));
            Assert.IsInstanceOfType(disk.Actions[1], typeof(PurgeLimitActionConfig));

            Assert.IsNotNull(config.System);
            Assert.IsNotNull(config.System.PreventPopups);
            Assert.IsTrue(config.System.PreventPopups.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.System.PreventPopups.CheckInterval);
            Assert.AreEqual(0, config.System.PreventPopups.Popups.Count);

            Assert.AreEqual(2, config.Applications.Count);

            var process = config.Applications[0] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("Update", process.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(1), process.RelaunchDelay);
            Assert.AreEqual(@"..\..\Progs\Update\Update.exe", process.ExecutablePath);
            Assert.AreEqual(ProcessWindowStyle.Minimized, process.WindowMode);
            Assert.IsNotNull(process.RamLimit);
            Assert.IsTrue(process.RamLimit.Enabled);
            Assert.AreEqual(200, process.RamLimit.MaxRamMb);
            Assert.AreEqual(2, process.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(process.RamLimit.Actions[0], typeof(RelaunchLimitActionConfig));
            Assert.IsInstanceOfType(process.RamLimit.Actions[1], typeof(RebootLimitActionConfig));
            Assert.IsNull(process.CpuLimit);

            process = config.Applications[1] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("HardwareManager", process.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(1), process.RelaunchDelay);
            Assert.AreEqual(@"..\..\Progs\HardwareManager\HardwareManager.exe", process.ExecutablePath);
            Assert.AreEqual(ProcessWindowStyle.Minimized, process.WindowMode);
            Assert.IsNotNull(process.RamLimit);
            Assert.IsTrue(process.RamLimit.Enabled);
            Assert.AreEqual(150, process.RamLimit.MaxRamMb);
            Assert.AreEqual(2, process.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(process.RamLimit.Actions[0], typeof(RelaunchLimitActionConfig));
            Assert.IsInstanceOfType(process.RamLimit.Actions[1], typeof(RebootLimitActionConfig));
            Assert.IsNotNull(process.CpuLimit);
            Assert.IsTrue(process.CpuLimit.Enabled);
            Assert.AreEqual(30, process.CpuLimit.MaxCpuPercentage);
            Assert.AreEqual(1, process.CpuLimit.Actions.Count);
            Assert.IsInstanceOfType(process.CpuLimit.Actions[0], typeof(RelaunchLimitActionConfig));
        }

        /// <summary>
        /// Tests <c>SystemManager.xml</c> version 2.4.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\SystemManager_v2.4.xml")]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit test code")]
        public void TestV2_4()
        {
            var configManager = new ConfigManager<SystemManagerConfig>();
            configManager.FileName = "SystemManager_v2.4.xml";
            configManager.XmlSchema = SystemManagerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Defaults);
            Assert.IsNull(config.Defaults.Component);
            Assert.IsNotNull(config.Defaults.Process);
            Assert.AreEqual(TimeSpan.Zero, config.Defaults.Process.LaunchDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.Defaults.Process.RelaunchDelay);
            Assert.AreEqual(ProcessWindowStyle.Normal, config.Defaults.Process.WindowMode);
            Assert.IsTrue(config.Defaults.Process.KillIfRunning);
            Assert.IsNull(config.Defaults.Process.Arguments);
            Assert.IsNull(config.Defaults.Process.CpuLimit);
            Assert.IsNull(config.Defaults.Process.ExecutablePath);
            Assert.IsNull(config.Defaults.Process.LaunchWaitFor);
            Assert.IsNull(config.Defaults.Process.WorkingDirectory);
            Assert.IsFalse(config.Defaults.Process.UseWatchdog);

            Assert.AreEqual(0, config.SplashScreens.X);
            Assert.AreEqual(0, config.SplashScreens.Y);
            Assert.AreEqual(-1, config.SplashScreens.Width);
            Assert.AreEqual(-1, config.SplashScreens.Height);
            Assert.AreEqual(2, config.SplashScreens.Items.Count);

            var splashScreen = config.SplashScreens.Items[0];
            Assert.AreEqual("Boot", splashScreen.Name);
            Assert.IsTrue(splashScreen.Enabled);
            Assert.AreEqual("Black", splashScreen.Foreground);
            Assert.AreEqual("#E6ECF0", splashScreen.Background);
            Assert.AreEqual(1, splashScreen.ShowOn.Count);
            Assert.IsInstanceOfType(splashScreen.ShowOn[0], typeof(SystemBootTriggerConfig));
            Assert.AreEqual(1, splashScreen.HideOn.Count);
            Assert.IsInstanceOfType(splashScreen.HideOn[0], typeof(SystemShutdownTriggerConfig));
            Assert.AreEqual(4, splashScreen.Items.Count);
            Assert.IsInstanceOfType(splashScreen.Items[0], typeof(LogoSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[1], typeof(SystemSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[2], typeof(NetworkSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[3], typeof(ApplicationsSplashScreenItem));

            splashScreen = config.SplashScreens.Items[1];
            Assert.AreEqual("Announcement", splashScreen.Name);
            Assert.IsTrue(splashScreen.Enabled);
            Assert.AreEqual("Black", splashScreen.Foreground);
            Assert.AreEqual("#FF0000", splashScreen.Background);
            Assert.AreEqual(1, splashScreen.ShowOn.Count);
            Assert.IsInstanceOfType(splashScreen.ShowOn[0], typeof(InputTriggerConfig));
            Assert.AreEqual(1, splashScreen.HideOn.Count);
            Assert.IsInstanceOfType(splashScreen.HideOn[0], typeof(InputTriggerConfig));
            Assert.AreEqual(3, splashScreen.Items.Count);
            Assert.IsInstanceOfType(splashScreen.Items[0], typeof(LogoSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[1], typeof(SystemSplashScreenItem));
            Assert.IsInstanceOfType(splashScreen.Items[2], typeof(NetworkSplashScreenItem));

            Assert.IsNotNull(config.System);
            Assert.IsTrue(config.System.KickWatchdog);

            Assert.IsNotNull(config.System.RamLimit);
            Assert.IsTrue(config.System.RamLimit.Enabled);
            Assert.AreEqual(100, config.System.RamLimit.FreeRamMb);
            Assert.IsNull(config.System.RamLimit.FreeRamPercentage);
            Assert.AreEqual(1, config.System.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(config.System.RamLimit.Actions[0], typeof(RebootLimitActionConfig));

            Assert.IsNotNull(config.System.CpuLimit);
            Assert.IsTrue(config.System.CpuLimit.Enabled);
            Assert.AreEqual(98, config.System.CpuLimit.MaxCpuPercentage);
            Assert.AreEqual(1, config.System.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(config.System.RamLimit.Actions[0], typeof(RebootLimitActionConfig));

            Assert.IsNotNull(config.System.DiskLimits);
            Assert.IsTrue(config.System.DiskLimits.Enabled);
            Assert.AreEqual(2, config.System.DiskLimits.Disks.Count);

            var disk = config.System.DiskLimits.Disks[0];
            Assert.AreEqual("C:\\", disk.Path);
            Assert.IsTrue(disk.Enabled);
            Assert.AreEqual(5, disk.FreeSpaceMb);
            Assert.IsNull(disk.FreeSpacePercentage);
            Assert.AreEqual(1, disk.Actions.Count);
            Assert.IsInstanceOfType(disk.Actions[0], typeof(RebootLimitActionConfig));

            disk = config.System.DiskLimits.Disks[1];
            Assert.AreEqual("D:\\", disk.Path);
            Assert.IsTrue(disk.Enabled);
            Assert.AreEqual(10, disk.FreeSpaceMb);
            Assert.AreEqual(5, disk.FreeSpacePercentage);
            Assert.AreEqual(2, disk.Actions.Count);
            Assert.IsInstanceOfType(disk.Actions[0], typeof(PurgeLimitActionConfig));
            Assert.IsInstanceOfType(disk.Actions[1], typeof(PurgeLimitActionConfig));

            Assert.IsNotNull(config.System);
            Assert.IsNotNull(config.System.PreventPopups);
            Assert.IsTrue(config.System.PreventPopups.Enabled);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.System.PreventPopups.CheckInterval);
            Assert.AreEqual(0, config.System.PreventPopups.Popups.Count);

            Assert.AreEqual(2, config.Applications.Count);

            var process = config.Applications[0] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("Update", process.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(1), process.RelaunchDelay);
            Assert.AreEqual(@"..\..\Progs\Update\Update.exe", process.ExecutablePath);
            Assert.AreEqual(ProcessWindowStyle.Minimized, process.WindowMode);
            Assert.IsNotNull(process.RamLimit);
            Assert.IsTrue(process.RamLimit.Enabled);
            Assert.AreEqual(200, process.RamLimit.MaxRamMb);
            Assert.AreEqual(2, process.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(process.RamLimit.Actions[0], typeof(RelaunchLimitActionConfig));
            Assert.IsInstanceOfType(process.RamLimit.Actions[1], typeof(RebootLimitActionConfig));
            Assert.IsNull(process.CpuLimit);

            process = config.Applications[1] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("HardwareManager", process.Name);
            Assert.AreEqual(TimeSpan.FromSeconds(1), process.RelaunchDelay);
            Assert.AreEqual(@"..\..\Progs\HardwareManager\HardwareManager.exe", process.ExecutablePath);
            Assert.AreEqual(ProcessWindowStyle.Minimized, process.WindowMode);
            Assert.IsNotNull(process.RamLimit);
            Assert.IsTrue(process.RamLimit.Enabled);
            Assert.AreEqual(150, process.RamLimit.MaxRamMb);
            Assert.AreEqual(2, process.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(process.RamLimit.Actions[0], typeof(RelaunchLimitActionConfig));
            Assert.IsInstanceOfType(process.RamLimit.Actions[1], typeof(RebootLimitActionConfig));
            Assert.IsNotNull(process.CpuLimit);
            Assert.IsTrue(process.CpuLimit.Enabled);
            Assert.AreEqual(30, process.CpuLimit.MaxCpuPercentage);
            Assert.AreEqual(1, process.CpuLimit.Actions.Count);
            Assert.IsInstanceOfType(process.CpuLimit.Actions[0], typeof(RelaunchLimitActionConfig));
        }

        /// <summary>
        /// Tests that <c>SystemManager.xml</c> can be saved with binary serialization (used for .cache file).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Compatibility\SystemManager_v2.4.xml")]
        public void TestBinarySerialization()
        {
            var configManager = new ConfigManager<SystemManagerConfig>();
            configManager.FileName = "SystemManager_v2.4.xml";
            configManager.XmlSchema = SystemManagerConfig.Schema;
            var config = configManager.Config;

            Assert.IsNotNull(config);
            var memory = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(memory, config);
            Assert.IsTrue(memory.Position > 0);
        }

        // ReSharper restore InconsistentNaming
    }
}
