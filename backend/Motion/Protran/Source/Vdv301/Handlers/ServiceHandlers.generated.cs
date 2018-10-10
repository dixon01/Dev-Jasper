

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Protran.VDV301;
using Gorba.Common.Protocols.Ximple;

using Gorba.Common.Protocols.Vdv301.Messages;
using Gorba.Common.Protocols.Vdv301.Services;

using Gorba.Common.Utility.Core;

using Gorba.Motion.Common.IbisIP;

using Gorba.Motion.Protran.Core.Utils;

using NLog;

namespace Gorba.Motion.Protran.Vdv301.Handlers
{    
    public partial class CustomerInformationServiceHandler : ServiceHandlerBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<CustomerInformationServiceHandler>();

        private readonly object serviceLock = new object();

        private readonly DataContext dataContext = new DataContext();

        private IHandlerContext context;

        private CustomerInformationServiceConfig config;
        
        private GetAllDataHandler handlerGetAllData;
        private ITimer timeoutGetAllData;
                
        private GetCurrentAnnouncementHandler handlerGetCurrentAnnouncement;
        private ITimer timeoutGetCurrentAnnouncement;
                
        private GetCurrentConnectionInformationHandler handlerGetCurrentConnectionInformation;
        private ITimer timeoutGetCurrentConnectionInformation;
                
        private GetCurrentDisplayContentHandler handlerGetCurrentDisplayContent;
        private ITimer timeoutGetCurrentDisplayContent;
                
        private GetCurrentStopPointHandler handlerGetCurrentStopPoint;
        private ITimer timeoutGetCurrentStopPoint;
                
        private GetCurrentStopIndexHandler handlerGetCurrentStopIndex;
        private ITimer timeoutGetCurrentStopIndex;
                
        private GetTripDataHandler handlerGetTripData;
        private ITimer timeoutGetTripData;
                
        private GetVehicleDataHandler handlerGetVehicleData;
        private ITimer timeoutGetVehicleData;
        
        private ICustomerInformationService service;

        public override void Configure(IHandlerContext context)
        {
            this.context = context;
            this.config = context.Config.Services.CustomerInformationService;

            if (this.config.GetAllData != null && this.config.GetAllData.Subscribe)
            {
                if (this.config.GetAllData.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetAllData = TimerFactory.Current.CreateTimer("Timeout-GetAllData");
                    this.timeoutGetAllData.Interval = this.config.GetAllData.SubscriptionTimeout;
                    this.timeoutGetAllData.Elapsed += this.TimeoutGetAllDataOnElapsed;
                }

                this.handlerGetAllData = new GetAllDataHandler(
                    this.config.GetAllData, this.HandlerFactory, context);
            }

            if (this.config.GetCurrentAnnouncement != null && this.config.GetCurrentAnnouncement.Subscribe)
            {
                if (this.config.GetCurrentAnnouncement.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetCurrentAnnouncement = TimerFactory.Current.CreateTimer("Timeout-GetCurrentAnnouncement");
                    this.timeoutGetCurrentAnnouncement.Interval = this.config.GetCurrentAnnouncement.SubscriptionTimeout;
                    this.timeoutGetCurrentAnnouncement.Elapsed += this.TimeoutGetCurrentAnnouncementOnElapsed;
                }

                this.handlerGetCurrentAnnouncement = new GetCurrentAnnouncementHandler(
                    this.config.GetCurrentAnnouncement, this.HandlerFactory, context);
            }

            if (this.config.GetCurrentConnectionInformation != null && this.config.GetCurrentConnectionInformation.Subscribe)
            {
                if (this.config.GetCurrentConnectionInformation.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetCurrentConnectionInformation = TimerFactory.Current.CreateTimer("Timeout-GetCurrentConnectionInformation");
                    this.timeoutGetCurrentConnectionInformation.Interval = this.config.GetCurrentConnectionInformation.SubscriptionTimeout;
                    this.timeoutGetCurrentConnectionInformation.Elapsed += this.TimeoutGetCurrentConnectionInformationOnElapsed;
                }

                this.handlerGetCurrentConnectionInformation = new GetCurrentConnectionInformationHandler(
                    this.config.GetCurrentConnectionInformation, this.HandlerFactory, context);
            }

            if (this.config.GetCurrentDisplayContent != null && this.config.GetCurrentDisplayContent.Subscribe)
            {
                if (this.config.GetCurrentDisplayContent.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetCurrentDisplayContent = TimerFactory.Current.CreateTimer("Timeout-GetCurrentDisplayContent");
                    this.timeoutGetCurrentDisplayContent.Interval = this.config.GetCurrentDisplayContent.SubscriptionTimeout;
                    this.timeoutGetCurrentDisplayContent.Elapsed += this.TimeoutGetCurrentDisplayContentOnElapsed;
                }

                this.handlerGetCurrentDisplayContent = new GetCurrentDisplayContentHandler(
                    this.config.GetCurrentDisplayContent, this.HandlerFactory, context);
            }

            if (this.config.GetCurrentStopPoint != null && this.config.GetCurrentStopPoint.Subscribe)
            {
                if (this.config.GetCurrentStopPoint.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetCurrentStopPoint = TimerFactory.Current.CreateTimer("Timeout-GetCurrentStopPoint");
                    this.timeoutGetCurrentStopPoint.Interval = this.config.GetCurrentStopPoint.SubscriptionTimeout;
                    this.timeoutGetCurrentStopPoint.Elapsed += this.TimeoutGetCurrentStopPointOnElapsed;
                }

                this.handlerGetCurrentStopPoint = new GetCurrentStopPointHandler(
                    this.config.GetCurrentStopPoint, this.HandlerFactory, context);
            }

            if (this.config.GetCurrentStopIndex != null && this.config.GetCurrentStopIndex.Subscribe)
            {
                if (this.config.GetCurrentStopIndex.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetCurrentStopIndex = TimerFactory.Current.CreateTimer("Timeout-GetCurrentStopIndex");
                    this.timeoutGetCurrentStopIndex.Interval = this.config.GetCurrentStopIndex.SubscriptionTimeout;
                    this.timeoutGetCurrentStopIndex.Elapsed += this.TimeoutGetCurrentStopIndexOnElapsed;
                }

                this.handlerGetCurrentStopIndex = new GetCurrentStopIndexHandler(
                    this.config.GetCurrentStopIndex, this.HandlerFactory, context);
            }

            if (this.config.GetTripData != null && this.config.GetTripData.Subscribe)
            {
                if (this.config.GetTripData.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetTripData = TimerFactory.Current.CreateTimer("Timeout-GetTripData");
                    this.timeoutGetTripData.Interval = this.config.GetTripData.SubscriptionTimeout;
                    this.timeoutGetTripData.Elapsed += this.TimeoutGetTripDataOnElapsed;
                }

                this.handlerGetTripData = new GetTripDataHandler(
                    this.config.GetTripData, this.HandlerFactory, context);
            }

            if (this.config.GetVehicleData != null && this.config.GetVehicleData.Subscribe)
            {
                if (this.config.GetVehicleData.SubscriptionTimeout > TimeSpan.Zero)
                {
                    this.timeoutGetVehicleData = TimerFactory.Current.CreateTimer("Timeout-GetVehicleData");
                    this.timeoutGetVehicleData.Interval = this.config.GetVehicleData.SubscriptionTimeout;
                    this.timeoutGetVehicleData.Elapsed += this.TimeoutGetVehicleDataOnElapsed;
                }

                this.handlerGetVehicleData = new GetVehicleDataHandler(
                    this.config.GetVehicleData, this.HandlerFactory, context);
            }
        }

        public override void Start()
        {
            this.context.CustomerInformationServiceChanged += this.ContextOnCustomerInformationServiceChanged;

            if (this.context.CustomerInformationService == null)
            {
                return;
            }

            this.Subscribe();
        }

        public override void Stop()
        {
            this.context.CustomerInformationServiceChanged -= this.ContextOnCustomerInformationServiceChanged;

            this.Unsubscribe();
        }

        private void Subscribe()
        {
            this.service = this.context.CustomerInformationService;
            if (this.service == null)
            {
                return;
            }
            
            if (this.config.GetAllData != null && this.config.GetAllData.Subscribe)
            {
                if (this.timeoutGetAllData != null)
                {
                    this.timeoutGetAllData.Enabled = true;
                }

                this.service.AllDataChanged += this.ServiceOnAllDataChanged;
            }
        
            if (this.config.GetCurrentAnnouncement != null && this.config.GetCurrentAnnouncement.Subscribe)
            {
                if (this.timeoutGetCurrentAnnouncement != null)
                {
                    this.timeoutGetCurrentAnnouncement.Enabled = true;
                }

                this.service.CurrentAnnouncementChanged += this.ServiceOnCurrentAnnouncementChanged;
            }
        
            if (this.config.GetCurrentConnectionInformation != null && this.config.GetCurrentConnectionInformation.Subscribe)
            {
                if (this.timeoutGetCurrentConnectionInformation != null)
                {
                    this.timeoutGetCurrentConnectionInformation.Enabled = true;
                }

                this.service.CurrentConnectionInformationChanged += this.ServiceOnCurrentConnectionInformationChanged;
            }
        
            if (this.config.GetCurrentDisplayContent != null && this.config.GetCurrentDisplayContent.Subscribe)
            {
                if (this.timeoutGetCurrentDisplayContent != null)
                {
                    this.timeoutGetCurrentDisplayContent.Enabled = true;
                }

                this.service.CurrentDisplayContentChanged += this.ServiceOnCurrentDisplayContentChanged;
            }
        
            if (this.config.GetCurrentStopPoint != null && this.config.GetCurrentStopPoint.Subscribe)
            {
                if (this.timeoutGetCurrentStopPoint != null)
                {
                    this.timeoutGetCurrentStopPoint.Enabled = true;
                }

                this.service.CurrentStopPointChanged += this.ServiceOnCurrentStopPointChanged;
            }
        
            if (this.config.GetCurrentStopIndex != null && this.config.GetCurrentStopIndex.Subscribe)
            {
                if (this.timeoutGetCurrentStopIndex != null)
                {
                    this.timeoutGetCurrentStopIndex.Enabled = true;
                }

                this.service.CurrentStopIndexChanged += this.ServiceOnCurrentStopIndexChanged;
            }
        
            if (this.config.GetTripData != null && this.config.GetTripData.Subscribe)
            {
                if (this.timeoutGetTripData != null)
                {
                    this.timeoutGetTripData.Enabled = true;
                }

                this.service.TripDataChanged += this.ServiceOnTripDataChanged;
            }
        
            if (this.config.GetVehicleData != null && this.config.GetVehicleData.Subscribe)
            {
                if (this.timeoutGetVehicleData != null)
                {
                    this.timeoutGetVehicleData.Enabled = true;
                }

                this.service.VehicleDataChanged += this.ServiceOnVehicleDataChanged;
            }
        
        }

        private void Unsubscribe()
        {
            if (this.service == null)
            {
                return;
            }
            
            if (this.config.GetAllData != null && this.config.GetAllData.Subscribe)
            {
                if (this.timeoutGetAllData != null)
                {
                    this.timeoutGetAllData.Enabled = false;
                }

                this.service.AllDataChanged -= this.ServiceOnAllDataChanged;
            }
        
            if (this.config.GetCurrentAnnouncement != null && this.config.GetCurrentAnnouncement.Subscribe)
            {
                if (this.timeoutGetCurrentAnnouncement != null)
                {
                    this.timeoutGetCurrentAnnouncement.Enabled = false;
                }

                this.service.CurrentAnnouncementChanged -= this.ServiceOnCurrentAnnouncementChanged;
            }
        
            if (this.config.GetCurrentConnectionInformation != null && this.config.GetCurrentConnectionInformation.Subscribe)
            {
                if (this.timeoutGetCurrentConnectionInformation != null)
                {
                    this.timeoutGetCurrentConnectionInformation.Enabled = false;
                }

                this.service.CurrentConnectionInformationChanged -= this.ServiceOnCurrentConnectionInformationChanged;
            }
        
            if (this.config.GetCurrentDisplayContent != null && this.config.GetCurrentDisplayContent.Subscribe)
            {
                if (this.timeoutGetCurrentDisplayContent != null)
                {
                    this.timeoutGetCurrentDisplayContent.Enabled = false;
                }

                this.service.CurrentDisplayContentChanged -= this.ServiceOnCurrentDisplayContentChanged;
            }
        
            if (this.config.GetCurrentStopPoint != null && this.config.GetCurrentStopPoint.Subscribe)
            {
                if (this.timeoutGetCurrentStopPoint != null)
                {
                    this.timeoutGetCurrentStopPoint.Enabled = false;
                }

                this.service.CurrentStopPointChanged -= this.ServiceOnCurrentStopPointChanged;
            }
        
            if (this.config.GetCurrentStopIndex != null && this.config.GetCurrentStopIndex.Subscribe)
            {
                if (this.timeoutGetCurrentStopIndex != null)
                {
                    this.timeoutGetCurrentStopIndex.Enabled = false;
                }

                this.service.CurrentStopIndexChanged -= this.ServiceOnCurrentStopIndexChanged;
            }
        
            if (this.config.GetTripData != null && this.config.GetTripData.Subscribe)
            {
                if (this.timeoutGetTripData != null)
                {
                    this.timeoutGetTripData.Enabled = false;
                }

                this.service.TripDataChanged -= this.ServiceOnTripDataChanged;
            }
        
            if (this.config.GetVehicleData != null && this.config.GetVehicleData.Subscribe)
            {
                if (this.timeoutGetVehicleData != null)
                {
                    this.timeoutGetVehicleData.Enabled = false;
                }

                this.service.VehicleDataChanged -= this.ServiceOnVehicleDataChanged;
            }
        
            this.service = null;
        }

        private void ContextOnCustomerInformationServiceChanged(object sender, EventArgs e)
        {
            this.Unsubscribe();
            this.Subscribe();
        }
        

        private void ServiceOnAllDataChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceAllData> e)
        {
            if (this.timeoutGetAllData != null)
            {
                this.timeoutGetAllData.Enabled = false;
                this.timeoutGetAllData.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetAllData.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetAllData.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetAllDataOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetAllData ({0}), re-subscribing to the entire service",
                this.config.GetAllData.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        private void ServiceOnCurrentAnnouncementChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData> e)
        {
            if (this.timeoutGetCurrentAnnouncement != null)
            {
                this.timeoutGetCurrentAnnouncement.Enabled = false;
                this.timeoutGetCurrentAnnouncement.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetCurrentAnnouncement.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetCurrentAnnouncement.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetCurrentAnnouncementOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetCurrentAnnouncement ({0}), re-subscribing to the entire service",
                this.config.GetCurrentAnnouncement.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        private void ServiceOnCurrentConnectionInformationChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData> e)
        {
            if (this.timeoutGetCurrentConnectionInformation != null)
            {
                this.timeoutGetCurrentConnectionInformation.Enabled = false;
                this.timeoutGetCurrentConnectionInformation.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetCurrentConnectionInformation.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetCurrentConnectionInformation.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetCurrentConnectionInformationOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetCurrentConnectionInformation ({0}), re-subscribing to the entire service",
                this.config.GetCurrentConnectionInformation.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        private void ServiceOnCurrentDisplayContentChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData> e)
        {
            if (this.timeoutGetCurrentDisplayContent != null)
            {
                this.timeoutGetCurrentDisplayContent.Enabled = false;
                this.timeoutGetCurrentDisplayContent.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetCurrentDisplayContent.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetCurrentDisplayContent.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetCurrentDisplayContentOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetCurrentDisplayContent ({0}), re-subscribing to the entire service",
                this.config.GetCurrentDisplayContent.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        private void ServiceOnCurrentStopPointChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData> e)
        {
            if (this.timeoutGetCurrentStopPoint != null)
            {
                this.timeoutGetCurrentStopPoint.Enabled = false;
                this.timeoutGetCurrentStopPoint.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetCurrentStopPoint.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetCurrentStopPoint.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetCurrentStopPointOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetCurrentStopPoint ({0}), re-subscribing to the entire service",
                this.config.GetCurrentStopPoint.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        private void ServiceOnCurrentStopIndexChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData> e)
        {
            if (this.timeoutGetCurrentStopIndex != null)
            {
                this.timeoutGetCurrentStopIndex.Enabled = false;
                this.timeoutGetCurrentStopIndex.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetCurrentStopIndex.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetCurrentStopIndex.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetCurrentStopIndexOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetCurrentStopIndex ({0}), re-subscribing to the entire service",
                this.config.GetCurrentStopIndex.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        private void ServiceOnTripDataChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceTripData> e)
        {
            if (this.timeoutGetTripData != null)
            {
                this.timeoutGetTripData.Enabled = false;
                this.timeoutGetTripData.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetTripData.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetTripData.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetTripDataOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetTripData ({0}), re-subscribing to the entire service",
                this.config.GetTripData.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        private void ServiceOnVehicleDataChanged(object sender, DataUpdateEventArgs<CustomerInformationServiceVehicleData> e)
        {
            if (this.timeoutGetVehicleData != null)
            {
                this.timeoutGetVehicleData.Enabled = false;
                this.timeoutGetVehicleData.Enabled = true;
            }

            this.RaiseDataUpdated(new DataUpdateEventArgs<object>(e.Value));
            lock (this.serviceLock)
            {
                this.handlerGetVehicleData.PrepareData(e.Value, 0, this.dataContext);
                var ximple = new Ximple();
                this.handlerGetVehicleData.HandleData(e.Value, ximple, 0, this.dataContext);
                if (ximple.Cells.Count > 0)
                {
                    this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
                }
            }
        }
        
        private void TimeoutGetVehicleDataOnElapsed(object sender, EventArgs e)
        {
            Logger.Warn(
                "Subscription timeout on GetVehicleData ({0}), re-subscribing to the entire service",
                this.config.GetVehicleData.SubscriptionTimeout);
            this.Unsubscribe();
            this.Subscribe();
        }

        /// <summary>
        /// This class can be extended to share data between the different handlers
        /// </summary>
        private partial class DataContext
        {
        }

        private partial class GetAllDataHandler
        {
            private readonly CustomerInformationServiceConfig.GetAllDataConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly List<ElementHandler> handlersVehicleRef;

            private readonly List<ElementHandler> handlersDefaultLanguage;

            private readonly TripInformationStructureHandler handlerTripInformation;

            private readonly List<ElementHandler> handlersCurrentStopIndex;

            private readonly List<ElementHandler> handlersRouteDeviation;

            private readonly List<ElementHandler> handlersDoorState;

            private readonly List<ElementHandler> handlersInPanic;

            private readonly List<ElementHandler> handlersVehicleStopRequested;

            private readonly List<ElementHandler> handlersExitSide;

            private readonly List<ElementHandler> handlersMovingDirectionForward;

            private readonly List<ElementHandler> handlersVehicleMode;

            public GetAllDataHandler(
                CustomerInformationServiceConfig.GetAllDataConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.VehicleRef != null && this.config.VehicleRef.Count > 0)
                {
                    this.handlersVehicleRef = new List<ElementHandler>(this.config.VehicleRef.Count);
                    foreach (var child in this.config.VehicleRef)
                    {
                        this.handlersVehicleRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DefaultLanguage != null && this.config.DefaultLanguage.Count > 0)
                {
                    this.handlersDefaultLanguage = new List<ElementHandler>(this.config.DefaultLanguage.Count);
                    foreach (var child in this.config.DefaultLanguage)
                    {
                        this.handlersDefaultLanguage.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.TripInformation != null)
                {
                    this.handlerTripInformation = new TripInformationStructureHandler(
                        this.config.TripInformation, handlerFactory, context);
                }

                if (this.config.CurrentStopIndex != null && this.config.CurrentStopIndex.Count > 0)
                {
                    this.handlersCurrentStopIndex = new List<ElementHandler>(this.config.CurrentStopIndex.Count);
                    foreach (var child in this.config.CurrentStopIndex)
                    {
                        this.handlersCurrentStopIndex.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.RouteDeviation != null && this.config.RouteDeviation.Count > 0)
                {
                    this.handlersRouteDeviation = new List<ElementHandler>(this.config.RouteDeviation.Count);
                    foreach (var child in this.config.RouteDeviation)
                    {
                        this.handlersRouteDeviation.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DoorState != null && this.config.DoorState.Count > 0)
                {
                    this.handlersDoorState = new List<ElementHandler>(this.config.DoorState.Count);
                    foreach (var child in this.config.DoorState)
                    {
                        this.handlersDoorState.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.InPanic != null && this.config.InPanic.Count > 0)
                {
                    this.handlersInPanic = new List<ElementHandler>(this.config.InPanic.Count);
                    foreach (var child in this.config.InPanic)
                    {
                        this.handlersInPanic.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.VehicleStopRequested != null && this.config.VehicleStopRequested.Count > 0)
                {
                    this.handlersVehicleStopRequested = new List<ElementHandler>(this.config.VehicleStopRequested.Count);
                    foreach (var child in this.config.VehicleStopRequested)
                    {
                        this.handlersVehicleStopRequested.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.ExitSide != null && this.config.ExitSide.Count > 0)
                {
                    this.handlersExitSide = new List<ElementHandler>(this.config.ExitSide.Count);
                    foreach (var child in this.config.ExitSide)
                    {
                        this.handlersExitSide.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.MovingDirectionForward != null && this.config.MovingDirectionForward.Count > 0)
                {
                    this.handlersMovingDirectionForward = new List<ElementHandler>(this.config.MovingDirectionForward.Count);
                    foreach (var child in this.config.MovingDirectionForward)
                    {
                        this.handlersMovingDirectionForward.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.VehicleMode != null && this.config.VehicleMode.Count > 0)
                {
                    this.handlersVehicleMode = new List<ElementHandler>(this.config.VehicleMode.Count);
                    foreach (var child in this.config.VehicleMode)
                    {
                        this.handlersVehicleMode.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(CustomerInformationServiceAllData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                this.PrepareVehicleRef(data.VehicleRef, row, dataContext);

                this.PrepareDefaultLanguage(data.DefaultLanguage, row, dataContext);

                bool prepared = false;
                this.PrepareTripInformation(data.TripInformation, row, dataContext, ref prepared);
                if (!prepared && this.handlerTripInformation != null && data.TripInformation != null && data.TripInformation.Length > 0)
                {
                    this.handlerTripInformation.PrepareData(data.TripInformation[0], row, dataContext);
                }

                this.PrepareCurrentStopIndex(data.CurrentStopIndex, row, dataContext);

                this.PrepareRouteDeviation(data.RouteDeviation, row, dataContext);

                this.PrepareDoorState(data.DoorState, row, dataContext);

                this.PrepareInPanic(data.InPanic, row, dataContext);

                this.PrepareVehicleStopRequested(data.VehicleStopRequested, row, dataContext);

                this.PrepareExitSide(data.ExitSide, row, dataContext);

                this.PrepareMovingDirectionForward(data.MovingDirectionForward, row, dataContext);

                this.PrepareVehicleMode(data.VehicleMode, row, dataContext);
            }

            public void HandleData(CustomerInformationServiceAllData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleVehicleRef(data.VehicleRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleRef != null && data.VehicleRef != null)
                {
                    foreach (var handler in this.handlersVehicleRef)
                    {
                        try
                        {
                            handler.Handle(data.VehicleRef, "VehicleRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.VehicleRef");
                        }
                    }
                }

                handled = false;
                this.HandleDefaultLanguage(data.DefaultLanguage, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDefaultLanguage != null && data.DefaultLanguage != null)
                {
                    foreach (var handler in this.handlersDefaultLanguage)
                    {
                        try
                        {
                            handler.Handle(data.DefaultLanguage, "DefaultLanguage", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.DefaultLanguage");
                        }
                    }
                }

                handled = false;
                this.HandleTripInformation(data.TripInformation, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerTripInformation != null && data.TripInformation != null && data.TripInformation.Length > 0)
                {
                    this.handlerTripInformation.HandleData(data.TripInformation[0], ximple, row, dataContext);
                }

                handled = false;
                this.HandleCurrentStopIndex(data.CurrentStopIndex, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersCurrentStopIndex != null && data.CurrentStopIndex != null)
                {
                    foreach (var handler in this.handlersCurrentStopIndex)
                    {
                        try
                        {
                            handler.Handle(data.CurrentStopIndex, "CurrentStopIndex", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.CurrentStopIndex");
                        }
                    }
                }

                handled = false;
                this.HandleRouteDeviation(data.RouteDeviation, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersRouteDeviation != null)
                {
                    foreach (var handler in this.handlersRouteDeviation)
                    {
                        try
                        {
                            handler.Handle(data.RouteDeviation, "RouteDeviation", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.RouteDeviation");
                        }
                    }
                }

                handled = false;
                this.HandleDoorState(data.DoorState, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDoorState != null && data.DoorStateSpecified)
                {
                    foreach (var handler in this.handlersDoorState)
                    {
                        try
                        {
                            handler.Handle(data.DoorState, "DoorState", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.DoorState");
                        }
                    }
                }

                handled = false;
                this.HandleInPanic(data.InPanic, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersInPanic != null && data.InPanic != null)
                {
                    foreach (var handler in this.handlersInPanic)
                    {
                        try
                        {
                            handler.Handle(data.InPanic, "InPanic", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.InPanic");
                        }
                    }
                }

                handled = false;
                this.HandleVehicleStopRequested(data.VehicleStopRequested, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleStopRequested != null && data.VehicleStopRequested != null)
                {
                    foreach (var handler in this.handlersVehicleStopRequested)
                    {
                        try
                        {
                            handler.Handle(data.VehicleStopRequested, "VehicleStopRequested", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.VehicleStopRequested");
                        }
                    }
                }

                handled = false;
                this.HandleExitSide(data.ExitSide, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersExitSide != null && data.ExitSideSpecified)
                {
                    foreach (var handler in this.handlersExitSide)
                    {
                        try
                        {
                            handler.Handle(data.ExitSide, "ExitSide", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.ExitSide");
                        }
                    }
                }

                handled = false;
                this.HandleMovingDirectionForward(data.MovingDirectionForward, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersMovingDirectionForward != null && data.MovingDirectionForward != null)
                {
                    foreach (var handler in this.handlersMovingDirectionForward)
                    {
                        try
                        {
                            handler.Handle(data.MovingDirectionForward, "MovingDirectionForward", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.MovingDirectionForward");
                        }
                    }
                }

                handled = false;
                this.HandleVehicleMode(data.VehicleMode, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleMode != null && data.VehicleModeSpecified)
                {
                    foreach (var handler in this.handlersVehicleMode)
                    {
                        try
                        {
                            handler.Handle(data.VehicleMode, "VehicleMode", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetAllData.VehicleMode");
                        }
                    }
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            partial void PrepareVehicleRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareDefaultLanguage(IBISIPlanguage item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareTripInformation(
                TripInformationStructure[] item, int row, DataContext dataContext, ref bool prepared);

            partial void PrepareCurrentStopIndex(IBISIPint item, int row, DataContext dataContext);

            partial void PrepareRouteDeviation(RouteDeviationEnumeration item, int row, DataContext dataContext);

            partial void PrepareDoorState(DoorOpenStateEnumeration item, int row, DataContext dataContext);

            partial void PrepareInPanic(IBISIPboolean item, int row, DataContext dataContext);

            partial void PrepareVehicleStopRequested(IBISIPboolean item, int row, DataContext dataContext);

            partial void PrepareExitSide(ExitSideEnumeration item, int row, DataContext dataContext);

            partial void PrepareMovingDirectionForward(IBISIPboolean item, int row, DataContext dataContext);

            partial void PrepareVehicleMode(VehicleModeEnumeration item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDefaultLanguage(
                IBISIPlanguage item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTripInformation(
                TripInformationStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleCurrentStopIndex(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleRouteDeviation(
                RouteDeviationEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDoorState(
                DoorOpenStateEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleInPanic(
                IBISIPboolean item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleStopRequested(
                IBISIPboolean item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleExitSide(
                ExitSideEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleMovingDirectionForward(
                IBISIPboolean item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleMode(
                VehicleModeEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class GetCurrentAnnouncementHandler
        {
            private readonly CustomerInformationServiceConfig.GetCurrentAnnouncementConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly AnnouncementStructureHandler handlerCurrentAnnouncement;

            public GetCurrentAnnouncementHandler(
                CustomerInformationServiceConfig.GetCurrentAnnouncementConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.CurrentAnnouncement != null)
                {
                    this.handlerCurrentAnnouncement = new AnnouncementStructureHandler(
                        this.config.CurrentAnnouncement, handlerFactory, context);
                }

            }

            public void PrepareData(CustomerInformationServiceCurrentAnnouncementData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                bool prepared = false;
                this.PrepareCurrentAnnouncement(data.CurrentAnnouncement, row, dataContext, ref prepared);
                if (!prepared && this.handlerCurrentAnnouncement != null && data.CurrentAnnouncement != null)
                {
                    this.handlerCurrentAnnouncement.PrepareData(data.CurrentAnnouncement, row, dataContext);
                }
            }

            public void HandleData(CustomerInformationServiceCurrentAnnouncementData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetCurrentAnnouncement.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleCurrentAnnouncement(data.CurrentAnnouncement, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerCurrentAnnouncement != null && data.CurrentAnnouncement != null)
                {
                    this.handlerCurrentAnnouncement.HandleData(data.CurrentAnnouncement, ximple, row, dataContext);
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareCurrentAnnouncement(
                AnnouncementStructure item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleCurrentAnnouncement(
                AnnouncementStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class GetCurrentConnectionInformationHandler
        {
            private readonly CustomerInformationServiceConfig.GetCurrentConnectionInformationConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly ConnectionStructureHandler handlerCurrentConnection;

            public GetCurrentConnectionInformationHandler(
                CustomerInformationServiceConfig.GetCurrentConnectionInformationConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.CurrentConnection != null)
                {
                    this.handlerCurrentConnection = new ConnectionStructureHandler(
                        this.config.CurrentConnection, handlerFactory, context);
                }

            }

            public void PrepareData(CustomerInformationServiceCurrentConnectionInformationData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                bool prepared = false;
                this.PrepareCurrentConnection(data.CurrentConnection, row, dataContext, ref prepared);
                if (!prepared && this.handlerCurrentConnection != null && data.CurrentConnection != null)
                {
                    this.handlerCurrentConnection.PrepareData(data.CurrentConnection, row, dataContext);
                }
            }

            public void HandleData(CustomerInformationServiceCurrentConnectionInformationData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetCurrentConnectionInformation.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleCurrentConnection(data.CurrentConnection, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerCurrentConnection != null && data.CurrentConnection != null)
                {
                    this.handlerCurrentConnection.HandleData(data.CurrentConnection, ximple, row, dataContext);
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareCurrentConnection(
                ConnectionStructure item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleCurrentConnection(
                ConnectionStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class GetCurrentDisplayContentHandler
        {
            private readonly CustomerInformationServiceConfig.GetCurrentDisplayContentConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly DisplayContentStructureHandler handlerCurrentDisplayContent;

            public GetCurrentDisplayContentHandler(
                CustomerInformationServiceConfig.GetCurrentDisplayContentConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.CurrentDisplayContent != null)
                {
                    this.handlerCurrentDisplayContent = new DisplayContentStructureHandler(
                        this.config.CurrentDisplayContent, handlerFactory, context);
                }

            }

            public void PrepareData(CustomerInformationServiceCurrentDisplayContentData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                bool prepared = false;
                this.PrepareCurrentDisplayContent(data.CurrentDisplayContent, row, dataContext, ref prepared);
                if (!prepared && this.handlerCurrentDisplayContent != null && data.CurrentDisplayContent != null)
                {
                    this.handlerCurrentDisplayContent.PrepareData(data.CurrentDisplayContent, row, dataContext);
                }
            }

            public void HandleData(CustomerInformationServiceCurrentDisplayContentData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetCurrentDisplayContent.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleCurrentDisplayContent(data.CurrentDisplayContent, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerCurrentDisplayContent != null && data.CurrentDisplayContent != null)
                {
                    this.handlerCurrentDisplayContent.HandleData(data.CurrentDisplayContent, ximple, row, dataContext);
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareCurrentDisplayContent(
                DisplayContentStructure item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleCurrentDisplayContent(
                DisplayContentStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class GetCurrentStopPointHandler
        {
            private readonly CustomerInformationServiceConfig.GetCurrentStopPointConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly StopInformationStructureHandler handlerCurrentStopPoint;

            public GetCurrentStopPointHandler(
                CustomerInformationServiceConfig.GetCurrentStopPointConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.CurrentStopPoint != null)
                {
                    this.handlerCurrentStopPoint = new StopInformationStructureHandler(
                        this.config.CurrentStopPoint, handlerFactory, context);
                }

            }

            public void PrepareData(CustomerInformationServiceCurrentStopPointData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                bool prepared = false;
                this.PrepareCurrentStopPoint(data.CurrentStopPoint, row, dataContext, ref prepared);
                if (!prepared && this.handlerCurrentStopPoint != null && data.CurrentStopPoint != null)
                {
                    this.handlerCurrentStopPoint.PrepareData(data.CurrentStopPoint, row, dataContext);
                }
            }

            public void HandleData(CustomerInformationServiceCurrentStopPointData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetCurrentStopPoint.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleCurrentStopPoint(data.CurrentStopPoint, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerCurrentStopPoint != null && data.CurrentStopPoint != null)
                {
                    this.handlerCurrentStopPoint.HandleData(data.CurrentStopPoint, ximple, row, dataContext);
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareCurrentStopPoint(
                StopInformationStructure item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleCurrentStopPoint(
                StopInformationStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class GetCurrentStopIndexHandler
        {
            private readonly CustomerInformationServiceConfig.GetCurrentStopIndexConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly List<ElementHandler> handlersCurrentStopIndex;

            public GetCurrentStopIndexHandler(
                CustomerInformationServiceConfig.GetCurrentStopIndexConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.CurrentStopIndex != null && this.config.CurrentStopIndex.Count > 0)
                {
                    this.handlersCurrentStopIndex = new List<ElementHandler>(this.config.CurrentStopIndex.Count);
                    foreach (var child in this.config.CurrentStopIndex)
                    {
                        this.handlersCurrentStopIndex.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(CustomerInformationServiceCurrentStopIndexData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                this.PrepareCurrentStopIndex(data.CurrentStopIndex, row, dataContext);
            }

            public void HandleData(CustomerInformationServiceCurrentStopIndexData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetCurrentStopIndex.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleCurrentStopIndex(data.CurrentStopIndex, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersCurrentStopIndex != null && data.CurrentStopIndex != null)
                {
                    foreach (var handler in this.handlersCurrentStopIndex)
                    {
                        try
                        {
                            handler.Handle(data.CurrentStopIndex, "CurrentStopIndex", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetCurrentStopIndex.CurrentStopIndex");
                        }
                    }
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            partial void PrepareCurrentStopIndex(IBISIPint item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleCurrentStopIndex(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class GetTripDataHandler
        {
            private readonly CustomerInformationServiceConfig.GetTripDataConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly List<ElementHandler> handlersVehicleRef;

            private readonly List<ElementHandler> handlersDefaultLanguage;

            private readonly TripInformationStructureHandler handlerTripInformation;

            private readonly List<ElementHandler> handlersCurrentStopIndex;

            public GetTripDataHandler(
                CustomerInformationServiceConfig.GetTripDataConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.VehicleRef != null && this.config.VehicleRef.Count > 0)
                {
                    this.handlersVehicleRef = new List<ElementHandler>(this.config.VehicleRef.Count);
                    foreach (var child in this.config.VehicleRef)
                    {
                        this.handlersVehicleRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DefaultLanguage != null && this.config.DefaultLanguage.Count > 0)
                {
                    this.handlersDefaultLanguage = new List<ElementHandler>(this.config.DefaultLanguage.Count);
                    foreach (var child in this.config.DefaultLanguage)
                    {
                        this.handlersDefaultLanguage.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.TripInformation != null)
                {
                    this.handlerTripInformation = new TripInformationStructureHandler(
                        this.config.TripInformation, handlerFactory, context);
                }

                if (this.config.CurrentStopIndex != null && this.config.CurrentStopIndex.Count > 0)
                {
                    this.handlersCurrentStopIndex = new List<ElementHandler>(this.config.CurrentStopIndex.Count);
                    foreach (var child in this.config.CurrentStopIndex)
                    {
                        this.handlersCurrentStopIndex.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(CustomerInformationServiceTripData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                this.PrepareVehicleRef(data.VehicleRef, row, dataContext);

                this.PrepareDefaultLanguage(data.DefaultLanguage, row, dataContext);

                bool prepared = false;
                this.PrepareTripInformation(data.TripInformation, row, dataContext, ref prepared);
                if (!prepared && this.handlerTripInformation != null && data.TripInformation != null)
                {
                    this.handlerTripInformation.PrepareData(data.TripInformation, row, dataContext);
                }

                this.PrepareCurrentStopIndex(data.CurrentStopIndex, row, dataContext);
            }

            public void HandleData(CustomerInformationServiceTripData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetTripData.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleVehicleRef(data.VehicleRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleRef != null && data.VehicleRef != null)
                {
                    foreach (var handler in this.handlersVehicleRef)
                    {
                        try
                        {
                            handler.Handle(data.VehicleRef, "VehicleRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetTripData.VehicleRef");
                        }
                    }
                }

                handled = false;
                this.HandleDefaultLanguage(data.DefaultLanguage, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDefaultLanguage != null && data.DefaultLanguage != null)
                {
                    foreach (var handler in this.handlersDefaultLanguage)
                    {
                        try
                        {
                            handler.Handle(data.DefaultLanguage, "DefaultLanguage", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetTripData.DefaultLanguage");
                        }
                    }
                }

                handled = false;
                this.HandleTripInformation(data.TripInformation, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerTripInformation != null && data.TripInformation != null)
                {
                    this.handlerTripInformation.HandleData(data.TripInformation, ximple, row, dataContext);
                }

                handled = false;
                this.HandleCurrentStopIndex(data.CurrentStopIndex, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersCurrentStopIndex != null && data.CurrentStopIndex != null)
                {
                    foreach (var handler in this.handlersCurrentStopIndex)
                    {
                        try
                        {
                            handler.Handle(data.CurrentStopIndex, "CurrentStopIndex", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetTripData.CurrentStopIndex");
                        }
                    }
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            partial void PrepareVehicleRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareDefaultLanguage(IBISIPlanguage item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareTripInformation(
                TripInformationStructure item, int row, DataContext dataContext, ref bool prepared);

            partial void PrepareCurrentStopIndex(IBISIPint item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDefaultLanguage(
                IBISIPlanguage item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTripInformation(
                TripInformationStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleCurrentStopIndex(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class GetVehicleDataHandler
        {
            private readonly CustomerInformationServiceConfig.GetVehicleDataConfig config;

            private readonly List<ElementHandler> handlersTimeStamp;

            private readonly List<ElementHandler> handlersVehicleRef;

            private readonly List<ElementHandler> handlersRouteDeviation;

            private readonly List<ElementHandler> handlersDoorState;

            private readonly List<ElementHandler> handlersInPanic;

            private readonly List<ElementHandler> handlersVehicleStopRequested;

            private readonly List<ElementHandler> handlersExitSide;

            private readonly List<ElementHandler> handlersMovingDirectionForward;

            private readonly List<ElementHandler> handlersVehicleMode;

            public GetVehicleDataHandler(
                CustomerInformationServiceConfig.GetVehicleDataConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TimeStamp != null && this.config.TimeStamp.Count > 0)
                {
                    this.handlersTimeStamp = new List<ElementHandler>(this.config.TimeStamp.Count);
                    foreach (var child in this.config.TimeStamp)
                    {
                        this.handlersTimeStamp.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.VehicleRef != null && this.config.VehicleRef.Count > 0)
                {
                    this.handlersVehicleRef = new List<ElementHandler>(this.config.VehicleRef.Count);
                    foreach (var child in this.config.VehicleRef)
                    {
                        this.handlersVehicleRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.RouteDeviation != null && this.config.RouteDeviation.Count > 0)
                {
                    this.handlersRouteDeviation = new List<ElementHandler>(this.config.RouteDeviation.Count);
                    foreach (var child in this.config.RouteDeviation)
                    {
                        this.handlersRouteDeviation.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DoorState != null && this.config.DoorState.Count > 0)
                {
                    this.handlersDoorState = new List<ElementHandler>(this.config.DoorState.Count);
                    foreach (var child in this.config.DoorState)
                    {
                        this.handlersDoorState.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.InPanic != null && this.config.InPanic.Count > 0)
                {
                    this.handlersInPanic = new List<ElementHandler>(this.config.InPanic.Count);
                    foreach (var child in this.config.InPanic)
                    {
                        this.handlersInPanic.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.VehicleStopRequested != null && this.config.VehicleStopRequested.Count > 0)
                {
                    this.handlersVehicleStopRequested = new List<ElementHandler>(this.config.VehicleStopRequested.Count);
                    foreach (var child in this.config.VehicleStopRequested)
                    {
                        this.handlersVehicleStopRequested.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.ExitSide != null && this.config.ExitSide.Count > 0)
                {
                    this.handlersExitSide = new List<ElementHandler>(this.config.ExitSide.Count);
                    foreach (var child in this.config.ExitSide)
                    {
                        this.handlersExitSide.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.MovingDirectionForward != null && this.config.MovingDirectionForward.Count > 0)
                {
                    this.handlersMovingDirectionForward = new List<ElementHandler>(this.config.MovingDirectionForward.Count);
                    foreach (var child in this.config.MovingDirectionForward)
                    {
                        this.handlersMovingDirectionForward.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.VehicleMode != null && this.config.VehicleMode.Count > 0)
                {
                    this.handlersVehicleMode = new List<ElementHandler>(this.config.VehicleMode.Count);
                    foreach (var child in this.config.VehicleMode)
                    {
                        this.handlersVehicleMode.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(CustomerInformationServiceVehicleData data, int row, DataContext dataContext)
            {
                this.PrepareTimeStamp(data.TimeStamp, row, dataContext);

                this.PrepareVehicleRef(data.VehicleRef, row, dataContext);

                this.PrepareRouteDeviation(data.RouteDeviation, row, dataContext);

                this.PrepareDoorState(data.DoorState, row, dataContext);

                this.PrepareInPanic(data.InPanic, row, dataContext);

                this.PrepareVehicleStopRequested(data.VehicleStopRequested, row, dataContext);

                this.PrepareExitSide(data.ExitSide, row, dataContext);

                this.PrepareMovingDirectionForward(data.MovingDirectionForward, row, dataContext);

                this.PrepareVehicleMode(data.VehicleMode, row, dataContext);
            }

            public void HandleData(CustomerInformationServiceVehicleData data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTimeStamp(data.TimeStamp, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimeStamp != null && data.TimeStamp != null)
                {
                    foreach (var handler in this.handlersTimeStamp)
                    {
                        try
                        {
                            handler.Handle(data.TimeStamp, "TimeStamp", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.TimeStamp");
                        }
                    }
                }

                handled = false;
                this.HandleVehicleRef(data.VehicleRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleRef != null && data.VehicleRef != null)
                {
                    foreach (var handler in this.handlersVehicleRef)
                    {
                        try
                        {
                            handler.Handle(data.VehicleRef, "VehicleRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.VehicleRef");
                        }
                    }
                }

                handled = false;
                this.HandleRouteDeviation(data.RouteDeviation, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersRouteDeviation != null)
                {
                    foreach (var handler in this.handlersRouteDeviation)
                    {
                        try
                        {
                            handler.Handle(data.RouteDeviation, "RouteDeviation", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.RouteDeviation");
                        }
                    }
                }

                handled = false;
                this.HandleDoorState(data.DoorState, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDoorState != null && data.DoorStateSpecified)
                {
                    foreach (var handler in this.handlersDoorState)
                    {
                        try
                        {
                            handler.Handle(data.DoorState, "DoorState", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.DoorState");
                        }
                    }
                }

                handled = false;
                this.HandleInPanic(data.InPanic, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersInPanic != null && data.InPanic != null)
                {
                    foreach (var handler in this.handlersInPanic)
                    {
                        try
                        {
                            handler.Handle(data.InPanic, "InPanic", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.InPanic");
                        }
                    }
                }

                handled = false;
                this.HandleVehicleStopRequested(data.VehicleStopRequested, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleStopRequested != null && data.VehicleStopRequested != null)
                {
                    foreach (var handler in this.handlersVehicleStopRequested)
                    {
                        try
                        {
                            handler.Handle(data.VehicleStopRequested, "VehicleStopRequested", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.VehicleStopRequested");
                        }
                    }
                }

                handled = false;
                this.HandleExitSide(data.ExitSide, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersExitSide != null && data.ExitSideSpecified)
                {
                    foreach (var handler in this.handlersExitSide)
                    {
                        try
                        {
                            handler.Handle(data.ExitSide, "ExitSide", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.ExitSide");
                        }
                    }
                }

                handled = false;
                this.HandleMovingDirectionForward(data.MovingDirectionForward, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersMovingDirectionForward != null && data.MovingDirectionForward != null)
                {
                    foreach (var handler in this.handlersMovingDirectionForward)
                    {
                        try
                        {
                            handler.Handle(data.MovingDirectionForward, "MovingDirectionForward", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.MovingDirectionForward");
                        }
                    }
                }

                handled = false;
                this.HandleVehicleMode(data.VehicleMode, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleMode != null && data.VehicleModeSpecified)
                {
                    foreach (var handler in this.handlersVehicleMode)
                    {
                        try
                        {
                            handler.Handle(data.VehicleMode, "VehicleMode", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform GetVehicleData.VehicleMode");
                        }
                    }
                }
            }
            

            partial void PrepareTimeStamp(IBISIPdateTime item, int row, DataContext dataContext);

            partial void PrepareVehicleRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareRouteDeviation(RouteDeviationEnumeration item, int row, DataContext dataContext);

            partial void PrepareDoorState(DoorOpenStateEnumeration item, int row, DataContext dataContext);

            partial void PrepareInPanic(IBISIPboolean item, int row, DataContext dataContext);

            partial void PrepareVehicleStopRequested(IBISIPboolean item, int row, DataContext dataContext);

            partial void PrepareExitSide(ExitSideEnumeration item, int row, DataContext dataContext);

            partial void PrepareMovingDirectionForward(IBISIPboolean item, int row, DataContext dataContext);

            partial void PrepareVehicleMode(VehicleModeEnumeration item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimeStamp(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleRouteDeviation(
                RouteDeviationEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDoorState(
                DoorOpenStateEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleInPanic(
                IBISIPboolean item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleStopRequested(
                IBISIPboolean item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleExitSide(
                ExitSideEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleMovingDirectionForward(
                IBISIPboolean item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleMode(
                VehicleModeEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class TripInformationStructureHandler
        {
            private readonly CustomerInformationServiceConfig.TripInformationStructureConfig config;

            private readonly List<ElementHandler> handlersTripRef;

            private readonly StopSequenceStructureHandler handlerStopSequence;

            private readonly List<ElementHandler> handlersLocationState;

            private readonly List<ElementHandler> handlersTimetableDelay;

            private readonly List<ElementHandler> handlersAdditionalTextMessage;

            private readonly AdditionalAnnouncementStructureHandler handlerAdditionalAnnouncement;

            public TripInformationStructureHandler(
                CustomerInformationServiceConfig.TripInformationStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.TripRef != null && this.config.TripRef.Count > 0)
                {
                    this.handlersTripRef = new List<ElementHandler>(this.config.TripRef.Count);
                    foreach (var child in this.config.TripRef)
                    {
                        this.handlersTripRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.StopSequence != null)
                {
                    this.handlerStopSequence = new StopSequenceStructureHandler(
                        this.config.StopSequence, handlerFactory, context);
                }

                if (this.config.LocationState != null && this.config.LocationState.Count > 0)
                {
                    this.handlersLocationState = new List<ElementHandler>(this.config.LocationState.Count);
                    foreach (var child in this.config.LocationState)
                    {
                        this.handlersLocationState.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.TimetableDelay != null && this.config.TimetableDelay.Count > 0)
                {
                    this.handlersTimetableDelay = new List<ElementHandler>(this.config.TimetableDelay.Count);
                    foreach (var child in this.config.TimetableDelay)
                    {
                        this.handlersTimetableDelay.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.AdditionalTextMessage != null && this.config.AdditionalTextMessage.Count > 0)
                {
                    this.handlersAdditionalTextMessage = new List<ElementHandler>(this.config.AdditionalTextMessage.Count);
                    foreach (var child in this.config.AdditionalTextMessage)
                    {
                        this.handlersAdditionalTextMessage.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.AdditionalAnnouncement != null)
                {
                    this.handlerAdditionalAnnouncement = new AdditionalAnnouncementStructureHandler(
                        this.config.AdditionalAnnouncement, handlerFactory, context);
                }

            }

            public void PrepareData(TripInformationStructure data, int row, DataContext dataContext)
            {
                this.PrepareTripRef(data.TripRef, row, dataContext);

                bool prepared = false;
                this.PrepareStopSequence(data.StopSequence, row, dataContext, ref prepared);
                if (!prepared && this.handlerStopSequence != null && data.StopSequence != null)
                {
                    this.handlerStopSequence.PrepareData(data.StopSequence, row, dataContext);
                }

                this.PrepareLocationState(data.LocationState, row, dataContext);

                this.PrepareTimetableDelay(data.TimetableDelay, row, dataContext);

                this.PrepareAdditionalTextMessage(data.AdditionalTextMessage, row, dataContext);

                prepared = false;
                this.PrepareAdditionalAnnouncement(data.AdditionalAnnouncement, row, dataContext, ref prepared);
                if (!prepared && this.handlerAdditionalAnnouncement != null && data.AdditionalAnnouncement != null && data.AdditionalAnnouncement.Length > 0)
                {
                    this.handlerAdditionalAnnouncement.PrepareData(data.AdditionalAnnouncement[0], row, dataContext);
                }
            }

            public void HandleData(TripInformationStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleTripRef(data.TripRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTripRef != null && data.TripRef != null)
                {
                    foreach (var handler in this.handlersTripRef)
                    {
                        try
                        {
                            handler.Handle(data.TripRef, "TripRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform TripInformationStructure.TripRef");
                        }
                    }
                }

                handled = false;
                this.HandleStopSequence(data.StopSequence, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerStopSequence != null && data.StopSequence != null)
                {
                    this.handlerStopSequence.HandleData(data.StopSequence, ximple, row, dataContext);
                }

                handled = false;
                this.HandleLocationState(data.LocationState, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersLocationState != null && data.LocationStateSpecified)
                {
                    foreach (var handler in this.handlersLocationState)
                    {
                        try
                        {
                            handler.Handle(data.LocationState, "LocationState", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform TripInformationStructure.LocationState");
                        }
                    }
                }

                handled = false;
                this.HandleTimetableDelay(data.TimetableDelay, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersTimetableDelay != null && data.TimetableDelay != null)
                {
                    foreach (var handler in this.handlersTimetableDelay)
                    {
                        try
                        {
                            handler.Handle(data.TimetableDelay, "TimetableDelay", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform TripInformationStructure.TimetableDelay");
                        }
                    }
                }

                handled = false;
                this.HandleAdditionalTextMessage(data.AdditionalTextMessage, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAdditionalTextMessage != null && data.AdditionalTextMessage != null)
                {
                    foreach (var handler in this.handlersAdditionalTextMessage)
                    {
                        try
                        {
                            handler.Handle(data.AdditionalTextMessage, "AdditionalTextMessage", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform TripInformationStructure.AdditionalTextMessage");
                        }
                    }
                }

                handled = false;
                this.HandleAdditionalAnnouncement(data.AdditionalAnnouncement, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerAdditionalAnnouncement != null && data.AdditionalAnnouncement != null && data.AdditionalAnnouncement.Length > 0)
                {
                    this.handlerAdditionalAnnouncement.HandleData(data.AdditionalAnnouncement[0], ximple, row, dataContext);
                }
            }
            

            partial void PrepareTripRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareStopSequence(
                StopSequenceStructure item, int row, DataContext dataContext, ref bool prepared);

            partial void PrepareLocationState(LocationStateEnumeration item, int row, DataContext dataContext);

            partial void PrepareTimetableDelay(IBISIPint item, int row, DataContext dataContext);

            partial void PrepareAdditionalTextMessage(IBISIPstring item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareAdditionalAnnouncement(
                AdditionalAnnouncementStructure[] item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTripRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopSequence(
                StopSequenceStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleLocationState(
                LocationStateEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTimetableDelay(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAdditionalTextMessage(
                IBISIPstring item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAdditionalAnnouncement(
                AdditionalAnnouncementStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class StopSequenceStructureHandler
        {
            private readonly CustomerInformationServiceConfig.StopSequenceStructureConfig config;

            private readonly StopInformationStructureHandler handlerStopPoint;

            public StopSequenceStructureHandler(
                CustomerInformationServiceConfig.StopSequenceStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.StopPoint != null)
                {
                    this.handlerStopPoint = new StopInformationStructureHandler(
                        this.config.StopPoint, handlerFactory, context);
                }

            }

            public void PrepareData(StopSequenceStructure data, int row, DataContext dataContext)
            {
                bool prepared = false;
                this.PrepareStopPoint(data.StopPoint, row, dataContext, ref prepared);
                if (!prepared && this.handlerStopPoint != null && data.StopPoint != null && data.StopPoint.Length > 0)
                {
                    this.handlerStopPoint.PrepareData(data.StopPoint[0], row, dataContext);
                }
            }

            public void HandleData(StopSequenceStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleStopPoint(data.StopPoint, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerStopPoint != null && data.StopPoint != null && data.StopPoint.Length > 0)
                {
                    this.handlerStopPoint.HandleData(data.StopPoint[0], ximple, row, dataContext);
                }
            }
            

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareStopPoint(
                StopInformationStructure[] item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopPoint(
                StopInformationStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class StopInformationStructureHandler
        {
            private readonly CustomerInformationServiceConfig.StopInformationStructureConfig config;

            private readonly List<ElementHandler> handlersStopIndex;

            private readonly List<ElementHandler> handlersStopRef;

            private readonly List<TranslatedElementHandler> handlersStopName;

            private readonly List<TranslatedElementHandler> handlersStopAlternativeName;

            private readonly List<ElementHandler> handlersPlatform;

            private readonly DisplayContentStructureHandler handlerDisplayContent;

            private readonly AnnouncementStructureHandler handlerStopAnnouncement;

            private readonly List<ElementHandler> handlersArrivalScheduled;

            private readonly List<ElementHandler> handlersDepartureScheduled;

            private readonly List<ElementHandler> handlersRecordedArrivalTime;

            private readonly List<ElementHandler> handlersDistanceToNextStop;

            private readonly ConnectionStructureHandler handlerConnection;

            private readonly List<ArrayElementHandler> handlersFareZone;

            public StopInformationStructureHandler(
                CustomerInformationServiceConfig.StopInformationStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.StopIndex != null && this.config.StopIndex.Count > 0)
                {
                    this.handlersStopIndex = new List<ElementHandler>(this.config.StopIndex.Count);
                    foreach (var child in this.config.StopIndex)
                    {
                        this.handlersStopIndex.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.StopRef != null && this.config.StopRef.Count > 0)
                {
                    this.handlersStopRef = new List<ElementHandler>(this.config.StopRef.Count);
                    foreach (var child in this.config.StopRef)
                    {
                        this.handlersStopRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.StopName != null && this.config.StopName.Count > 0)
                {
                    this.handlersStopName = new List<TranslatedElementHandler>(this.config.StopName.Count);
                    foreach (var child in this.config.StopName)
                    {
                        this.handlersStopName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.StopAlternativeName != null && this.config.StopAlternativeName.Count > 0)
                {
                    this.handlersStopAlternativeName = new List<TranslatedElementHandler>(this.config.StopAlternativeName.Count);
                    foreach (var child in this.config.StopAlternativeName)
                    {
                        this.handlersStopAlternativeName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.Platform != null && this.config.Platform.Count > 0)
                {
                    this.handlersPlatform = new List<ElementHandler>(this.config.Platform.Count);
                    foreach (var child in this.config.Platform)
                    {
                        this.handlersPlatform.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DisplayContent != null)
                {
                    this.handlerDisplayContent = new DisplayContentStructureHandler(
                        this.config.DisplayContent, handlerFactory, context);
                }

                if (this.config.StopAnnouncement != null)
                {
                    this.handlerStopAnnouncement = new AnnouncementStructureHandler(
                        this.config.StopAnnouncement, handlerFactory, context);
                }

                if (this.config.ArrivalScheduled != null && this.config.ArrivalScheduled.Count > 0)
                {
                    this.handlersArrivalScheduled = new List<ElementHandler>(this.config.ArrivalScheduled.Count);
                    foreach (var child in this.config.ArrivalScheduled)
                    {
                        this.handlersArrivalScheduled.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DepartureScheduled != null && this.config.DepartureScheduled.Count > 0)
                {
                    this.handlersDepartureScheduled = new List<ElementHandler>(this.config.DepartureScheduled.Count);
                    foreach (var child in this.config.DepartureScheduled)
                    {
                        this.handlersDepartureScheduled.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.RecordedArrivalTime != null && this.config.RecordedArrivalTime.Count > 0)
                {
                    this.handlersRecordedArrivalTime = new List<ElementHandler>(this.config.RecordedArrivalTime.Count);
                    foreach (var child in this.config.RecordedArrivalTime)
                    {
                        this.handlersRecordedArrivalTime.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DistanceToNextStop != null && this.config.DistanceToNextStop.Count > 0)
                {
                    this.handlersDistanceToNextStop = new List<ElementHandler>(this.config.DistanceToNextStop.Count);
                    foreach (var child in this.config.DistanceToNextStop)
                    {
                        this.handlersDistanceToNextStop.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.Connection != null)
                {
                    this.handlerConnection = new ConnectionStructureHandler(
                        this.config.Connection, handlerFactory, context);
                }

                if (this.config.FareZone != null && this.config.FareZone.Count > 0)
                {
                    this.handlersFareZone = new List<ArrayElementHandler>(this.config.FareZone.Count);
                    foreach (var child in this.config.FareZone)
                    {
                        this.handlersFareZone.Add(handlerFactory.CreateArrayElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(StopInformationStructure data, int row, DataContext dataContext)
            {
                this.PrepareStopIndex(data.StopIndex, row, dataContext);

                this.PrepareStopRef(data.StopRef, row, dataContext);

                this.PrepareStopName(data.StopName, row, dataContext);

                this.PrepareStopAlternativeName(data.StopAlternativeName, row, dataContext);

                this.PreparePlatform(data.Platform, row, dataContext);

                bool prepared = false;
                this.PrepareDisplayContent(data.DisplayContent, row, dataContext, ref prepared);
                if (!prepared && this.handlerDisplayContent != null && data.DisplayContent != null && data.DisplayContent.Length > 0)
                {
                    this.handlerDisplayContent.PrepareData(data.DisplayContent[0], row, dataContext);
                }

                prepared = false;
                this.PrepareStopAnnouncement(data.StopAnnouncement, row, dataContext, ref prepared);
                if (!prepared && this.handlerStopAnnouncement != null && data.StopAnnouncement != null && data.StopAnnouncement.Length > 0)
                {
                    this.handlerStopAnnouncement.PrepareData(data.StopAnnouncement[0], row, dataContext);
                }

                this.PrepareArrivalScheduled(data.ArrivalScheduled, row, dataContext);

                this.PrepareDepartureScheduled(data.DepartureScheduled, row, dataContext);

                this.PrepareRecordedArrivalTime(data.RecordedArrivalTime, row, dataContext);

                this.PrepareDistanceToNextStop(data.DistanceToNextStop, row, dataContext);

                prepared = false;
                this.PrepareConnection(data.Connection, row, dataContext, ref prepared);
                if (!prepared && this.handlerConnection != null && data.Connection != null && data.Connection.Length > 0)
                {
                    this.handlerConnection.PrepareData(data.Connection[0], row, dataContext);
                }

                this.PrepareFareZone(data.FareZone, row, dataContext);
            }

            public void HandleData(StopInformationStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleStopIndex(data.StopIndex, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersStopIndex != null && data.StopIndex != null)
                {
                    foreach (var handler in this.handlersStopIndex)
                    {
                        try
                        {
                            handler.Handle(data.StopIndex, "StopIndex", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.StopIndex");
                        }
                    }
                }

                handled = false;
                this.HandleStopRef(data.StopRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersStopRef != null && data.StopRef != null)
                {
                    foreach (var handler in this.handlersStopRef)
                    {
                        try
                        {
                            handler.Handle(data.StopRef, "StopRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.StopRef");
                        }
                    }
                }

                handled = false;
                this.HandleStopName(data.StopName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersStopName != null && data.StopName != null)
                {
                    foreach (var handler in this.handlersStopName)
                    {
                        try
                        {
                            handler.Handle(data.StopName, "StopName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.StopName");
                        }
                    }
                }

                handled = false;
                this.HandleStopAlternativeName(data.StopAlternativeName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersStopAlternativeName != null && data.StopAlternativeName != null)
                {
                    foreach (var handler in this.handlersStopAlternativeName)
                    {
                        try
                        {
                            handler.Handle(data.StopAlternativeName, "StopAlternativeName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.StopAlternativeName");
                        }
                    }
                }

                handled = false;
                this.HandlePlatform(data.Platform, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPlatform != null && data.Platform != null)
                {
                    foreach (var handler in this.handlersPlatform)
                    {
                        try
                        {
                            handler.Handle(data.Platform, "Platform", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.Platform");
                        }
                    }
                }

                handled = false;
                this.HandleDisplayContent(data.DisplayContent, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerDisplayContent != null && data.DisplayContent != null && data.DisplayContent.Length > 0)
                {
                    this.handlerDisplayContent.HandleData(data.DisplayContent[0], ximple, row, dataContext);
                }

                handled = false;
                this.HandleStopAnnouncement(data.StopAnnouncement, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerStopAnnouncement != null && data.StopAnnouncement != null && data.StopAnnouncement.Length > 0)
                {
                    this.handlerStopAnnouncement.HandleData(data.StopAnnouncement[0], ximple, row, dataContext);
                }

                handled = false;
                this.HandleArrivalScheduled(data.ArrivalScheduled, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersArrivalScheduled != null && data.ArrivalScheduled != null)
                {
                    foreach (var handler in this.handlersArrivalScheduled)
                    {
                        try
                        {
                            handler.Handle(data.ArrivalScheduled, "ArrivalScheduled", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.ArrivalScheduled");
                        }
                    }
                }

                handled = false;
                this.HandleDepartureScheduled(data.DepartureScheduled, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDepartureScheduled != null && data.DepartureScheduled != null)
                {
                    foreach (var handler in this.handlersDepartureScheduled)
                    {
                        try
                        {
                            handler.Handle(data.DepartureScheduled, "DepartureScheduled", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.DepartureScheduled");
                        }
                    }
                }

                handled = false;
                this.HandleRecordedArrivalTime(data.RecordedArrivalTime, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersRecordedArrivalTime != null && data.RecordedArrivalTime != null)
                {
                    foreach (var handler in this.handlersRecordedArrivalTime)
                    {
                        try
                        {
                            handler.Handle(data.RecordedArrivalTime, "RecordedArrivalTime", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.RecordedArrivalTime");
                        }
                    }
                }

                handled = false;
                this.HandleDistanceToNextStop(data.DistanceToNextStop, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDistanceToNextStop != null && data.DistanceToNextStop != null)
                {
                    foreach (var handler in this.handlersDistanceToNextStop)
                    {
                        try
                        {
                            handler.Handle(data.DistanceToNextStop, "DistanceToNextStop", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.DistanceToNextStop");
                        }
                    }
                }

                handled = false;
                this.HandleConnection(data.Connection, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerConnection != null && data.Connection != null && data.Connection.Length > 0)
                {
                    this.handlerConnection.HandleData(data.Connection[0], ximple, row, dataContext);
                }

                handled = false;
                this.HandleFareZone(data.FareZone, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersFareZone != null && data.FareZone != null)
                {
                    foreach (var handler in this.handlersFareZone)
                    {
                        try
                        {
                            handler.Handle(data.FareZone, "FareZone", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform StopInformationStructure.FareZone");
                        }
                    }
                }
            }
            

            partial void PrepareStopIndex(IBISIPint item, int row, DataContext dataContext);

            partial void PrepareStopRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareStopName(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PrepareStopAlternativeName(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PreparePlatform(IBISIPstring item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareDisplayContent(
                DisplayContentStructure[] item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareStopAnnouncement(
                AnnouncementStructure[] item, int row, DataContext dataContext, ref bool prepared);

            partial void PrepareArrivalScheduled(IBISIPdateTime item, int row, DataContext dataContext);

            partial void PrepareDepartureScheduled(IBISIPdateTime item, int row, DataContext dataContext);

            partial void PrepareRecordedArrivalTime(IBISIPdateTime item, int row, DataContext dataContext);

            partial void PrepareDistanceToNextStop(IBISIPint item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareConnection(
                ConnectionStructure[] item, int row, DataContext dataContext, ref bool prepared);

            partial void PrepareFareZone(IBISIPNMTOKEN[] item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopIndex(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopAlternativeName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePlatform(
                IBISIPstring item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDisplayContent(
                DisplayContentStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopAnnouncement(
                AnnouncementStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleArrivalScheduled(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDepartureScheduled(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleRecordedArrivalTime(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDistanceToNextStop(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleConnection(
                ConnectionStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleFareZone(
                IBISIPNMTOKEN[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class DisplayContentStructureHandler
        {
            private readonly CustomerInformationServiceConfig.DisplayContentStructureConfig config;

            private readonly List<ElementHandler> handlersDisplayContentRef;

            private readonly LineInformationStructureHandler handlerLineInformation;

            private readonly DestinationStructureHandler handlerDestination;

            private readonly ViaPointStructureHandler handlerViaPoint;

            private readonly List<TranslatedElementHandler> handlersAdditionalInformation;

            private readonly List<ElementHandler> handlersPriority;

            private readonly List<ElementHandler> handlersPeriodDuration;

            private readonly List<ElementHandler> handlersDuration;

            public DisplayContentStructureHandler(
                CustomerInformationServiceConfig.DisplayContentStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.DisplayContentRef != null && this.config.DisplayContentRef.Count > 0)
                {
                    this.handlersDisplayContentRef = new List<ElementHandler>(this.config.DisplayContentRef.Count);
                    foreach (var child in this.config.DisplayContentRef)
                    {
                        this.handlersDisplayContentRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.LineInformation != null)
                {
                    this.handlerLineInformation = new LineInformationStructureHandler(
                        this.config.LineInformation, handlerFactory, context);
                }

                if (this.config.Destination != null)
                {
                    this.handlerDestination = new DestinationStructureHandler(
                        this.config.Destination, handlerFactory, context);
                }

                if (this.config.ViaPoint != null)
                {
                    this.handlerViaPoint = new ViaPointStructureHandler(
                        this.config.ViaPoint, handlerFactory, context);
                }

                if (this.config.AdditionalInformation != null && this.config.AdditionalInformation.Count > 0)
                {
                    this.handlersAdditionalInformation = new List<TranslatedElementHandler>(this.config.AdditionalInformation.Count);
                    foreach (var child in this.config.AdditionalInformation)
                    {
                        this.handlersAdditionalInformation.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.Priority != null && this.config.Priority.Count > 0)
                {
                    this.handlersPriority = new List<ElementHandler>(this.config.Priority.Count);
                    foreach (var child in this.config.Priority)
                    {
                        this.handlersPriority.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.PeriodDuration != null && this.config.PeriodDuration.Count > 0)
                {
                    this.handlersPeriodDuration = new List<ElementHandler>(this.config.PeriodDuration.Count);
                    foreach (var child in this.config.PeriodDuration)
                    {
                        this.handlersPeriodDuration.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.Duration != null && this.config.Duration.Count > 0)
                {
                    this.handlersDuration = new List<ElementHandler>(this.config.Duration.Count);
                    foreach (var child in this.config.Duration)
                    {
                        this.handlersDuration.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(DisplayContentStructure data, int row, DataContext dataContext)
            {
                this.PrepareDisplayContentRef(data.DisplayContentRef, row, dataContext);

                bool prepared = false;
                this.PrepareLineInformation(data.LineInformation, row, dataContext, ref prepared);
                if (!prepared && this.handlerLineInformation != null && data.LineInformation != null)
                {
                    this.handlerLineInformation.PrepareData(data.LineInformation, row, dataContext);
                }

                prepared = false;
                this.PrepareDestination(data.Destination, row, dataContext, ref prepared);
                if (!prepared && this.handlerDestination != null && data.Destination != null)
                {
                    this.handlerDestination.PrepareData(data.Destination, row, dataContext);
                }

                prepared = false;
                this.PrepareViaPoint(data.ViaPoint, row, dataContext, ref prepared);
                if (!prepared && this.handlerViaPoint != null && data.ViaPoint != null && data.ViaPoint.Length > 0)
                {
                    this.handlerViaPoint.PrepareData(data.ViaPoint[0], row, dataContext);
                }

                this.PrepareAdditionalInformation(data.AdditionalInformation, row, dataContext);

                this.PreparePriority(data.Priority, row, dataContext);

                this.PreparePeriodDuration(data.PeriodDuration, row, dataContext);

                this.PrepareDuration(data.Duration, row, dataContext);
            }

            public void HandleData(DisplayContentStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleDisplayContentRef(data.DisplayContentRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDisplayContentRef != null && data.DisplayContentRef != null)
                {
                    foreach (var handler in this.handlersDisplayContentRef)
                    {
                        try
                        {
                            handler.Handle(data.DisplayContentRef, "DisplayContentRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DisplayContentStructure.DisplayContentRef");
                        }
                    }
                }

                handled = false;
                this.HandleLineInformation(data.LineInformation, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerLineInformation != null && data.LineInformation != null)
                {
                    this.handlerLineInformation.HandleData(data.LineInformation, ximple, row, dataContext);
                }

                handled = false;
                this.HandleDestination(data.Destination, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerDestination != null && data.Destination != null)
                {
                    this.handlerDestination.HandleData(data.Destination, ximple, row, dataContext);
                }

                handled = false;
                this.HandleViaPoint(data.ViaPoint, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerViaPoint != null && data.ViaPoint != null && data.ViaPoint.Length > 0)
                {
                    this.handlerViaPoint.HandleData(data.ViaPoint[0], ximple, row, dataContext);
                }

                handled = false;
                this.HandleAdditionalInformation(data.AdditionalInformation, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAdditionalInformation != null && data.AdditionalInformation != null)
                {
                    foreach (var handler in this.handlersAdditionalInformation)
                    {
                        try
                        {
                            handler.Handle(data.AdditionalInformation, "AdditionalInformation", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DisplayContentStructure.AdditionalInformation");
                        }
                    }
                }

                handled = false;
                this.HandlePriority(data.Priority, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPriority != null && data.Priority != null)
                {
                    foreach (var handler in this.handlersPriority)
                    {
                        try
                        {
                            handler.Handle(data.Priority, "Priority", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DisplayContentStructure.Priority");
                        }
                    }
                }

                handled = false;
                this.HandlePeriodDuration(data.PeriodDuration, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPeriodDuration != null && data.PeriodDuration != null)
                {
                    foreach (var handler in this.handlersPeriodDuration)
                    {
                        try
                        {
                            handler.Handle(data.PeriodDuration, "PeriodDuration", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DisplayContentStructure.PeriodDuration");
                        }
                    }
                }

                handled = false;
                this.HandleDuration(data.Duration, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDuration != null && data.Duration != null)
                {
                    foreach (var handler in this.handlersDuration)
                    {
                        try
                        {
                            handler.Handle(data.Duration, "Duration", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DisplayContentStructure.Duration");
                        }
                    }
                }
            }
            

            partial void PrepareDisplayContentRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareLineInformation(
                LineInformationStructure item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareDestination(
                DestinationStructure item, int row, DataContext dataContext, ref bool prepared);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareViaPoint(
                ViaPointStructure[] item, int row, DataContext dataContext, ref bool prepared);

            partial void PrepareAdditionalInformation(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PreparePriority(IBISIPnonNegativeInteger item, int row, DataContext dataContext);

            partial void PreparePeriodDuration(IBISIPduration item, int row, DataContext dataContext);

            partial void PrepareDuration(IBISIPduration item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDisplayContentRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleLineInformation(
                LineInformationStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDestination(
                DestinationStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleViaPoint(
                ViaPointStructure[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAdditionalInformation(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePriority(
                IBISIPnonNegativeInteger item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePeriodDuration(
                IBISIPduration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDuration(
                IBISIPduration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class LineInformationStructureHandler
        {
            private readonly CustomerInformationServiceConfig.LineInformationStructureConfig config;

            private readonly List<ElementHandler> handlersLineRef;

            private readonly List<TranslatedElementHandler> handlersLineName;

            private readonly List<TranslatedElementHandler> handlersLineShortName;

            private readonly List<ElementHandler> handlersLineNumber;

            public LineInformationStructureHandler(
                CustomerInformationServiceConfig.LineInformationStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.LineRef != null && this.config.LineRef.Count > 0)
                {
                    this.handlersLineRef = new List<ElementHandler>(this.config.LineRef.Count);
                    foreach (var child in this.config.LineRef)
                    {
                        this.handlersLineRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.LineName != null && this.config.LineName.Count > 0)
                {
                    this.handlersLineName = new List<TranslatedElementHandler>(this.config.LineName.Count);
                    foreach (var child in this.config.LineName)
                    {
                        this.handlersLineName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.LineShortName != null && this.config.LineShortName.Count > 0)
                {
                    this.handlersLineShortName = new List<TranslatedElementHandler>(this.config.LineShortName.Count);
                    foreach (var child in this.config.LineShortName)
                    {
                        this.handlersLineShortName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.LineNumber != null && this.config.LineNumber.Count > 0)
                {
                    this.handlersLineNumber = new List<ElementHandler>(this.config.LineNumber.Count);
                    foreach (var child in this.config.LineNumber)
                    {
                        this.handlersLineNumber.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(LineInformationStructure data, int row, DataContext dataContext)
            {
                this.PrepareLineRef(data.LineRef, row, dataContext);

                this.PrepareLineName(data.LineName, row, dataContext);

                this.PrepareLineShortName(data.LineShortName, row, dataContext);

                this.PrepareLineNumber(data.LineNumber, row, dataContext);
            }

            public void HandleData(LineInformationStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleLineRef(data.LineRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersLineRef != null && data.LineRef != null)
                {
                    foreach (var handler in this.handlersLineRef)
                    {
                        try
                        {
                            handler.Handle(data.LineRef, "LineRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform LineInformationStructure.LineRef");
                        }
                    }
                }

                handled = false;
                this.HandleLineName(data.LineName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersLineName != null && data.LineName != null)
                {
                    foreach (var handler in this.handlersLineName)
                    {
                        try
                        {
                            handler.Handle(data.LineName, "LineName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform LineInformationStructure.LineName");
                        }
                    }
                }

                handled = false;
                this.HandleLineShortName(data.LineShortName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersLineShortName != null && data.LineShortName != null)
                {
                    foreach (var handler in this.handlersLineShortName)
                    {
                        try
                        {
                            handler.Handle(data.LineShortName, "LineShortName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform LineInformationStructure.LineShortName");
                        }
                    }
                }

                handled = false;
                this.HandleLineNumber(data.LineNumber, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersLineNumber != null && data.LineNumber != null)
                {
                    foreach (var handler in this.handlersLineNumber)
                    {
                        try
                        {
                            handler.Handle(data.LineNumber, "LineNumber", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform LineInformationStructure.LineNumber");
                        }
                    }
                }
            }
            

            partial void PrepareLineRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareLineName(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PrepareLineShortName(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PrepareLineNumber(IBISIPint item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleLineRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleLineName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleLineShortName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleLineNumber(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class DestinationStructureHandler
        {
            private readonly CustomerInformationServiceConfig.DestinationStructureConfig config;

            private readonly List<ElementHandler> handlersDestinationRef;

            private readonly List<TranslatedElementHandler> handlersDestinationName;

            private readonly List<TranslatedElementHandler> handlersDestinationShortName;

            public DestinationStructureHandler(
                CustomerInformationServiceConfig.DestinationStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.DestinationRef != null && this.config.DestinationRef.Count > 0)
                {
                    this.handlersDestinationRef = new List<ElementHandler>(this.config.DestinationRef.Count);
                    foreach (var child in this.config.DestinationRef)
                    {
                        this.handlersDestinationRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DestinationName != null && this.config.DestinationName.Count > 0)
                {
                    this.handlersDestinationName = new List<TranslatedElementHandler>(this.config.DestinationName.Count);
                    foreach (var child in this.config.DestinationName)
                    {
                        this.handlersDestinationName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.DestinationShortName != null && this.config.DestinationShortName.Count > 0)
                {
                    this.handlersDestinationShortName = new List<TranslatedElementHandler>(this.config.DestinationShortName.Count);
                    foreach (var child in this.config.DestinationShortName)
                    {
                        this.handlersDestinationShortName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(DestinationStructure data, int row, DataContext dataContext)
            {
                this.PrepareDestinationRef(data.DestinationRef, row, dataContext);

                this.PrepareDestinationName(data.DestinationName, row, dataContext);

                this.PrepareDestinationShortName(data.DestinationShortName, row, dataContext);
            }

            public void HandleData(DestinationStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleDestinationRef(data.DestinationRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDestinationRef != null && data.DestinationRef != null)
                {
                    foreach (var handler in this.handlersDestinationRef)
                    {
                        try
                        {
                            handler.Handle(data.DestinationRef, "DestinationRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DestinationStructure.DestinationRef");
                        }
                    }
                }

                handled = false;
                this.HandleDestinationName(data.DestinationName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDestinationName != null && data.DestinationName != null)
                {
                    foreach (var handler in this.handlersDestinationName)
                    {
                        try
                        {
                            handler.Handle(data.DestinationName, "DestinationName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DestinationStructure.DestinationName");
                        }
                    }
                }

                handled = false;
                this.HandleDestinationShortName(data.DestinationShortName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDestinationShortName != null && data.DestinationShortName != null)
                {
                    foreach (var handler in this.handlersDestinationShortName)
                    {
                        try
                        {
                            handler.Handle(data.DestinationShortName, "DestinationShortName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform DestinationStructure.DestinationShortName");
                        }
                    }
                }
            }
            

            partial void PrepareDestinationRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareDestinationName(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PrepareDestinationShortName(InternationalTextType[] item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDestinationRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDestinationName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDestinationShortName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class ViaPointStructureHandler
        {
            private readonly CustomerInformationServiceConfig.ViaPointStructureConfig config;

            private readonly List<ElementHandler> handlersViaPointRef;

            private readonly List<ElementHandler> handlersPlaceRef;

            private readonly List<TranslatedElementHandler> handlersPlaceName;

            private readonly List<TranslatedElementHandler> handlersPlaceShortName;

            private readonly List<ElementHandler> handlersViaPointDisplayPriority;

            public ViaPointStructureHandler(
                CustomerInformationServiceConfig.ViaPointStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.ViaPointRef != null && this.config.ViaPointRef.Count > 0)
                {
                    this.handlersViaPointRef = new List<ElementHandler>(this.config.ViaPointRef.Count);
                    foreach (var child in this.config.ViaPointRef)
                    {
                        this.handlersViaPointRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.PlaceRef != null && this.config.PlaceRef.Count > 0)
                {
                    this.handlersPlaceRef = new List<ElementHandler>(this.config.PlaceRef.Count);
                    foreach (var child in this.config.PlaceRef)
                    {
                        this.handlersPlaceRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.PlaceName != null && this.config.PlaceName.Count > 0)
                {
                    this.handlersPlaceName = new List<TranslatedElementHandler>(this.config.PlaceName.Count);
                    foreach (var child in this.config.PlaceName)
                    {
                        this.handlersPlaceName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.PlaceShortName != null && this.config.PlaceShortName.Count > 0)
                {
                    this.handlersPlaceShortName = new List<TranslatedElementHandler>(this.config.PlaceShortName.Count);
                    foreach (var child in this.config.PlaceShortName)
                    {
                        this.handlersPlaceShortName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.ViaPointDisplayPriority != null && this.config.ViaPointDisplayPriority.Count > 0)
                {
                    this.handlersViaPointDisplayPriority = new List<ElementHandler>(this.config.ViaPointDisplayPriority.Count);
                    foreach (var child in this.config.ViaPointDisplayPriority)
                    {
                        this.handlersViaPointDisplayPriority.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(ViaPointStructure data, int row, DataContext dataContext)
            {
                this.PrepareViaPointRef(data.ViaPointRef, row, dataContext);

                this.PreparePlaceRef(data.PlaceRef, row, dataContext);

                this.PreparePlaceName(data.PlaceName, row, dataContext);

                this.PreparePlaceShortName(data.PlaceShortName, row, dataContext);

                this.PrepareViaPointDisplayPriority(data.ViaPointDisplayPriority, row, dataContext);
            }

            public void HandleData(ViaPointStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleViaPointRef(data.ViaPointRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersViaPointRef != null && data.ViaPointRef != null)
                {
                    foreach (var handler in this.handlersViaPointRef)
                    {
                        try
                        {
                            handler.Handle(data.ViaPointRef, "ViaPointRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ViaPointStructure.ViaPointRef");
                        }
                    }
                }

                handled = false;
                this.HandlePlaceRef(data.PlaceRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPlaceRef != null && data.PlaceRef != null)
                {
                    foreach (var handler in this.handlersPlaceRef)
                    {
                        try
                        {
                            handler.Handle(data.PlaceRef, "PlaceRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ViaPointStructure.PlaceRef");
                        }
                    }
                }

                handled = false;
                this.HandlePlaceName(data.PlaceName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPlaceName != null && data.PlaceName != null)
                {
                    foreach (var handler in this.handlersPlaceName)
                    {
                        try
                        {
                            handler.Handle(data.PlaceName, "PlaceName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ViaPointStructure.PlaceName");
                        }
                    }
                }

                handled = false;
                this.HandlePlaceShortName(data.PlaceShortName, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPlaceShortName != null && data.PlaceShortName != null)
                {
                    foreach (var handler in this.handlersPlaceShortName)
                    {
                        try
                        {
                            handler.Handle(data.PlaceShortName, "PlaceShortName", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ViaPointStructure.PlaceShortName");
                        }
                    }
                }

                handled = false;
                this.HandleViaPointDisplayPriority(data.ViaPointDisplayPriority, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersViaPointDisplayPriority != null && data.ViaPointDisplayPriority != null)
                {
                    foreach (var handler in this.handlersViaPointDisplayPriority)
                    {
                        try
                        {
                            handler.Handle(data.ViaPointDisplayPriority, "ViaPointDisplayPriority", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ViaPointStructure.ViaPointDisplayPriority");
                        }
                    }
                }
            }
            

            partial void PrepareViaPointRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PreparePlaceRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PreparePlaceName(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PreparePlaceShortName(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PrepareViaPointDisplayPriority(IBISIPint item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleViaPointRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePlaceRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePlaceName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePlaceShortName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleViaPointDisplayPriority(
                IBISIPint item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class AnnouncementStructureHandler
        {
            private readonly CustomerInformationServiceConfig.AnnouncementStructureConfig config;

            private readonly List<ElementHandler> handlersAnnouncementRef;

            private readonly List<TranslatedElementHandler> handlersAnnouncementText;

            private readonly List<TranslatedElementHandler> handlersAnnouncementTTSText;

            public AnnouncementStructureHandler(
                CustomerInformationServiceConfig.AnnouncementStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.AnnouncementRef != null && this.config.AnnouncementRef.Count > 0)
                {
                    this.handlersAnnouncementRef = new List<ElementHandler>(this.config.AnnouncementRef.Count);
                    foreach (var child in this.config.AnnouncementRef)
                    {
                        this.handlersAnnouncementRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.AnnouncementText != null && this.config.AnnouncementText.Count > 0)
                {
                    this.handlersAnnouncementText = new List<TranslatedElementHandler>(this.config.AnnouncementText.Count);
                    foreach (var child in this.config.AnnouncementText)
                    {
                        this.handlersAnnouncementText.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.AnnouncementTTSText != null && this.config.AnnouncementTTSText.Count > 0)
                {
                    this.handlersAnnouncementTTSText = new List<TranslatedElementHandler>(this.config.AnnouncementTTSText.Count);
                    foreach (var child in this.config.AnnouncementTTSText)
                    {
                        this.handlersAnnouncementTTSText.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(AnnouncementStructure data, int row, DataContext dataContext)
            {
                this.PrepareAnnouncementRef(data.AnnouncementRef, row, dataContext);

                this.PrepareAnnouncementText(data.AnnouncementText, row, dataContext);

                this.PrepareAnnouncementTTSText(data.AnnouncementTTSText, row, dataContext);
            }

            public void HandleData(AnnouncementStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleAnnouncementRef(data.AnnouncementRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAnnouncementRef != null && data.AnnouncementRef != null)
                {
                    foreach (var handler in this.handlersAnnouncementRef)
                    {
                        try
                        {
                            handler.Handle(data.AnnouncementRef, "AnnouncementRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform AnnouncementStructure.AnnouncementRef");
                        }
                    }
                }

                handled = false;
                this.HandleAnnouncementText(data.AnnouncementText, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAnnouncementText != null && data.AnnouncementText != null)
                {
                    foreach (var handler in this.handlersAnnouncementText)
                    {
                        try
                        {
                            handler.Handle(data.AnnouncementText, "AnnouncementText", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform AnnouncementStructure.AnnouncementText");
                        }
                    }
                }

                handled = false;
                this.HandleAnnouncementTTSText(data.AnnouncementTTSText, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAnnouncementTTSText != null && data.AnnouncementTTSText != null)
                {
                    foreach (var handler in this.handlersAnnouncementTTSText)
                    {
                        try
                        {
                            handler.Handle(data.AnnouncementTTSText, "AnnouncementTTSText", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform AnnouncementStructure.AnnouncementTTSText");
                        }
                    }
                }
            }
            

            partial void PrepareAnnouncementRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareAnnouncementText(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PrepareAnnouncementTTSText(InternationalTextType[] item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAnnouncementRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAnnouncementText(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAnnouncementTTSText(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class ConnectionStructureHandler
        {
            private readonly CustomerInformationServiceConfig.ConnectionStructureConfig config;

            private readonly List<ElementHandler> handlersStopRef;

            private readonly List<ElementHandler> handlersConnectionRef;

            private readonly List<ElementHandler> handlersConnectionType;

            private readonly DisplayContentStructureHandler handlerDisplayContent;

            private readonly List<ElementHandler> handlersPlatform;

            private readonly List<ElementHandler> handlersConnectionState;

            private readonly VehicleStructureHandler handlerTransportMode;

            private readonly List<ElementHandler> handlersExpectedDepatureTime;

            public ConnectionStructureHandler(
                CustomerInformationServiceConfig.ConnectionStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.StopRef != null && this.config.StopRef.Count > 0)
                {
                    this.handlersStopRef = new List<ElementHandler>(this.config.StopRef.Count);
                    foreach (var child in this.config.StopRef)
                    {
                        this.handlersStopRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.ConnectionRef != null && this.config.ConnectionRef.Count > 0)
                {
                    this.handlersConnectionRef = new List<ElementHandler>(this.config.ConnectionRef.Count);
                    foreach (var child in this.config.ConnectionRef)
                    {
                        this.handlersConnectionRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.ConnectionType != null && this.config.ConnectionType.Count > 0)
                {
                    this.handlersConnectionType = new List<ElementHandler>(this.config.ConnectionType.Count);
                    foreach (var child in this.config.ConnectionType)
                    {
                        this.handlersConnectionType.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DisplayContent != null)
                {
                    this.handlerDisplayContent = new DisplayContentStructureHandler(
                        this.config.DisplayContent, handlerFactory, context);
                }

                if (this.config.Platform != null && this.config.Platform.Count > 0)
                {
                    this.handlersPlatform = new List<ElementHandler>(this.config.Platform.Count);
                    foreach (var child in this.config.Platform)
                    {
                        this.handlersPlatform.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.ConnectionState != null && this.config.ConnectionState.Count > 0)
                {
                    this.handlersConnectionState = new List<ElementHandler>(this.config.ConnectionState.Count);
                    foreach (var child in this.config.ConnectionState)
                    {
                        this.handlersConnectionState.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.TransportMode != null)
                {
                    this.handlerTransportMode = new VehicleStructureHandler(
                        this.config.TransportMode, handlerFactory, context);
                }

                if (this.config.ExpectedDepatureTime != null && this.config.ExpectedDepatureTime.Count > 0)
                {
                    this.handlersExpectedDepatureTime = new List<ElementHandler>(this.config.ExpectedDepatureTime.Count);
                    foreach (var child in this.config.ExpectedDepatureTime)
                    {
                        this.handlersExpectedDepatureTime.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(ConnectionStructure data, int row, DataContext dataContext)
            {
                this.PrepareStopRef(data.StopRef, row, dataContext);

                this.PrepareConnectionRef(data.ConnectionRef, row, dataContext);

                this.PrepareConnectionType(data.ConnectionType, row, dataContext);

                bool prepared = false;
                this.PrepareDisplayContent(data.DisplayContent, row, dataContext, ref prepared);
                if (!prepared && this.handlerDisplayContent != null && data.DisplayContent != null)
                {
                    this.handlerDisplayContent.PrepareData(data.DisplayContent, row, dataContext);
                }

                this.PreparePlatform(data.Platform, row, dataContext);

                this.PrepareConnectionState(data.ConnectionState, row, dataContext);

                prepared = false;
                this.PrepareTransportMode(data.TransportMode, row, dataContext, ref prepared);
                if (!prepared && this.handlerTransportMode != null && data.TransportMode != null)
                {
                    this.handlerTransportMode.PrepareData(data.TransportMode, row, dataContext);
                }

                this.PrepareExpectedDepatureTime(data.ExpectedDepatureTime, row, dataContext);
            }

            public void HandleData(ConnectionStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleStopRef(data.StopRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersStopRef != null && data.StopRef != null)
                {
                    foreach (var handler in this.handlersStopRef)
                    {
                        try
                        {
                            handler.Handle(data.StopRef, "StopRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ConnectionStructure.StopRef");
                        }
                    }
                }

                handled = false;
                this.HandleConnectionRef(data.ConnectionRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersConnectionRef != null && data.ConnectionRef != null)
                {
                    foreach (var handler in this.handlersConnectionRef)
                    {
                        try
                        {
                            handler.Handle(data.ConnectionRef, "ConnectionRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ConnectionStructure.ConnectionRef");
                        }
                    }
                }

                handled = false;
                this.HandleConnectionType(data.ConnectionType, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersConnectionType != null)
                {
                    foreach (var handler in this.handlersConnectionType)
                    {
                        try
                        {
                            handler.Handle(data.ConnectionType, "ConnectionType", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ConnectionStructure.ConnectionType");
                        }
                    }
                }

                handled = false;
                this.HandleDisplayContent(data.DisplayContent, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerDisplayContent != null && data.DisplayContent != null)
                {
                    this.handlerDisplayContent.HandleData(data.DisplayContent, ximple, row, dataContext);
                }

                handled = false;
                this.HandlePlatform(data.Platform, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPlatform != null && data.Platform != null)
                {
                    foreach (var handler in this.handlersPlatform)
                    {
                        try
                        {
                            handler.Handle(data.Platform, "Platform", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ConnectionStructure.Platform");
                        }
                    }
                }

                handled = false;
                this.HandleConnectionState(data.ConnectionState, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersConnectionState != null && data.ConnectionStateSpecified)
                {
                    foreach (var handler in this.handlersConnectionState)
                    {
                        try
                        {
                            handler.Handle(data.ConnectionState, "ConnectionState", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ConnectionStructure.ConnectionState");
                        }
                    }
                }

                handled = false;
                this.HandleTransportMode(data.TransportMode, ximple, row, dataContext, ref handled);
                if (!handled && this.handlerTransportMode != null && data.TransportMode != null)
                {
                    this.handlerTransportMode.HandleData(data.TransportMode, ximple, row, dataContext);
                }

                handled = false;
                this.HandleExpectedDepatureTime(data.ExpectedDepatureTime, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersExpectedDepatureTime != null && data.ExpectedDepatureTime != null)
                {
                    foreach (var handler in this.handlersExpectedDepatureTime)
                    {
                        try
                        {
                            handler.Handle(data.ExpectedDepatureTime, "ExpectedDepatureTime", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform ConnectionStructure.ExpectedDepatureTime");
                        }
                    }
                }
            }
            

            partial void PrepareStopRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareConnectionRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareConnectionType(ConnectionTypeEnumeration item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareDisplayContent(
                DisplayContentStructure item, int row, DataContext dataContext, ref bool prepared);

            partial void PreparePlatform(IBISIPstring item, int row, DataContext dataContext);

            partial void PrepareConnectionState(ConnectionStateEnumeration item, int row, DataContext dataContext);

            // if implemented, the prepared flag should be set to true to prevent automatic updating of the item by the generated code
            partial void PrepareTransportMode(
                VehicleStructure item, int row, DataContext dataContext, ref bool prepared);

            partial void PrepareExpectedDepatureTime(IBISIPdateTime item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleStopRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleConnectionRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleConnectionType(
                ConnectionTypeEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDisplayContent(
                DisplayContentStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePlatform(
                IBISIPstring item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleConnectionState(
                ConnectionStateEnumeration item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleTransportMode(
                VehicleStructure item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleExpectedDepatureTime(
                IBISIPdateTime item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class VehicleStructureHandler
        {
            private readonly CustomerInformationServiceConfig.VehicleStructureConfig config;

            private readonly List<ElementHandler> handlersVehicleTypeRef;

            private readonly List<TranslatedElementHandler> handlersName;

            public VehicleStructureHandler(
                CustomerInformationServiceConfig.VehicleStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.VehicleTypeRef != null && this.config.VehicleTypeRef.Count > 0)
                {
                    this.handlersVehicleTypeRef = new List<ElementHandler>(this.config.VehicleTypeRef.Count);
                    foreach (var child in this.config.VehicleTypeRef)
                    {
                        this.handlersVehicleTypeRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.Name != null && this.config.Name.Count > 0)
                {
                    this.handlersName = new List<TranslatedElementHandler>(this.config.Name.Count);
                    foreach (var child in this.config.Name)
                    {
                        this.handlersName.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(VehicleStructure data, int row, DataContext dataContext)
            {
                this.PrepareVehicleTypeRef(data.VehicleTypeRef, row, dataContext);

                this.PrepareName(data.Name, row, dataContext);
            }

            public void HandleData(VehicleStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleVehicleTypeRef(data.VehicleTypeRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersVehicleTypeRef != null && data.VehicleTypeRef != null)
                {
                    foreach (var handler in this.handlersVehicleTypeRef)
                    {
                        try
                        {
                            handler.Handle(data.VehicleTypeRef, "VehicleTypeRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform VehicleStructure.VehicleTypeRef");
                        }
                    }
                }

                handled = false;
                this.HandleName(data.Name, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersName != null && data.Name != null)
                {
                    foreach (var handler in this.handlersName)
                    {
                        try
                        {
                            handler.Handle(data.Name, "Name", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform VehicleStructure.Name");
                        }
                    }
                }
            }
            

            partial void PrepareVehicleTypeRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareName(InternationalTextType[] item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleVehicleTypeRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleName(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class AdditionalAnnouncementStructureHandler
        {
            private readonly CustomerInformationServiceConfig.AdditionalAnnouncementStructureConfig config;

            private readonly List<ElementHandler> handlersAnnouncementRef;

            private readonly List<TranslatedElementHandler> handlersAnnouncementText;

            private readonly List<TranslatedElementHandler> handlersAnnouncementTTSText;

            public AdditionalAnnouncementStructureHandler(
                CustomerInformationServiceConfig.AdditionalAnnouncementStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.AnnouncementRef != null && this.config.AnnouncementRef.Count > 0)
                {
                    this.handlersAnnouncementRef = new List<ElementHandler>(this.config.AnnouncementRef.Count);
                    foreach (var child in this.config.AnnouncementRef)
                    {
                        this.handlersAnnouncementRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.AnnouncementText != null && this.config.AnnouncementText.Count > 0)
                {
                    this.handlersAnnouncementText = new List<TranslatedElementHandler>(this.config.AnnouncementText.Count);
                    foreach (var child in this.config.AnnouncementText)
                    {
                        this.handlersAnnouncementText.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

                if (this.config.AnnouncementTTSText != null && this.config.AnnouncementTTSText.Count > 0)
                {
                    this.handlersAnnouncementTTSText = new List<TranslatedElementHandler>(this.config.AnnouncementTTSText.Count);
                    foreach (var child in this.config.AnnouncementTTSText)
                    {
                        this.handlersAnnouncementTTSText.Add(handlerFactory.CreateTranslatedElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(AdditionalAnnouncementStructure data, int row, DataContext dataContext)
            {
                this.PrepareAnnouncementRef(data.AnnouncementRef, row, dataContext);

                this.PrepareAnnouncementText(data.AnnouncementText, row, dataContext);

                this.PrepareAnnouncementTTSText(data.AnnouncementTTSText, row, dataContext);
            }

            public void HandleData(AdditionalAnnouncementStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandleAnnouncementRef(data.AnnouncementRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAnnouncementRef != null && data.AnnouncementRef != null)
                {
                    foreach (var handler in this.handlersAnnouncementRef)
                    {
                        try
                        {
                            handler.Handle(data.AnnouncementRef, "AnnouncementRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform AdditionalAnnouncementStructure.AnnouncementRef");
                        }
                    }
                }

                handled = false;
                this.HandleAnnouncementText(data.AnnouncementText, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAnnouncementText != null && data.AnnouncementText != null)
                {
                    foreach (var handler in this.handlersAnnouncementText)
                    {
                        try
                        {
                            handler.Handle(data.AnnouncementText, "AnnouncementText", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform AdditionalAnnouncementStructure.AnnouncementText");
                        }
                    }
                }

                handled = false;
                this.HandleAnnouncementTTSText(data.AnnouncementTTSText, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersAnnouncementTTSText != null && data.AnnouncementTTSText != null)
                {
                    foreach (var handler in this.handlersAnnouncementTTSText)
                    {
                        try
                        {
                            handler.Handle(data.AnnouncementTTSText, "AnnouncementTTSText", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform AdditionalAnnouncementStructure.AnnouncementTTSText");
                        }
                    }
                }
            }
            

            partial void PrepareAnnouncementRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareAnnouncementText(InternationalTextType[] item, int row, DataContext dataContext);

            partial void PrepareAnnouncementTTSText(InternationalTextType[] item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAnnouncementRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAnnouncementText(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleAnnouncementTTSText(
                InternationalTextType[] item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }

        private partial class SpecificPointStructureHandler
        {
            private readonly CustomerInformationServiceConfig.SpecificPointStructureConfig config;

            private readonly List<ElementHandler> handlersPointRef;

            private readonly List<ElementHandler> handlersDistanceToPreviousPoint;

            public SpecificPointStructureHandler(
                CustomerInformationServiceConfig.SpecificPointStructureConfig config,
                IElementHandlerFactory handlerFactory,
                IHandlerConfigContext context)
            {
                this.config = config;
                
                if (this.config.PointRef != null && this.config.PointRef.Count > 0)
                {
                    this.handlersPointRef = new List<ElementHandler>(this.config.PointRef.Count);
                    foreach (var child in this.config.PointRef)
                    {
                        this.handlersPointRef.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

                if (this.config.DistanceToPreviousPoint != null && this.config.DistanceToPreviousPoint.Count > 0)
                {
                    this.handlersDistanceToPreviousPoint = new List<ElementHandler>(this.config.DistanceToPreviousPoint.Count);
                    foreach (var child in this.config.DistanceToPreviousPoint)
                    {
                        this.handlersDistanceToPreviousPoint.Add(handlerFactory.CreateElementHandler(child, context));
                    }
                }

            }

            public void PrepareData(SpecificPointStructure data, int row, DataContext dataContext)
            {
                this.PreparePointRef(data.PointRef, row, dataContext);

                this.PrepareDistanceToPreviousPoint(data.DistanceToPreviousPoint, row, dataContext);
            }

            public void HandleData(SpecificPointStructure data, Ximple ximple, int row, DataContext dataContext)
            {
                bool handled;

                handled = false;
                this.HandlePointRef(data.PointRef, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersPointRef != null && data.PointRef != null)
                {
                    foreach (var handler in this.handlersPointRef)
                    {
                        try
                        {
                            handler.Handle(data.PointRef, "PointRef", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform SpecificPointStructure.PointRef");
                        }
                    }
                }

                handled = false;
                this.HandleDistanceToPreviousPoint(data.DistanceToPreviousPoint, ximple, row, dataContext, ref handled);
                if (!handled && this.handlersDistanceToPreviousPoint != null && data.DistanceToPreviousPoint != null)
                {
                    foreach (var handler in this.handlersDistanceToPreviousPoint)
                    {
                        try
                        {
                            handler.Handle(data.DistanceToPreviousPoint, "DistanceToPreviousPoint", ximple, row);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, 
                                "Couldn't transform SpecificPointStructure.DistanceToPreviousPoint");
                        }
                    }
                }
            }
            

            partial void PreparePointRef(IBISIPNMTOKEN item, int row, DataContext dataContext);

            partial void PrepareDistanceToPreviousPoint(IBISIPdouble item, int row, DataContext dataContext);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandlePointRef(
                IBISIPNMTOKEN item, Ximple ximple, int row, DataContext dataContext, ref bool handled);

            // if implemented, the handled flag should be set to true to prevent automatic handling of the item by the generated code
            partial void HandleDistanceToPreviousPoint(
                IBISIPdouble item, Ximple ximple, int row, DataContext dataContext, ref bool handled);
        }


    }
    
}

