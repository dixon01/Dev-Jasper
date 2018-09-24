

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

using Gorba.Common.Utility.Core;

using NLog;

namespace Gorba.Common.Configuration.Protran.VDV301
{
    [Serializable]
    public class ServicesConfig
    {
        public ServicesConfig()
        {
            this.CustomerInformationService = new CustomerInformationServiceConfig();
        }
        
        public CustomerInformationServiceConfig CustomerInformationService { get; set; }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool ValidateHttpRequests { get; set; }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool ValidateHttpResponses { get; set; }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool VerifyVersion { get; set; }
    }
    
    [Serializable]
    public partial class CustomerInformationServiceConfig : ServiceConfigBase
    {
        public GetAllDataConfig GetAllData { get; set; }

        public GetCurrentAnnouncementConfig GetCurrentAnnouncement { get; set; }

        public GetCurrentConnectionInformationConfig GetCurrentConnectionInformation { get; set; }

        public GetCurrentDisplayContentConfig GetCurrentDisplayContent { get; set; }

        public GetCurrentStopPointConfig GetCurrentStopPoint { get; set; }

        public GetCurrentStopIndexConfig GetCurrentStopIndex { get; set; }

        public GetTripDataConfig GetTripData { get; set; }

        public GetVehicleDataConfig GetVehicleData { get; set; }

        [Serializable]
        public class GetAllDataConfig : OperationConfigBase
        {
            public GetAllDataConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
                this.VehicleRef = new List<DataItemConfig>();
                this.DefaultLanguage = new List<DataItemConfig>();
                this.CurrentStopIndex = new List<DataItemConfig>();
                this.RouteDeviation = new List<DataItemConfig>();
                this.DoorState = new List<DataItemConfig>();
                this.InPanic = new List<DataItemConfig>();
                this.VehicleStopRequested = new List<DataItemConfig>();
                this.ExitSide = new List<DataItemConfig>();
                this.MovingDirectionForward = new List<DataItemConfig>();
                this.VehicleMode = new List<DataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            [XmlElement("VehicleRef")]
            public List<DataItemConfig> VehicleRef { get; set; }

            [XmlElement("DefaultLanguage")]
            public List<DataItemConfig> DefaultLanguage { get; set; }

            public TripInformationStructureConfig TripInformation { get; set; }

            [XmlElement("CurrentStopIndex")]
            public List<DataItemConfig> CurrentStopIndex { get; set; }

            [XmlElement("RouteDeviation")]
            public List<DataItemConfig> RouteDeviation { get; set; }

            [XmlElement("DoorState")]
            public List<DataItemConfig> DoorState { get; set; }

            [XmlElement("InPanic")]
            public List<DataItemConfig> InPanic { get; set; }

            [XmlElement("VehicleStopRequested")]
            public List<DataItemConfig> VehicleStopRequested { get; set; }

            [XmlElement("ExitSide")]
            public List<DataItemConfig> ExitSide { get; set; }

            [XmlElement("MovingDirectionForward")]
            public List<DataItemConfig> MovingDirectionForward { get; set; }

            [XmlElement("VehicleMode")]
            public List<DataItemConfig> VehicleMode { get; set; }

        }

        [Serializable]
        public class GetCurrentAnnouncementConfig : OperationConfigBase
        {
            public GetCurrentAnnouncementConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            public AnnouncementStructureConfig CurrentAnnouncement { get; set; }

        }

        [Serializable]
        public class GetCurrentConnectionInformationConfig : OperationConfigBase
        {
            public GetCurrentConnectionInformationConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            public ConnectionStructureConfig CurrentConnection { get; set; }

        }

        [Serializable]
        public class GetCurrentDisplayContentConfig : OperationConfigBase
        {
            public GetCurrentDisplayContentConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            public DisplayContentStructureConfig CurrentDisplayContent { get; set; }

        }

        [Serializable]
        public class GetCurrentStopPointConfig : OperationConfigBase
        {
            public GetCurrentStopPointConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            public StopInformationStructureConfig CurrentStopPoint { get; set; }

        }

        [Serializable]
        public class GetCurrentStopIndexConfig : OperationConfigBase
        {
            public GetCurrentStopIndexConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
                this.CurrentStopIndex = new List<DataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            [XmlElement("CurrentStopIndex")]
            public List<DataItemConfig> CurrentStopIndex { get; set; }

        }

        [Serializable]
        public class GetTripDataConfig : OperationConfigBase
        {
            public GetTripDataConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
                this.VehicleRef = new List<DataItemConfig>();
                this.DefaultLanguage = new List<DataItemConfig>();
                this.CurrentStopIndex = new List<DataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            [XmlElement("VehicleRef")]
            public List<DataItemConfig> VehicleRef { get; set; }

            [XmlElement("DefaultLanguage")]
            public List<DataItemConfig> DefaultLanguage { get; set; }

            public TripInformationStructureConfig TripInformation { get; set; }

            [XmlElement("CurrentStopIndex")]
            public List<DataItemConfig> CurrentStopIndex { get; set; }

        }

        [Serializable]
        public class GetVehicleDataConfig : OperationConfigBase
        {
            public GetVehicleDataConfig()
            {
                this.TimeStamp = new List<DateTimeDataItemConfig>();
                this.VehicleRef = new List<DataItemConfig>();
                this.RouteDeviation = new List<DataItemConfig>();
                this.DoorState = new List<DataItemConfig>();
                this.InPanic = new List<DataItemConfig>();
                this.VehicleStopRequested = new List<DataItemConfig>();
                this.ExitSide = new List<DataItemConfig>();
                this.MovingDirectionForward = new List<DataItemConfig>();
                this.VehicleMode = new List<DataItemConfig>();
            }
            [XmlElement("TimeStamp")]
            public List<DateTimeDataItemConfig> TimeStamp { get; set; }

            [XmlElement("VehicleRef")]
            public List<DataItemConfig> VehicleRef { get; set; }

            [XmlElement("RouteDeviation")]
            public List<DataItemConfig> RouteDeviation { get; set; }

            [XmlElement("DoorState")]
            public List<DataItemConfig> DoorState { get; set; }

            [XmlElement("InPanic")]
            public List<DataItemConfig> InPanic { get; set; }

            [XmlElement("VehicleStopRequested")]
            public List<DataItemConfig> VehicleStopRequested { get; set; }

            [XmlElement("ExitSide")]
            public List<DataItemConfig> ExitSide { get; set; }

            [XmlElement("MovingDirectionForward")]
            public List<DataItemConfig> MovingDirectionForward { get; set; }

            [XmlElement("VehicleMode")]
            public List<DataItemConfig> VehicleMode { get; set; }

        }

        [Serializable]
        public class TripInformationStructureConfig
        {
            public TripInformationStructureConfig()
            {
                this.TripRef = new List<DataItemConfig>();
                this.LocationState = new List<DataItemConfig>();
                this.TimetableDelay = new List<DataItemConfig>();
                this.AdditionalTextMessage = new List<DataItemConfig>();
            }
            [XmlElement("TripRef")]
            public List<DataItemConfig> TripRef { get; set; }

            public StopSequenceStructureConfig StopSequence { get; set; }

            [XmlElement("LocationState")]
            public List<DataItemConfig> LocationState { get; set; }

            [XmlElement("TimetableDelay")]
            public List<DataItemConfig> TimetableDelay { get; set; }

            [XmlElement("AdditionalTextMessage")]
            public List<DataItemConfig> AdditionalTextMessage { get; set; }

            public AdditionalAnnouncementStructureConfig AdditionalAnnouncement { get; set; }

        }

        [Serializable]
        public class StopSequenceStructureConfig
        {
            public StopSequenceStructureConfig()
            {
            }
            public StopInformationStructureConfig StopPoint { get; set; }

        }

        [Serializable]
        public class StopInformationStructureConfig
        {
            public StopInformationStructureConfig()
            {
                this.StopIndex = new List<DataItemConfig>();
                this.StopRef = new List<DataItemConfig>();
                this.StopName = new List<InternationalTextDataItemConfig>();
                this.StopAlternativeName = new List<InternationalTextDataItemConfig>();
                this.Platform = new List<DataItemConfig>();
                this.ArrivalScheduled = new List<DateTimeDataItemConfig>();
                this.DepartureScheduled = new List<DateTimeDataItemConfig>();
                this.RecordedArrivalTime = new List<DateTimeDataItemConfig>();
                this.DistanceToNextStop = new List<DataItemConfig>();
                this.FareZone = new List<DataItemConfig>();
            }
            [XmlElement("StopIndex")]
            public List<DataItemConfig> StopIndex { get; set; }

            [XmlElement("StopRef")]
            public List<DataItemConfig> StopRef { get; set; }

            [XmlElement("StopName")]
            public List<InternationalTextDataItemConfig> StopName { get; set; }

            [XmlElement("StopAlternativeName")]
            public List<InternationalTextDataItemConfig> StopAlternativeName { get; set; }

            [XmlElement("Platform")]
            public List<DataItemConfig> Platform { get; set; }

            public DisplayContentStructureConfig DisplayContent { get; set; }

            public AnnouncementStructureConfig StopAnnouncement { get; set; }

            [XmlElement("ArrivalScheduled")]
            public List<DateTimeDataItemConfig> ArrivalScheduled { get; set; }

            [XmlElement("DepartureScheduled")]
            public List<DateTimeDataItemConfig> DepartureScheduled { get; set; }

            [XmlElement("RecordedArrivalTime")]
            public List<DateTimeDataItemConfig> RecordedArrivalTime { get; set; }

            [XmlElement("DistanceToNextStop")]
            public List<DataItemConfig> DistanceToNextStop { get; set; }

            public ConnectionStructureConfig Connection { get; set; }

            [XmlElement("FareZone")]
            public List<DataItemConfig> FareZone { get; set; }

        }

        [Serializable]
        public class DisplayContentStructureConfig
        {
            public DisplayContentStructureConfig()
            {
                this.DisplayContentRef = new List<DataItemConfig>();
                this.AdditionalInformation = new List<InternationalTextDataItemConfig>();
                this.Priority = new List<DataItemConfig>();
                this.PeriodDuration = new List<DataItemConfig>();
                this.Duration = new List<DataItemConfig>();
            }
            [XmlElement("DisplayContentRef")]
            public List<DataItemConfig> DisplayContentRef { get; set; }

            public LineInformationStructureConfig LineInformation { get; set; }

            public DestinationStructureConfig Destination { get; set; }

            public ViaPointStructureConfig ViaPoint { get; set; }

            [XmlElement("AdditionalInformation")]
            public List<InternationalTextDataItemConfig> AdditionalInformation { get; set; }

            [XmlElement("Priority")]
            public List<DataItemConfig> Priority { get; set; }

            [XmlElement("PeriodDuration")]
            public List<DataItemConfig> PeriodDuration { get; set; }

            [XmlElement("Duration")]
            public List<DataItemConfig> Duration { get; set; }

        }

        [Serializable]
        public class LineInformationStructureConfig
        {
            public LineInformationStructureConfig()
            {
                this.LineRef = new List<DataItemConfig>();
                this.LineName = new List<InternationalTextDataItemConfig>();
                this.LineShortName = new List<InternationalTextDataItemConfig>();
                this.LineNumber = new List<DataItemConfig>();
            }
            [XmlElement("LineRef")]
            public List<DataItemConfig> LineRef { get; set; }

            [XmlElement("LineName")]
            public List<InternationalTextDataItemConfig> LineName { get; set; }

            [XmlElement("LineShortName")]
            public List<InternationalTextDataItemConfig> LineShortName { get; set; }

            [XmlElement("LineNumber")]
            public List<DataItemConfig> LineNumber { get; set; }

        }

        [Serializable]
        public class DestinationStructureConfig
        {
            public DestinationStructureConfig()
            {
                this.DestinationRef = new List<DataItemConfig>();
                this.DestinationName = new List<InternationalTextDataItemConfig>();
                this.DestinationShortName = new List<InternationalTextDataItemConfig>();
            }
            [XmlElement("DestinationRef")]
            public List<DataItemConfig> DestinationRef { get; set; }

            [XmlElement("DestinationName")]
            public List<InternationalTextDataItemConfig> DestinationName { get; set; }

            [XmlElement("DestinationShortName")]
            public List<InternationalTextDataItemConfig> DestinationShortName { get; set; }

        }

        [Serializable]
        public class ViaPointStructureConfig
        {
            public ViaPointStructureConfig()
            {
                this.ViaPointRef = new List<DataItemConfig>();
                this.PlaceRef = new List<DataItemConfig>();
                this.PlaceName = new List<InternationalTextDataItemConfig>();
                this.PlaceShortName = new List<InternationalTextDataItemConfig>();
                this.ViaPointDisplayPriority = new List<DataItemConfig>();
            }
            [XmlElement("ViaPointRef")]
            public List<DataItemConfig> ViaPointRef { get; set; }

            [XmlElement("PlaceRef")]
            public List<DataItemConfig> PlaceRef { get; set; }

            [XmlElement("PlaceName")]
            public List<InternationalTextDataItemConfig> PlaceName { get; set; }

            [XmlElement("PlaceShortName")]
            public List<InternationalTextDataItemConfig> PlaceShortName { get; set; }

            [XmlElement("ViaPointDisplayPriority")]
            public List<DataItemConfig> ViaPointDisplayPriority { get; set; }

        }

        [Serializable]
        public class AnnouncementStructureConfig
        {
            public AnnouncementStructureConfig()
            {
                this.AnnouncementRef = new List<DataItemConfig>();
                this.AnnouncementText = new List<InternationalTextDataItemConfig>();
                this.AnnouncementTTSText = new List<InternationalTextDataItemConfig>();
            }
            [XmlElement("AnnouncementRef")]
            public List<DataItemConfig> AnnouncementRef { get; set; }

            [XmlElement("AnnouncementText")]
            public List<InternationalTextDataItemConfig> AnnouncementText { get; set; }

            [XmlElement("AnnouncementTTSText")]
            public List<InternationalTextDataItemConfig> AnnouncementTTSText { get; set; }

        }

        [Serializable]
        public class ConnectionStructureConfig
        {
            public ConnectionStructureConfig()
            {
                this.StopRef = new List<DataItemConfig>();
                this.ConnectionRef = new List<DataItemConfig>();
                this.ConnectionType = new List<DataItemConfig>();
                this.Platform = new List<DataItemConfig>();
                this.ConnectionState = new List<DataItemConfig>();
                this.ExpectedDepatureTime = new List<DateTimeDataItemConfig>();
            }
            [XmlElement("StopRef")]
            public List<DataItemConfig> StopRef { get; set; }

            [XmlElement("ConnectionRef")]
            public List<DataItemConfig> ConnectionRef { get; set; }

            [XmlElement("ConnectionType")]
            public List<DataItemConfig> ConnectionType { get; set; }

            public DisplayContentStructureConfig DisplayContent { get; set; }

            [XmlElement("Platform")]
            public List<DataItemConfig> Platform { get; set; }

            [XmlElement("ConnectionState")]
            public List<DataItemConfig> ConnectionState { get; set; }

            public VehicleStructureConfig TransportMode { get; set; }

            [XmlElement("ExpectedDepatureTime")]
            public List<DateTimeDataItemConfig> ExpectedDepatureTime { get; set; }

        }

        [Serializable]
        public class VehicleStructureConfig
        {
            public VehicleStructureConfig()
            {
                this.VehicleTypeRef = new List<DataItemConfig>();
                this.Name = new List<InternationalTextDataItemConfig>();
            }
            [XmlElement("VehicleTypeRef")]
            public List<DataItemConfig> VehicleTypeRef { get; set; }

            [XmlElement("Name")]
            public List<InternationalTextDataItemConfig> Name { get; set; }

        }

        [Serializable]
        public class AdditionalAnnouncementStructureConfig
        {
            public AdditionalAnnouncementStructureConfig()
            {
                this.AnnouncementRef = new List<DataItemConfig>();
                this.AnnouncementText = new List<InternationalTextDataItemConfig>();
                this.AnnouncementTTSText = new List<InternationalTextDataItemConfig>();
            }
            [XmlElement("AnnouncementRef")]
            public List<DataItemConfig> AnnouncementRef { get; set; }

            [XmlElement("AnnouncementText")]
            public List<InternationalTextDataItemConfig> AnnouncementText { get; set; }

            [XmlElement("AnnouncementTTSText")]
            public List<InternationalTextDataItemConfig> AnnouncementTTSText { get; set; }

        }

        [Serializable]
        public class SpecificPointStructureConfig
        {
            public SpecificPointStructureConfig()
            {
                this.PointRef = new List<DataItemConfig>();
                this.DistanceToPreviousPoint = new List<DataItemConfig>();
            }
            [XmlElement("PointRef")]
            public List<DataItemConfig> PointRef { get; set; }

            [XmlElement("DistanceToPreviousPoint")]
            public List<DataItemConfig> DistanceToPreviousPoint { get; set; }

        }
    }
    }

