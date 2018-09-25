// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareManagementController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareManagementController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.HardwareManager.Core.Common;
    using Gorba.Motion.HardwareManager.Core.Gps;
    using Gorba.Motion.HardwareManager.Core.Settings;
    using Gorba.Motion.HardwareManager.Core.TimeSync;
    using Gorba.Motion.HardwareManager.Core.Vdv301;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The hardware management controller is the main executing class for Hardware Manager.
    /// </summary>
    public class HardwareManagementController : IDisposable, IManageableObject
    {
        private static readonly Logger Logger = LogHelper.GetLogger<HardwareManagementController>();

        private readonly SystemTimeOutput systemTime;

        private readonly IPort systemVolume;

        private readonly List<IHardwareHandler> hardwareHandlers = new List<IHardwareHandler>();

        private readonly HardwareManagerConfig config;

        private readonly SettingsHandler settingsHandler;

        private readonly TemperatureObserver temperatureObserver;

        private readonly UdcpHandler udcpHandler;

        private readonly SntpTimeSyncController timeSyncController;

        private readonly IbisIpServiceController ibisIpController;

        private readonly GpsClientBase gpsClient;

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareManagementController"/> class.
        /// </summary>
        public HardwareManagementController()
        {
            var configManager = new ConfigManager<HardwareManagerConfig>
                {
                    FileName = PathManager.Instance.GetPath(FileType.Config, "HardwareManager.xml"),
                    EnableCaching = true,
                    XmlSchema = HardwareManagerConfig.Schema
                };

            this.config = configManager.Config;

            this.systemTime = new SystemTimeOutput(this.config.BroadcastTimeChanges);
            this.systemVolume = new VolumeInputOutput();

            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(
                "HardwareManager", root, this);
            root.AddChild(provider);

            this.settingsHandler = new SettingsHandler(this.config.Settings);

            this.temperatureObserver = new TemperatureObserver();

            this.udcpHandler = new UdcpHandler();

            var factory = new HardwareHandlerFactory();
            var handler = factory.CreateHardwareHandler(this.config);
            Logger.Info("Creating default Handlers....");
            if (handler != null)
            {
                ServiceLocator.Current.GetInstance<IServiceContainer>().RegisterInstance(handler);
                this.hardwareHandlers.Add(handler);
            }

            if (this.config.Sntp != null && this.config.Sntp.Enabled)
            {
                this.timeSyncController = new SntpTimeSyncController(this.config.Sntp, this.systemTime);
            }
            else
            {
                Logger.Info("Sntp Disabled");
            }

            if (this.config.Vdv301 != null && this.config.Vdv301.Enabled)
            {
                this.ibisIpController = new IbisIpServiceController(this.config.Vdv301, this.systemTime);
            }
            else
            {
                Logger.Info("Vdv301 Disabled");
            }

            if (this.config.Gps != null)
            {
                this.gpsClient = GpsClientBase.Create(this.config.Gps);
            }
            else
            {
                Logger.Info("Gps Disabled");
            }
        }

        /// <summary>
        /// Starts this controller.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            Logger.Trace("Starting");

            this.settingsHandler.Start();
            this.temperatureObserver.Start();
            try
            {
                this.udcpHandler.Start();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not start UDCP Handler");
            }

            GioomClient.Instance.RegisterPort(this.systemTime);
            GioomClient.Instance.RegisterPort(this.systemVolume);
            foreach (var hardwareHandler in this.hardwareHandlers)
            {
                hardwareHandler.Start();
            }

            if (this.timeSyncController != null)
            {
                this.timeSyncController.Start();
            }

            if (this.ibisIpController != null)
            {
                this.ibisIpController.Start();
            }

            if (this.gpsClient != null)
            {
                this.gpsClient.Start();
            }

            Logger.Debug("Started");
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            Logger.Trace("Stopping");

            this.temperatureObserver.Stop();
            this.settingsHandler.Stop();
            this.udcpHandler.Stop();

            GioomClient.Instance.DeregisterPort(this.systemTime);
            GioomClient.Instance.DeregisterPort(this.systemVolume);
            foreach (var hardwareHandler in this.hardwareHandlers)
            {
                hardwareHandler.Stop();
            }

            if (this.timeSyncController != null)
            {
                this.timeSyncController.Stop();
            }

            if (this.ibisIpController != null)
            {
                this.ibisIpController.Stop();
            }

            if (this.gpsClient != null)
            {
                this.gpsClient.Stop();
            }

            Logger.Debug("Stopped");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Stop();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var handler in this.hardwareHandlers)
            {
                yield return parent.Factory.CreateManagementProvider(handler.Name, parent, handler);
            }

            yield return parent.Factory.CreateManagementProvider("Temperature", parent, this.temperatureObserver);
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<int>("Volume", this.systemVolume.IntegerValue, true);
        }
    }
}