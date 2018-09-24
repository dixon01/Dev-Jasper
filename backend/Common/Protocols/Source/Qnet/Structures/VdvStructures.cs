// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VdvStructures.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Definitions for subtype of VDV datagrams
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Gorba.Common.Protocols.Qnet.Structures
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Definitions for subtype of VDV datagrams
    /// </summary>
    public enum VdvSubType : byte
    {
        /// <summary>
        /// Vdv Alive type
        /// </summary>
        VdvAlive = 0,

        /// <summary>
        /// Scheduled timetable data subtype for vdv messages => VDV_TYP_FAHRPLAN = 1,
        /// </summary>        
        ScheduledTimetable = 1,

        /// <summary>
        /// Realtime data subtype for vdv messages => VDV_TYP_FAHRPLANLAGE = 2,
        /// </summary>       
        RealtimeData = 2,

        /// <summary>
        /// Type to indicate a trip deletion => VDV_TYP_FAHRTLOESCHEN = 3
        /// </summary>
        DeleteTrip = 3,

        /// <summary>
        /// Special line notice message => VDV_TYP_SPEZIALTEXT = 4
        /// </summary>
        SpecialLineText = 4,

        /// <summary>
        /// Delete special line notice message => SpecialLineTextDeletion = 5
        /// </summary>
        SpecialLineTextDeletion = 5,

        /// <summary>
        /// Reference text send to dispositions => VDV_TYP_REF_TEXT = 6
        /// </summary>
        ReferenceText = 6,

        /// <summary>
        /// Scheduled data request message from iqube (no data, only a request)
        /// => VDV_TYP_FAHRPLAN_ANFRAGE = 10
        /// </summary>
        VdvSubtypScheduledTimetableDataRequest = 10,
       
        /// <summary>
        /// Scheduled data request message from iqube (no data, only a request)
        /// => VDV_TYP_REF_TEXT_ANFRAGE = 11
        /// </summary>
        VdvSubtypRefTextRequest = 11,

        /// <summary>
        /// </summary>
        VDV_TYP_ALARM = 20,

        /// <summary>
        /// </summary>
        VDV_TYP_ZANASY = 30,
    }

    /// <summary>
    /// Definition of VDV reference text type
    /// </summary>
    public enum VdvReferenceTextType : byte
    {
        /// <summary>
        /// Bus line text reference for VDV text 
        /// </summary>
        VdvRefLinetext = 1,

        /// <summary>
        /// Destination text reference for VDV text
        /// </summary>
        VdvRefDestinationtext = 2,

        /// <summary>
        /// Stop position text reference for VDV text
        /// </summary>
        VdvRefStopPositionstext = 3
    }

    /// <summary>
    /// Definition of traffic jam types
    /// </summary>
    public enum VdvTrafficType : byte
    {
        /// <summary>
        /// All is ok, there is no traffic
        /// </summary>
        NoTraffic = 0,      
 
        /// <summary>
        /// Currently traffic jam
        /// </summary>
        TrafficJam = 1,     

        /// <summary>
        /// Bus has to wait
        /// </summary>
        WaitingBus = 2   
    }

    /// <summary>
    /// Structure that contains fields for scheduled timetable for vdv (VdvFahrplan) datagram.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VdvScheduledTimetableStruct
    {
        /// <summary>
        /// ITCS unique identifier 
        /// </summary>
        public ushort ITCSId;

        /// <summary>
        /// Line identifier
        /// </summary>
        public uint LineId;

        /// <summary>
        /// Identifier of the trip destination
        /// </summary>
        public ushort DestinationId;

        /// <summary>
        /// Identifier of the trip.
        /// </summary>
        public uint TripId;

        /// <summary>
        /// Sequence counter of the stop of the trip.
        /// </summary>
        public ushort StopSequenceCounter;

        /// <summary>
        /// Scheduled arrival time in week seconds.
        /// </summary>
        public uint ScheduledArrivalTime;

        /// <summary>
        /// Scheduled departure time in week seconds.
        /// </summary>
        public uint ScheduledDepartureTime;

        /// <summary>
        /// Identifier of the text reference for line text 
        /// </summary>
        public uint LineTextId;

        /// <summary>
        /// Identifier of the text reference for destination 1 text 
        /// </summary>
        public uint Destination1TextId;

        /// <summary>
        /// Identifier of the text reference for destination 2 text 
        /// </summary>
        public uint Destination2TextId;

        /// <summary>
        /// Identifier of the text reference for the stop 
        /// </summary>
        public uint PlateformTextId;
    }

    /// <summary>
    /// Struct that contains fields for realtime data for vdv (fahrplanlage) datagram
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VdvRealtimeTimetableStruct
    {
        /// <summary>
        /// ITCS unique identifier 
        /// </summary>
        public ushort ITCSId;

        /// <summary>
        /// Line identifier
        /// </summary>
        public uint LineId;

        /// <summary>
        /// Identifier of the trip destination
        /// </summary>
        public ushort DestinationId;

        /// <summary>
        /// Identifier of the trip.
        /// </summary>
        public uint TripId;

        /// <summary>
        /// Sequence counter of the stop of the trip.
        /// </summary>
        public ushort StopSequenceCounter;

        /// <summary>
        /// Indicates if the message contained realtime info (1) or not (0) => Fahrstatus
        /// </summary>
        public ushort RealtimeInfo;

        /// <summary>
        /// Clear down ref; identifier to check-in/out bus at station (used with iTag radio messages) => AnAbmeldeld
        /// </summary>
        public uint CheckInOutId;

        /// <summary>
        /// Scheduled arrival time in week seconds.
        /// </summary>
        public uint ScheduledArrivalTime;

        /// <summary>
        /// Estimated arrival time in week seconds.
        /// </summary>
        public uint EstimatedArrivalTime; 

        /// <summary>
        /// Scheduled departure time in week seconds.
        /// </summary>
        public uint ScheduledDepartureTime;

        /// <summary>
        /// Estimated departure time in week seconds.
        /// </summary>
        public uint EstimatedDepartureTime; 

        /// <summary>
        /// Indicates whether the bus is at the station or not => AufAZB
        /// </summary>
        public ushort IsBusAtStation; 

        /// <summary>
        /// Indicates the traffic state => StauIndikator
        /// 0 = Ok, 1 = traffic, 2 = Bus has to wait
        /// </summary>
        public ushort TrafficJamIndicator;

        /// <summary>
        /// Identifier of the text reference for line text 
        /// </summary>
        public uint LineTextId;

        /// <summary>
        /// Identifier of the text reference for destination 1 text 
        /// </summary>
        public uint Destination1TextId;

        /// <summary>
        /// Identifier of the text reference for destination 2 text 
        /// </summary>
        public uint Destination2TextId;

        /// <summary>
        /// Identifier of the text reference for the plateform (= lane) 
        /// </summary>
        public uint PlateformTextId;
    }

    /// <summary>
    /// Structure that defines fields fot VDV ref text data message. It enables to send reference texts to iqubes.
    /// (Legacy code = tVdvreftext)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct VdvRefTextStruct
    {
        /// <summary>
        /// Identifer of the text
        /// </summary>
        public uint TextReferenceId;

        /// <summary>
        /// Font used to display the text
        /// </summary>
        public byte Font;

        /// <summary>
        /// Type of ...
        /// </summary>
        public byte Type;

        /// <summary>
        /// Content of the displayed text
        /// </summary>
        private fixed byte displayText[32];

        /// <summary>
        /// Text to speach content text
        /// </summary>
        private fixed byte ttsText[80];

        /// <summary>
        /// Gets or sets the text corresponding to the lane.
        /// </summary>
        public string DisplayText
        {
            get
            {
                fixed (byte* ptr = this.displayText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, 32);
                }
            }

            set
            {
                fixed (byte* ptr = this.displayText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, 32);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text to speach content text
        /// </summary>
        public string TTSText
        {

            get
            {
                fixed (byte* ptr = this.ttsText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, 80);
                }
            }

            set
            {
                fixed (byte* ptr = this.ttsText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, 80);
                }
            }
        }
    }

    /// <summary>
    /// Structure that defines fields fot VDV delete trip message. It enables to remove a trip from iqubes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VdvDeleteTripStruct
    {
        /// <summary>
        /// ITCS unique identifier 
        /// </summary>
        public ushort ITCSId;

        /// <summary>
        /// Line identifier
        /// </summary>
        public uint LineId;

        /// <summary>
        /// Identifier of the trip destination
        /// </summary>
        public ushort DestinationId;

        /// <summary>
        /// Identifier of the trip.
        /// </summary>
        public uint TripId;

        /// <summary>
        /// Sequence counter of the stop of the trip.
        /// </summary>
        public ushort StopSequenceCounter;
    }

    /// <summary>
    /// Structure that defines fields for VDV message that enables to send special line text to be displayed until the expiration date/time.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi,  Pack = 1)]
    public unsafe struct VdvSpecialLineTextStruct
    {
        /// <summary>
        /// ITCS unique identifier 
        /// </summary>
        public ushort ITCSId;

        /// <summary>
        /// Line number
        /// </summary>
        public uint LineNumber;

        /// <summary>
        /// Identifier of the destination
        /// </summary>
        public ushort DestinationId;

        /// <summary>
        /// Expiration date time of the displayed message
        /// </summary>
        public DateTimeStruct ExpirationTime;

        /// <summary>
        /// Content of the displayed text
        /// </summary>
        private fixed byte displayText[121];

        /// <summary>
        /// Gets or sets the text corresponding to the lane.
        /// </summary>
        public string DisplayText
        {
            get
            {
                fixed (byte* ptr = this.displayText)
                {
                    return StructuresHelper.UnsafeCopyByteArrayPtrToString(ptr, 121);
                }
            }

            set
            {
                fixed (byte* ptr = this.displayText)
                {
                    StructuresHelper.UnsafeCopyStringToByteArrayPtr(value, ptr, 121);
                }
            }
        }
    }

    /// <summary>
    /// Structure that defines fields for VDV message that enables to delete special line text. 
    /// It enables to stop a special line text message to be displayed before expiration date.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VdvSpecialLineTextDeletionStruct
    {
        /// <summary>
        /// ITCS unique identifier 
        /// </summary>
        public ushort ITCSId;

        /// <summary>
        /// Line number
        /// </summary>
        public uint LineNumber;

        /// <summary>
        /// Identifier of the direction
        /// </summary>
        public ushort DestinationId;
    }

    /// <summary>
    /// Union for the different vdv message types
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct VdvMsgStruct
    {
        /// <summary>
        /// Union at offset 0 : VdvScheduledTimetableStruct type. See <see cref="VdvScheduledTimetableStruct"/> for more details. 
        /// </summary>
        [FieldOffset(0)]
        public VdvScheduledTimetableStruct ScheduledTimetable; // tVdvFahrplan Fahrplan

        /// <summary>
        /// Union at offset 0 : VdvRealtimeTimetableStruct type. See <see cref="VdvRealtimeTimetableStruct"/> for more details. 
        /// </summary>
        [FieldOffset(0)]
        public VdvRealtimeTimetableStruct RealtimeData;

        /// <summary>
        /// Union at offset 0 : VdvDeleteTripStruct type. See <see cref="VdvDeleteTripStruct"/> for more details.
        /// </summary>
        [FieldOffset(0)]
        public VdvDeleteTripStruct DeleteTrip;

        /// <summary>
        /// Union at offset 0 : VdvRefTextStruct type. See <see cref="VdvRefTextStruct"/> for more details.
        /// </summary>
        [FieldOffset(0)]
        public VdvRefTextStruct RefText;

        /// <summary>
        /// Union at offset 0 : VdvSpecialLineTextStruct type. See <see cref="VdvSpecialLineTextStruct"/> for more details.
        /// </summary>
        [FieldOffset(0)]
        public VdvSpecialLineTextStruct SpecialLineText;

        /// <summary>
        /// Union at offset 0 : VdvSpecialLineTextDeletionStruct type. See <see cref="VdvSpecialLineTextDeletionStruct"/> for more details.
        /// </summary>
        [FieldOffset(0)]
        public VdvSpecialLineTextDeletionStruct SpecialLineTextDeletion;
    }
}