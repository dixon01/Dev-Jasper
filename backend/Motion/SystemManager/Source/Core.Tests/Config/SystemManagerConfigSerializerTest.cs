// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerConfigSerializerTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerConfigSerializerTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Tests.Config
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.SystemManagement.ServiceModel;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for serialization of <see cref="Gorba.Common.Configuration.SystemManager.SystemManagerConfig"/>.
    /// </summary>
    [TestClass]
    public class SystemManagerConfigSerializerTest
    {
        /// <summary>
        /// Tests if a sample config file can be de-serialized.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Config\SystemManager1.xml")]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Unit test")]
        public void DeserializeTest()
        {
            var configManager = new ConfigManager<SystemManagerConfig>();
            configManager.FileName = "SystemManager1.xml";
            configManager.EnableCaching = true;
            var config = configManager.Config;

            Assert.IsNotNull(config);

            Assert.IsNotNull(config.Defaults);
            Assert.IsNotNull(config.Defaults.Process);
            Assert.IsNull(config.Defaults.Process.LaunchWaitFor);
            Assert.IsNotNull(config.Defaults.Process.LaunchDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(0), config.Defaults.Process.LaunchDelay);
            Assert.IsNotNull(config.Defaults.Process.RelaunchDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(10), config.Defaults.Process.RelaunchDelay);
            Assert.IsFalse(config.Defaults.Process.UseWatchdog);
            Assert.IsNull(config.Defaults.Process.ExecutablePath);
            Assert.IsNull(config.Defaults.Process.WorkingDirectory);
            Assert.IsNull(config.Defaults.Process.Arguments);
            Assert.IsNotNull(config.Defaults.Process.WindowMode);
            Assert.AreEqual(ProcessWindowStyle.Normal, config.Defaults.Process.WindowMode);
            Assert.IsNotNull(config.Defaults.Process.Priority);
            Assert.AreEqual(ProcessPriorityClass.Normal, config.Defaults.Process.Priority);
            Assert.IsTrue(config.Defaults.Process.KillIfRunning);
            Assert.IsNull(config.Defaults.Process.RamLimit);
            Assert.IsNotNull(config.Defaults.Process.CpuLimit);
            Assert.AreEqual(10, config.Defaults.Process.CpuLimit.MaxCpuPercentage);
            Assert.IsTrue(config.Defaults.Process.CpuLimit.Enabled);
            Assert.AreEqual(1, config.Defaults.Process.CpuLimit.Actions.Count);
            Assert.IsInstanceOfType(config.Defaults.Process.CpuLimit.Actions[0], typeof(RelaunchLimitActionConfig));

            Assert.IsNotNull(config.SplashScreens);
            Assert.AreEqual(4, config.SplashScreens.Items.Count);

            var splashScreen = config.SplashScreens.Items[0];
            Assert.IsTrue(splashScreen.Enabled);
            Assert.AreEqual("Boot", splashScreen.Name);
            Assert.AreEqual("White", splashScreen.Foreground);
            Assert.AreEqual("#000066", splashScreen.Background);
            Assert.AreEqual(1, splashScreen.ShowOn.Count);
            Assert.IsInstanceOfType(splashScreen.ShowOn[0], typeof(SystemBootTriggerConfig));
            Assert.AreEqual(1, splashScreen.HideOn.Count);
            Assert.IsInstanceOfType(splashScreen.HideOn[0], typeof(ApplicationStateChangeTriggerConfig));
            var trigger = (ApplicationStateChangeTriggerConfig)splashScreen.HideOn[0];
            Assert.AreEqual("Infomedia", trigger.Application);
            Assert.AreEqual(ApplicationState.Running, trigger.State);
            Assert.AreEqual(3, splashScreen.Items.Count);
            var logoItem = splashScreen.Items[0] as LogoSplashScreenItem;
            Assert.IsNotNull(logoItem);
            var systemItem = splashScreen.Items[1] as SystemSplashScreenItem;
            Assert.IsNotNull(systemItem);
            Assert.IsTrue(systemItem.MachineName);
            Assert.IsFalse(systemItem.Ram);
            Assert.IsFalse(systemItem.Cpu);
            Assert.IsFalse(systemItem.Uptime);
            Assert.IsTrue(systemItem.Serial);
            var appsItem = splashScreen.Items[2] as ApplicationsSplashScreenItem;
            Assert.IsNotNull(appsItem);
            Assert.IsTrue(appsItem.Version);
            Assert.IsTrue(appsItem.State);
            Assert.IsFalse(appsItem.Ram);
            Assert.IsFalse(appsItem.Cpu);
            Assert.IsFalse(appsItem.Uptime);
            Assert.IsFalse(appsItem.LaunchReason);
            Assert.IsFalse(appsItem.ExitReason);
            Assert.AreEqual(1, appsItem.Visibility.Count);
            var hide = appsItem.Visibility[0] as ApplicationsSplashScreenHide;
            Assert.IsNotNull(hide);
            Assert.AreEqual("FileZilla", hide.Application);

            splashScreen = config.SplashScreens.Items[1];
            Assert.IsFalse(splashScreen.Enabled);
            Assert.AreEqual("Shutdown", splashScreen.Name);
            Assert.AreEqual("Red", splashScreen.Foreground);
            Assert.AreEqual("#000066", splashScreen.Background);
            Assert.AreEqual(1, splashScreen.ShowOn.Count);
            Assert.IsInstanceOfType(splashScreen.ShowOn[0], typeof(SystemShutdownTriggerConfig));
            Assert.IsNotNull(splashScreen.HideOn);
            Assert.AreEqual(0, splashScreen.HideOn.Count);
            Assert.AreEqual(1, splashScreen.Items.Count);
            logoItem = splashScreen.Items[0] as LogoSplashScreenItem;
            Assert.IsNotNull(logoItem);
            Assert.AreEqual("custom.jpg", logoItem.Filename);

            splashScreen = config.SplashScreens.Items[2];
            Assert.IsTrue(splashScreen.Enabled);
            Assert.AreEqual("Infomedia", splashScreen.Name);
            Assert.AreEqual("White", splashScreen.Foreground);
            Assert.AreEqual("Black", splashScreen.Background);
            Assert.AreEqual(4, splashScreen.ShowOn.Count);
            trigger = splashScreen.ShowOn[0] as ApplicationStateChangeTriggerConfig;
            Assert.IsNotNull(trigger);
            Assert.AreEqual("Infomedia", trigger.Application);
            Assert.AreEqual(ApplicationState.Exiting, trigger.State);
            trigger = splashScreen.ShowOn[1] as ApplicationStateChangeTriggerConfig;
            Assert.IsNotNull(trigger);
            Assert.AreEqual("Infomedia", trigger.Application);
            Assert.AreEqual(ApplicationState.AwaitingLaunch, trigger.State);
            trigger = splashScreen.ShowOn[2] as ApplicationStateChangeTriggerConfig;
            Assert.IsNotNull(trigger);
            Assert.AreEqual("Infomedia", trigger.Application);
            Assert.AreEqual(ApplicationState.Launching, trigger.State);
            trigger = splashScreen.ShowOn[3] as ApplicationStateChangeTriggerConfig;
            Assert.IsNotNull(trigger);
            Assert.AreEqual("Infomedia", trigger.Application);
            Assert.AreEqual(ApplicationState.Starting, trigger.State);
            Assert.AreEqual(1, splashScreen.HideOn.Count);
            trigger = splashScreen.HideOn[0] as ApplicationStateChangeTriggerConfig;
            Assert.IsNotNull(trigger);
            Assert.AreEqual("Infomedia", trigger.Application);
            Assert.AreEqual(ApplicationState.Running, trigger.State);
            Assert.AreEqual(2, splashScreen.Items.Count);
            systemItem = splashScreen.Items[0] as SystemSplashScreenItem;
            Assert.IsNotNull(systemItem);
            Assert.IsTrue(systemItem.MachineName);
            Assert.IsTrue(systemItem.Ram);
            Assert.IsTrue(systemItem.Cpu);
            Assert.IsFalse(systemItem.Uptime);
            Assert.IsFalse(systemItem.Serial);
            appsItem = splashScreen.Items[1] as ApplicationsSplashScreenItem;
            Assert.IsNotNull(appsItem);
            Assert.IsFalse(appsItem.Version);
            Assert.IsTrue(appsItem.State);
            Assert.IsTrue(appsItem.Ram);
            Assert.IsTrue(appsItem.Cpu);
            Assert.IsFalse(appsItem.Uptime);
            Assert.IsFalse(appsItem.LaunchReason);
            Assert.IsTrue(appsItem.ExitReason);
            Assert.AreEqual(1, appsItem.Visibility.Count);
            var show = appsItem.Visibility[0] as ApplicationsSplashScreenShow;
            Assert.IsNotNull(show);
            Assert.AreEqual("Infomedia", show.Application);

            splashScreen = config.SplashScreens.Items[3];
            Assert.IsTrue(splashScreen.Enabled);
            Assert.AreEqual("Button", splashScreen.Name);
            Assert.AreEqual("Black", splashScreen.Foreground);
            Assert.AreEqual("#E6ECF0", splashScreen.Background);
            Assert.AreEqual(1, splashScreen.ShowOn.Count);
            var input = splashScreen.ShowOn[0] as InputTriggerConfig;
            Assert.IsNotNull(input);
            Assert.AreEqual("*", input.Unit);
            Assert.IsNull(input.Application);
            Assert.AreEqual("Button", input.Name);
            Assert.AreEqual(1, input.Value);
            Assert.AreEqual(1, splashScreen.HideOn.Count);
            var timeout = splashScreen.HideOn[0] as TimeoutTriggerConfig;
            Assert.IsNotNull(timeout);
            Assert.AreEqual(TimeSpan.FromMinutes(1), timeout.Delay);
            Assert.AreEqual(3, splashScreen.Items.Count);
            logoItem = splashScreen.Items[0] as LogoSplashScreenItem;
            Assert.IsNotNull(logoItem);
            systemItem = splashScreen.Items[1] as SystemSplashScreenItem;
            Assert.IsNotNull(systemItem);
            Assert.IsTrue(systemItem.MachineName);
            Assert.IsFalse(systemItem.Ram);
            Assert.IsFalse(systemItem.Cpu);
            Assert.IsTrue(systemItem.Uptime);
            Assert.IsTrue(systemItem.Serial);
            var networkItem = splashScreen.Items[2] as NetworkSplashScreenItem;
            Assert.IsNotNull(networkItem);
            Assert.IsTrue(networkItem.Name);
            Assert.IsTrue(networkItem.Ip);
            Assert.IsFalse(networkItem.Gateway);
            Assert.IsTrue(networkItem.Mac);
            Assert.IsFalse(networkItem.Status);
            Assert.AreEqual("Up", networkItem.StatusFilter);

            Assert.IsNotNull(config.System);
            Assert.IsNotNull(config.System.RebootAt);
            Assert.AreEqual(new TimeSpan(2, 0, 0), config.System.RebootAt);
            Assert.IsNotNull(config.System.RebootAfter);
            Assert.AreEqual(new TimeSpan(23, 59, 0), config.System.RebootAfter);
            Assert.IsTrue(config.System.KickWatchdog);

            Assert.IsNotNull(config.System.CpuLimit);
            Assert.IsTrue(config.System.CpuLimit.Enabled);
            Assert.AreEqual(98, config.System.CpuLimit.MaxCpuPercentage);
            Assert.AreEqual(1, config.System.CpuLimit.Actions.Count);
            Assert.IsInstanceOfType(config.System.CpuLimit.Actions[0], typeof(RebootLimitActionConfig));

            Assert.IsNotNull(config.System.RamLimit);
            Assert.IsNotNull(config.System.RamLimit.FreeRamMb);
            Assert.AreEqual(150, config.System.RamLimit.FreeRamMb);
            Assert.IsNotNull(config.System.RamLimit.FreeRamPercentage);
            Assert.AreEqual(5, config.System.RamLimit.FreeRamPercentage);
            Assert.AreEqual(2, config.System.RamLimit.Actions.Count);
            var relaunch = config.System.RamLimit.Actions[0] as RelaunchLimitActionConfig;
            Assert.IsNotNull(relaunch);
            Assert.AreEqual("Infomedia", relaunch.Application);
            Assert.IsInstanceOfType(config.System.RamLimit.Actions[1], typeof(RebootLimitActionConfig));

            Assert.IsNotNull(config.System.DiskLimits);
            Assert.IsTrue(config.System.DiskLimits.Enabled);
            Assert.AreEqual(2, config.System.DiskLimits.Disks.Count);
            var disk = config.System.DiskLimits.Disks[0];
            Assert.AreEqual("C:\\", disk.Path);
            Assert.IsTrue(disk.Enabled);
            Assert.IsNotNull(disk.FreeSpaceMb);
            Assert.AreEqual(5, disk.FreeSpaceMb);
            Assert.IsNull(disk.FreeSpacePercentage);
            Assert.AreEqual(1, disk.Actions.Count);
            Assert.IsInstanceOfType(disk.Actions[0], typeof(RebootLimitActionConfig));
            disk = config.System.DiskLimits.Disks[1];
            Assert.AreEqual("D:\\", disk.Path);
            Assert.IsTrue(disk.Enabled);
            Assert.IsNotNull(disk.FreeSpaceMb);
            Assert.AreEqual(10, disk.FreeSpaceMb);
            Assert.IsNotNull(disk.FreeSpacePercentage);
            Assert.AreEqual(5, disk.FreeSpacePercentage);
            Assert.AreEqual(3, disk.Actions.Count);
            var purge = disk.Actions[0] as PurgeLimitActionConfig;
            Assert.IsNotNull(purge);
            Assert.AreEqual(@"D:\temp\", purge.Path);
            purge = disk.Actions[1] as PurgeLimitActionConfig;
            Assert.IsNotNull(purge);
            Assert.AreEqual(@"D:\logs\archive\", purge.Path);
            purge = disk.Actions[2] as PurgeLimitActionConfig;
            Assert.IsNotNull(purge);
            Assert.AreEqual(@"D:\logs\*.icl", purge.Path);

            Assert.IsNotNull(config.System.PreventPopups);
            Assert.IsTrue(config.System.PreventPopups.Enabled);

            Assert.IsNotNull(config.Applications);
            Assert.AreEqual(4, config.Applications.Count);
            var process = config.Applications[0] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("FileZilla", process.Name);
            Assert.IsTrue(process.Enabled);
            Assert.IsFalse(process.UseWatchdog);
            Assert.AreEqual(@"D:\Progs\FileZillaServer\FileZilla server.exe", process.ExecutablePath);
            Assert.AreEqual(@"D:\Progs\FileZillaServer\", process.WorkingDirectory);
            Assert.AreEqual("/compat /start", process.Arguments);
            Assert.IsNull(process.LaunchWaitFor);
            Assert.IsNotNull(process.LaunchDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(5), process.LaunchDelay);
            Assert.IsNull(process.RelaunchDelay);
            Assert.IsNotNull(process.WindowMode);
            Assert.AreEqual(ProcessWindowStyle.Minimized, process.WindowMode);
            Assert.IsNull(process.Priority);
            Assert.IsTrue(process.KillIfRunning);
            Assert.IsNull(process.RamLimit);
            Assert.IsNull(process.CpuLimit);

            process = config.Applications[1] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("MediServer", process.Name);
            Assert.IsTrue(process.Enabled);
            Assert.IsTrue(process.UseWatchdog);
            Assert.AreEqual(@"D:\Progs\MediServer\MediServer.exe", process.ExecutablePath);
            Assert.AreEqual(@"D:\Progs\MediServer\", process.WorkingDirectory);
            Assert.IsNull(process.Arguments);
            Assert.IsNull(process.LaunchWaitFor);
            Assert.IsNull(process.LaunchDelay);
            Assert.IsNotNull(process.RelaunchDelay);
            Assert.AreEqual(TimeSpan.FromDays(365 * 100), process.RelaunchDelay);
            Assert.IsNotNull(process.WindowMode);
            Assert.AreEqual(ProcessWindowStyle.Hidden, process.WindowMode);
            Assert.IsNull(process.Priority);
            Assert.IsFalse(process.KillIfRunning);
            Assert.IsNotNull(process.RamLimit);
            Assert.AreEqual(5, process.RamLimit.MaxRamMb);
            Assert.AreEqual(1, process.RamLimit.Actions.Count);
            relaunch = process.RamLimit.Actions[0] as RelaunchLimitActionConfig;
            Assert.IsNotNull(relaunch);
            Assert.IsNull(relaunch.Application);
            Assert.IsNull(process.CpuLimit);

            process = config.Applications[2] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("Protran", process.Name);
            Assert.IsTrue(process.Enabled);
            Assert.IsTrue(process.UseWatchdog);
            Assert.AreEqual(@"D:\Progs\Protran\Protran.exe", process.ExecutablePath);
            Assert.AreEqual(@"D:\Progs\Protran\", process.WorkingDirectory);
            Assert.IsNull(process.Arguments);
            Assert.IsNotNull(process.LaunchWaitFor);
            Assert.AreEqual("MediServer", process.LaunchWaitFor.Application);
            Assert.AreEqual(ApplicationState.Running, process.LaunchWaitFor.State);
            Assert.IsNull(process.LaunchDelay);
            Assert.IsNull(process.RelaunchDelay);
            Assert.IsNotNull(process.WindowMode);
            Assert.AreEqual(ProcessWindowStyle.Hidden, process.WindowMode);
            Assert.IsNotNull(process.Priority);
            Assert.AreEqual(ProcessPriorityClass.AboveNormal, process.Priority);
            Assert.IsTrue(process.KillIfRunning);
            Assert.IsNotNull(process.RamLimit);
            Assert.AreEqual(100, process.RamLimit.MaxRamMb);
            Assert.AreEqual(1, process.RamLimit.Actions.Count);
            Assert.IsInstanceOfType(process.RamLimit.Actions[0], typeof(RebootLimitActionConfig));
            Assert.IsNotNull(process.CpuLimit);
            Assert.AreEqual(60, process.CpuLimit.MaxCpuPercentage);
            Assert.AreEqual(3, process.CpuLimit.Actions.Count);
            relaunch = process.CpuLimit.Actions[0] as RelaunchLimitActionConfig;
            Assert.IsNotNull(relaunch);
            Assert.IsNull(relaunch.Application);
            relaunch = process.CpuLimit.Actions[1] as RelaunchLimitActionConfig;
            Assert.IsNotNull(relaunch);
            Assert.IsNull(relaunch.Application);
            Assert.IsInstanceOfType(process.CpuLimit.Actions[2], typeof(RebootLimitActionConfig));

            process = config.Applications[3] as ProcessConfig;
            Assert.IsNotNull(process);
            Assert.AreEqual("Infomedia", process.Name);
            Assert.IsTrue(process.Enabled);
            Assert.IsTrue(process.UseWatchdog);
            Assert.AreEqual(@"D:\Progs\InfoMedia\Infomedia.exe", process.ExecutablePath);
            Assert.IsNull(process.WorkingDirectory);
            Assert.IsNull(process.Arguments);
            Assert.IsNotNull(process.LaunchWaitFor);
            Assert.AreEqual("Protran", process.LaunchWaitFor.Application);
            Assert.AreEqual(ApplicationState.Running, process.LaunchWaitFor.State);
            Assert.IsNull(process.LaunchDelay);
            Assert.IsNotNull(process.RelaunchDelay);
            Assert.AreEqual(TimeSpan.FromSeconds(2), process.RelaunchDelay);
            Assert.IsNotNull(process.WindowMode);
            Assert.AreEqual(ProcessWindowStyle.Maximized, process.WindowMode);
            Assert.IsNull(process.Priority);
            Assert.IsTrue(process.KillIfRunning);
            Assert.IsNotNull(process.RamLimit);
            Assert.AreEqual(150, process.RamLimit.MaxRamMb);
            Assert.AreEqual(1, process.RamLimit.Actions.Count);
            relaunch = process.RamLimit.Actions[0] as RelaunchLimitActionConfig;
            Assert.IsNotNull(relaunch);
            Assert.IsNull(relaunch.Application);
            Assert.IsNotNull(process.CpuLimit);
            Assert.AreEqual(65, process.CpuLimit.MaxCpuPercentage);
            Assert.AreEqual(1, process.CpuLimit.Actions.Count);
            relaunch = process.CpuLimit.Actions[0] as RelaunchLimitActionConfig;
            Assert.IsNotNull(relaunch);
            Assert.IsNull(relaunch.Application);
        }
    }
}
