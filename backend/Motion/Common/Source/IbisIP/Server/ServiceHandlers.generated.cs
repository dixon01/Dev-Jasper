
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Protocols.Vdv301.Messages;
using Gorba.Common.Protocols.Vdv301.Services;

using Gorba.Common.Utility.Core;

using NLog;

namespace Gorba.Motion.Common.IbisIP.Server
{
    internal abstract partial class HttpServiceHandlerBase
    {
        public static HttpServiceHandlerBase Create<TService>(TService service)
            where TService : IVdv301HttpService
        {
            if (!(service is IVdv301ServiceImpl))
            {
                throw new ArgumentException("Service must implement IVdv301ServiceImpl");
            }
            
            var customerInformationService = service as ICustomerInformationService;
            if (customerInformationService != null)
            {
                return new CustomerInformationServiceHandler(customerInformationService);
            }
         
            var deviceManagementService = service as IDeviceManagementService;
            if (deviceManagementService != null)
            {
                return new DeviceManagementServiceHandler(deviceManagementService);
            }
         
            throw new NotSupportedException(string.Format("Service {0} not supported", service.GetType().FullName));
        }
    }
    
    internal abstract partial class UdpServiceHandlerBase
    {
        public static UdpServiceHandlerBase Create<TService>(TService service)
            where TService : IVdv301UdpService
        {
            if (!(service is IVdv301ServiceImpl))
            {
                throw new ArgumentException("Service must implement IVdv301ServiceImpl");
            }
            
            var networkLocationService = service as INetworkLocationService;
            if (networkLocationService != null)
            {
                return new NetworkLocationServiceHandler(networkLocationService);
            }
         
            var distanceLocationService = service as IDistanceLocationService;
            if (distanceLocationService != null)
            {
                return new DistanceLocationServiceHandler(distanceLocationService);
            }
         
            var gNSSLocationService = service as IGNSSLocationService;
            if (gNSSLocationService != null)
            {
                return new GNSSLocationServiceHandler(gNSSLocationService);
            }
         
            throw new NotSupportedException(string.Format("Service {0} not supported", service.GetType().FullName));
        }
    }
        

    internal partial class CustomerInformationServiceHandler : HttpServiceHandlerBase
    {
        private readonly ICustomerInformationService service;            
        private readonly List<Subscription<CustomerInformationServiceGetAllDataResponseStructure>> subscriptionsAllData;            
        private readonly List<Subscription<CustomerInformationServiceGetCurrentAnnouncementResponseStructure>> subscriptionsCurrentAnnouncement;            
        private readonly List<Subscription<CustomerInformationServiceGetCurrentConnectionInformationResponseStructure>> subscriptionsCurrentConnectionInformation;            
        private readonly List<Subscription<CustomerInformationServiceGetCurrentDisplayContentResponseStructure>> subscriptionsCurrentDisplayContent;            
        private readonly List<Subscription<CustomerInformationServiceGetCurrentStopPointResponseStructure>> subscriptionsCurrentStopPoint;            
        private readonly List<Subscription<CustomerInformationServiceGetCurrentStopIndexResponseStructure>> subscriptionsCurrentStopIndex;            
        private readonly List<Subscription<CustomerInformationServiceGetTripDataResponseStructure>> subscriptionsTripData;            
        private readonly List<Subscription<CustomerInformationServiceGetVehicleDataResponseStructure>> subscriptionsVehicleData;

        public CustomerInformationServiceHandler(ICustomerInformationService service)
        {
            this.service = service;            
            this.subscriptionsAllData = new List<Subscription<CustomerInformationServiceGetAllDataResponseStructure>>();            
            this.subscriptionsCurrentAnnouncement = new List<Subscription<CustomerInformationServiceGetCurrentAnnouncementResponseStructure>>();            
            this.subscriptionsCurrentConnectionInformation = new List<Subscription<CustomerInformationServiceGetCurrentConnectionInformationResponseStructure>>();            
            this.subscriptionsCurrentDisplayContent = new List<Subscription<CustomerInformationServiceGetCurrentDisplayContentResponseStructure>>();            
            this.subscriptionsCurrentStopPoint = new List<Subscription<CustomerInformationServiceGetCurrentStopPointResponseStructure>>();            
            this.subscriptionsCurrentStopIndex = new List<Subscription<CustomerInformationServiceGetCurrentStopIndexResponseStructure>>();            
            this.subscriptionsTripData = new List<Subscription<CustomerInformationServiceGetTripDataResponseStructure>>();            
            this.subscriptionsVehicleData = new List<Subscription<CustomerInformationServiceGetVehicleDataResponseStructure>>();
        }

        public override string Name
        {
            get
            {
                return "CustomerInformationService";
            }
        }

        public override void HandleListRequest(HttpServer.Request request)
        {
            using (var writer = new StreamWriter(request.GetResponse().GetResponseStream()))
            {
                writer.WriteLine("<html>");
                writer.WriteLine("<head><title>Operations of CustomerInformationService</title></head>");
                writer.WriteLine("<body>");
                writer.WriteLine("<h1>Operations of CustomerInformationService</h1>");
                writer.WriteLine("<ul>");            
                writer.WriteLine("<li><a href=\"GetAllData\">GetAllData</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeAllData\">SubscribeAllData</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeAllData\">UnsubscribeAllData</a></li>");            
                writer.WriteLine("<li><a href=\"GetCurrentAnnouncement\">GetCurrentAnnouncement</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeCurrentAnnouncement\">SubscribeCurrentAnnouncement</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeCurrentAnnouncement\">UnsubscribeCurrentAnnouncement</a></li>");            
                writer.WriteLine("<li><a href=\"GetCurrentConnectionInformation\">GetCurrentConnectionInformation</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeCurrentConnectionInformation\">SubscribeCurrentConnectionInformation</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeCurrentConnectionInformation\">UnsubscribeCurrentConnectionInformation</a></li>");            
                writer.WriteLine("<li><a href=\"GetCurrentDisplayContent\">GetCurrentDisplayContent</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeCurrentDisplayContent\">SubscribeCurrentDisplayContent</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeCurrentDisplayContent\">UnsubscribeCurrentDisplayContent</a></li>");            
                writer.WriteLine("<li><a href=\"GetCurrentStopPoint\">GetCurrentStopPoint</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeCurrentStopPoint\">SubscribeCurrentStopPoint</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeCurrentStopPoint\">UnsubscribeCurrentStopPoint</a></li>");            
                writer.WriteLine("<li><a href=\"GetCurrentStopIndex\">GetCurrentStopIndex</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeCurrentStopIndex\">SubscribeCurrentStopIndex</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeCurrentStopIndex\">UnsubscribeCurrentStopIndex</a></li>");            
                writer.WriteLine("<li><a href=\"GetTripData\">GetTripData</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeTripData\">SubscribeTripData</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeTripData\">UnsubscribeTripData</a></li>");            
                writer.WriteLine("<li><a href=\"GetVehicleData\">GetVehicleData</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeVehicleData\">SubscribeVehicleData</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeVehicleData\">UnsubscribeVehicleData</a></li>");
                writer.WriteLine("<li><a href=\"post.html?RetrievePartialStopSequence\">RetrievePartialStopSequence</a></li>");
                writer.WriteLine("</ul>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        public override void HandleRequest(string operationName, HttpServer.Request request)
        {
            switch (operationName)
            {
                case "GetAllData":
                    this.HandleGetAllData(request);
                    break;            
                case "SubscribeAllData":
                    this.HandleSubscribeAllData(request);
                    break;
                case "UnsubscribeAllData":
                    this.HandleUnsubscribeAllData(request);
                    break;
                case "GetCurrentAnnouncement":
                    this.HandleGetCurrentAnnouncement(request);
                    break;            
                case "SubscribeCurrentAnnouncement":
                    this.HandleSubscribeCurrentAnnouncement(request);
                    break;
                case "UnsubscribeCurrentAnnouncement":
                    this.HandleUnsubscribeCurrentAnnouncement(request);
                    break;
                case "GetCurrentConnectionInformation":
                    this.HandleGetCurrentConnectionInformation(request);
                    break;            
                case "SubscribeCurrentConnectionInformation":
                    this.HandleSubscribeCurrentConnectionInformation(request);
                    break;
                case "UnsubscribeCurrentConnectionInformation":
                    this.HandleUnsubscribeCurrentConnectionInformation(request);
                    break;
                case "GetCurrentDisplayContent":
                    this.HandleGetCurrentDisplayContent(request);
                    break;            
                case "SubscribeCurrentDisplayContent":
                    this.HandleSubscribeCurrentDisplayContent(request);
                    break;
                case "UnsubscribeCurrentDisplayContent":
                    this.HandleUnsubscribeCurrentDisplayContent(request);
                    break;
                case "GetCurrentStopPoint":
                    this.HandleGetCurrentStopPoint(request);
                    break;            
                case "SubscribeCurrentStopPoint":
                    this.HandleSubscribeCurrentStopPoint(request);
                    break;
                case "UnsubscribeCurrentStopPoint":
                    this.HandleUnsubscribeCurrentStopPoint(request);
                    break;
                case "GetCurrentStopIndex":
                    this.HandleGetCurrentStopIndex(request);
                    break;            
                case "SubscribeCurrentStopIndex":
                    this.HandleSubscribeCurrentStopIndex(request);
                    break;
                case "UnsubscribeCurrentStopIndex":
                    this.HandleUnsubscribeCurrentStopIndex(request);
                    break;
                case "GetTripData":
                    this.HandleGetTripData(request);
                    break;            
                case "SubscribeTripData":
                    this.HandleSubscribeTripData(request);
                    break;
                case "UnsubscribeTripData":
                    this.HandleUnsubscribeTripData(request);
                    break;
                case "GetVehicleData":
                    this.HandleGetVehicleData(request);
                    break;            
                case "SubscribeVehicleData":
                    this.HandleSubscribeVehicleData(request);
                    break;
                case "UnsubscribeVehicleData":
                    this.HandleUnsubscribeVehicleData(request);
                    break;
                case "RetrievePartialStopSequence":
                    this.HandleRetrievePartialStopSequence(request);
                    break;
                default:
                    throw new NotSupportedException("Operation " + operationName + " is not supported by CustomerInformationService");
            }
        }
        
        protected override object GetDefaultPostObject(string operationName)
        {
            if (operationName.StartsWith("Subscribe"))
            {
                return this.CreateDefaultPostObject<SubscribeRequestStructure>();
            }

            if (operationName.StartsWith("Unsubscribe"))
            {
                return this.CreateDefaultPostObject<UnsubscribeRequestStructure>();
            }
            
            switch (operationName)
            {
                case "RetrievePartialStopSequence":
                    return this.CreateDefaultPostObject<CustomerInformationServiceRetrievePartialStopSequenceRequestStructure>();
                default:
                    throw new NotSupportedException("Operation " + operationName + " is not supported by CustomerInformationService");
            }
        }
        

        private void HandleGetAllData(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetAllDataResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetAllData only supports GET");
                }
            
                var result = this.service.GetAllData();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetAllData, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeAllData(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsAllData, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsAllData.Count == 1)
            {
                this.service.AllDataChanged += this.ServiceOnAllDataChanged;
            }

            var value = new CustomerInformationServiceGetAllDataResponseStructure();
            value.Item = this.service.GetAllData();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeAllData(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsAllData, request);
            if (this.subscriptionsAllData.Count == 0)
            {
                this.service.AllDataChanged -= this.ServiceOnAllDataChanged;
            }
        }

        private void HandleGetCurrentAnnouncement(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetCurrentAnnouncementResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetCurrentAnnouncement only supports GET");
                }
            
                var result = this.service.GetCurrentAnnouncement();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetCurrentAnnouncement, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeCurrentAnnouncement(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsCurrentAnnouncement, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsCurrentAnnouncement.Count == 1)
            {
                this.service.CurrentAnnouncementChanged += this.ServiceOnCurrentAnnouncementChanged;
            }

            var value = new CustomerInformationServiceGetCurrentAnnouncementResponseStructure();
            value.Item = this.service.GetCurrentAnnouncement();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeCurrentAnnouncement(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsCurrentAnnouncement, request);
            if (this.subscriptionsCurrentAnnouncement.Count == 0)
            {
                this.service.CurrentAnnouncementChanged -= this.ServiceOnCurrentAnnouncementChanged;
            }
        }

        private void HandleGetCurrentConnectionInformation(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetCurrentConnectionInformationResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetCurrentConnectionInformation only supports GET");
                }
            
                var result = this.service.GetCurrentConnectionInformation();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetCurrentConnectionInformation, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeCurrentConnectionInformation(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsCurrentConnectionInformation, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsCurrentConnectionInformation.Count == 1)
            {
                this.service.CurrentConnectionInformationChanged += this.ServiceOnCurrentConnectionInformationChanged;
            }

            var value = new CustomerInformationServiceGetCurrentConnectionInformationResponseStructure();
            value.Item = this.service.GetCurrentConnectionInformation();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeCurrentConnectionInformation(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsCurrentConnectionInformation, request);
            if (this.subscriptionsCurrentConnectionInformation.Count == 0)
            {
                this.service.CurrentConnectionInformationChanged -= this.ServiceOnCurrentConnectionInformationChanged;
            }
        }

        private void HandleGetCurrentDisplayContent(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetCurrentDisplayContentResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetCurrentDisplayContent only supports GET");
                }
            
                var result = this.service.GetCurrentDisplayContent();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetCurrentDisplayContent, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeCurrentDisplayContent(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsCurrentDisplayContent, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsCurrentDisplayContent.Count == 1)
            {
                this.service.CurrentDisplayContentChanged += this.ServiceOnCurrentDisplayContentChanged;
            }

            var value = new CustomerInformationServiceGetCurrentDisplayContentResponseStructure();
            value.Item = this.service.GetCurrentDisplayContent();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeCurrentDisplayContent(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsCurrentDisplayContent, request);
            if (this.subscriptionsCurrentDisplayContent.Count == 0)
            {
                this.service.CurrentDisplayContentChanged -= this.ServiceOnCurrentDisplayContentChanged;
            }
        }

        private void HandleGetCurrentStopPoint(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetCurrentStopPointResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetCurrentStopPoint only supports GET");
                }
            
                var result = this.service.GetCurrentStopPoint();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetCurrentStopPoint, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeCurrentStopPoint(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsCurrentStopPoint, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsCurrentStopPoint.Count == 1)
            {
                this.service.CurrentStopPointChanged += this.ServiceOnCurrentStopPointChanged;
            }

            var value = new CustomerInformationServiceGetCurrentStopPointResponseStructure();
            value.Item = this.service.GetCurrentStopPoint();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeCurrentStopPoint(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsCurrentStopPoint, request);
            if (this.subscriptionsCurrentStopPoint.Count == 0)
            {
                this.service.CurrentStopPointChanged -= this.ServiceOnCurrentStopPointChanged;
            }
        }

        private void HandleGetCurrentStopIndex(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetCurrentStopIndexResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetCurrentStopIndex only supports GET");
                }
            
                var result = this.service.GetCurrentStopIndex();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetCurrentStopIndex, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeCurrentStopIndex(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsCurrentStopIndex, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsCurrentStopIndex.Count == 1)
            {
                this.service.CurrentStopIndexChanged += this.ServiceOnCurrentStopIndexChanged;
            }

            var value = new CustomerInformationServiceGetCurrentStopIndexResponseStructure();
            value.Item = this.service.GetCurrentStopIndex();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeCurrentStopIndex(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsCurrentStopIndex, request);
            if (this.subscriptionsCurrentStopIndex.Count == 0)
            {
                this.service.CurrentStopIndexChanged -= this.ServiceOnCurrentStopIndexChanged;
            }
        }

        private void HandleGetTripData(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetTripDataResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetTripData only supports GET");
                }
            
                var result = this.service.GetTripData();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetTripData, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeTripData(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsTripData, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsTripData.Count == 1)
            {
                this.service.TripDataChanged += this.ServiceOnTripDataChanged;
            }

            var value = new CustomerInformationServiceGetTripDataResponseStructure();
            value.Item = this.service.GetTripData();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeTripData(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsTripData, request);
            if (this.subscriptionsTripData.Count == 0)
            {
                this.service.TripDataChanged -= this.ServiceOnTripDataChanged;
            }
        }

        private void HandleGetVehicleData(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceGetVehicleDataResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetVehicleData only supports GET");
                }
            
                var result = this.service.GetVehicleData();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetVehicleData, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeVehicleData(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsVehicleData, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsVehicleData.Count == 1)
            {
                this.service.VehicleDataChanged += this.ServiceOnVehicleDataChanged;
            }

            var value = new CustomerInformationServiceGetVehicleDataResponseStructure();
            value.Item = this.service.GetVehicleData();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeVehicleData(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsVehicleData, request);
            if (this.subscriptionsVehicleData.Count == 0)
            {
                this.service.VehicleDataChanged -= this.ServiceOnVehicleDataChanged;
            }
        }

        private void HandleRetrievePartialStopSequence(HttpServer.Request request)
        {
            var resultWrapper = new CustomerInformationServiceRetrievePartialStopSequenceResponseStructure();
            try
            {
                if (request.Method != "POST")
                {
                    throw new NotSupportedException("RetrievePartialStopSequence only supports POST");
                }

                CustomerInformationServiceRetrievePartialStopSequenceRequestStructure args;
                using (var input = request.GetRequestStream())
                {
                    args = this.Deserialize<CustomerInformationServiceRetrievePartialStopSequenceRequestStructure>(input);
                }
                
                var result = this.service.RetrievePartialStopSequence(args.StartingStopIndex, args.NumberOfStopPoints);
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching RetrievePartialStopSequence, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void ServiceOnAllDataChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceAllData> e)
        {
            var value = new CustomerInformationServiceGetAllDataResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsAllData, value);
        }            

        private void ServiceOnCurrentAnnouncementChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData> e)
        {
            var value = new CustomerInformationServiceGetCurrentAnnouncementResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsCurrentAnnouncement, value);
        }            

        private void ServiceOnCurrentConnectionInformationChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData> e)
        {
            var value = new CustomerInformationServiceGetCurrentConnectionInformationResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsCurrentConnectionInformation, value);
        }            

        private void ServiceOnCurrentDisplayContentChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData> e)
        {
            var value = new CustomerInformationServiceGetCurrentDisplayContentResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsCurrentDisplayContent, value);
        }            

        private void ServiceOnCurrentStopPointChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData> e)
        {
            var value = new CustomerInformationServiceGetCurrentStopPointResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsCurrentStopPoint, value);
        }            

        private void ServiceOnCurrentStopIndexChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData> e)
        {
            var value = new CustomerInformationServiceGetCurrentStopIndexResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsCurrentStopIndex, value);
        }            

        private void ServiceOnTripDataChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceTripData> e)
        {
            var value = new CustomerInformationServiceGetTripDataResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsTripData, value);
        }            

        private void ServiceOnVehicleDataChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceVehicleData> e)
        {
            var value = new CustomerInformationServiceGetVehicleDataResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsVehicleData, value);
        }
    }
    

    internal partial class DeviceManagementServiceHandler : HttpServiceHandlerBase
    {
        private readonly IDeviceManagementService service;            
        private readonly List<Subscription<DeviceManagementServiceGetDeviceInformationResponseStructure>> subscriptionsDeviceInformation;            
        private readonly List<Subscription<DeviceManagementServiceGetDeviceConfigurationResponseStructure>> subscriptionsDeviceConfiguration;            
        private readonly List<Subscription<DeviceManagementServiceGetDeviceStatusResponseStructure>> subscriptionsDeviceStatus;            
        private readonly List<Subscription<DeviceManagementServiceGetDeviceErrorMessagesResponseStructure>> subscriptionsDeviceErrorMessages;            
        private readonly List<Subscription<DeviceManagementServiceGetServiceInformationResponseStructure>> subscriptionsServiceInformation;            
        private readonly List<Subscription<DeviceManagementServiceGetServiceStatusResponseStructure>> subscriptionsServiceStatus;

        public DeviceManagementServiceHandler(IDeviceManagementService service)
        {
            this.service = service;            
            this.subscriptionsDeviceInformation = new List<Subscription<DeviceManagementServiceGetDeviceInformationResponseStructure>>();            
            this.subscriptionsDeviceConfiguration = new List<Subscription<DeviceManagementServiceGetDeviceConfigurationResponseStructure>>();            
            this.subscriptionsDeviceStatus = new List<Subscription<DeviceManagementServiceGetDeviceStatusResponseStructure>>();            
            this.subscriptionsDeviceErrorMessages = new List<Subscription<DeviceManagementServiceGetDeviceErrorMessagesResponseStructure>>();            
            this.subscriptionsServiceInformation = new List<Subscription<DeviceManagementServiceGetServiceInformationResponseStructure>>();            
            this.subscriptionsServiceStatus = new List<Subscription<DeviceManagementServiceGetServiceStatusResponseStructure>>();
        }

        public override string Name
        {
            get
            {
                return "DeviceManagementService";
            }
        }

        public override void HandleListRequest(HttpServer.Request request)
        {
            using (var writer = new StreamWriter(request.GetResponse().GetResponseStream()))
            {
                writer.WriteLine("<html>");
                writer.WriteLine("<head><title>Operations of DeviceManagementService</title></head>");
                writer.WriteLine("<body>");
                writer.WriteLine("<h1>Operations of DeviceManagementService</h1>");
                writer.WriteLine("<ul>");            
                writer.WriteLine("<li><a href=\"GetDeviceInformation\">GetDeviceInformation</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeDeviceInformation\">SubscribeDeviceInformation</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeDeviceInformation\">UnsubscribeDeviceInformation</a></li>");            
                writer.WriteLine("<li><a href=\"GetDeviceConfiguration\">GetDeviceConfiguration</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeDeviceConfiguration\">SubscribeDeviceConfiguration</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeDeviceConfiguration\">UnsubscribeDeviceConfiguration</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SetDeviceConfiguration\">SetDeviceConfiguration</a></li>");            
                writer.WriteLine("<li><a href=\"GetDeviceStatus\">GetDeviceStatus</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeDeviceStatus\">SubscribeDeviceStatus</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeDeviceStatus\">UnsubscribeDeviceStatus</a></li>");            
                writer.WriteLine("<li><a href=\"GetDeviceErrorMessages\">GetDeviceErrorMessages</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeDeviceErrorMessages\">SubscribeDeviceErrorMessages</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeDeviceErrorMessages\">UnsubscribeDeviceErrorMessages</a></li>");
                writer.WriteLine("<li><a href=\"RestartDevice\">RestartDevice</a></li>");
                writer.WriteLine("<li><a href=\"DeactivateDevice\">DeactivateDevice</a></li>");
                writer.WriteLine("<li><a href=\"ActivateDevice\">ActivateDevice</a></li>");            
                writer.WriteLine("<li><a href=\"GetServiceInformation\">GetServiceInformation</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeServiceInformation\">SubscribeServiceInformation</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeServiceInformation\">UnsubscribeServiceInformation</a></li>");            
                writer.WriteLine("<li><a href=\"GetServiceStatus\">GetServiceStatus</a></li>");
                writer.WriteLine("<li><a href=\"post.html?SubscribeServiceStatus\">SubscribeServiceStatus</a></li>");
                writer.WriteLine("<li><a href=\"post.html?UnsubscribeServiceStatus\">UnsubscribeServiceStatus</a></li>");
                writer.WriteLine("<li><a href=\"post.html?StartService\">StartService</a></li>");
                writer.WriteLine("<li><a href=\"post.html?RestartService\">RestartService</a></li>");
                writer.WriteLine("<li><a href=\"post.html?StopService\">StopService</a></li>");
                writer.WriteLine("</ul>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        public override void HandleRequest(string operationName, HttpServer.Request request)
        {
            switch (operationName)
            {
                case "GetDeviceInformation":
                    this.HandleGetDeviceInformation(request);
                    break;            
                case "SubscribeDeviceInformation":
                    this.HandleSubscribeDeviceInformation(request);
                    break;
                case "UnsubscribeDeviceInformation":
                    this.HandleUnsubscribeDeviceInformation(request);
                    break;
                case "GetDeviceConfiguration":
                    this.HandleGetDeviceConfiguration(request);
                    break;            
                case "SubscribeDeviceConfiguration":
                    this.HandleSubscribeDeviceConfiguration(request);
                    break;
                case "UnsubscribeDeviceConfiguration":
                    this.HandleUnsubscribeDeviceConfiguration(request);
                    break;
                case "SetDeviceConfiguration":
                    this.HandleSetDeviceConfiguration(request);
                    break;
                case "GetDeviceStatus":
                    this.HandleGetDeviceStatus(request);
                    break;            
                case "SubscribeDeviceStatus":
                    this.HandleSubscribeDeviceStatus(request);
                    break;
                case "UnsubscribeDeviceStatus":
                    this.HandleUnsubscribeDeviceStatus(request);
                    break;
                case "GetDeviceErrorMessages":
                    this.HandleGetDeviceErrorMessages(request);
                    break;            
                case "SubscribeDeviceErrorMessages":
                    this.HandleSubscribeDeviceErrorMessages(request);
                    break;
                case "UnsubscribeDeviceErrorMessages":
                    this.HandleUnsubscribeDeviceErrorMessages(request);
                    break;
                case "RestartDevice":
                    this.HandleRestartDevice(request);
                    break;
                case "DeactivateDevice":
                    this.HandleDeactivateDevice(request);
                    break;
                case "ActivateDevice":
                    this.HandleActivateDevice(request);
                    break;
                case "GetServiceInformation":
                    this.HandleGetServiceInformation(request);
                    break;            
                case "SubscribeServiceInformation":
                    this.HandleSubscribeServiceInformation(request);
                    break;
                case "UnsubscribeServiceInformation":
                    this.HandleUnsubscribeServiceInformation(request);
                    break;
                case "GetServiceStatus":
                    this.HandleGetServiceStatus(request);
                    break;            
                case "SubscribeServiceStatus":
                    this.HandleSubscribeServiceStatus(request);
                    break;
                case "UnsubscribeServiceStatus":
                    this.HandleUnsubscribeServiceStatus(request);
                    break;
                case "StartService":
                    this.HandleStartService(request);
                    break;
                case "RestartService":
                    this.HandleRestartService(request);
                    break;
                case "StopService":
                    this.HandleStopService(request);
                    break;
                default:
                    throw new NotSupportedException("Operation " + operationName + " is not supported by DeviceManagementService");
            }
        }
        
        protected override object GetDefaultPostObject(string operationName)
        {
            if (operationName.StartsWith("Subscribe"))
            {
                return this.CreateDefaultPostObject<SubscribeRequestStructure>();
            }

            if (operationName.StartsWith("Unsubscribe"))
            {
                return this.CreateDefaultPostObject<UnsubscribeRequestStructure>();
            }
            
            switch (operationName)
            {
                case "SetDeviceConfiguration":
                    return this.CreateDefaultPostObject<DeviceManagementServiceSetDeviceConfigurationRequestStructure>();
                case "StartService":
                    return this.CreateDefaultPostObject<DeviceManagementServiceStartServiceRequestStructure>();
                case "RestartService":
                    return this.CreateDefaultPostObject<DeviceManagementServiceRestartServiceRequestStructure>();
                case "StopService":
                    return this.CreateDefaultPostObject<DeviceManagementServiceStopServiceRequestStructure>();
                default:
                    throw new NotSupportedException("Operation " + operationName + " is not supported by DeviceManagementService");
            }
        }
        

        private void HandleGetDeviceInformation(HttpServer.Request request)
        {
            var resultWrapper = new DeviceManagementServiceGetDeviceInformationResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetDeviceInformation only supports GET");
                }
            
                var result = this.service.GetDeviceInformation();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetDeviceInformation, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeDeviceInformation(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsDeviceInformation, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsDeviceInformation.Count == 1)
            {
                this.service.DeviceInformationChanged += this.ServiceOnDeviceInformationChanged;
            }

            var value = new DeviceManagementServiceGetDeviceInformationResponseStructure();
            value.Item = this.service.GetDeviceInformation();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeDeviceInformation(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsDeviceInformation, request);
            if (this.subscriptionsDeviceInformation.Count == 0)
            {
                this.service.DeviceInformationChanged -= this.ServiceOnDeviceInformationChanged;
            }
        }

        private void HandleGetDeviceConfiguration(HttpServer.Request request)
        {
            var resultWrapper = new DeviceManagementServiceGetDeviceConfigurationResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetDeviceConfiguration only supports GET");
                }
            
                var result = this.service.GetDeviceConfiguration();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetDeviceConfiguration, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeDeviceConfiguration(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsDeviceConfiguration, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsDeviceConfiguration.Count == 1)
            {
                this.service.DeviceConfigurationChanged += this.ServiceOnDeviceConfigurationChanged;
            }

            var value = new DeviceManagementServiceGetDeviceConfigurationResponseStructure();
            value.Item = this.service.GetDeviceConfiguration();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeDeviceConfiguration(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsDeviceConfiguration, request);
            if (this.subscriptionsDeviceConfiguration.Count == 0)
            {
                this.service.DeviceConfigurationChanged -= this.ServiceOnDeviceConfigurationChanged;
            }
        }

        private void HandleSetDeviceConfiguration(HttpServer.Request request)
        {
            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                if (request.Method != "POST")
                {
                    throw new NotSupportedException("SetDeviceConfiguration only supports POST");
                }

                DeviceManagementServiceSetDeviceConfigurationRequestStructure args;
                using (var input = request.GetRequestStream())
                {
                    args = this.Deserialize<DeviceManagementServiceSetDeviceConfigurationRequestStructure>(input);
                }
                
                var result = this.service.SetDeviceConfiguration(args.DeviceID);
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching SetDeviceConfiguration, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }

        private void HandleGetDeviceStatus(HttpServer.Request request)
        {
            var resultWrapper = new DeviceManagementServiceGetDeviceStatusResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetDeviceStatus only supports GET");
                }
            
                var result = this.service.GetDeviceStatus();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetDeviceStatus, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeDeviceStatus(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsDeviceStatus, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsDeviceStatus.Count == 1)
            {
                this.service.DeviceStatusChanged += this.ServiceOnDeviceStatusChanged;
            }

            var value = new DeviceManagementServiceGetDeviceStatusResponseStructure();
            value.Item = this.service.GetDeviceStatus();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeDeviceStatus(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsDeviceStatus, request);
            if (this.subscriptionsDeviceStatus.Count == 0)
            {
                this.service.DeviceStatusChanged -= this.ServiceOnDeviceStatusChanged;
            }
        }

        private void HandleGetDeviceErrorMessages(HttpServer.Request request)
        {
            var resultWrapper = new DeviceManagementServiceGetDeviceErrorMessagesResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetDeviceErrorMessages only supports GET");
                }
            
                var result = this.service.GetDeviceErrorMessages();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetDeviceErrorMessages, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeDeviceErrorMessages(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsDeviceErrorMessages, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsDeviceErrorMessages.Count == 1)
            {
                this.service.DeviceErrorMessagesChanged += this.ServiceOnDeviceErrorMessagesChanged;
            }

            var value = new DeviceManagementServiceGetDeviceErrorMessagesResponseStructure();
            value.Item = this.service.GetDeviceErrorMessages();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeDeviceErrorMessages(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsDeviceErrorMessages, request);
            if (this.subscriptionsDeviceErrorMessages.Count == 0)
            {
                this.service.DeviceErrorMessagesChanged -= this.ServiceOnDeviceErrorMessagesChanged;
            }
        }

        private void HandleRestartDevice(HttpServer.Request request)
        {
            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("RestartDevice only supports GET");
                }
            
                var result = this.service.RestartDevice();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching RestartDevice, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }

        private void HandleDeactivateDevice(HttpServer.Request request)
        {
            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("DeactivateDevice only supports GET");
                }
            
                var result = this.service.DeactivateDevice();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching DeactivateDevice, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }

        private void HandleActivateDevice(HttpServer.Request request)
        {
            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("ActivateDevice only supports GET");
                }
            
                var result = this.service.ActivateDevice();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching ActivateDevice, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }

        private void HandleGetServiceInformation(HttpServer.Request request)
        {
            var resultWrapper = new DeviceManagementServiceGetServiceInformationResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetServiceInformation only supports GET");
                }
            
                var result = this.service.GetServiceInformation();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetServiceInformation, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeServiceInformation(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsServiceInformation, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsServiceInformation.Count == 1)
            {
                this.service.ServiceInformationChanged += this.ServiceOnServiceInformationChanged;
            }

            var value = new DeviceManagementServiceGetServiceInformationResponseStructure();
            value.Item = this.service.GetServiceInformation();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeServiceInformation(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsServiceInformation, request);
            if (this.subscriptionsServiceInformation.Count == 0)
            {
                this.service.ServiceInformationChanged -= this.ServiceOnServiceInformationChanged;
            }
        }

        private void HandleGetServiceStatus(HttpServer.Request request)
        {
            var resultWrapper = new DeviceManagementServiceGetServiceStatusResponseStructure();
            try
            {
                if (request.Method != "GET")
                {
                    throw new NotSupportedException("GetServiceStatus only supports GET");
                }
            
                var result = this.service.GetServiceStatus();
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching GetServiceStatus, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void HandleSubscribeServiceStatus(HttpServer.Request request)
        {
            var subscription = this.AddSubscription(this.subscriptionsServiceStatus, request);
            if (subscription == null)
            {
                return;
            }

            if (this.subscriptionsServiceStatus.Count == 1)
            {
                this.service.ServiceStatusChanged += this.ServiceOnServiceStatusChanged;
            }

            var value = new DeviceManagementServiceGetServiceStatusResponseStructure();
            value.Item = this.service.GetServiceStatus();
            subscription.Notify(value);
        }
            
        private void HandleUnsubscribeServiceStatus(HttpServer.Request request)
        {
            this.RemoveSubscription(this.subscriptionsServiceStatus, request);
            if (this.subscriptionsServiceStatus.Count == 0)
            {
                this.service.ServiceStatusChanged -= this.ServiceOnServiceStatusChanged;
            }
        }

        private void HandleStartService(HttpServer.Request request)
        {
            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                if (request.Method != "POST")
                {
                    throw new NotSupportedException("StartService only supports POST");
                }

                DeviceManagementServiceStartServiceRequestStructure args;
                using (var input = request.GetRequestStream())
                {
                    args = this.Deserialize<DeviceManagementServiceStartServiceRequestStructure>(input);
                }
                
                var result = this.service.StartService(args.ServiceSpecification);
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching StartService, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }

        private void HandleRestartService(HttpServer.Request request)
        {
            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                if (request.Method != "POST")
                {
                    throw new NotSupportedException("RestartService only supports POST");
                }

                DeviceManagementServiceRestartServiceRequestStructure args;
                using (var input = request.GetRequestStream())
                {
                    args = this.Deserialize<DeviceManagementServiceRestartServiceRequestStructure>(input);
                }
                
                var result = this.service.RestartService(args.ServiceSpecification);
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching RestartService, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }

        private void HandleStopService(HttpServer.Request request)
        {
            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                if (request.Method != "POST")
                {
                    throw new NotSupportedException("StopService only supports POST");
                }

                DeviceManagementServiceStopServiceRequestStructure args;
                using (var input = request.GetRequestStream())
                {
                    args = this.Deserialize<DeviceManagementServiceStopServiceRequestStructure>(input);
                }
                
                var result = this.service.StopService(args.ServiceSpecification);
                resultWrapper.Item = result;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching StopService, returning error");
                resultWrapper.Item = new IBISIPstring
                    {
                        Value = ex.GetType().Name + ": " + ex.Message
                    };
            }
            
            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }            

        private void ServiceOnDeviceInformationChanged(object sender, DataUpdateEventArgs<DeviceManagementServiceGetDeviceInformationResponseDataStructure> e)
        {
            var value = new DeviceManagementServiceGetDeviceInformationResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsDeviceInformation, value);
        }            

        private void ServiceOnDeviceConfigurationChanged(object sender, DataUpdateEventArgs<DeviceManagementServiceGetDeviceConfigurationResponseDataStructure> e)
        {
            var value = new DeviceManagementServiceGetDeviceConfigurationResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsDeviceConfiguration, value);
        }            

        private void ServiceOnDeviceStatusChanged(object sender, DataUpdateEventArgs<DeviceManagementServiceGetDeviceStatusResponseDataStructure> e)
        {
            var value = new DeviceManagementServiceGetDeviceStatusResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsDeviceStatus, value);
        }            

        private void ServiceOnDeviceErrorMessagesChanged(object sender, DataUpdateEventArgs<DeviceManagementServiceGetDeviceErrorMessagesResponseDataStructure> e)
        {
            var value = new DeviceManagementServiceGetDeviceErrorMessagesResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsDeviceErrorMessages, value);
        }            

        private void ServiceOnServiceInformationChanged(object sender, DataUpdateEventArgs<DeviceManagementServiceGetServiceInformationResponseDataStructure> e)
        {
            var value = new DeviceManagementServiceGetServiceInformationResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsServiceInformation, value);
        }            

        private void ServiceOnServiceStatusChanged(object sender, DataUpdateEventArgs<DeviceManagementServiceGetServiceStatusResponseDataStructure> e)
        {
            var value = new DeviceManagementServiceGetServiceStatusResponseStructure();
            value.Item = e.Value;
            this.NotifySubscriptions(this.subscriptionsServiceStatus, value);
        }
    }
    

    internal partial class NetworkLocationServiceHandler : UdpServiceHandlerBase
    {
        private readonly INetworkLocationService service;

        public NetworkLocationServiceHandler(INetworkLocationService service)
        {
            this.service = service;
        }

        public override string Name
        {
            get
            {
                return "NetworkLocationService";
            }
        }

        public override void Start()
        {
            base.Start();
            this.service.NetworkLocationChanged += this.ServiceOnNetworkLocationChanged;
        }

        public override void Stop()
        {
            this.service.NetworkLocationChanged -= this.ServiceOnNetworkLocationChanged;
            base.Stop();
        }

        private void ServiceOnNetworkLocationChanged(object sender, DataUpdateEventArgs<NetworkLocationServiceDataStructure> e)
        {
            this.SendDatagram(e.Value);
        }
    }    

    internal partial class DistanceLocationServiceHandler : UdpServiceHandlerBase
    {
        private readonly IDistanceLocationService service;

        public DistanceLocationServiceHandler(IDistanceLocationService service)
        {
            this.service = service;
        }

        public override string Name
        {
            get
            {
                return "DistanceLocationService";
            }
        }

        public override void Start()
        {
            base.Start();
            this.service.DistanceLocationChanged += this.ServiceOnDistanceLocationChanged;
        }

        public override void Stop()
        {
            this.service.DistanceLocationChanged -= this.ServiceOnDistanceLocationChanged;
            base.Stop();
        }

        private void ServiceOnDistanceLocationChanged(object sender, DataUpdateEventArgs<DistanceLocationServiceDataStructure> e)
        {
            this.SendDatagram(e.Value);
        }
    }    

    internal partial class GNSSLocationServiceHandler : UdpServiceHandlerBase
    {
        private readonly IGNSSLocationService service;

        public GNSSLocationServiceHandler(IGNSSLocationService service)
        {
            this.service = service;
        }

        public override string Name
        {
            get
            {
                return "GNSSLocationService";
            }
        }

        public override void Start()
        {
            base.Start();
            this.service.GNSSLocationChanged += this.ServiceOnGNSSLocationChanged;
        }

        public override void Stop()
        {
            this.service.GNSSLocationChanged -= this.ServiceOnGNSSLocationChanged;
            base.Stop();
        }

        private void ServiceOnGNSSLocationChanged(object sender, DataUpdateEventArgs<GNSSLocationServiceDataStructure> e)
        {
            this.SendDatagram(e.Value);
        }
    }}

