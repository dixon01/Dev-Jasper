
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Protocols.Vdv301.Messages;
using Gorba.Common.Protocols.Vdv301.Services;

using Gorba.Common.Utility.Core;

using Gorba.Motion.Common.IbisIP.Server;

using NLog;

namespace Gorba.Motion.Common.IbisIP.Client
{
    public static partial class ServiceClientProxyFactory
    {
        public static T Create<T>(string hostName, int port, string path)
            where T : class, IVdv301Service
        {
            return Create<T>(hostName, port, path, null);
        }

        public static T Create<T>(string hostName, int port, string path, IbisHttpServer localServer)
            where T : class, IVdv301Service
        {
            if (path != null)
            {
                path = path.Trim('/');
                if (path.Length > 0)
                {
                    path += "/";
                }
            }

            var url = string.Format("http://{0}:{1}/{2}{3}", hostName, port, path, typeof(T).Name.Substring(1));
            if (typeof(T) == typeof(ICustomerInformationService))
            {
                return (T)(object)new CustomerInformationServiceClientProxy(url, localServer);
            }
        
            if (typeof(T) == typeof(IDeviceManagementService))
            {
                return (T)(object)new DeviceManagementServiceClientProxy(url, localServer);
            }
        
            throw new NotSupportedException(string.Format("Service {0} not supported", typeof(T).FullName));
        }

        public static T Create<T>(IPAddress address, int port)
            where T : class, IVdv301Service
        {
            var endPoint = new IPEndPoint(address, port);
            if (typeof(T) == typeof(INetworkLocationService))
            {
                return (T)(object)new NetworkLocationServiceClientProxy(endPoint);
            }
        
            if (typeof(T) == typeof(IDistanceLocationService))
            {
                return (T)(object)new DistanceLocationServiceClientProxy(endPoint);
            }
        
            if (typeof(T) == typeof(IGNSSLocationService))
            {
                return (T)(object)new GNSSLocationServiceClientProxy(endPoint);
            }
        
            throw new NotSupportedException(string.Format("Service {0} not supported", typeof(T).FullName));
        }
    }    

    internal partial class CustomerInformationServiceClientProxy : HttpClientProxyBase, ICustomerInformationService
    {
        private readonly EventHandlerList eventHandlers = new EventHandlerList();
        private readonly IbisHttpServer localServer;
        
        private Subscription<CustomerInformationServiceGetAllDataResponseStructure> subscriptionAllData;
        private Subscription<CustomerInformationServiceGetCurrentAnnouncementResponseStructure> subscriptionCurrentAnnouncement;
        private Subscription<CustomerInformationServiceGetCurrentConnectionInformationResponseStructure> subscriptionCurrentConnectionInformation;
        private Subscription<CustomerInformationServiceGetCurrentDisplayContentResponseStructure> subscriptionCurrentDisplayContent;
        private Subscription<CustomerInformationServiceGetCurrentStopPointResponseStructure> subscriptionCurrentStopPoint;
        private Subscription<CustomerInformationServiceGetCurrentStopIndexResponseStructure> subscriptionCurrentStopIndex;
        private Subscription<CustomerInformationServiceGetTripDataResponseStructure> subscriptionTripData;
        private Subscription<CustomerInformationServiceGetVehicleDataResponseStructure> subscriptionVehicleData;

        public CustomerInformationServiceClientProxy(string url, IbisHttpServer localServer)
            : base(url)
        {
            this.localServer = localServer;
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceAllData>> AllDataChanged
        {
            add
            {
                this.eventHandlers.AddHandler("AllData", value);
                if (this.subscriptionAllData == null)
                {
                    this.subscriptionAllData = new Subscription<CustomerInformationServiceGetAllDataResponseStructure>();
                    this.subscriptionAllData.Updated += this.SubscriptionAllDataOnUpdated;
                    this.subscriptionAllData.Start(this.localServer);
                    this.Subscribe("AllData", this.subscriptionAllData.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("AllData", value);
                if (this.eventHandlers["AllData"] == null && this.subscriptionAllData != null)
                {
                    this.Unsubscribe("AllData", this.subscriptionAllData.CallbackUri);
                    this.subscriptionAllData.Stop();
                    this.subscriptionAllData.Updated -= this.SubscriptionAllDataOnUpdated;
                    this.subscriptionAllData = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData>> CurrentAnnouncementChanged
        {
            add
            {
                this.eventHandlers.AddHandler("CurrentAnnouncement", value);
                if (this.subscriptionCurrentAnnouncement == null)
                {
                    this.subscriptionCurrentAnnouncement = new Subscription<CustomerInformationServiceGetCurrentAnnouncementResponseStructure>();
                    this.subscriptionCurrentAnnouncement.Updated += this.SubscriptionCurrentAnnouncementOnUpdated;
                    this.subscriptionCurrentAnnouncement.Start(this.localServer);
                    this.Subscribe("CurrentAnnouncement", this.subscriptionCurrentAnnouncement.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("CurrentAnnouncement", value);
                if (this.eventHandlers["CurrentAnnouncement"] == null && this.subscriptionCurrentAnnouncement != null)
                {
                    this.Unsubscribe("CurrentAnnouncement", this.subscriptionCurrentAnnouncement.CallbackUri);
                    this.subscriptionCurrentAnnouncement.Stop();
                    this.subscriptionCurrentAnnouncement.Updated -= this.SubscriptionCurrentAnnouncementOnUpdated;
                    this.subscriptionCurrentAnnouncement = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData>> CurrentConnectionInformationChanged
        {
            add
            {
                this.eventHandlers.AddHandler("CurrentConnectionInformation", value);
                if (this.subscriptionCurrentConnectionInformation == null)
                {
                    this.subscriptionCurrentConnectionInformation = new Subscription<CustomerInformationServiceGetCurrentConnectionInformationResponseStructure>();
                    this.subscriptionCurrentConnectionInformation.Updated += this.SubscriptionCurrentConnectionInformationOnUpdated;
                    this.subscriptionCurrentConnectionInformation.Start(this.localServer);
                    this.Subscribe("CurrentConnectionInformation", this.subscriptionCurrentConnectionInformation.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("CurrentConnectionInformation", value);
                if (this.eventHandlers["CurrentConnectionInformation"] == null && this.subscriptionCurrentConnectionInformation != null)
                {
                    this.Unsubscribe("CurrentConnectionInformation", this.subscriptionCurrentConnectionInformation.CallbackUri);
                    this.subscriptionCurrentConnectionInformation.Stop();
                    this.subscriptionCurrentConnectionInformation.Updated -= this.SubscriptionCurrentConnectionInformationOnUpdated;
                    this.subscriptionCurrentConnectionInformation = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData>> CurrentDisplayContentChanged
        {
            add
            {
                this.eventHandlers.AddHandler("CurrentDisplayContent", value);
                if (this.subscriptionCurrentDisplayContent == null)
                {
                    this.subscriptionCurrentDisplayContent = new Subscription<CustomerInformationServiceGetCurrentDisplayContentResponseStructure>();
                    this.subscriptionCurrentDisplayContent.Updated += this.SubscriptionCurrentDisplayContentOnUpdated;
                    this.subscriptionCurrentDisplayContent.Start(this.localServer);
                    this.Subscribe("CurrentDisplayContent", this.subscriptionCurrentDisplayContent.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("CurrentDisplayContent", value);
                if (this.eventHandlers["CurrentDisplayContent"] == null && this.subscriptionCurrentDisplayContent != null)
                {
                    this.Unsubscribe("CurrentDisplayContent", this.subscriptionCurrentDisplayContent.CallbackUri);
                    this.subscriptionCurrentDisplayContent.Stop();
                    this.subscriptionCurrentDisplayContent.Updated -= this.SubscriptionCurrentDisplayContentOnUpdated;
                    this.subscriptionCurrentDisplayContent = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData>> CurrentStopPointChanged
        {
            add
            {
                this.eventHandlers.AddHandler("CurrentStopPoint", value);
                if (this.subscriptionCurrentStopPoint == null)
                {
                    this.subscriptionCurrentStopPoint = new Subscription<CustomerInformationServiceGetCurrentStopPointResponseStructure>();
                    this.subscriptionCurrentStopPoint.Updated += this.SubscriptionCurrentStopPointOnUpdated;
                    this.subscriptionCurrentStopPoint.Start(this.localServer);
                    this.Subscribe("CurrentStopPoint", this.subscriptionCurrentStopPoint.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("CurrentStopPoint", value);
                if (this.eventHandlers["CurrentStopPoint"] == null && this.subscriptionCurrentStopPoint != null)
                {
                    this.Unsubscribe("CurrentStopPoint", this.subscriptionCurrentStopPoint.CallbackUri);
                    this.subscriptionCurrentStopPoint.Stop();
                    this.subscriptionCurrentStopPoint.Updated -= this.SubscriptionCurrentStopPointOnUpdated;
                    this.subscriptionCurrentStopPoint = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData>> CurrentStopIndexChanged
        {
            add
            {
                this.eventHandlers.AddHandler("CurrentStopIndex", value);
                if (this.subscriptionCurrentStopIndex == null)
                {
                    this.subscriptionCurrentStopIndex = new Subscription<CustomerInformationServiceGetCurrentStopIndexResponseStructure>();
                    this.subscriptionCurrentStopIndex.Updated += this.SubscriptionCurrentStopIndexOnUpdated;
                    this.subscriptionCurrentStopIndex.Start(this.localServer);
                    this.Subscribe("CurrentStopIndex", this.subscriptionCurrentStopIndex.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("CurrentStopIndex", value);
                if (this.eventHandlers["CurrentStopIndex"] == null && this.subscriptionCurrentStopIndex != null)
                {
                    this.Unsubscribe("CurrentStopIndex", this.subscriptionCurrentStopIndex.CallbackUri);
                    this.subscriptionCurrentStopIndex.Stop();
                    this.subscriptionCurrentStopIndex.Updated -= this.SubscriptionCurrentStopIndexOnUpdated;
                    this.subscriptionCurrentStopIndex = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceTripData>> TripDataChanged
        {
            add
            {
                this.eventHandlers.AddHandler("TripData", value);
                if (this.subscriptionTripData == null)
                {
                    this.subscriptionTripData = new Subscription<CustomerInformationServiceGetTripDataResponseStructure>();
                    this.subscriptionTripData.Updated += this.SubscriptionTripDataOnUpdated;
                    this.subscriptionTripData.Start(this.localServer);
                    this.Subscribe("TripData", this.subscriptionTripData.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("TripData", value);
                if (this.eventHandlers["TripData"] == null && this.subscriptionTripData != null)
                {
                    this.Unsubscribe("TripData", this.subscriptionTripData.CallbackUri);
                    this.subscriptionTripData.Stop();
                    this.subscriptionTripData.Updated -= this.SubscriptionTripDataOnUpdated;
                    this.subscriptionTripData = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceVehicleData>> VehicleDataChanged
        {
            add
            {
                this.eventHandlers.AddHandler("VehicleData", value);
                if (this.subscriptionVehicleData == null)
                {
                    this.subscriptionVehicleData = new Subscription<CustomerInformationServiceGetVehicleDataResponseStructure>();
                    this.subscriptionVehicleData.Updated += this.SubscriptionVehicleDataOnUpdated;
                    this.subscriptionVehicleData.Start(this.localServer);
                    this.Subscribe("VehicleData", this.subscriptionVehicleData.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("VehicleData", value);
                if (this.eventHandlers["VehicleData"] == null && this.subscriptionVehicleData != null)
                {
                    this.Unsubscribe("VehicleData", this.subscriptionVehicleData.CallbackUri);
                    this.subscriptionVehicleData.Stop();
                    this.subscriptionVehicleData.Updated -= this.SubscriptionVehicleDataOnUpdated;
                    this.subscriptionVehicleData = null;
                }
            }
        }

        public CustomerInformationServiceAllData GetAllData()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetAllDataResponseStructure>(
                "GetAllData");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceAllData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServiceCurrentAnnouncementData GetCurrentAnnouncement()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetCurrentAnnouncementResponseStructure>(
                "GetCurrentAnnouncement");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceCurrentAnnouncementData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServiceCurrentConnectionInformationData GetCurrentConnectionInformation()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetCurrentConnectionInformationResponseStructure>(
                "GetCurrentConnectionInformation");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceCurrentConnectionInformationData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServiceCurrentDisplayContentData GetCurrentDisplayContent()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetCurrentDisplayContentResponseStructure>(
                "GetCurrentDisplayContent");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceCurrentDisplayContentData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServiceCurrentStopPointData GetCurrentStopPoint()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetCurrentStopPointResponseStructure>(
                "GetCurrentStopPoint");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceCurrentStopPointData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServiceCurrentStopIndexData GetCurrentStopIndex()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetCurrentStopIndexResponseStructure>(
                "GetCurrentStopIndex");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceCurrentStopIndexData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServiceTripData GetTripData()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetTripDataResponseStructure>(
                "GetTripData");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceTripData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServiceVehicleData GetVehicleData()
        {
            var responseWrapper = this.ExecuteGetRequest<CustomerInformationServiceGetVehicleDataResponseStructure>(
                "GetVehicleData");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServiceVehicleData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public CustomerInformationServicePartialStopSequenceData RetrievePartialStopSequence(IBISIPint startingStopIndex, IBISIPint numberOfStopPoints)
        {
            var requestWrapper = new CustomerInformationServiceRetrievePartialStopSequenceRequestStructure();
            requestWrapper.StartingStopIndex = startingStopIndex;
            requestWrapper.NumberOfStopPoints = numberOfStopPoints;
            var responseWrapper = this.ExecutePostRequest<CustomerInformationServiceRetrievePartialStopSequenceResponseStructure>(
                "RetrievePartialStopSequence", requestWrapper);

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as CustomerInformationServicePartialStopSequenceData;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        private void SubscriptionAllDataOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetAllDataResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceAllData;
            var handler = this.eventHandlers["AllData"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceAllData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceAllData>(value));
            }
        }

        private void SubscriptionCurrentAnnouncementOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetCurrentAnnouncementResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceCurrentAnnouncementData;
            var handler = this.eventHandlers["CurrentAnnouncement"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData>(value));
            }
        }

        private void SubscriptionCurrentConnectionInformationOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetCurrentConnectionInformationResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceCurrentConnectionInformationData;
            var handler = this.eventHandlers["CurrentConnectionInformation"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData>(value));
            }
        }

        private void SubscriptionCurrentDisplayContentOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetCurrentDisplayContentResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceCurrentDisplayContentData;
            var handler = this.eventHandlers["CurrentDisplayContent"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData>(value));
            }
        }

        private void SubscriptionCurrentStopPointOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetCurrentStopPointResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceCurrentStopPointData;
            var handler = this.eventHandlers["CurrentStopPoint"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData>(value));
            }
        }

        private void SubscriptionCurrentStopIndexOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetCurrentStopIndexResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceCurrentStopIndexData;
            var handler = this.eventHandlers["CurrentStopIndex"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData>(value));
            }
        }

        private void SubscriptionTripDataOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetTripDataResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceTripData;
            var handler = this.eventHandlers["TripData"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceTripData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceTripData>(value));
            }
        }

        private void SubscriptionVehicleDataOnUpdated(object source, DataUpdateEventArgs<CustomerInformationServiceGetVehicleDataResponseStructure> e)
        {
            var value = e.Value.Item as CustomerInformationServiceVehicleData;
            var handler = this.eventHandlers["VehicleData"] as EventHandler<DataUpdateEventArgs<CustomerInformationServiceVehicleData>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<CustomerInformationServiceVehicleData>(value));
            }
        }
    }    

    internal partial class DeviceManagementServiceClientProxy : HttpClientProxyBase, IDeviceManagementService
    {
        private readonly EventHandlerList eventHandlers = new EventHandlerList();
        private readonly IbisHttpServer localServer;
        
        private Subscription<DeviceManagementServiceGetDeviceInformationResponseStructure> subscriptionDeviceInformation;
        private Subscription<DeviceManagementServiceGetDeviceConfigurationResponseStructure> subscriptionDeviceConfiguration;
        private Subscription<DeviceManagementServiceGetDeviceStatusResponseStructure> subscriptionDeviceStatus;
        private Subscription<DeviceManagementServiceGetDeviceErrorMessagesResponseStructure> subscriptionDeviceErrorMessages;
        private Subscription<DeviceManagementServiceGetServiceInformationResponseStructure> subscriptionServiceInformation;
        private Subscription<DeviceManagementServiceGetServiceStatusResponseStructure> subscriptionServiceStatus;

        public DeviceManagementServiceClientProxy(string url, IbisHttpServer localServer)
            : base(url)
        {
            this.localServer = localServer;
        }

        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure>> DeviceInformationChanged
        {
            add
            {
                this.eventHandlers.AddHandler("DeviceInformation", value);
                if (this.subscriptionDeviceInformation == null)
                {
                    this.subscriptionDeviceInformation = new Subscription<DeviceManagementServiceGetDeviceInformationResponseStructure>();
                    this.subscriptionDeviceInformation.Updated += this.SubscriptionDeviceInformationOnUpdated;
                    this.subscriptionDeviceInformation.Start(this.localServer);
                    this.Subscribe("DeviceInformation", this.subscriptionDeviceInformation.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("DeviceInformation", value);
                if (this.eventHandlers["DeviceInformation"] == null && this.subscriptionDeviceInformation != null)
                {
                    this.Unsubscribe("DeviceInformation", this.subscriptionDeviceInformation.CallbackUri);
                    this.subscriptionDeviceInformation.Stop();
                    this.subscriptionDeviceInformation.Updated -= this.SubscriptionDeviceInformationOnUpdated;
                    this.subscriptionDeviceInformation = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure>> DeviceConfigurationChanged
        {
            add
            {
                this.eventHandlers.AddHandler("DeviceConfiguration", value);
                if (this.subscriptionDeviceConfiguration == null)
                {
                    this.subscriptionDeviceConfiguration = new Subscription<DeviceManagementServiceGetDeviceConfigurationResponseStructure>();
                    this.subscriptionDeviceConfiguration.Updated += this.SubscriptionDeviceConfigurationOnUpdated;
                    this.subscriptionDeviceConfiguration.Start(this.localServer);
                    this.Subscribe("DeviceConfiguration", this.subscriptionDeviceConfiguration.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("DeviceConfiguration", value);
                if (this.eventHandlers["DeviceConfiguration"] == null && this.subscriptionDeviceConfiguration != null)
                {
                    this.Unsubscribe("DeviceConfiguration", this.subscriptionDeviceConfiguration.CallbackUri);
                    this.subscriptionDeviceConfiguration.Stop();
                    this.subscriptionDeviceConfiguration.Updated -= this.SubscriptionDeviceConfigurationOnUpdated;
                    this.subscriptionDeviceConfiguration = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure>> DeviceStatusChanged
        {
            add
            {
                this.eventHandlers.AddHandler("DeviceStatus", value);
                if (this.subscriptionDeviceStatus == null)
                {
                    this.subscriptionDeviceStatus = new Subscription<DeviceManagementServiceGetDeviceStatusResponseStructure>();
                    this.subscriptionDeviceStatus.Updated += this.SubscriptionDeviceStatusOnUpdated;
                    this.subscriptionDeviceStatus.Start(this.localServer);
                    this.Subscribe("DeviceStatus", this.subscriptionDeviceStatus.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("DeviceStatus", value);
                if (this.eventHandlers["DeviceStatus"] == null && this.subscriptionDeviceStatus != null)
                {
                    this.Unsubscribe("DeviceStatus", this.subscriptionDeviceStatus.CallbackUri);
                    this.subscriptionDeviceStatus.Stop();
                    this.subscriptionDeviceStatus.Updated -= this.SubscriptionDeviceStatusOnUpdated;
                    this.subscriptionDeviceStatus = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure>> DeviceErrorMessagesChanged
        {
            add
            {
                this.eventHandlers.AddHandler("DeviceErrorMessages", value);
                if (this.subscriptionDeviceErrorMessages == null)
                {
                    this.subscriptionDeviceErrorMessages = new Subscription<DeviceManagementServiceGetDeviceErrorMessagesResponseStructure>();
                    this.subscriptionDeviceErrorMessages.Updated += this.SubscriptionDeviceErrorMessagesOnUpdated;
                    this.subscriptionDeviceErrorMessages.Start(this.localServer);
                    this.Subscribe("DeviceErrorMessages", this.subscriptionDeviceErrorMessages.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("DeviceErrorMessages", value);
                if (this.eventHandlers["DeviceErrorMessages"] == null && this.subscriptionDeviceErrorMessages != null)
                {
                    this.Unsubscribe("DeviceErrorMessages", this.subscriptionDeviceErrorMessages.CallbackUri);
                    this.subscriptionDeviceErrorMessages.Stop();
                    this.subscriptionDeviceErrorMessages.Updated -= this.SubscriptionDeviceErrorMessagesOnUpdated;
                    this.subscriptionDeviceErrorMessages = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure>> ServiceInformationChanged
        {
            add
            {
                this.eventHandlers.AddHandler("ServiceInformation", value);
                if (this.subscriptionServiceInformation == null)
                {
                    this.subscriptionServiceInformation = new Subscription<DeviceManagementServiceGetServiceInformationResponseStructure>();
                    this.subscriptionServiceInformation.Updated += this.SubscriptionServiceInformationOnUpdated;
                    this.subscriptionServiceInformation.Start(this.localServer);
                    this.Subscribe("ServiceInformation", this.subscriptionServiceInformation.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("ServiceInformation", value);
                if (this.eventHandlers["ServiceInformation"] == null && this.subscriptionServiceInformation != null)
                {
                    this.Unsubscribe("ServiceInformation", this.subscriptionServiceInformation.CallbackUri);
                    this.subscriptionServiceInformation.Stop();
                    this.subscriptionServiceInformation.Updated -= this.SubscriptionServiceInformationOnUpdated;
                    this.subscriptionServiceInformation = null;
                }
            }
        }

        public event EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure>> ServiceStatusChanged
        {
            add
            {
                this.eventHandlers.AddHandler("ServiceStatus", value);
                if (this.subscriptionServiceStatus == null)
                {
                    this.subscriptionServiceStatus = new Subscription<DeviceManagementServiceGetServiceStatusResponseStructure>();
                    this.subscriptionServiceStatus.Updated += this.SubscriptionServiceStatusOnUpdated;
                    this.subscriptionServiceStatus.Start(this.localServer);
                    this.Subscribe("ServiceStatus", this.subscriptionServiceStatus.CallbackUri);
                }
            }

            remove
            {
                this.eventHandlers.RemoveHandler("ServiceStatus", value);
                if (this.eventHandlers["ServiceStatus"] == null && this.subscriptionServiceStatus != null)
                {
                    this.Unsubscribe("ServiceStatus", this.subscriptionServiceStatus.CallbackUri);
                    this.subscriptionServiceStatus.Stop();
                    this.subscriptionServiceStatus.Updated -= this.SubscriptionServiceStatusOnUpdated;
                    this.subscriptionServiceStatus = null;
                }
            }
        }

        public DeviceManagementServiceGetDeviceInformationResponseDataStructure GetDeviceInformation()
        {
            var responseWrapper = this.ExecuteGetRequest<DeviceManagementServiceGetDeviceInformationResponseStructure>(
                "GetDeviceInformation");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DeviceManagementServiceGetDeviceInformationResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DeviceManagementServiceGetDeviceConfigurationResponseDataStructure GetDeviceConfiguration()
        {
            var responseWrapper = this.ExecuteGetRequest<DeviceManagementServiceGetDeviceConfigurationResponseStructure>(
                "GetDeviceConfiguration");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DeviceManagementServiceGetDeviceConfigurationResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DataAcceptedResponseDataStructure SetDeviceConfiguration(IBISIPint deviceID)
        {
            var requestWrapper = new DeviceManagementServiceSetDeviceConfigurationRequestStructure();
            requestWrapper.DeviceID = deviceID;
            var responseWrapper = this.ExecutePostRequest<DataAcceptedResponseStructure>(
                "SetDeviceConfiguration", requestWrapper);

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DataAcceptedResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DeviceManagementServiceGetDeviceStatusResponseDataStructure GetDeviceStatus()
        {
            var responseWrapper = this.ExecuteGetRequest<DeviceManagementServiceGetDeviceStatusResponseStructure>(
                "GetDeviceStatus");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DeviceManagementServiceGetDeviceStatusResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure GetDeviceErrorMessages()
        {
            var responseWrapper = this.ExecuteGetRequest<DeviceManagementServiceGetDeviceErrorMessagesResponseStructure>(
                "GetDeviceErrorMessages");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DataAcceptedResponseDataStructure RestartDevice()
        {
            var responseWrapper = this.ExecuteGetRequest<DataAcceptedResponseStructure>(
                "RestartDevice");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DataAcceptedResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DataAcceptedResponseDataStructure DeactivateDevice()
        {
            var responseWrapper = this.ExecuteGetRequest<DataAcceptedResponseStructure>(
                "DeactivateDevice");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DataAcceptedResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DataAcceptedResponseDataStructure ActivateDevice()
        {
            var responseWrapper = this.ExecuteGetRequest<DataAcceptedResponseStructure>(
                "ActivateDevice");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DataAcceptedResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DeviceManagementServiceGetServiceInformationResponseDataStructure GetServiceInformation()
        {
            var responseWrapper = this.ExecuteGetRequest<DeviceManagementServiceGetServiceInformationResponseStructure>(
                "GetServiceInformation");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DeviceManagementServiceGetServiceInformationResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DeviceManagementServiceGetServiceStatusResponseDataStructure GetServiceStatus()
        {
            var responseWrapper = this.ExecuteGetRequest<DeviceManagementServiceGetServiceStatusResponseStructure>(
                "GetServiceStatus");

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DeviceManagementServiceGetServiceStatusResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DataAcceptedResponseDataStructure StartService(ServiceSpecificationStructure serviceSpecification)
        {
            var requestWrapper = new DeviceManagementServiceStartServiceRequestStructure();
            requestWrapper.ServiceSpecification = serviceSpecification;
            var responseWrapper = this.ExecutePostRequest<DataAcceptedResponseStructure>(
                "StartService", requestWrapper);

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DataAcceptedResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DataAcceptedResponseDataStructure RestartService(ServiceSpecificationStructure serviceSpecification)
        {
            var requestWrapper = new DeviceManagementServiceRestartServiceRequestStructure();
            requestWrapper.ServiceSpecification = serviceSpecification;
            var responseWrapper = this.ExecutePostRequest<DataAcceptedResponseStructure>(
                "RestartService", requestWrapper);

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DataAcceptedResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        public DataAcceptedResponseDataStructure StopService(ServiceSpecificationStructure serviceSpecification)
        {
            var requestWrapper = new DeviceManagementServiceStopServiceRequestStructure();
            requestWrapper.ServiceSpecification = serviceSpecification;
            var responseWrapper = this.ExecutePostRequest<DataAcceptedResponseStructure>(
                "StopService", requestWrapper);

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as DataAcceptedResponseDataStructure;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
            
            return result;
        }

        private void SubscriptionDeviceInformationOnUpdated(object source, DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseStructure> e)
        {
            var value = e.Value.Item as DeviceManagementServiceGetDeviceInformationResponseDataStructure;
            var handler = this.eventHandlers["DeviceInformation"] as EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure>(value));
            }
        }

        private void SubscriptionDeviceConfigurationOnUpdated(object source, DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseStructure> e)
        {
            var value = e.Value.Item as DeviceManagementServiceGetDeviceConfigurationResponseDataStructure;
            var handler = this.eventHandlers["DeviceConfiguration"] as EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure>(value));
            }
        }

        private void SubscriptionDeviceStatusOnUpdated(object source, DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseStructure> e)
        {
            var value = e.Value.Item as DeviceManagementServiceGetDeviceStatusResponseDataStructure;
            var handler = this.eventHandlers["DeviceStatus"] as EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure>(value));
            }
        }

        private void SubscriptionDeviceErrorMessagesOnUpdated(object source, DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseStructure> e)
        {
            var value = e.Value.Item as DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure;
            var handler = this.eventHandlers["DeviceErrorMessages"] as EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure>(value));
            }
        }

        private void SubscriptionServiceInformationOnUpdated(object source, DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseStructure> e)
        {
            var value = e.Value.Item as DeviceManagementServiceGetServiceInformationResponseDataStructure;
            var handler = this.eventHandlers["ServiceInformation"] as EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure>(value));
            }
        }

        private void SubscriptionServiceStatusOnUpdated(object source, DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseStructure> e)
        {
            var value = e.Value.Item as DeviceManagementServiceGetServiceStatusResponseDataStructure;
            var handler = this.eventHandlers["ServiceStatus"] as EventHandler<DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure>>;
            if (value != null && handler != null)
            {
                handler(this, new DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure>(value));
            }
        }
    }    

    internal partial class NetworkLocationServiceClientProxy : UdpClientProxyBase, INetworkLocationService
    {
        private readonly XmlSerializer serializer;
        private EventHandler<DataUpdateEventArgs<NetworkLocationServiceDataStructure>> eventHandler;

        public NetworkLocationServiceClientProxy(IPEndPoint endPoint)
            : base(endPoint)
        {
            this.serializer = new XmlSerializer(typeof(NetworkLocationServiceDataStructure));
        }
        
        public event EventHandler<DataUpdateEventArgs<NetworkLocationServiceDataStructure>> NetworkLocationChanged
        {
            add
            {
                var subscribe = this.eventHandler == null;
                this.eventHandler = (EventHandler<DataUpdateEventArgs<NetworkLocationServiceDataStructure>>)Delegate.Combine(this.eventHandler, value);
                if (subscribe)
                {
                    this.Subscribe();
                }
            }

            remove
            {
                this.eventHandler = (EventHandler<DataUpdateEventArgs<NetworkLocationServiceDataStructure>>)Delegate.Remove(this.eventHandler, value);
                if (this.eventHandler == null)
                {
                    this.Unsubscribe();
                }
            }
        }

        protected override void HandleDatagram(byte[] data, int offset, int size)
        {
            var handler = this.eventHandler;
            if (handler == null)
            {
                return;
            }

            using (var input = new MemoryStream(data, offset, size))
            {
                var received = (NetworkLocationServiceDataStructure)this.serializer.Deserialize(input);
                handler(this, new DataUpdateEventArgs<NetworkLocationServiceDataStructure>(received));
            }
        }
    }    

    internal partial class DistanceLocationServiceClientProxy : UdpClientProxyBase, IDistanceLocationService
    {
        private readonly XmlSerializer serializer;
        private EventHandler<DataUpdateEventArgs<DistanceLocationServiceDataStructure>> eventHandler;

        public DistanceLocationServiceClientProxy(IPEndPoint endPoint)
            : base(endPoint)
        {
            this.serializer = new XmlSerializer(typeof(DistanceLocationServiceDataStructure));
        }
        
        public event EventHandler<DataUpdateEventArgs<DistanceLocationServiceDataStructure>> DistanceLocationChanged
        {
            add
            {
                var subscribe = this.eventHandler == null;
                this.eventHandler = (EventHandler<DataUpdateEventArgs<DistanceLocationServiceDataStructure>>)Delegate.Combine(this.eventHandler, value);
                if (subscribe)
                {
                    this.Subscribe();
                }
            }

            remove
            {
                this.eventHandler = (EventHandler<DataUpdateEventArgs<DistanceLocationServiceDataStructure>>)Delegate.Remove(this.eventHandler, value);
                if (this.eventHandler == null)
                {
                    this.Unsubscribe();
                }
            }
        }

        protected override void HandleDatagram(byte[] data, int offset, int size)
        {
            var handler = this.eventHandler;
            if (handler == null)
            {
                return;
            }

            using (var input = new MemoryStream(data, offset, size))
            {
                var received = (DistanceLocationServiceDataStructure)this.serializer.Deserialize(input);
                handler(this, new DataUpdateEventArgs<DistanceLocationServiceDataStructure>(received));
            }
        }
    }    

    internal partial class GNSSLocationServiceClientProxy : UdpClientProxyBase, IGNSSLocationService
    {
        private readonly XmlSerializer serializer;
        private EventHandler<DataUpdateEventArgs<GNSSLocationServiceDataStructure>> eventHandler;

        public GNSSLocationServiceClientProxy(IPEndPoint endPoint)
            : base(endPoint)
        {
            this.serializer = new XmlSerializer(typeof(GNSSLocationServiceDataStructure));
        }
        
        public event EventHandler<DataUpdateEventArgs<GNSSLocationServiceDataStructure>> GNSSLocationChanged
        {
            add
            {
                var subscribe = this.eventHandler == null;
                this.eventHandler = (EventHandler<DataUpdateEventArgs<GNSSLocationServiceDataStructure>>)Delegate.Combine(this.eventHandler, value);
                if (subscribe)
                {
                    this.Subscribe();
                }
            }

            remove
            {
                this.eventHandler = (EventHandler<DataUpdateEventArgs<GNSSLocationServiceDataStructure>>)Delegate.Remove(this.eventHandler, value);
                if (this.eventHandler == null)
                {
                    this.Unsubscribe();
                }
            }
        }

        protected override void HandleDatagram(byte[] data, int offset, int size)
        {
            var handler = this.eventHandler;
            if (handler == null)
            {
                return;
            }

            using (var input = new MemoryStream(data, offset, size))
            {
                var received = (GNSSLocationServiceDataStructure)this.serializer.Deserialize(input);
                handler(this, new DataUpdateEventArgs<GNSSLocationServiceDataStructure>(received));
            }
        }
    }
}

