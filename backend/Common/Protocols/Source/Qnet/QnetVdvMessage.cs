// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetVdvMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of the qnet vdv messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    using Gorba.Common.Protocols.Qnet.Structures;

    /// <summary>
    /// Implementation of the qnet vdv messages
    /// </summary>
    public class QnetVdvMessage : QnetMessageBase
    {
        #region Fields
        /// <summary>
        /// Stores the vdv message
        /// </summary>
        private QnetMessageStruct vdvMessage;

        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QnetVdvMessage"/> class.
        /// </summary>
        /// <param name="srcAddr">
        /// The qnet source address of the sender of the message.
        /// </param>
        /// <param name="destAddr">
        /// The qnet destination address of the message.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetVdvMessage(ushort srcAddr, ushort destAddr, ushort gatewayAddress)
            : base(srcAddr, destAddr, gatewayAddress)
        {
            this.VdvMessage = new QnetMessageStruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetVdvMessage"/> class.
        /// The source and destination addresses are set with <see cref="QnetConstantes.QnetAddrNone"/> by default.
        /// </summary>
        public QnetVdvMessage()
            : this(QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone)
        {
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the qnet message for vdv messages.
        /// </summary>
        public QnetMessageStruct VdvMessage
        {
            get
            {
                return this.vdvMessage;
            }

            private set
            {
                this.vdvMessage = value;
            }
        }

        #endregion Properties

        #region Public methods

        /// <summary>
        /// Fill the right fields of the vdv message structure with subtype = VDV_TYP_FAHRTLOESCHEN
        /// </summary>
        /// <param name="itcsId">Unique itcs identifier of the trip to be deleted (because a same trip could be assured
        /// by several itcs.</param>
        /// <param name="lineId">Unique identifier of the bus line of the trip to be deleted.</param>
        /// <param name="destinationId">Unique identifier of the destination of the trip to be deleted.</param>
        /// <param name="tripId">Unique identifier of the trip to be deleted.</param>
        /// <param name="stopSequenceCounter">Stop sequence number in the trip.</param>
        public void SetDeleteTripMessage(
            ushort itcsId, uint lineId, ushort destinationId, uint tripId, ushort stopSequenceCounter)
        {
            this.vdvMessage.Dta.VdvMsg.DeleteTrip.ITCSId = itcsId;
            this.vdvMessage.Dta.VdvMsg.DeleteTrip.LineId = lineId;
            this.vdvMessage.Dta.VdvMsg.DeleteTrip.DestinationId = destinationId;
            this.vdvMessage.Dta.VdvMsg.DeleteTrip.TripId = tripId;
            this.vdvMessage.Dta.VdvMsg.DeleteTrip.StopSequenceCounter = stopSequenceCounter;

            this.FillHeader(VdvSubType.DeleteTrip);
        }

        /// <summary>
        /// Sets the data of the vdv message for Ref data text
        /// </summary>
        /// <param name="textRefId">Identifier of the reference of text</param>
        /// <param name="font">Font used to display the text.</param>
        /// <param name="refType">Type of the reference text</param>
        /// <param name="displayText">Text to display</param>
        /// <param name="ttsText">Text used for text to speech</param>
        public void SetReferenceTextMessage(uint textRefId, byte font, byte refType, string displayText, string ttsText)
        {
            this.vdvMessage.Dta.VdvMsg.RefText.TextReferenceId = textRefId;
            this.vdvMessage.Dta.VdvMsg.RefText.Font = font;
            this.vdvMessage.Dta.VdvMsg.RefText.Type = refType;
            this.vdvMessage.Dta.VdvMsg.RefText.DisplayText = displayText;
            this.vdvMessage.Dta.VdvMsg.RefText.TTSText = ttsText;

            this.FillHeader(VdvSubType.ReferenceText);
        }

        /// <summary>
        ///  Sets the data of the vdv message for scheduled timetable data.
        /// </summary>
        /// <param name="itcsId">
        /// The itcs id.
        /// </param>
        /// <param name="lineId">
        /// The line id.
        /// </param>
        /// <param name="destinationId">
        /// The destination id.
        /// </param>
        /// <param name="tripId">
        /// The trip id.
        /// </param>
        /// <param name="stopSequenceCounter">
        /// The stop sequence counter.
        /// </param>
        /// <param name="scheduledArrivalTime">
        /// The scheduled arrival time.
        /// </param>
        /// <param name="scheduledDepartureTime">
        /// The scheduled departure time.
        /// </param>
        /// <param name="lineTextId">
        /// The line text id.
        /// </param>
        /// <param name="destText1Id">
        /// The destination text 1 id.
        /// </param>
        /// <param name="destText2Id">
        /// The destination text 2 id.
        /// </param>
        /// <param name="stopPositionsTextId">
        /// The stop positions text id.
        /// </param>
        public void SetScheduledTimetableDataMessage(
            ushort itcsId,
            uint lineId,
            ushort destinationId,
            uint tripId,
            ushort stopSequenceCounter,
            uint scheduledArrivalTime,
            uint scheduledDepartureTime,
            uint lineTextId,
            uint destText1Id,
            uint destText2Id,
            uint stopPositionsTextId)
        {
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.ITCSId = itcsId;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.LineId = lineId;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.DestinationId = destinationId;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.TripId = tripId;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.StopSequenceCounter = stopSequenceCounter;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.ScheduledArrivalTime = scheduledArrivalTime;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.ScheduledDepartureTime = scheduledDepartureTime;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.LineTextId = lineTextId;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.Destination1TextId = destText1Id;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.Destination2TextId = destText2Id;
            this.vdvMessage.Dta.VdvMsg.ScheduledTimetable.PlateformTextId = stopPositionsTextId;

            this.FillHeader(VdvSubType.ScheduledTimetable);
        }

        /// <summary>
        /// Defines the message as real time data setting all necessary fields.
        /// </summary>
        /// <param name="itcsId">
        /// The itcs id.
        /// </param>
        /// <param name="lineId">
        /// The line id.
        /// </param>
        /// <param name="destinationId">
        /// The destination id.
        /// </param>
        /// <param name="tripId">
        /// The trip id.
        /// </param>
        /// <param name="stopSequenceCounter">
        /// The stop sequence counter.
        /// </param>
        /// <param name="areRealtimeInfo">
        /// The are realtime info.
        /// </param>
        /// <param name="checkInOutId">
        /// The check in out id.
        /// </param>
        /// <param name="scheduledArrivalTime">
        /// The scheduled arrival time.
        /// </param>
        /// <param name="estimatedArrivalTime">
        /// The estimated arrival time.
        /// </param>
        /// <param name="scheduledDepartureTime">
        /// The scheduled departure time.
        /// </param>
        /// <param name="estimatedDepartureTime">
        /// The estimated departure time.
        /// </param>
        /// <param name="isBusAtStation">
        /// The is bus at station.
        /// </param>
        /// <param name="trafficJamIndicator">
        /// The traffic jam indicator.
        /// </param>
        /// <param name="lineTextId">
        /// The line text id.
        /// </param>
        /// <param name="destText1Id">
        /// The destination text 1 id.
        /// </param>
        /// <param name="destText2Id">
        /// The destination text 2 id.
        /// </param>
        /// <param name="plateformTextId">
        /// The stop positions text id.
        /// </param>
        public void SetRealtimeDataMessage(
            ushort itcsId,
            uint lineId,
            ushort destinationId,
            uint tripId,
            ushort stopSequenceCounter,
            ushort areRealtimeInfo,
            uint checkInOutId,
            uint scheduledArrivalTime,
            uint estimatedArrivalTime,
            uint scheduledDepartureTime,
            uint estimatedDepartureTime,
            ushort isBusAtStation,
            ushort trafficJamIndicator,
            uint lineTextId,
            uint destText1Id,
            uint destText2Id,
            uint plateformTextId)
        {
            this.vdvMessage.Dta.VdvMsg.RealtimeData.ITCSId = itcsId;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.LineId = lineId;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.DestinationId = destinationId;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.TripId = tripId;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.StopSequenceCounter = stopSequenceCounter;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.RealtimeInfo = areRealtimeInfo;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.ScheduledArrivalTime = scheduledArrivalTime;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.ScheduledDepartureTime = scheduledDepartureTime;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.EstimatedArrivalTime  = estimatedArrivalTime;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.EstimatedDepartureTime = estimatedDepartureTime;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.IsBusAtStation = isBusAtStation;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.TrafficJamIndicator = trafficJamIndicator;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.LineTextId = lineTextId;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.Destination1TextId = destText1Id;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.Destination2TextId = destText2Id;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.PlateformTextId = plateformTextId;
            this.vdvMessage.Dta.VdvMsg.RealtimeData.CheckInOutId = checkInOutId;

            this.FillHeader(VdvSubType.RealtimeData);
        }

        /// <summary>
        /// Sets the data of the vdv message for special line text.
        /// </summary>
        /// <param name="itcsId">
        /// The itcs id.
        /// </param>
        /// <param name="lineNumber">
        /// The line number where the message will be displayed on.
        /// </param>
        /// <param name="destinationId">
        /// The destination id.
        /// </param>
        /// <param name="expirationTime">
        /// The expiration Time.
        /// </param>
        /// <param name="displayMessage">
        /// The message to be displayed until expiration time or until the unit receives a deletion special line text
        /// message.
        /// </param>
        public void SetSpecialLineText(
            ushort itcsId,
            uint lineNumber,
            ushort destinationId,
            DateTime expirationTime,
            string displayMessage)
        {
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ITCSId = itcsId;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.LineNumber = lineNumber;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.DestinationId = destinationId;

            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Time.Hour = (short)expirationTime.Hour;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Time.Minute = (short)expirationTime.Minute;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Time.Second = (short)expirationTime.Second;

            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Date.Day = (short)expirationTime.Day;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Date.Month = (short)expirationTime.Month;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ExpirationTime.Date.Year = (short)expirationTime.Year;

            this.vdvMessage.Dta.VdvMsg.SpecialLineText.DisplayText = displayMessage;

            this.FillHeader(VdvSubType.SpecialLineText);
        }

        /// <summary>
        /// Sets the data of the vdv message for special line text deletion
        /// </summary>
        /// <param name="itcsId">
        /// The itcs id.
        /// </param>
        /// <param name="lineNumber">
        /// The line number where the message will be removed from.
        /// </param>
        /// <param name="destinationId">
        /// The destination id.
        /// </param>
        public void SetSpecialLineTextDeletion(
            ushort itcsId,
            uint lineNumber,
            ushort destinationId)
        {
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.ITCSId = itcsId;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.LineNumber = lineNumber;
            this.vdvMessage.Dta.VdvMsg.SpecialLineText.DestinationId = destinationId;

            this.FillHeader(VdvSubType.SpecialLineTextDeletion);
        }

        /// <summary>
        /// Sets the vdv message alive subtype.
        /// </summary>
        public void SetAlive()
        {
            this.FillHeader(VdvSubType.VdvAlive);
        }

        #endregion Public methods

        #region Protected methods
        /// <summary>
        ///  Returns the length of the data in a byte [0..255]
        /// </summary>
        /// <returns>
        /// (Byte) data length
        /// </returns>
        protected override byte GetDataLenght()
        {
            return MessageConstantes.VdvMessageLength;
        }

        #endregion Protected methods

        #region Private methods

        /// <summary>
        /// Fill the vdv message header according to the given vdv subtype
        /// </summary>
        /// <param name="vdvSubType">
        /// The subtype of the vdv message to be sent.
        /// </param>
        private void FillHeader(VdvSubType vdvSubType)
        {
            this.vdvMessage.Hdr.Type = (byte)MSGtyp.MsgTypVdv;
            this.vdvMessage.Hdr.SubTyp = (byte)vdvSubType;
            this.vdvMessage.Hdr.TimeStruct = DosDateTime.DatetimeToDosDateTime(DateTime.Now);
        }
        #endregion Private methods
    }
}
