// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisIpServiceController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisIpServiceController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Vdv301
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.HardwareManager.Vdv301;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.IbisIP.Discovery;
    using Gorba.Motion.HardwareManager.Core.Common;

    using NLog;

    /// <summary>
    /// The IBIS-IP (VDV 301) service controller.
    /// </summary>
    public class IbisIpServiceController
    {
        private static readonly Logger Logger = LogHelper.GetLogger<IbisIpServiceController>();

        private readonly List<IApplicationStateObserver> waitingObservers = new List<IApplicationStateObserver>();

        private readonly Vdv301Config config;

        private readonly SystemTimeOutput systemTime;

        private readonly ITimer autoRegisterTimer;

        private DnsSdIbisServiceLocator serviceLocator;

        private DeviceManagementService deviceManagementService;

        private IServiceQuery<ITimeService> timeServiceQuery;

        private IbisIpTimeSyncController timeSync;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisIpServiceController"/> class.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        /// <param name="systemTime">
        /// The system time port to be used to update the time.
        /// </param>
        public IbisIpServiceController(Vdv301Config config, SystemTimeOutput systemTime)
        {
            this.config = config;
            this.systemTime = systemTime;

            this.autoRegisterTimer = TimerFactory.Current.CreateTimer("DeviceManagementService-AutoRegister");
            this.autoRegisterTimer.AutoReset = false;
            this.autoRegisterTimer.Interval = TimeSpan.FromSeconds(20);
            this.autoRegisterTimer.Elapsed += this.AutoRegisterTimerOnElapsed;
        }

        /// <summary>
        /// Starts this controller.
        /// </summary>
        public void Start()
        {
            if (this.serviceLocator != null)
            {
                return;
            }

            Logger.Info("Starting");

            this.serviceLocator = new DnsSdIbisServiceLocator();
            this.serviceLocator.Configure(
                this.config.ValidateHttpRequests, this.config.ValidateHttpResponses, this.config.VerifyVersion);

            if (this.config.TimeSync.Enabled)
            {
                this.timeServiceQuery = this.serviceLocator.QueryServices<ITimeService>();
                this.timeServiceQuery.ServicesChanged += this.TimeServiceQueryOnServicesChanged;
                this.timeServiceQuery.Start();
            }

            this.autoRegisterTimer.Enabled = true;
            SystemManagerClient.Instance.BeginGetApplicationInfos(this.GotApplicationInfos, null);
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        public void Stop()
        {
            if (this.serviceLocator == null)
            {
                return;
            }

            Logger.Info("Stopping");

            this.autoRegisterTimer.Dispose();

            if (this.timeServiceQuery != null)
            {
                this.timeServiceQuery.Stop();
                this.timeServiceQuery.ServicesChanged -= this.TimeServiceQueryOnServicesChanged;
            }

            if (this.timeSync != null)
            {
                this.timeSync.Stop();
            }

            lock (this.waitingObservers)
            {
                foreach (var observer in this.waitingObservers)
                {
                    observer.Dispose();
                }

                this.waitingObservers.Clear();
            }

            if (this.deviceManagementService != null)
            {
                this.deviceManagementService.Dispose();
                this.deviceManagementService = null;
            }

            this.serviceLocator.Dispose();
            this.serviceLocator = null;
        }

        private static bool IsReadyState(ApplicationState state)
        {
            return state != ApplicationState.AwaitingLaunch
                   && state != ApplicationState.Launching
                   && state != ApplicationState.Starting;
        }

        private void GotApplicationInfos(IAsyncResult ar)
        {
            this.autoRegisterTimer.Enabled = false;
            var apps = SystemManagerClient.Instance.EndGetApplicationInfos(ar);

            if (this.serviceLocator == null)
            {
                // ignore if we were already stopped again
                return;
            }

            foreach (var app in apps)
            {
                if (!IsReadyState(app.State))
                {
                    var observer = SystemManagerClient.Instance.CreateApplicationStateObserver(app);
                    observer.StateChanged += this.ObserverOnStateChanged;

                    lock (this.waitingObservers)
                    {
                        if (!this.CheckObserver(observer))
                        {
                            this.waitingObservers.Add(observer);
                        }
                    }
                }
            }

            lock (this.waitingObservers)
            {
                if (this.waitingObservers.Count != 0)
                {
                    foreach (var waitingObserver in this.waitingObservers)
                    {
                        Logger.Trace("Waiting for {0}", waitingObserver.ApplicationName);
                    }

                    return;
                }
            }

            // all applications are already running, let's start the device service
            this.StartAutoStartServices();
        }

        private void StartAutoStartServices()
        {
            lock (this.waitingObservers)
            {
                if (this.deviceManagementService != null)
                {
                    return;
                }

                this.deviceManagementService = new DeviceManagementService(this.config);
            }

            Logger.Debug("Registering DeviceManagementService");
            this.serviceLocator.RegisterService<IDeviceManagementService>(this.deviceManagementService);

            foreach (var service in this.config.AutoStartServices)
            {
                Logger.Debug("Registering {0}", service);

                switch (service)
                {
                    // TODO: also start other auto-start services here
                    default:
                        Logger.Warn("{0} is not available in Hardware Manager", service);
                        break;
                }
            }
        }

        private bool CheckObserver(IApplicationStateObserver observer)
        {
            if (!IsReadyState(observer.State))
            {
                return false;
            }

            observer.StateChanged -= this.ObserverOnStateChanged;
            observer.Dispose();

            return true;
        }

        private void ObserverOnStateChanged(object sender, EventArgs eventArgs)
        {
            var observer = (IApplicationStateObserver)sender;
            if (!this.CheckObserver(observer))
            {
                return;
            }

            lock (this.waitingObservers)
            {
                this.waitingObservers.Remove(observer);

                if (this.waitingObservers.Count > 0)
                {
                    foreach (var waitingObserver in this.waitingObservers)
                    {
                        Logger.Trace("Still waiting for {0}", waitingObserver.ApplicationName);
                    }

                    return;
                }
            }

            // finally all applications are already running, let's start the device service
            this.StartAutoStartServices();
        }

        private void AutoRegisterTimerOnElapsed(object s, EventArgs e)
        {
            Logger.Warn("Didn't get application information from System Manager, starting services anyways");
            this.StartAutoStartServices();
        }

        private void TimeServiceQueryOnServicesChanged(object sender, EventArgs eventArgs)
        {
            lock (this)
            {
                var services = this.timeServiceQuery.Services;
                if (this.timeSync != null)
                {
                    this.timeSync.Stop();
                    this.timeSync = null;
                }

                if (services.Length == 0)
                {
                    return;
                }

                var service = services[0];
                this.timeSync = new IbisIpTimeSyncController(
                    service.IPAddress, service.Port, this.config.TimeSync, this.systemTime);
                this.timeSync.Start();
            }
        }
    }
}
