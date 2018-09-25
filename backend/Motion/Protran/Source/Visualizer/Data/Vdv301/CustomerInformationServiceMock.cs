// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerInformationServiceMock.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomerInformationServiceMock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Vdv301
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The mock for the VDV 301 CustomerInformationService.
    /// </summary>
    public class CustomerInformationServiceMock : ICustomerInformationService
    {
        private readonly EventHandlerList eventHandlers = new EventHandlerList();

        private CustomerInformationServiceAllData allData;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerInformationServiceMock"/> class.
        /// </summary>
        public CustomerInformationServiceMock()
        {
            this.allData = new CustomerInformationServiceAllData();
        }

        private delegate T Function<T>();

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetAllData"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceAllData>> AllDataChanged
        {
            add
            {
                this.eventHandlers.AddHandler("AllData", value);
                this.RaiseLater(value, this.GetAllData);
            }

            remove
            {
                this.eventHandlers.RemoveHandler("AllData", value);
            }
        }

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetCurrentAnnouncement"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentAnnouncementData>>
            CurrentAnnouncementChanged
            {
                add
                {
                    this.eventHandlers.AddHandler("CurrentAnnouncement", value);
                    this.RaiseLater(value, this.GetCurrentAnnouncement);
                }

                remove
                {
                    this.eventHandlers.RemoveHandler("CurrentAnnouncement", value);
                }
            }

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetCurrentConnectionInformation"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentConnectionInformationData>>
            CurrentConnectionInformationChanged
            {
                add
                {
                    this.eventHandlers.AddHandler("CurrentConnectionInformation", value);
                    this.RaiseLater(value, this.GetCurrentConnectionInformation);
                }

                remove
                {
                    this.eventHandlers.RemoveHandler("CurrentConnectionInformation", value);
                }
            }

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetCurrentDisplayContent"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentDisplayContentData>>
            CurrentDisplayContentChanged
            {
                add
                {
                    this.eventHandlers.AddHandler("CurrentDisplayContent", value);
                    this.RaiseLater(value, this.GetCurrentDisplayContent);
                }

                remove
                {
                    this.eventHandlers.RemoveHandler("CurrentDisplayContent", value);
                }
            }

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetCurrentStopPoint"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopPointData>>
            CurrentStopPointChanged
            {
                add
                {
                    this.eventHandlers.AddHandler("CurrentStopPoint", value);
                    this.RaiseLater(value, this.GetCurrentStopPoint);
                }

                remove
                {
                    this.eventHandlers.RemoveHandler("CurrentStopPoint", value);
                }
            }

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetCurrentStopIndex"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceCurrentStopIndexData>>
            CurrentStopIndexChanged
            {
                add
                {
                    this.eventHandlers.AddHandler("CurrentStopIndex", value);
                    this.RaiseLater(value, this.GetCurrentStopIndex);
                }

                remove
                {
                    this.eventHandlers.RemoveHandler("CurrentStopIndex", value);
                }
            }

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetTripData"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceTripData>> TripDataChanged
        {
            add
            {
                this.eventHandlers.AddHandler("TripData", value);
                this.RaiseLater(value, this.GetTripData);
            }

            remove
            {
                this.eventHandlers.RemoveHandler("TripData", value);
            }
        }

        /// <summary>
        /// Event that is fired whenever the result of <see cref="GetVehicleData"/> changes.
        /// </summary>
        public event EventHandler<DataUpdateEventArgs<CustomerInformationServiceVehicleData>> VehicleDataChanged
        {
            add
            {
                this.eventHandlers.AddHandler("VehicleData", value);
                this.RaiseLater(value, this.GetVehicleData);
            }

            remove
            {
                this.eventHandlers.RemoveHandler("VehicleData", value);
            }
        }

        /// <summary>
        /// Gets all data.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceAllData"/>.
        /// </returns>
        public CustomerInformationServiceAllData GetAllData()
        {
            return new CustomerInformationServiceAllData
                       {
                           TimeStamp = this.CreateTimeStamp(),
                           VehicleRef = this.allData.VehicleRef,
                           DefaultLanguage = this.allData.DefaultLanguage,
                           TripInformation = this.allData.TripInformation,
                           CurrentStopIndex = this.allData.CurrentStopIndex,
                           RouteDeviation = this.allData.RouteDeviation,
                           DoorStateSpecified = this.allData.DoorStateSpecified,
                           DoorState = this.allData.DoorState,
                           InPanic = this.allData.InPanic,
                           VehicleStopRequested = this.allData.VehicleStopRequested,
                           ExitSideSpecified = this.allData.ExitSideSpecified,
                           ExitSide = this.allData.ExitSide,
                           MovingDirectionForward = this.allData.MovingDirectionForward,
                           VehicleModeSpecified = this.allData.VehicleModeSpecified,
                           VehicleMode = this.allData.VehicleMode
                       };
        }

        /// <summary>
        /// Gets the current announcement.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentAnnouncementData"/>.
        /// </returns>
        public CustomerInformationServiceCurrentAnnouncementData GetCurrentAnnouncement()
        {
            var announcement = new CustomerInformationServiceCurrentAnnouncementData
                                   {
                                       TimeStamp =
                                           this.CreateTimeStamp()
                                   };
            var stop = this.GetCurrentStopPoint();
            if (stop.CurrentStopPoint != null && stop.CurrentStopPoint.StopAnnouncement != null
                && stop.CurrentStopPoint.StopAnnouncement.Length > 0)
            {
                announcement.CurrentAnnouncement = stop.CurrentStopPoint.StopAnnouncement[0];
            }

            return announcement;
        }

        /// <summary>
        /// Gets the current connection information.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentConnectionInformationData"/>.
        /// </returns>
        public CustomerInformationServiceCurrentConnectionInformationData GetCurrentConnectionInformation()
        {
            var connections = new CustomerInformationServiceCurrentConnectionInformationData
                                  {
                                      TimeStamp =
                                          this
                                          .CreateTimeStamp()
                                  };
            var stop = this.GetCurrentStopPoint();
            if (stop.CurrentStopPoint != null && stop.CurrentStopPoint.Connection != null
                && stop.CurrentStopPoint.Connection.Length > 0)
            {
                connections.CurrentConnection = stop.CurrentStopPoint.Connection[0];
            }

            return connections;
        }

        /// <summary>
        /// Gets the current display content.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentDisplayContentData"/>.
        /// </returns>
        public CustomerInformationServiceCurrentDisplayContentData GetCurrentDisplayContent()
        {
            var displayContent = new CustomerInformationServiceCurrentDisplayContentData
                                     {
                                         TimeStamp =
                                             this.CreateTimeStamp()
                                     };
            var stop = this.GetCurrentStopPoint();
            if (stop.CurrentStopPoint != null && stop.CurrentStopPoint.DisplayContent != null
                && stop.CurrentStopPoint.DisplayContent.Length > 0)
            {
                displayContent.CurrentDisplayContent = stop.CurrentStopPoint.DisplayContent[0];
            }

            return displayContent;
        }

        /// <summary>
        /// Gets the current stop point.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentStopPointData"/>.
        /// </returns>
        public CustomerInformationServiceCurrentStopPointData GetCurrentStopPoint()
        {
            var stopPoint = new CustomerInformationServiceCurrentStopPointData { TimeStamp = this.CreateTimeStamp() };
            var trip = this.GetTripData();
            if (trip.CurrentStopIndex != null && !trip.CurrentStopIndex.ErrorCodeSpecified
                && trip.TripInformation != null && trip.TripInformation.StopSequence != null)
            {
                foreach (var stop in trip.TripInformation.StopSequence)
                {
                    if (stop.StopIndex.Value == trip.CurrentStopIndex.Value)
                    {
                        stopPoint.CurrentStopPoint = stop;
                        break;
                    }
                }
            }

            return stopPoint;
        }

        /// <summary>
        /// Gets the current stop point index.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceCurrentStopIndexData"/>.
        /// </returns>
        public CustomerInformationServiceCurrentStopIndexData GetCurrentStopIndex()
        {
            return new CustomerInformationServiceCurrentStopIndexData
                       {
                           TimeStamp = this.CreateTimeStamp(),
                           CurrentStopIndex = this.allData.CurrentStopIndex
                       };
        }

        /// <summary>
        /// Gets the current trip data.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceTripData"/>.
        /// </returns>
        public CustomerInformationServiceTripData GetTripData()
        {
            var stopIndex = this.GetCurrentStopIndex();
            var tripData = new CustomerInformationServiceTripData
                               {
                                   TimeStamp = this.CreateTimeStamp(),
                                   CurrentStopIndex = stopIndex.CurrentStopIndex,
                                   DefaultLanguage = this.allData.DefaultLanguage,
                                   VehicleRef = this.allData.VehicleRef
                               };
            if (this.allData.TripInformation != null && this.allData.TripInformation.Length != 0)
            {
                tripData.TripInformation = this.allData.TripInformation[0];
            }

            return tripData;
        }

        /// <summary>
        /// Gets the vehicle data.
        /// </summary>
        /// <returns>
        /// The <see cref="CustomerInformationServiceVehicleData"/>.
        /// </returns>
        public CustomerInformationServiceVehicleData GetVehicleData()
        {
            return new CustomerInformationServiceVehicleData
                       {
                           TimeStamp = this.CreateTimeStamp(),
                           VehicleRef = this.allData.VehicleRef,
                           RouteDeviation = this.allData.RouteDeviation,
                           DoorStateSpecified = this.allData.DoorStateSpecified,
                           DoorState = this.allData.DoorState,
                           InPanic = this.allData.InPanic,
                           VehicleStopRequested = this.allData.VehicleStopRequested,
                           ExitSideSpecified = this.allData.ExitSideSpecified,
                           ExitSide = this.allData.ExitSide,
                           MovingDirectionForward =
                               this.allData.MovingDirectionForward,
                           VehicleModeSpecified = this.allData.VehicleModeSpecified,
                           VehicleMode = this.allData.VehicleMode
                       };
        }

        /// <summary>
        /// Retrieves a partial stop sequence.
        /// </summary>
        /// <param name="startingStopIndex">
        /// The starting stop index.
        /// </param>
        /// <param name="numberOfStopPoints">
        /// The number of stop points.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerInformationServicePartialStopSequenceData"/>.
        /// </returns>
        public CustomerInformationServicePartialStopSequenceData RetrievePartialStopSequence(
            IBISIPint startingStopIndex, IBISIPint numberOfStopPoints)
        {
            var stopSequence = new CustomerInformationServicePartialStopSequenceData
                                   {
                                       TimeStamp =
                                           this.CreateTimeStamp()
                                   };
            var stops = new List<StopInformationStructure>(numberOfStopPoints.Value);
            var tripData = this.GetTripData();
            if (tripData.TripInformation != null && tripData.TripInformation.StopSequence != null)
            {
                for (int offset = 0; offset < numberOfStopPoints.Value; offset++)
                {
                    var index = startingStopIndex.Value + offset;
                    var stop = Array.Find(tripData.TripInformation.StopSequence, s => s.StopIndex.Value == index);
                    if (stop != null)
                    {
                        stops.Add(stop);
                    }
                }
            }

            stopSequence.StopSequence = stops.ToArray();
            return stopSequence;
        }

        /// <summary>
        /// Sets all data in this service.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        internal void SetAllData(CustomerInformationServiceAllData data)
        {
            this.allData = data;

            // TODO: we could be more intelligent and only raise those events when really something changed
            this.RaiseEvent("AllData", this.GetAllData);
            this.RaiseEvent("CurrentAnnouncement", this.GetCurrentAnnouncement);
            this.RaiseEvent("CurrentConnectionInformation", this.GetCurrentConnectionInformation);
            this.RaiseEvent("CurrentDisplayContent", this.GetCurrentDisplayContent);
            this.RaiseEvent("CurrentStopIndex", this.GetCurrentStopIndex);
            this.RaiseEvent("CurrentStopPoint", this.GetCurrentStopPoint);
            this.RaiseEvent("TripData", this.GetTripData);
            this.RaiseEvent("VehicleData", this.GetVehicleData);
        }

        private void RaiseEvent<T>(string eventName, Function<T> getter)
        {
            var handler = (EventHandler<DataUpdateEventArgs<T>>)this.eventHandlers[eventName];
            if (handler != null)
            {
                handler(this, new DataUpdateEventArgs<T>(getter()));
            }
        }

        private IBISIPdateTime CreateTimeStamp()
        {
            return new IBISIPdateTime(TimeProvider.Current.Now);
        }

        private void RaiseLater<T>(EventHandler<DataUpdateEventArgs<T>> eventHandler, Function<T> getter)
        {
            var timer = TimerFactory.Current.CreateTimer("RaiseLater");
            timer.AutoReset = false;
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Elapsed += (sender, args) => eventHandler(this, new DataUpdateEventArgs<T>(getter()));
            timer.Enabled = true;
        }
    }
}