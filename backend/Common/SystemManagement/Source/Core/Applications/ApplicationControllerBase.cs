// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Core.Persistence;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using ApplicationReasonInfo = Gorba.Common.SystemManagement.Core.Persistence.ApplicationReasonInfo;

    /// <summary>
    /// Base class for all application controllers.
    /// </summary>
    public abstract class ApplicationControllerBase : IDisposable, IManageableObject
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private const int StartupAliveFailCount = 20;
        private const int AliveFailCount = 6;

        private static readonly TimeSpan AliveTimeout = TimeSpan.FromSeconds(10);

        private static readonly TimeSpan MinimumLaunchDelay = TimeSpan.FromMilliseconds(100);

        private static int instanceCounter;

        private readonly ApplicationConfigBase config;

        private readonly ApplicationManager manager;

        private readonly ITimer launchTimer;
        private readonly ITimer aliveTimer;

        private readonly ManualResetEvent exitedWait = new ManualResetEvent(true);

        private readonly int instanceId;

        private bool controllerRunning;

        private bool shouldRelaunch;

        private ApplicationState state;

        private string version;

        private DateTime startTimeUtc = DateTime.MinValue;

        private volatile bool aliveReceived;

        private int startupAliveMissedCount;
        private int aliveMissedCount;

        private IPersistenceContext<ApplicationPersistence> persistenceContext;

        private ApplicationReasonInfo nextLaunchReason;

        private ApplicationReasonInfo nextExitReason;

        private ApplicationRelaunchAttribute nextReaunchAttribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationControllerBase"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="manager">
        /// The manager creating this controller.
        /// </param>
        protected ApplicationControllerBase(ApplicationConfigBase config, ApplicationManager manager)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName + "-" + config.Name.Replace('.', '_'));

            this.config = config;
            this.manager = manager;

            this.instanceId = Interlocked.Increment(ref instanceCounter);

            this.MessageHandler = new ApplicationMessageHandler(this);
            this.MessageHandler.AliveResponseReceived += this.MessageHandlerOnAliveResponseReceived;

            this.launchTimer = TimerFactory.Current.CreateTimer("Launch-" + config.Name);
            this.launchTimer.AutoReset = false;
            this.launchTimer.Elapsed += this.LaunchTimerOnElapsed;

            this.aliveTimer = TimerFactory.Current.CreateTimer("Watchdog-" + config.Name);
            this.aliveTimer.AutoReset = true;
            this.aliveTimer.Interval = AliveTimeout;
            this.aliveTimer.Elapsed += this.AliveTimerOnElapsed;

            this.nextReaunchAttribute = ApplicationRelaunchAttribute.Unknown;

            this.SetupPersistence();
        }

        /// <summary>
        /// Event that is fired whenever <see cref="State"/> changes.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Gets the name of this controller.
        /// </summary>
        public string Name
        {
            get
            {
                return this.config.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this controller is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.config.Enabled;
            }
        }

        /// <summary>
        /// Gets or sets the state.
        /// Setting this value will trigger <see cref="StateChanged"/>.
        /// </summary>
        public ApplicationState State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.SetState(value, ApplicationReason.Unknown, string.Empty);
            }
        }

        /// <summary>
        /// Gets the unique ID of this application.
        /// </summary>
        public string ApplicationId
        {
            get
            {
                return string.Format("{0}<{1}>", this.GetType().Name[0], this.instanceId);
            }
        }

        /// <summary>
        /// Gets the message handler.
        /// </summary>
        protected ApplicationMessageHandler MessageHandler { get; private set; }

        /// <summary>
        /// Gets the main file path.
        /// </summary>
        protected abstract string FilePath { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationControllerBase"/> depending on the type of config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="manager">
        /// The manager creating the controller.
        /// </param>
        /// <returns>
        /// The <see cref="ApplicationControllerBase"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// if the config type is not supported.
        /// </exception>
        public static ApplicationControllerBase Create(ApplicationConfigBase config, ApplicationManager manager)
        {
            var process = config as ProcessConfig;
            if (process != null)
            {
                return new ProcessApplicationController(process, manager);
            }

            var component = config as ComponentConfig;
            if (component != null)
            {
                return new ComponentApplicationController(component, manager);
            }

            throw new NotSupportedException("Application type not supported " + config.GetType());
        }

        /// <summary>
        /// Starts this controller.
        /// </summary>
        public void Start()
        {
            if (this.controllerRunning)
            {
                return;
            }

            this.DoStart();
            this.controllerRunning = true;
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        public void Stop(ApplicationReason reason, string explanation)
        {
            this.Close(true, reason, explanation);
        }

        /// <summary>
        /// Waits until this controller is fully stopped (or until the timeout is reached).
        /// </summary>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool WaitForStopped(TimeSpan timeout)
        {
            return this.exitedWait.WaitOne((int)timeout.TotalMilliseconds, false);
        }

        /// <summary>
        /// The create application info.
        /// </summary>
        /// <returns>
        /// The <see cref="ApplicationInfo"/>.
        /// </returns>
        public virtual ApplicationInfo CreateApplicationInfo()
        {
            var info = new ApplicationInfo(this.ApplicationId);
            info.Name = this.Name;
            info.Version = this.version;
            info.Path = this.FilePath;
            info.State = this.State;
            info.StartTimeUtc = this.startTimeUtc;

            if (this.persistenceContext.Value.LaunchReasons.Count > 0)
            {
                info.LastLaunchReason = ConvertReason(this.persistenceContext.Value.LaunchReasons[0]);
            }

            if (this.persistenceContext.Value.ExitReasons.Count > 0)
            {
                info.LastExitReason = ConvertReason(this.persistenceContext.Value.ExitReasons[0]);
            }

            return info;
        }

        /// <summary>
        /// Requests a re-launch of this application.
        /// </summary>
        /// <param name="relaunchAttribute">
        /// The re-launch attribute used for alarming.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void RequestRelaunch(ApplicationRelaunchAttribute relaunchAttribute, string reason)
        {
            this.nextReaunchAttribute = relaunchAttribute;
            if (this.State == ApplicationState.Exited)
            {
                this.State = ApplicationState.AwaitingLaunch;
                this.StartLaunchTimer(this.config.LaunchDelay, ApplicationReason.Requested, reason);
            }
            else
            {
                this.Exit(ApplicationReason.ApplicationRelaunch, reason);
            }
        }

        /// <summary>
        /// Requests this application to exit.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void RequestExit(string reason)
        {
            this.shouldRelaunch = false;
            this.Exit(ApplicationReason.ApplicationExit, reason);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Close(false, ApplicationReason.Unknown, null);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Reasons", parent, new ManageableReasons(this));
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            const double MegaBytes = 1024 * 1024;

            var info = this.CreateApplicationInfo();
            var ramUsage = string.Format("{0:0.00}MB", info.RamBytes / MegaBytes);
            var cpuUsage = string.Format("{0:0} %", info.CpuUsage * 100);
            var uptime = TimeProvider.Current.UtcNow - this.startTimeUtc;
            var uptimeStr = string.Format("{0:##00}:{1:00}:{2:00}", uptime.TotalHours, uptime.Minutes, uptime.Seconds);

            yield return new ManagementProperty<string>("Name", info.Name, true);
            yield return new ManagementProperty<string>("Version", info.Version, true);
            yield return new ManagementProperty<string>("Path", info.Path, true);
            yield return new ManagementProperty<string>("State", info.State.ToString(), true);
            yield return new ManagementProperty<string>("Uptime", uptimeStr, true);
            yield return new ManagementProperty<string>("RAM Usage", ramUsage, true);
            yield return new ManagementProperty<string>("CPU Usage", cpuUsage, true);
            yield return new ManagementProperty<string>("Window State", info.WindowState.ToString(), true);
            yield return new ManagementProperty<bool>("Has Focus", info.HasFocus, true);
        }

        /// <summary>
        /// Sets the <see cref="State"/> providing a reason for the change.
        /// This method should always be used when setting the state to <see cref="ApplicationState.Exited"/>.
        /// </summary>
        /// <param name="newState">
        /// The new state.
        /// </param>
        /// <param name="reason">
        /// The reason for the change.
        /// </param>
        /// <param name="explanation">
        /// An explanation for the change.
        /// </param>
        protected void SetState(ApplicationState newState, ApplicationReason reason, string explanation)
        {
            if (this.state == newState)
            {
                return;
            }

            if (newState == ApplicationState.Exited && !this.HandleExited(reason, explanation))
            {
                return;
            }

            this.Logger.Debug("State changed {0}-->{1} ({2}: {3})", this.state, newState, reason, explanation);
            this.state = newState;
            this.RaiseStateChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="StateChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event argument.
        /// </param>
        protected virtual void RaiseStateChanged(EventArgs e)
        {
            var handler = this.StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Implementation of starting the controller.
        /// </summary>
        protected virtual void DoStart()
        {
            this.UpdateVersion();
            this.State = ApplicationState.AwaitingLaunch;
            this.nextReaunchAttribute = ApplicationRelaunchAttribute.SystemBoot;

            if (this.config.LaunchWaitFor != null)
            {
                var controller = this.manager.GetController(this.config.LaunchWaitFor.Application);
                if (controller != null)
                {
                    if (controller.State != this.config.LaunchWaitFor.State)
                    {
                        controller.StateChanged += this.DependentControllerOnStateChanged;
                        return;
                    }
                }
            }

            this.StartLaunchTimer(this.config.LaunchDelay, ApplicationReason.SystemBoot, string.Empty);
        }

        /// <summary>
        /// Exits the application controlled by this controller.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        protected void Exit(ApplicationReason reason, string explanation)
        {
            this.Logger.Debug("Exiting ({0}, '{1}')", reason, explanation);
            this.nextExitReason = new ApplicationReasonInfo(reason, explanation);
            this.launchTimer.Enabled = false;
            this.aliveTimer.Enabled = false;
            this.DoExit(reason, explanation);
        }

        /// <summary>
        /// Implementation of the launch of the application.
        /// Subclasses must make sure the <see cref="State"/> is changed according to
        /// the state of the application.
        /// </summary>
        protected abstract void DoLaunch();

        /// <summary>
        /// Implementation of the exit of the application.
        /// Subclasses must make sure the <see cref="State"/> is changed according to
        /// the state of the application.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation.
        /// </param>
        protected abstract void DoExit(ApplicationReason reason, string explanation);

        private static Client.ApplicationReasonInfo ConvertReason(
            ApplicationReasonInfo reasonInfo)
        {
            return new Client.ApplicationReasonInfo(
                reasonInfo.Reason, reasonInfo.Explanation, reasonInfo.TimestampUtc);
        }

        private void SetupPersistence()
        {
            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            this.persistenceContext = persistenceService.GetContext<ApplicationPersistence>(this.Name);
            if (this.persistenceContext.Value == null || !this.persistenceContext.Valid)
            {
                this.persistenceContext.Value = new ApplicationPersistence { Name = this.Name };
            }
        }

        private void DependentControllerOnStateChanged(object sender, EventArgs e)
        {
            var controller = (ApplicationControllerBase)sender;
            if (controller.State != this.config.LaunchWaitFor.State)
            {
                return;
            }

            controller.StateChanged -= this.DependentControllerOnStateChanged;
            this.StartLaunchTimer(
                this.config.LaunchDelay,
                ApplicationReason.SystemBoot,
                string.Format("{0} is in state {1}", controller.Name, controller.State));
        }

        private void LaunchTimerOnElapsed(object s, EventArgs e)
        {
            this.Logger.Debug("Launching");
            this.exitedWait.Reset();
            this.UpdateVersion();
            this.startTimeUtc = TimeProvider.Current.UtcNow;
            this.shouldRelaunch = true;
            this.DoLaunch();

            if (this.config.UseWatchdog)
            {
                this.aliveReceived = true;
                this.aliveMissedCount = 0;
                this.startupAliveMissedCount = 0;
                this.aliveTimer.Enabled = true;
            }

            var reason = this.nextLaunchReason;
            var attribute = this.nextReaunchAttribute;
            this.nextReaunchAttribute = ApplicationRelaunchAttribute.Unknown;
            this.nextLaunchReason = null;
            if (reason == null)
            {
                reason = new ApplicationReasonInfo(ApplicationReason.Unknown, string.Empty);
            }

            var message = string.Format("{0}: {1}", this.Name, reason.Explanation);
            MessageDispatcher.Instance.Broadcast(ApplicationAlarmFactory.CreateRelaunch(attribute, message));

            this.persistenceContext.Value.AddLaunchReason(reason);
            this.persistenceContext.Revalidate();
        }

        private void AliveTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            if (!this.aliveReceived)
            {
                this.aliveMissedCount++;
                this.Logger.Warn(
                    "Didn't receive alive response in time ({0}/{1})", this.aliveMissedCount, AliveFailCount);
                if (this.aliveMissedCount >= AliveFailCount)
                {
                    this.RequestRelaunch(ApplicationRelaunchAttribute.Watchdog, "Watchdog timeout");
                    return;
                }
            }
            else
            {
                this.aliveMissedCount = 0;
            }

            this.aliveReceived = false;

            if (this.MessageHandler.SendAliveRequest())
            {
                this.startupAliveMissedCount = 0;
                return;
            }

            // we couldn't send the alive request, meaning we didn't get the registration yet
            this.startupAliveMissedCount++;
            this.Logger.Warn(
                "Couldn't send alive request ({0}/{1})", this.startupAliveMissedCount, StartupAliveFailCount);
            this.aliveReceived = true;
            if (this.startupAliveMissedCount >= StartupAliveFailCount)
            {
                this.RequestRelaunch(ApplicationRelaunchAttribute.Watchdog, "Initial watchdog timeout");
            }
        }

        private void MessageHandlerOnAliveResponseReceived(object sender, EventArgs eventArgs)
        {
            this.aliveReceived = true;
        }

        private void StartLaunchTimer(TimeSpan? delay, ApplicationReason reason, string explanation)
        {
            if (delay == null || delay.Value <= MinimumLaunchDelay)
            {
                delay = MinimumLaunchDelay;
            }

            this.Logger.Debug("Launching in {0} ({1}, '{2}')", delay.Value, reason, explanation);
            this.nextLaunchReason = new ApplicationReasonInfo(reason, explanation);
            this.launchTimer.Interval = delay.Value;
            this.launchTimer.Enabled = true;
        }

        private bool HandleExited(ApplicationReason reason, string explanation)
        {
            var exitReason = this.nextExitReason ?? new ApplicationReasonInfo(reason, explanation);
            this.nextExitReason = null;

            this.Logger.Debug("Exited ({0}, '{1}')", exitReason.Reason, exitReason.Explanation);

            this.persistenceContext.Value.AddExitReason(exitReason);
            this.persistenceContext.Revalidate();

            this.aliveTimer.Enabled = false;
            this.startTimeUtc = DateTime.MinValue;
            this.exitedWait.Set();
            if (!this.controllerRunning || !this.shouldRelaunch)
            {
                return true;
            }

            this.Logger.Debug("Relaunching after exit");
            this.State = ApplicationState.AwaitingLaunch;
            this.StartLaunchTimer(
                this.config.RelaunchDelay,
                ApplicationReason.ApplicationRelaunch,
                "Application Exited: " + exitReason.Explanation);
            return false;
        }

        private void UpdateVersion()
        {
            try
            {
                this.version = ApplicationHelper.GetFileVersion(this.FilePath);
            }
            catch (Exception ex)
            {
                this.version = "unknown";
                this.Logger.Warn(ex, "Couldn't get version number for {0}", this.FilePath);
            }
        }

        private void Close(bool exitApplication, ApplicationReason reason, string explanation)
        {
            if (!this.controllerRunning)
            {
                return;
            }

            this.controllerRunning = false;
            this.launchTimer.Dispose();
            this.aliveTimer.Dispose();
            if (exitApplication)
            {
                this.shouldRelaunch = false;
                this.Exit(reason, explanation);
            }

            this.MessageHandler.Dispose();
        }

        private class ManageableReasons : IManageableTable
        {
            private readonly ApplicationControllerBase owner;

            public ManageableReasons(ApplicationControllerBase owner)
            {
                this.owner = owner;
            }

            public IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
            {
                yield break;
            }

            public IEnumerable<List<ManagementProperty>> GetRows()
            {
                var reasons = new List<KeyValuePair<bool, ApplicationReasonInfo>>();
                reasons.AddRange(this.owner.persistenceContext.Value.LaunchReasons.ConvertAll(
                    r => new KeyValuePair<bool, ApplicationReasonInfo>(true, r)));
                reasons.AddRange(this.owner.persistenceContext.Value.ExitReasons.ConvertAll(
                    r => new KeyValuePair<bool, ApplicationReasonInfo>(false, r)));
                reasons.Sort((a, b) => b.Value.TimestampUtc.CompareTo(a.Value.TimestampUtc));

                foreach (var reason in reasons)
                {
                    var localTime = reason.Value.TimestampUtc.ToLocalTime();
                    var rows = new List<ManagementProperty>
                                   {
                                       new ManagementProperty<string>(
                                           "Time", localTime.ToString("yyyy-MM-dd HH:mm:ss"), true),
                                       new ManagementProperty<string>("Type", reason.Key ? "Launch" : "Exit", true),
                                       new ManagementProperty<string>("Reason", reason.Value.Reason.ToString(), true),
                                       new ManagementProperty<string>("Explanation", reason.Value.Explanation, true)
                                   };
                    yield return rows;
                }
            }
        }
    }
}