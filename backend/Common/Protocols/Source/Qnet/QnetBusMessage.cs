// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetBusMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetBusMessage class. Inherites from QnetMessageBase.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// Defines the QnetBusMessage class. Inherits from QnetMessageBase.
    /// </summary>
    public class QnetBusMessage : QnetMessageBase
    {
        /*
        private UInt16 m_LineId = 0;
        private UInt16 m_BlockId = 0;
        private UInt16 m_TripId = 0;
        private UInt16 m_StopsNbr = 0;  // Ortsnumber passed
        private UInt16 m_BusId = 0;
        private UInt16 m_Distance = 0;  // from start station
        private UInt16 m_TimeToEndHP;   // scheduled time to end HP
        private UInt16 m_TminToEndHP;   // min. time to end HP
        private char m_hopcount;      // hopcount
        private Byte flags;           // flags
        private Byte validity;      // validity
        private Byte attribute;     // attribute
        private Int16 deltaTime;     // delta time from detector to next Iqube (+/-seconds)
        private Int16 deltaDist;     // delta distance from detector to next Iqube (+/- meter)
        private Int16 delayTime;     // absolute delay time (+/1seconds)
        private UInt16 prevOrt;      // ortsnumber of previous station
        private UInt16 waitTime;      // wait time (seconds)
        private int TimeStamp;     // bus message time stamp

        */

        // Don't make a auto property with busMessage otherwise there is a compilation error

        /// <summary>
        /// Stores the bus message object.
        /// </summary>
        private QnetMessageStruct busMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetBusMessage"/> class.
        /// </summary>
        /// <param name="srcAddr">
        /// The src addr.
        /// </param>
        /// <param name="destAddr">
        /// The dest addr.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetBusMessage(ushort srcAddr, ushort destAddr, ushort gatewayAddress)
            : base(srcAddr, destAddr, gatewayAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetBusMessage"/> class.
        /// </summary>
        public QnetBusMessage()
            : this(QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone)
        {
        }

        /// <summary>
        /// Gets BusMessage.
        /// </summary>
        public QnetMessageStruct BusMessage
        {
            get
            {
                return this.busMessage;
            }
        }

        /// <summary>
        /// Enables to set the field of the underlying BusMessageStruct
        /// </summary>
        /// <param name="lineId">
        /// The line id.
        /// </param>
        /// <param name="blockId">
        /// The block id.
        /// </param>
        /// <param name="tripId">
        /// The trip id.
        /// </param>
        /// <param name="stopsNbr">
        /// The stops NBR value.
        /// </param>
        /// <param name="busId">
        /// The bus id.
        /// </param>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <param name="timeToEndHP">
        /// The time to end.
        /// </param>
        /// <param name="tminToEndHP">
        /// The tmin to end.
        /// </param>
        /// <param name="hopcount">
        /// The hopcount.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <param name="validity">
        /// The validity.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="deltaTime">
        /// The delta time.
        /// </param>
        /// <param name="deltaDist">
        /// The delta distance.
        /// </param>
        /// <param name="delayTime">
        /// The delay time.
        /// </param>
        /// <param name="prevOrt">
        /// The previous place.
        /// </param>
        /// <param name="waitTime">
        /// The wait time.
        /// </param>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        public void FillBusMessage(
            ushort lineId,
            ushort blockId,
            ushort tripId,
            ushort stopsNbr,
            ushort busId,
            ushort distance,
            // ReSharper disable InconsistentNaming
            ushort timeToEndHP,
            ushort tminToEndHP,
            // ReSharper restore InconsistentNaming
            sbyte hopcount,
            byte flags,
            byte validity,
            byte attribute,
            short deltaTime,
            short deltaDist,
            short delayTime,
            ushort prevOrt,
            ushort waitTime,
            int timeStamp)
        {
            this.busMessage.Dta.BusMsg.LineId = lineId;
            this.busMessage.Dta.BusMsg.Umlauf = blockId;
            this.busMessage.Dta.BusMsg.TripId = tripId;
            this.busMessage.Dta.BusMsg.StopNumber = stopsNbr;
            this.busMessage.Dta.BusMsg.WagonId = busId;
            this.busMessage.Dta.BusMsg.Distance = distance;
            this.busMessage.Dta.BusMsg.TimeToEndHP = timeToEndHP;
            this.busMessage.Dta.BusMsg.TminToEndHP = tminToEndHP;
            this.busMessage.Dta.BusMsg.Hopcount = hopcount;
            this.busMessage.Dta.BusMsg.Flags = flags;
            this.busMessage.Dta.BusMsg.Validity = validity;
            this.busMessage.Dta.BusMsg.Attribute = attribute;
            this.busMessage.Dta.BusMsg.DeltaTime = deltaTime;
            this.busMessage.Dta.BusMsg.DeltaDist = deltaDist;
            this.busMessage.Dta.BusMsg.DelayTime = delayTime;
            this.busMessage.Dta.BusMsg.PreviousStop = prevOrt;
            this.busMessage.Dta.BusMsg.WaitTime = waitTime;

            this.busMessage.Hdr.Type = (byte)MSGtyp.MsgTypBus;
            this.busMessage.Hdr.SubTyp = 0;

            this.FillTimeHdr(timeStamp);
        }

        /// <summary>
        /// Enables to set the PositionBus fields of the underlying BusMessageStruct
        /// </summary>
        /// <param name="lineId">
        /// The line id.
        /// </param>
        /// <param name="blockId">
        /// The block id.
        /// </param>
        /// <param name="tripId">
        /// The trip id.
        /// </param>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        /// <param name="busId">
        /// The bus id.
        /// </param>
        /// <param name="vehicleType">
        /// The vehicle type.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        public void FillPositionBusMessage(
            ushort lineId,
            ushort blockId,
            ushort tripId,
            ushort stopId,
            ushort busId,
            byte vehicleType, // = flags
            byte attribute,
            int timeStamp)
        {
            this.FillBusMessage(
                lineId,
                blockId,
                tripId,
                stopId,
                busId,
                0,
                0,
                0,
                1,
                vehicleType,
                1,
                attribute,
                0,
                0,
                0,
                0,
                0,
                timeStamp);
        }

        /// <summary>
        /// Enables to set the PositionBus fields of the underlying BusMessageStruct
        /// </summary>
        /// <param name="lineId">
        /// The line id.
        /// </param>
        /// <param name="blockId">
        /// The block id.
        /// </param>
        /// <param name="tripId">
        /// The trip id.
        /// </param>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        /// <param name="busId">
        /// The bus id.
        /// </param>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <param name="vehicleType">
        /// The vehicle type.
        /// </param>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        public void FillPositionBusMessage(
            ushort lineId,
            ushort blockId,
            ushort tripId,
            ushort stopId,
            ushort busId,
            ushort distance,
            byte vehicleType, // = flags
            byte attribute,
            int timeStamp)
        {
            this.FillBusMessage(
                lineId,
                blockId,
                tripId,
                stopId,
                busId,
                distance,
                0,
                0,
                1,
                vehicleType,
                1,
                attribute,
                0,
                0,
                0,
                0,
                0,
                timeStamp);
        }

        /// <summary>
        /// Gets the length in bytes of the QnetBusMessage
        /// </summary>
        /// <returns>
        /// Returns the number of bytes of the QnetBusMessage
        /// </returns>
        protected override byte GetDataLenght()
        {
            return MessageConstantes.BusMessageLength;
        }

        /// <summary>
        /// Fill time bus message header according to the time stamp value.
        /// </summary>
        /// <param name="timeStamp">
        /// Time stamp as int.
        /// </param>
        private void FillTimeHdr(int timeStamp)
        {
            this.busMessage.Hdr.TimeStruct = timeStamp == -1
                                                 ? DosDateTime.DatetimeToDosDateTime(DateTime.Now)
                                                 : DosDateTime.TimestampToDosDateTime(timeStamp);
        }
    }
}