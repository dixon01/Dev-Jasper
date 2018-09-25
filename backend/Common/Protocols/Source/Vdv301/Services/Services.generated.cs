
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Protocols.Vdv301.Messages;

namespace Gorba.Common.Protocols.Vdv301.Services
{
    public class DataUpdateEventArgs<T> : EventArgs
    {
        public DataUpdateEventArgs(T value)
        {
            this.Value = value;
        }

        public T Value { get; private set; }
    }
    
    public interface IVdv301Service
    {
    }
    
    public interface IVdv301HttpService : IVdv301Service
    {
    }
    
    public interface IVdv301UdpService : IVdv301Service
    {
    }    

    public interface ICustomerInformationService : IVdv301HttpService
    {
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceAllData>> AllDataChanged;
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData>> CurrentAnnouncementChanged;
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData>> CurrentConnectionInformationChanged;
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData>> CurrentDisplayContentChanged;
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData>> CurrentStopPointChanged;
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData>> CurrentStopIndexChanged;
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceTripData>> TripDataChanged;
        event EventHandler<DataUpdateEventArgs<CustomerInformationServiceVehicleData>> VehicleDataChanged;
        CustomerInformationServiceAllData GetAllData();
        CustomerInformationServiceCurrentAnnouncementData GetCurrentAnnouncement();
        CustomerInformationServiceCurrentConnectionInformationData GetCurrentConnectionInformation();
        CustomerInformationServiceCurrentDisplayContentData GetCurrentDisplayContent();
        CustomerInformationServiceCurrentStopPointData GetCurrentStopPoint();
        CustomerInformationServiceCurrentStopIndexData GetCurrentStopIndex();
        CustomerInformationServiceTripData GetTripData();
        CustomerInformationServiceVehicleData GetVehicleData();
        CustomerInformationServicePartialStopSequenceData RetrievePartialStopSequence(IBISIPint startingStopIndex, IBISIPint numberOfStopPoints);
    }    

    public interface IDeviceManagementService : IVdv301HttpService
    {
        event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure>> DeviceInformationChanged;
        event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure>> DeviceConfigurationChanged;
        event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure>> DeviceStatusChanged;
        event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure>> DeviceErrorMessagesChanged;
        event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure>> ServiceInformationChanged;
        event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure>> ServiceStatusChanged;
        DeviceManagementServiceGetDeviceInformationResponseDataStructure GetDeviceInformation();
        DeviceManagementServiceGetDeviceConfigurationResponseDataStructure GetDeviceConfiguration();
        DataAcceptedResponseDataStructure SetDeviceConfiguration(IBISIPint deviceID);
        DeviceManagementServiceGetDeviceStatusResponseDataStructure GetDeviceStatus();
        DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure GetDeviceErrorMessages();
        DataAcceptedResponseDataStructure RestartDevice();
        DataAcceptedResponseDataStructure DeactivateDevice();
        DataAcceptedResponseDataStructure ActivateDevice();
        DeviceManagementServiceGetServiceInformationResponseDataStructure GetServiceInformation();
        DeviceManagementServiceGetServiceStatusResponseDataStructure GetServiceStatus();
        DataAcceptedResponseDataStructure StartService(ServiceSpecificationStructure serviceSpecification);
        DataAcceptedResponseDataStructure RestartService(ServiceSpecificationStructure serviceSpecification);
        DataAcceptedResponseDataStructure StopService(ServiceSpecificationStructure serviceSpecification);
    }    

    public interface INetworkLocationService : IVdv301UdpService
    {
        event EventHandler<DataUpdateEventArgs<NetworkLocationServiceDataStructure>> NetworkLocationChanged;
    }    

    public interface IDistanceLocationService : IVdv301UdpService
    {
        event EventHandler<DataUpdateEventArgs<DistanceLocationServiceDataStructure>> DistanceLocationChanged;
    }    

    public interface IGNSSLocationService : IVdv301UdpService
    {
        event EventHandler<DataUpdateEventArgs<GNSSLocationServiceDataStructure>> GNSSLocationChanged;
    }    

    public abstract partial class CustomerInformationServiceBase : ICustomerInformationService
    {
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceAllData>> AllDataChanged;

        
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData>> CurrentAnnouncementChanged;

        
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData>> CurrentConnectionInformationChanged;

        
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData>> CurrentDisplayContentChanged;

        
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData>> CurrentStopPointChanged;

        
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData>> CurrentStopIndexChanged;

        
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceTripData>> TripDataChanged;

        
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceVehicleData>> VehicleDataChanged;

        
        public abstract CustomerInformationServiceAllData GetAllData();
        
        public abstract CustomerInformationServiceCurrentAnnouncementData GetCurrentAnnouncement();
        
        public abstract CustomerInformationServiceCurrentConnectionInformationData GetCurrentConnectionInformation();
        
        public abstract CustomerInformationServiceCurrentDisplayContentData GetCurrentDisplayContent();
        
        public abstract CustomerInformationServiceCurrentStopPointData GetCurrentStopPoint();
        
        public abstract CustomerInformationServiceCurrentStopIndexData GetCurrentStopIndex();
        
        public abstract CustomerInformationServiceTripData GetTripData();
        
        public abstract CustomerInformationServiceVehicleData GetVehicleData();
        
        public abstract CustomerInformationServicePartialStopSequenceData RetrievePartialStopSequence(IBISIPint startingStopIndex, IBISIPint numberOfStopPoints);
        
        protected virtual void RaiseAllDataChanged(DataUpdateEventArgs<CustomerInformationServiceAllData> e)
        {
            var handler = this.AllDataChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseAllDataChanged()
        {
            var handler = this.AllDataChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceAllData>(this.GetAllData()));
            }
        }
        
        protected virtual void RaiseCurrentAnnouncementChanged(DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData> e)
        {
            var handler = this.CurrentAnnouncementChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseCurrentAnnouncementChanged()
        {
            var handler = this.CurrentAnnouncementChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData>(this.GetCurrentAnnouncement()));
            }
        }
        
        protected virtual void RaiseCurrentConnectionInformationChanged(DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData> e)
        {
            var handler = this.CurrentConnectionInformationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseCurrentConnectionInformationChanged()
        {
            var handler = this.CurrentConnectionInformationChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData>(this.GetCurrentConnectionInformation()));
            }
        }
        
        protected virtual void RaiseCurrentDisplayContentChanged(DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData> e)
        {
            var handler = this.CurrentDisplayContentChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseCurrentDisplayContentChanged()
        {
            var handler = this.CurrentDisplayContentChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData>(this.GetCurrentDisplayContent()));
            }
        }
        
        protected virtual void RaiseCurrentStopPointChanged(DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData> e)
        {
            var handler = this.CurrentStopPointChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseCurrentStopPointChanged()
        {
            var handler = this.CurrentStopPointChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData>(this.GetCurrentStopPoint()));
            }
        }
        
        protected virtual void RaiseCurrentStopIndexChanged(DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData> e)
        {
            var handler = this.CurrentStopIndexChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseCurrentStopIndexChanged()
        {
            var handler = this.CurrentStopIndexChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData>(this.GetCurrentStopIndex()));
            }
        }
        
        protected virtual void RaiseTripDataChanged(DataUpdateEventArgs<CustomerInformationServiceTripData> e)
        {
            var handler = this.TripDataChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseTripDataChanged()
        {
            var handler = this.TripDataChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceTripData>(this.GetTripData()));
            }
        }
        
        protected virtual void RaiseVehicleDataChanged(DataUpdateEventArgs<CustomerInformationServiceVehicleData> e)
        {
            var handler = this.VehicleDataChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseVehicleDataChanged()
        {
            var handler = this.VehicleDataChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceVehicleData>(this.GetVehicleData()));
            }
        }
        
    }    

    public abstract partial class DeviceManagementServiceBase : IDeviceManagementService
    {
        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure>> DeviceInformationChanged;

        
        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure>> DeviceConfigurationChanged;

        
        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure>> DeviceStatusChanged;

        
        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure>> DeviceErrorMessagesChanged;

        
        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure>> ServiceInformationChanged;

        
        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure>> ServiceStatusChanged;

        
        public abstract DeviceManagementServiceGetDeviceInformationResponseDataStructure GetDeviceInformation();
        
        public abstract DeviceManagementServiceGetDeviceConfigurationResponseDataStructure GetDeviceConfiguration();
        
        public abstract DataAcceptedResponseDataStructure SetDeviceConfiguration(IBISIPint deviceID);
        
        public abstract DeviceManagementServiceGetDeviceStatusResponseDataStructure GetDeviceStatus();
        
        public abstract DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure GetDeviceErrorMessages();
        
        public abstract DataAcceptedResponseDataStructure RestartDevice();
        
        public abstract DataAcceptedResponseDataStructure DeactivateDevice();
        
        public abstract DataAcceptedResponseDataStructure ActivateDevice();
        
        public abstract DeviceManagementServiceGetServiceInformationResponseDataStructure GetServiceInformation();
        
        public abstract DeviceManagementServiceGetServiceStatusResponseDataStructure GetServiceStatus();
        
        public abstract DataAcceptedResponseDataStructure StartService(ServiceSpecificationStructure serviceSpecification);
        
        public abstract DataAcceptedResponseDataStructure RestartService(ServiceSpecificationStructure serviceSpecification);
        
        public abstract DataAcceptedResponseDataStructure StopService(ServiceSpecificationStructure serviceSpecification);
        
        protected virtual void RaiseDeviceInformationChanged(DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure> e)
        {
            var handler = this.DeviceInformationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseDeviceInformationChanged()
        {
            var handler = this.DeviceInformationChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure>(this.GetDeviceInformation()));
            }
        }
        
        protected virtual void RaiseDeviceConfigurationChanged(DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure> e)
        {
            var handler = this.DeviceConfigurationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseDeviceConfigurationChanged()
        {
            var handler = this.DeviceConfigurationChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure>(this.GetDeviceConfiguration()));
            }
        }
        
        protected virtual void RaiseDeviceStatusChanged(DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure> e)
        {
            var handler = this.DeviceStatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseDeviceStatusChanged()
        {
            var handler = this.DeviceStatusChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure>(this.GetDeviceStatus()));
            }
        }
        
        protected virtual void RaiseDeviceErrorMessagesChanged(DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure> e)
        {
            var handler = this.DeviceErrorMessagesChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseDeviceErrorMessagesChanged()
        {
            var handler = this.DeviceErrorMessagesChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure>(this.GetDeviceErrorMessages()));
            }
        }
        
        protected virtual void RaiseServiceInformationChanged(DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure> e)
        {
            var handler = this.ServiceInformationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseServiceInformationChanged()
        {
            var handler = this.ServiceInformationChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure>(this.GetServiceInformation()));
            }
        }
        
        protected virtual void RaiseServiceStatusChanged(DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure> e)
        {
            var handler = this.ServiceStatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseServiceStatusChanged()
        {
            var handler = this.ServiceStatusChanged;
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure>(this.GetServiceStatus()));
            }
        }
        
    }    

    public abstract partial class NetworkLocationServiceBase : INetworkLocationService
    {
        public event EventHandler<DataUpdateEventArgs<NetworkLocationServiceDataStructure>> NetworkLocationChanged;

        protected virtual void RaiseNetworkLocationChanged(DataUpdateEventArgs<NetworkLocationServiceDataStructure> e)
        {
            var handler = this.NetworkLocationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }    

    public abstract partial class DistanceLocationServiceBase : IDistanceLocationService
    {
        public event EventHandler<DataUpdateEventArgs<DistanceLocationServiceDataStructure>> DistanceLocationChanged;

        protected virtual void RaiseDistanceLocationChanged(DataUpdateEventArgs<DistanceLocationServiceDataStructure> e)
        {
            var handler = this.DistanceLocationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }    

    public abstract partial class GNSSLocationServiceBase : IGNSSLocationService
    {
        public event EventHandler<DataUpdateEventArgs<GNSSLocationServiceDataStructure>> GNSSLocationChanged;

        protected virtual void RaiseGNSSLocationChanged(DataUpdateEventArgs<GNSSLocationServiceDataStructure> e)
        {
            var handler = this.GNSSLocationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}

