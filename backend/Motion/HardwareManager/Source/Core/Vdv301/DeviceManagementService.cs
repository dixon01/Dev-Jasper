// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceManagementService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeviceManagementService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Vdv301
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Gorba.Common.Configuration.HardwareManager.Vdv301;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.IbisIP;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The device management service for this computer.
    /// </summary>
    internal class DeviceManagementService : DeviceManagementServiceBase, IVdv301ServiceImpl, IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<DeviceManagementService>();

        private static readonly Regex NmTokenCleanup = new Regex(@"[^0-9a-z.-_:]+", RegexOptions.IgnoreCase);

        private readonly Vdv301Config config;

        private readonly List<ApplicationInfo> apps = new List<ApplicationInfo>();

        private readonly string serialNumber;

        private readonly IPersistenceContext<DeviceManagementServicePersistence> persistenceContext;

        private readonly List<MessageStructure> errorMessages = new List<MessageStructure>();

        private DeviceStateEnumeration deviceState = DeviceStateEnumeration.running;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceManagementService"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public DeviceManagementService(Vdv301Config config)
        {
            this.config = config;
            SystemManagerClient.Instance.BeginGetApplicationInfos(this.GotApplicationInfos, null);

            MessageDispatcher.Instance.Subscribe<Alarm>(this.HandleAlarm);

            try
            {
                var hardwareHandler = ServiceLocator.Current.GetInstance<IHardwareHandler>();
                if (hardwareHandler != null)
                {
                    this.serialNumber = hardwareHandler.SerialNumber;
                    if (this.serialNumber != null)
                    {
                        this.serialNumber = NmTokenCleanup.Replace(this.serialNumber, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Couldn't get hardware handler");
            }

            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            this.persistenceContext = persistenceService.GetContext<DeviceManagementServicePersistence>();
            if (this.persistenceContext.Value == null || !this.persistenceContext.Valid)
            {
                this.persistenceContext.Value = new DeviceManagementServicePersistence();
            }
        }

        /// <summary>
        /// Gets the device information.
        /// </summary>
        /// <returns>
        /// The device information.
        /// </returns>
        public override DeviceManagementServiceGetDeviceInformationResponseDataStructure GetDeviceInformation()
        {
            var data = new DeviceManagementServiceGetDeviceInformationResponseDataStructure
                {
                    TimeStamp = CreateTimeStamp(),
                    DeviceInformation = new DeviceInformationStructure
                        {
                            DeviceName = new IBISIPstring(ApplicationHelper.MachineName),
                            Manufacturer = new IBISIPstring("Gorba AG"),
                            SerialNumber = new IBISIPNMTOKEN
                                               {
                                                   ErrorCode = ErrorCodeEnumeration.DataNotValid,
                                                   ErrorCodeSpecified = this.serialNumber == null,
                                                   Value = this.serialNumber
                                               },
                            DeviceClass = this.GetDeviceClass()
                        }
                };

            var versions = new List<DataVersionStructure>(this.apps.Count);
            foreach (var applicationInfo in this.apps)
            {
                versions.Add(new DataVersionStructure
                                 {
                                     DataType = new IBISIPstring(applicationInfo.Name),
                                     VersionRef = new IBISIPNMTOKEN(applicationInfo.Version)
                                 });
            }

            data.DeviceInformation.DataVersionList = versions.ToArray();
            return data;
        }

        /// <summary>
        /// Gets the device configuration.
        /// </summary>
        /// <returns>
        /// The device configuration.
        /// </returns>
        public override DeviceManagementServiceGetDeviceConfigurationResponseDataStructure GetDeviceConfiguration()
        {
            return new DeviceManagementServiceGetDeviceConfigurationResponseDataStructure
                       {
                           TimeStamp = CreateTimeStamp(),
                           DeviceID = this.persistenceContext.Value.DeviceId
                       };
        }

        /// <summary>
        /// Sets the device configuration.
        /// </summary>
        /// <param name="deviceId">
        /// The device configuration.
        /// </param>
        /// <returns>
        /// The <see cref="DataAcceptedResponseDataStructure"/>.
        /// </returns>
        public override DataAcceptedResponseDataStructure SetDeviceConfiguration(IBISIPint deviceId)
        {
            this.persistenceContext.Value.DeviceId = deviceId;
            this.RaiseDeviceConfigurationChanged();
            return new DataAcceptedResponseDataStructure(TimeProvider.Current.Now, true);
        }

        /// <summary>
        /// Gets the device status.
        /// </summary>
        /// <returns>
        /// The device status.
        /// </returns>
        public override DeviceManagementServiceGetDeviceStatusResponseDataStructure GetDeviceStatus()
        {
            return new DeviceManagementServiceGetDeviceStatusResponseDataStructure
                       {
                           TimeStamp = CreateTimeStamp(),
                           DeviceState = this.deviceState
                       };
        }

        /// <summary>
        /// Gets the device error messages.
        /// </summary>
        /// <returns>
        /// The device error messages.
        /// </returns>
        public override DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure GetDeviceErrorMessages()
        {
            return new DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure
                       {
                           TimeStamp = CreateTimeStamp(),
                           ErrorMessage = this.errorMessages.ToArray()
                       };
        }

        /// <summary>
        /// Restarts this device.
        /// </summary>
        /// <returns>
        /// The <see cref="DataAcceptedResponseDataStructure"/>.
        /// </returns>
        public override DataAcceptedResponseDataStructure RestartDevice()
        {
            SystemManagerClient.Instance.Reboot("Requested by VDV 301");
            return new DataAcceptedResponseDataStructure(TimeProvider.Current.Now, true);
        }

        /// <summary>
        /// Deactivates this device.
        /// </summary>
        /// <returns>
        /// The <see cref="DataAcceptedResponseDataStructure"/>.
        /// </returns>
        public override DataAcceptedResponseDataStructure DeactivateDevice()
        {
            this.deviceState = DeviceStateEnumeration.notavailable;
            this.RaiseDeviceStatusChanged();
            return new DataAcceptedResponseDataStructure(TimeProvider.Current.Now, true);
        }

        /// <summary>
        /// Activates this device.
        /// </summary>
        /// <returns>
        /// The <see cref="DataAcceptedResponseDataStructure"/>.
        /// </returns>
        public override DataAcceptedResponseDataStructure ActivateDevice()
        {
            this.deviceState = DeviceStateEnumeration.running;
            this.RaiseDeviceStatusChanged();
            return new DataAcceptedResponseDataStructure(TimeProvider.Current.Now, true);
        }

        /// <summary>
        /// Gets the service information.
        /// </summary>
        /// <returns>
        /// The service information.
        /// </returns>
        public override DeviceManagementServiceGetServiceInformationResponseDataStructure GetServiceInformation()
        {
            var services = this.GetServices().ConvertAll(s => s.ToInformation());
            return new DeviceManagementServiceGetServiceInformationResponseDataStructure
                {
                    TimeStamp = CreateTimeStamp(),
                    ServiceInformationList = services.ToArray()
                };
        }

        /// <summary>
        /// Gets the service status.
        /// </summary>
        /// <returns>
        /// The service status.
        /// </returns>
        public override DeviceManagementServiceGetServiceStatusResponseDataStructure GetServiceStatus()
        {
            var services = this.GetServices().ConvertAll(s => s.ToSpecificationWithState());
            return new DeviceManagementServiceGetServiceStatusResponseDataStructure
                       {
                           TimeStamp = CreateTimeStamp(),
                           ServiceSpecificationWithStateList = services.ToArray()
                       };
        }

        /// <summary>
        /// Starts the given service.
        /// </summary>
        /// <param name="serviceSpecification">
        /// The service specification.
        /// </param>
        /// <returns>
        /// The <see cref="DataAcceptedResponseDataStructure"/>.
        /// </returns>
        public override DataAcceptedResponseDataStructure StartService(
            ServiceSpecificationStructure serviceSpecification)
        {
            try
            {
                VerifyServiceVersion(serviceSpecification.IBISIPVersion);

                // TODO: implement as soon as we have our own services
                throw new IbisIPException("Service doesn't exist on this system: " + serviceSpecification.ServiceName);
            }
            catch (IbisIPException ex)
            {
                return new DataAcceptedResponseDataStructure(
                    TimeProvider.Current.Now, false, ErrorCodeEnumeration.FaultData, ex.Message);
            }
        }

        /// <summary>
        /// Restarts the given service.
        /// </summary>
        /// <param name="serviceSpecification">
        /// The service specification.
        /// </param>
        /// <returns>
        /// The <see cref="DataAcceptedResponseDataStructure"/>.
        /// </returns>
        public override DataAcceptedResponseDataStructure RestartService(
            ServiceSpecificationStructure serviceSpecification)
        {
            try
            {
                VerifyServiceVersion(serviceSpecification.IBISIPVersion);

                // TODO: implement as soon as we have our own services
                throw new IbisIPException("Service doesn't exist on this system: " + serviceSpecification.ServiceName);
            }
            catch (IbisIPException ex)
            {
                return new DataAcceptedResponseDataStructure(
                    TimeProvider.Current.Now, false, ErrorCodeEnumeration.FaultData, ex.Message);
            }
        }

        /// <summary>
        /// Stops the given service.
        /// </summary>
        /// <param name="serviceSpecification">
        /// The service specification.
        /// </param>
        /// <returns>
        /// The <see cref="DataAcceptedResponseDataStructure"/>.
        /// </returns>
        public override DataAcceptedResponseDataStructure StopService(
            ServiceSpecificationStructure serviceSpecification)
        {
            try
            {
                VerifyServiceVersion(serviceSpecification.IBISIPVersion);

                // TODO: implement as soon as we have our own services
                throw new IbisIPException("Service doesn't exist on this system: " + serviceSpecification.ServiceName);
            }
            catch (IbisIPException ex)
            {
                return new DataAcceptedResponseDataStructure(
                    TimeProvider.Current.Now, false, ErrorCodeEnumeration.FaultData, ex.Message);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            MessageDispatcher.Instance.Unsubscribe<Alarm>(this.HandleAlarm);
        }

        private static IBISIPdateTime CreateTimeStamp()
        {
            return new IBISIPdateTime(TimeProvider.Current.Now);
        }

        private static void VerifyServiceVersion(IBISIPNMTOKEN version)
        {
            if (version == null || version.ErrorCodeSpecified)
            {
                throw new NotSupportedException("Can't determine version number");
            }

            if (new Version(version.Value) != Definitions.CurrentVersion)
            {
                throw new NotSupportedException("IBIS-IP version not supported: " + version);
            }
        }

        private void GotApplicationInfos(IAsyncResult ar)
        {
            this.apps.Clear();
            this.apps.AddRange(SystemManagerClient.Instance.EndGetApplicationInfos(ar));
            this.RaiseDeviceInformationChanged();
        }

        private List<ServiceInfo> GetServices()
        {
            // TODO: fill with service information as soon as we have our own services
            var services = new List<ServiceInfo>();

            // add ourselves
            services.Add(
                new ServiceInfo
                {
                    Autostart = true,
                    ServiceSpecification = new ServiceSpecificationStructure
                        {
                            ServiceName = ServiceNameEnumeration.DeviceManagementService,
                            IBISIPVersion = new IBISIPNMTOKEN(Definitions.CurrentVersion)
                        },
                    ServiceState = ServiceStateEnumeration.running
                });

            return services;
        }

        private void HandleAlarm(object sender, MessageEventArgs<Alarm> e)
        {
            var alarm = e.Message;
            if (alarm.Unit != ApplicationHelper.MachineName)
            {
                // we only report our own alarms
                return;
            }

            if (alarm.Severity == AlarmSeverity.Info)
            {
                // we only report warning level and above
                return;
            }

            var messageType = alarm.Severity == AlarmSeverity.Warning
                                  ? MessageTypeEnumeration.Warning
                                  : MessageTypeEnumeration.Error;
            var messageText = string.Format("{0} ({1})", alarm.Type, alarm.GetAttributeText());
            if (!string.IsNullOrEmpty(alarm.Message))
            {
                messageText += ": " + alarm.Message;
            }

            lock (this.errorMessages)
            {
                var message = new MessageStructure
                    {
                        MessageID = new IBISIPint(this.errorMessages.Count),
                        TimeStamp = CreateTimeStamp(),
                        MessageType = messageType,
                        MessageText = new IBISIPstring(messageText)
                    };
                this.errorMessages.Add(message);
            }

            this.RaiseDeviceErrorMessagesChanged();
        }

        private DeviceClassEnumeration GetDeviceClass()
        {
            DeviceClassEnumeration deviceClass;
            switch (this.config.DeviceClass)
            {
                case DeviceClass.OnBoardUnit:
                    deviceClass = DeviceClassEnumeration.OnBoardUnit;
                    break;
                case DeviceClass.SideDisplay:
                    deviceClass = DeviceClassEnumeration.SideDisplay;
                    break;
                case DeviceClass.FrontDisplay:
                    deviceClass = DeviceClassEnumeration.FrontDisplay;
                    break;
                case DeviceClass.InteriorDisplay:
                    deviceClass = DeviceClassEnumeration.InteriorDisplay;
                    break;
                case DeviceClass.Validator:
                    deviceClass = DeviceClassEnumeration.Validator;
                    break;
                case DeviceClass.TicketVendingMachine:
                    deviceClass = DeviceClassEnumeration.TicketVendingMachine;
                    break;
                case DeviceClass.AnnouncementSystem:
                    deviceClass = DeviceClassEnumeration.AnnouncementSystem;
                    break;
                case DeviceClass.MMI:
                    deviceClass = DeviceClassEnumeration.MMI;
                    break;
                case DeviceClass.VideoSystem:
                    deviceClass = DeviceClassEnumeration.VideoSystem;
                    break;
                case DeviceClass.APC:
                    deviceClass = DeviceClassEnumeration.APC;
                    break;
                case DeviceClass.MobileInterface:
                    deviceClass = DeviceClassEnumeration.MobileInterface;
                    break;
                case DeviceClass.TestDevice:
                    deviceClass = DeviceClassEnumeration.TestDevice;
                    break;
                default:
                    deviceClass = DeviceClassEnumeration.Other;
                    break;
            }

            return deviceClass;
        }

        private class ServiceInfo
        {
            public bool Autostart { get; set; }

            public ServiceSpecificationStructure ServiceSpecification { get; set; }

            public ServiceStateEnumeration ServiceState { get; set; }

            public ServiceSpecificationWithStateStructure ToSpecificationWithState()
            {
                return new ServiceSpecificationWithStateStructure
                    {
                        ServiceSpecification = this.ServiceSpecification,
                        ServiceState = this.ServiceState
                    };
            }

            public ServiceInformationStructure ToInformation()
            {
                return new ServiceInformationStructure
                    {
                        Autostart = new IBISIPboolean(this.Autostart),
                        Service = this.ServiceSpecification
                    };
            }
        }
    }
}