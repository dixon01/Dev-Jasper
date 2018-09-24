// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagementControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagementControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace Gorba.Common.SystemManagement.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.SystemManager.SplashScreen;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.SystemManagement.Core.Alarms;
    using Gorba.Common.SystemManagement.Core.Applications;
    using Gorba.Common.SystemManagement.Core.Persistence;
    using Gorba.Common.SystemManagement.Core.ResourceUsage;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The controller running all system manager activities.
    /// </summary>
    public abstract class SystemManagementControllerBase : IManageableObject
    {
        /// <summary>
        /// The name used in the Medi management tree.
        /// </summary>
        internal static readonly string ManagementName = "SystemManager";

        private const string RebootApplication = "shutdown.exe";
        private const string RebootArguments = "/r /t 2";

        private const string ShutdownApplication = "shutdown.exe";
        private const string ShutdownArguments = "/s /t 2";

        protected const string ShutDown = "ShutDown";

        private static readonly TimeSpan DefaultPersistenceValidity = TimeSpan.FromDays(30 * 365);

        private static readonly Logger Logger = LogHelper.GetLogger<SystemManagementControllerBase>();

        private readonly ConfigManager<SystemManagerConfig> configMgr;

        private readonly IPersistenceServiceImpl persistenceService;

        private readonly MessageHandler messageHandler;

        private readonly SystemResourcesObserver systemResourcesObserver;

        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private readonly IPersistenceContext<SystemManagerPersistence> persistenceContext;

        private readonly AlarmService alarmService;

        private readonly ITimer rebootAfterTimer;

        private readonly IDeadlineTimer rebootAtTimer;

        private bool running;

        private bool shouldExit;
        private bool shouldReboot;
        private bool shouldShutdown;

        private string rebootTimerReason;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManagementControllerBase"/> class.
        /// </summary>
        /// <param name="configFilePath">
        /// The full path to the config file.
        /// </param>
        protected SystemManagementControllerBase(string configFilePath)
        {
            this.configMgr = new ConfigManager<SystemManagerConfig>();
            this.configMgr.FileName = configFilePath;
            this.configMgr.EnableCaching = true;
            this.configMgr.XmlSchema = SystemManagerConfig.Schema;

            this.Config = this.configMgr.Config;
            this.ApplyConfigDefaults();

            this.persistenceService = (IPersistenceServiceImpl)
                ServiceLocator.Current.GetInstance<IPersistenceService>();
            this.persistenceService.Configure(
                PathManager.Instance.CreatePath(FileType.Data, "Persistence.xml"),
                DefaultPersistenceValidity,
                true);
            this.persistenceContext = this.persistenceService.GetContext<SystemManagerPersistence>();

            if (this.persistenceContext.Value == null || !this.persistenceContext.Valid)
            {
                this.persistenceContext.Value = new SystemManagerPersistence();
            }

            this.alarmService = new AlarmService();
            this.alarmService.Start();

            this.LogSystemBoot();

            this.messageHandler = new MessageHandler(this);
            this.ApplicationManager = new ApplicationManager(this.Config.Applications);
            var container = ServiceLocator.Current.GetInstance<IServiceContainer>();
            container.RegisterInstance(this);
            container.RegisterInstance(this.ApplicationManager);

            this.systemResourcesObserver = new SystemResourcesObserver(this.Config.System);

            this.rebootAfterTimer = TimerFactory.Current.CreateTimer("SystemRebootAfter");
            this.rebootAfterTimer.AutoReset = false;
            this.rebootAfterTimer.Elapsed += this.RebootTimerOnElapsed;

            this.rebootAtTimer = TimerFactory.Current.CreateDeadlineTimer("SystemRebootAt");
            this.rebootAtTimer.TriggerIfPassed = true;
            this.rebootAtTimer.Elapsed += this.RebootTimerOnElapsed;

            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(
                ManagementName,
                root,
                this);
            root.AddChild(provider);
        }

        /// <summary>
        /// Event that is risen when this controller is stopping.
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Gets the application manager.
        /// </summary>
        public ApplicationManager ApplicationManager { get; private set; }

        /// <summary>
        /// Gets the system resource information.
        /// </summary>
        public ISystemResourceInfo SystemResources
        {
            get
            {
                return this.systemResourcesObserver;
            }
        }

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        protected SystemManagerConfig Config { get; private set; }

        /// <summary>
        /// Runs this controller with the given options.
        /// </summary>
        /// <param name="context">
        /// The application context or null if none is available (i.e. this is a console application).
        /// </param>
        public void Run(ApplicationContext context)
        {
            if (this.running)
            {
                return;
            }

            this.runWait.Reset();

            this.running = true;
            Logger.Info("Starting System Manager");
            try
            {
                this.DoStart(context);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Couldn't start System Manager, stopping immediately");
                this.RequestReboot("Exception while starting System Manager");
            }

            this.runWait.WaitOne();

            if (this.shouldExit)
            {
                Logger.Info("System Manager exiting system for maintenance now");
            }
            else if (this.shouldShutdown)
            {
                Logger.Info("Shutting down system now");
                if (this.Config.System.EnableReboot)
                {
                    LogManager.Flush();
                    Process.Start(ShutdownApplication, ShutdownArguments);
                }
                else
                {
                    Logger.Warn("Shutdown disabled by configuration, System Manager will simply exit now");
                }
            }
            else if (this.shouldReboot)
            {
                Logger.Info("Rebooting system now");
                LogManager.Flush();
                if (this.Config.System.EnableReboot)
                {
                    Process.Start(RebootApplication, RebootArguments);
                }
                else
                {
                    Logger.Warn("Reboot disabled by configuration, System Manager will simply exit now");
                }
            }

            this.DoStop();
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        /// <param name="exitApplications">
        /// Flag to tell if all managed applications should be exited (true) or not (false).
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        public void Stop(bool exitApplications, ApplicationReason reason, string explanation)
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            Logger.Info("Stopping System Manager ({0}, '{1}')", reason, explanation);
            this.rebootAfterTimer.Enabled = false;
            this.rebootAtTimer.Enabled = false;
            this.systemResourcesObserver.Stop();

            this.RaiseStopping(EventArgs.Empty);
            this.ExecuteStop(exitApplications, reason, explanation);
        }

        /// <summary>
        /// Requests the system manager to close all applications and then reboot the system.
        /// </summary>
        /// <param name="reason">
        /// The reason for requesting the reboot.
        /// </param>
        public void RequestReboot(string reason)
        {
            this.shouldReboot = true;
            this.Stop(true, ApplicationReason.Requested, string.Format("Reboot: {0}", reason));
        }

        /// <summary>
        /// Requests the system manager to close all applications and then exit for maintenance.
        /// </summary>
        /// <param name="reason">
        /// The reason for requesting the exit.
        /// </param>
        public void RequestExit(string reason)
        {
            SplashScreenConfig shutdownSplashScreen = this.Config.SplashScreens.Items.FirstOrDefault(x => x.Name == ShutDown);
            if (shutdownSplashScreen != null)
            {
                var item = shutdownSplashScreen.Items.FirstOrDefault(x => x is ShutDownMessageSplashScreenItem);
                if (item != null)
                {
                    ((ShutDownMessageSplashScreenItem)item).ShutDownMessage = "The System will be shutting down for maintenance in..";
                }
            }

            this.shouldExit = true;
            this.Stop(true, ApplicationReason.Requested, string.Format("Exit: {0}", reason));
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Applications", parent, this.ApplicationManager);
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            const double MegaBytes = 1024 * 1024;

            yield return
                new ManagementProperty<string>(
                    "Overall CPU", string.Format("{0:0} %", this.SystemResources.CpuUsage * 100), true);

            var ramUsage = string.Format(
                "{0:0.00}MB / {1:0.00}MB",
                (this.SystemResources.TotalRam - this.SystemResources.AvailableRam) / MegaBytes,
                this.SystemResources.TotalRam / MegaBytes);

            yield return new ManagementProperty<string>("RAM Usage", ramUsage, true);
        }

        /// <summary>
        /// Implementation of starting the controller.
        /// </summary>
        /// <param name="context">
        /// The application context in which the controller is started.
        /// </param>
        protected virtual void DoStart(ApplicationContext context)
        {
            this.messageHandler.Start();
            this.ApplicationManager.Start();
            this.systemResourcesObserver.Start();

            this.StartRebootTimer(this.Config.System.RebootAt, this.Config.System.RebootAfter);
        }

        /// <summary>
        /// Implementation of stopping the controller.
        /// </summary>
        protected virtual void DoStop()
        {
        }

        /// <summary>
        /// Implementation of the stop of System Manager.
        /// </summary>
        /// <param name="exitApplications">
        /// Flag to tell if all managed applications should be exited (true) or not (false).
        /// </param>
        /// <param name="reason">
        /// The reason for stopping System Manager.
        /// </param>
        /// <param name="explanation">
        /// The explanation for stopping System Manager.
        /// </param>
        protected virtual void ExecuteStop(bool exitApplications, ApplicationReason reason, string explanation)
        {
            if (exitApplications)
            {
                this.ApplicationManager.Stop(reason, "System Manager exits: " + explanation);
            }
            else
            {
                this.ApplicationManager.Dispose();
            }

            this.messageHandler.Stop();

            this.persistenceContext.Value.Running = false;
            this.persistenceContext.Value.AddExitReason(new ApplicationReasonInfo(reason, explanation));
            this.persistenceContext.Revalidate();

            this.persistenceService.Save();

            this.alarmService.Stop();

            this.runWait.Set();
        }

        /// <summary>
        /// Shuts down the system.
        /// This will first stop all applications and then stop this controller before executing the shutdown command.
        /// </summary>
        /// <param name="reason">
        /// The reason for shutting down the system.
        /// </param>
        protected void Shutdown(string reason)
        {
            this.shouldShutdown = true;
            this.Stop(true, ApplicationReason.SystemShutdown, reason);
        }

        /// <summary>
        /// Raises the <see cref="Stopping"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStopping(EventArgs e)
        {
            var handler = this.Stopping;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void LogSystemBoot()
        {
            if (this.persistenceContext.Value.Running)
            {
                // we were not properly shut down, so the previous shutdown was a crash
                this.persistenceContext.Value.AddExitReason(
                    new ApplicationReasonInfo(ApplicationReason.SystemCrash, "Recovered after reboot"));
            }

            SystemRestartAttribute attribute;
            string bootReason;
            if (this.persistenceContext.Value.ExitReasons.Count == 0)
            {
                bootReason = "First boot";
                attribute = SystemRestartAttribute.Unknown;
            }
            else
            {
                var reason = this.persistenceContext.Value.ExitReasons[0];
                bootReason = string.Format("Reboot after {0}: {1}", reason.Reason, reason.Explanation);
                switch (reason.Reason)
                {
                    case ApplicationReason.Requested:
                        attribute = SystemRestartAttribute.User;
                        break;
                    case ApplicationReason.SystemCrash:
                        attribute = SystemRestartAttribute.PowerLoss;
                        break;
                    default:
                        attribute = SystemRestartAttribute.Unknown;
                        break;
                }
            }

            MessageDispatcher.Instance.Broadcast(SystemAlarmFactory.CreateRestart(attribute, bootReason));

            this.persistenceContext.Value.Running = true;
            this.persistenceContext.Value.AddLaunchReason(
                new ApplicationReasonInfo(ApplicationReason.SystemBoot, string.Empty));
            this.persistenceService.Save();
        }

        private void ApplyConfigDefaults()
        {
            foreach (var application in this.Config.Applications)
            {
                var process = application as ProcessConfig;
                if (process != null)
                {
                    this.ApplyConfigDefaults(process, this.Config.Defaults.Process);
                    continue;
                }

                var component = application as ComponentConfig;
                if (component != null)
                {
                    this.ApplyConfigDefaults(component, this.Config.Defaults.Component);
                }
            }

            foreach (var splashScreen in this.Config.SplashScreens.Items)
            {
                this.ApplyConfigDefaults(splashScreen);
            }
        }

        private void ApplyConfigDefaults(ProcessConfig processConfig, ProcessConfig defaultConfig)
        {
            processConfig.ExecutablePath = this.configMgr.GetAbsolutePathRelatedToConfig(processConfig.ExecutablePath);
            if (processConfig.WorkingDirectory == null && defaultConfig != null)
            {
                processConfig.WorkingDirectory = defaultConfig.WorkingDirectory;
            }

            if (processConfig.WorkingDirectory != null)
            {
                processConfig.WorkingDirectory =
                    this.configMgr.GetAbsolutePathRelatedToConfig(processConfig.WorkingDirectory);
            }

            if (defaultConfig == null)
            {
                return;
            }

            processConfig.CpuLimit = processConfig.CpuLimit ?? defaultConfig.CpuLimit;
            processConfig.RamLimit = processConfig.RamLimit ?? defaultConfig.RamLimit;
            processConfig.Priority = processConfig.Priority ?? defaultConfig.Priority;
            processConfig.WindowMode = processConfig.WindowMode ?? defaultConfig.WindowMode;

            if (!defaultConfig.KillIfRunning)
            {
                processConfig.KillIfRunning = false;
            }

            this.ApplyConfigDefaults((ApplicationConfigBase)processConfig, defaultConfig);
        }

        private void ApplyConfigDefaults(ComponentConfig componentConfig, ComponentConfig defaultConfig)
        {
            if (componentConfig.LibraryPath == null && defaultConfig != null)
            {
                componentConfig.LibraryPath = defaultConfig.LibraryPath;
            }

            componentConfig.LibraryPath = this.configMgr.GetAbsolutePathRelatedToConfig(componentConfig.LibraryPath);

            if (defaultConfig == null)
            {
                return;
            }

            componentConfig.ClassName = componentConfig.ClassName ?? defaultConfig.ClassName;

            if (defaultConfig.UseAppDomain)
            {
                componentConfig.UseAppDomain = true;
            }

            this.ApplyConfigDefaults((ApplicationConfigBase)componentConfig, defaultConfig);
        }

        private void ApplyConfigDefaults(ApplicationConfigBase applicationConfig, ApplicationConfigBase defaultConfig)
        {
            applicationConfig.LaunchDelay = applicationConfig.LaunchDelay ?? defaultConfig.LaunchDelay;
            applicationConfig.LaunchWaitFor = applicationConfig.LaunchWaitFor ?? defaultConfig.LaunchWaitFor;
            applicationConfig.RelaunchDelay = applicationConfig.RelaunchDelay ?? defaultConfig.RelaunchDelay;
        }

        private void ApplyConfigDefaults(SplashScreenConfig splashScreen)
        {
            foreach (var item in splashScreen.Items)
            {
                var logo = item as LogoSplashScreenItem;
                if (logo != null)
                {
                    if (!string.IsNullOrEmpty(logo.Filename))
                    {
                        logo.Filename = this.configMgr.GetAbsolutePathRelatedToConfig(logo.Filename);
                    }

                    continue;
                }
            }
        }

        private void StartRebootTimer(TimeSpan? rebootAtTime, TimeSpan? rebootAfter)
        {
            this.rebootAfterTimer.Enabled = false;
            this.rebootAtTimer.Enabled = false;

            if (rebootAtTime == null && rebootAfter == null)
            {
                Logger.Debug("Disabling reboot timer");
                return;
            }

            if (this.StartRebootAtTimer(rebootAtTime, rebootAfter) || rebootAfter == null)
            {
                return;
            }

            Logger.Debug("Setting reboot timer after {0}", rebootAfter);
            this.rebootTimerReason = "Requested after " + rebootAfter;
            this.rebootAfterTimer.Interval = rebootAfter.Value;
            this.rebootAfterTimer.Enabled = true;
        }

        private bool StartRebootAtTimer(TimeSpan? rebootAtTime, TimeSpan? rebootAfter)
        {
            if (rebootAtTime == null)
            {
                return false;
            }

            var now = TimeProvider.Current.Now;
            var rebootAt = now.Date + rebootAtTime.Value;
            if (rebootAtTime < now.TimeOfDay)
            {
                rebootAt += TimeSpan.FromDays(1);
            }

            if (rebootAfter != null && rebootAt > now + rebootAfter.Value)
            {
                return false;
            }

            Logger.Debug("Setting reboot timer at {0:s}", rebootAt);
            this.rebootTimerReason = "Requested at " + rebootAtTime;
            this.rebootAtTimer.UtcDeadline = rebootAt.ToUniversalTime();
            this.rebootAtTimer.Enabled = true;
            return true;
        }

        private void RebootTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.RequestReboot(this.rebootTimerReason);
        }
    }
}