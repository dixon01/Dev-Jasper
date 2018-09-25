// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetRealtimeMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of the qnet vdv messages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Gorba.Common.Protocols.Core;
    using Gorba.Common.Protocols.Qnet.Structures;

    using NLog;

    /// <summary>
    /// Implementation of the qnet vdv messages
    /// </summary>
    public class QnetRealtimeMessage : QnetMessageBase
    {
        /// <summary>
        /// DashesValue is represented by "--"
        /// </summary>
        private const int DashesValue = -2;

        private const int BlackValue = -1;

        private const string DashesRepresentation = "--";

        private const string BlackRepresentation = "  ";

        /// <summary>
        /// value = 1 means show leading zero, e.g.: [01]
        /// value = 0 means show leading zero, e.g.: [ 1]
        /// </summary>
        private const int ShowZero = 1;

        private static readonly Logger Logger = LogManager.GetLogger("GlobalLog");

        /// <summary>
        /// Stores the vdv message
        /// </summary>
        private QnetMessageStruct qnetMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetRealtimeMessage"/> class.
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
        public QnetRealtimeMessage(ushort srcAddr, ushort destAddr, ushort gatewayAddress)
            : base(srcAddr, destAddr, gatewayAddress)
        {
            this.QnetMessage = new QnetMessageStruct();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetRealtimeMessage"/> class.
        /// The source and destination addresses are set with <see cref="QnetConstantes.QnetAddrNone"/> by default.
        /// </summary>
        public QnetRealtimeMessage()
            : this(QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone, QnetConstantes.QnetAddrNone)
        {
        }

        /// <summary>
        /// Gets the qnet message for vdv messages.
        /// </summary>
        public QnetMessageStruct QnetMessage
        {
            get
            {
                return this.qnetMessage;
            }

            private set
            {
                this.qnetMessage = value;
            }
        }

        /// <summary>
        /// Converts a datagram (array of bytes) into a QnetSNMPMessage object.
        /// </summary>
        /// <param name="sourceAddr">
        /// The source address.
        /// </param>
        /// <param name="destAddr">
        /// The destination address.
        /// </param>
        /// <param name="data">
        /// Data that represents a Qnet datagram received datagram
        /// </param>
        /// <param name="gatewayAddress">The address of the gateway.</param>
        /// <returns>
        /// Returns a QnetRealtimeDataMessage object from the datagram. Returns null in case of a conversion error.
        /// </returns>
        /// <exception cref="QnetProtocolStackException">
        /// Thrown if the received display type is less than 0 or greater than 4.
        /// </exception>
        public static QnetMessageBase ConvertDataToQnetMessage(
            ushort sourceAddr, ushort destAddr, byte[] data, ushort gatewayAddress)
        {
            Logger.Debug("Converting data ({0} byte(s)) to Qnet message", data.Length);
            if (data.Length > 0)
            {
                Logger.Trace("Data: {0}", BitConverter.ToString(data));
            }

            var qnetDataMessage = new QnetRealtimeDataMessage(sourceAddr, destAddr, gatewayAddress);
            try
            {
                var qnetMsg = ProtocolPacket.ByteArrayToStruct<QnetMessageStruct>(data);

                // todo test the type of the cmd and throw exception if not ggod.
                var realtimeData = qnetMsg.Dta.RealtimeMonitoring.Data;
                var displayType = realtimeData.DisplayType;

                if (displayType > 4)
                {
                    throw new QnetProtocolStackException(
                        "QnetRealTimeMessage::ConvertDataToQnetMessage : The display type should be greater or eq than"
                        + " 0 and less than 5");
                }

                qnetDataMessage.DisplayType = (RealtimeDisplayType)realtimeData.DisplayType;

                switch (qnetDataMessage.DisplayType)
                {
                    case RealtimeDisplayType.DisplayTypeC:
                        Logger.Trace("Received realtime data for display type C");
                        DisplayTypeCFromRealtimeData(qnetDataMessage, realtimeData);
                        break;

                    case RealtimeDisplayType.DisplayTypeS:
                        Logger.Trace("Received realtime data for display type S");
                        DisplayTypeSFromRealtimeData(qnetDataMessage, realtimeData);
                        break;

                    case RealtimeDisplayType.DisplayTypeL:
                        Logger.Trace("Received realtime data for display type L");
                        DisplayTypeLFromRealtimeData(qnetDataMessage, realtimeData);
                        break;

                    case RealtimeDisplayType.DisplayTypeM:
                        Logger.Trace("Received realtime data for display type M");
                        DisplayTypeMFromRealtimeData(qnetDataMessage, realtimeData);
                        break;

                    case RealtimeDisplayType.InfoLine:
                        Logger.Trace("Received realtime data for InfoLine");
                        DisplayTypeInfolineFromRealtimeData(qnetDataMessage, realtimeData);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("QnetRealtimeMessage::ConvertDataToQnetMessage exception occured: {0}", ex.Message);
                qnetDataMessage = null;
            }

            Logger.Debug("Returning Qnet data message {0}", qnetDataMessage);
            return qnetDataMessage;
        }

        /// <summary>
        /// Fills the right fields of the RealtimeMonitoring message structure for Start command.
        /// </summary>
        /// <param name="startMode">
        /// Starting mode : See <see cref="RealtimeStartMode"/> for more details.
        /// </param>
        /// <param name="interval">
        /// Interval of data request from Realtime monitoring server in seconds.
        /// </param>
        public void SetStartMessage(ushort startMode, ushort interval)
        {
            this.qnetMessage.Dta.RealtimeMonitoring.Command = (short)RealtimeMonitorCommand.Start;
            this.qnetMessage.Dta.RealtimeMonitoring.Start.Interval = interval;
            this.qnetMessage.Dta.RealtimeMonitoring.Start.Mode = startMode;

            this.FillHeader();
        }

        /// <summary>
        /// Fills the right fields of the RealtimeMonitoring for GetData command.
        /// </summary>
        public void SetDataRequestMessage()
        {
            this.qnetMessage.Dta.RealtimeMonitoring.Command = (short)RealtimeMonitorCommand.GetData;

            this.FillHeader();
        }

        /// <summary>
        /// Fills the right fields of the RealtimeMonitoring for Stop command.
        /// </summary>
        public void SetStopMessage()
        {
            this.qnetMessage.Dta.RealtimeMonitoring.Command = (short)RealtimeMonitorCommand.Stop;

            this.FillHeader();
        }

        /// <summary>
        ///  Returns the length of the data in a byte [0..255]
        /// </summary>
        /// <returns>
        /// (Byte) data length.
        /// </returns>
        protected override byte GetDataLenght()
        {
            return MessageConstantes.RealtimeMonitoringMessageLenght;
        }

        private static void DisplayTypeCFromRealtimeData(
            QnetRealtimeDataMessage dataMessage, RealtimeDataStruct realtimeData)
        {
            for (int i = 0; i < realtimeData.RealtimeDataTypeC.Data.Length; i++)
            {
                dataMessage.DisplayTypeCRows[i].UnitsChar =
                    ConvertCharForTypeCToString(realtimeData.RealtimeDataTypeC.Data[i].DisplayValue.PositionUnits);
                dataMessage.DisplayTypeCRows[i].TensChar =
                    ConvertCharForTypeCToString(realtimeData.RealtimeDataTypeC.Data[i].DisplayValue.PositionTens);

                dataMessage.DisplayTypeCRows[i].Blink = (realtimeData.RealtimeDataTypeC.Data[i].Attributes & 1) == 1;
            }
        }

        private static void DisplayTypeInfolineFromRealtimeData(
            QnetRealtimeDataMessage dataMessage, RealtimeDataStruct realtimeData)
        {
            dataMessage.DisplayTypeInfolineRow.RowNumber = realtimeData.RealtimeInfoLine.RowNumber;
            dataMessage.DisplayTypeInfolineRow.Infoline.Text = realtimeData.RealtimeInfoLine.Text;
            SetQnetRealtimeDataFromAttributes(
                dataMessage.DisplayTypeInfolineRow.Infoline, realtimeData.RealtimeInfoLine.Attributes);
        }

        private static void DisplayTypeMFromRealtimeData(
            QnetRealtimeDataMessage dataMessage, RealtimeDataStruct realtimeData)
        {
            for (int i = 0; i < QnetDisplayTypeMRow.RowCount; i++)
            {
                dataMessage.DisplayTypeMRows[i].RowNumber = realtimeData.RealtimeDataTypeM.RowsNumber[i];

                dataMessage.DisplayTypeMRows[i].Line.Text = realtimeData.RealtimeDataTypeM.Line[i].Text;
                SetQnetRealtimeDataFromAttributes(
                    dataMessage.DisplayTypeMRows[i].Line, realtimeData.RealtimeDataTypeM.Line[i].Attributes);

                dataMessage.DisplayTypeMRows[i].DepartureTime.Text =
                    realtimeData.RealtimeDataTypeM.DepartureTime[i].Text;
                SetQnetRealtimeDataFromAttributes(
                    dataMessage.DisplayTypeMRows[i].DepartureTime,
                    realtimeData.RealtimeDataTypeM.DepartureTime[i].Attributes);

                dataMessage.DisplayTypeMRows[i].Destination.Text = realtimeData.RealtimeDataTypeM.Destination[i].Text;
                SetQnetRealtimeDataFromAttributes(
                    dataMessage.DisplayTypeMRows[i].Destination,
                    realtimeData.RealtimeDataTypeM.Destination[i].Attributes);

                dataMessage.DisplayTypeMRows[i].DepartureTime.Text =
                    realtimeData.RealtimeDataTypeM.DepartureTime[i].Text;
            }
        }

        private static void DisplayTypeLFromRealtimeData(
            QnetRealtimeDataMessage dataMessage, RealtimeDataStruct realtimeData)
        {
            for (int i = 0; i < QnetDisplayTypeLRow.RowCount; i++)
            {
                dataMessage.DisplayTypeLRows[i].RowNumber = realtimeData.RealtimeDataTypeL.RowsNumbers[i];

                dataMessage.DisplayTypeLRows[i].Line.Text = realtimeData.RealtimeDataTypeL.Lines[i].Text;
                SetQnetRealtimeDataFromAttributes(
                    dataMessage.DisplayTypeLRows[i].Line, realtimeData.RealtimeDataTypeL.Lines[i].Attributes);

                dataMessage.DisplayTypeLRows[i].DepartureTime.Text =
                    realtimeData.RealtimeDataTypeL.DepartureTimes[i].Text;
                SetQnetRealtimeDataFromAttributes(
                    dataMessage.DisplayTypeLRows[i].DepartureTime,
                    realtimeData.RealtimeDataTypeL.DepartureTimes[i].Attributes);

                dataMessage.DisplayTypeLRows[i].Destination.Text = realtimeData.RealtimeDataTypeL.Destinations[i].Text;
                SetQnetRealtimeDataFromAttributes(
                    dataMessage.DisplayTypeLRows[i].Destination,
                    realtimeData.RealtimeDataTypeL.Destinations[i].Attributes);

                dataMessage.DisplayTypeLRows[i].DepartureTime.Text =
                    realtimeData.RealtimeDataTypeL.DepartureTimes[i].Text;

                dataMessage.DisplayTypeLRows[i].Lane.Text = realtimeData.RealtimeDataTypeL.Lanes[i].Text;
                SetQnetRealtimeDataFromAttributes(
                    dataMessage.DisplayTypeLRows[i].Lane, realtimeData.RealtimeDataTypeL.Lanes[i].Attributes);
            }
        }

        /// <summary>
        /// Bit-coded: 0x1-0x2:Alignment; 0x4:Blink; 0x8:Scroll; 0x10:AutoInfoLine; 0x80:Valid
        /// </summary>
        /// <param name="qnetRealtimeDataValue">
        /// Data to assigned.
        /// </param>
        /// <param name="attributes">
        /// Attributes to decode.
        /// </param>
        private static void SetQnetRealtimeDataFromAttributes(
            QnetRealtimeDataValue qnetRealtimeDataValue, byte attributes)
        {
            qnetRealtimeDataValue.Alignment = attributes & 0x3;
            qnetRealtimeDataValue.IsBlinking = (attributes & 0x4) == 0x4;
            qnetRealtimeDataValue.IsScrolling = (attributes & 0x8) == 0x8;
            qnetRealtimeDataValue.AutoInfoLine = (attributes & 0x10) == 0x10;
            qnetRealtimeDataValue.IsValid = (attributes & 0x80) == 0x80;
        }

        private static void DisplayTypeSFromRealtimeData(
            QnetRealtimeDataMessage dataMessage, RealtimeDataStruct realtimeData)
        {
            int displayedValue = realtimeData.RealtimeDataTypeS.DisplayValue;

            if (displayedValue < -2 && displayedValue > 99)
            {
                throw new QnetProtocolStackException(
                    string.Format("Displayed value {0} of realtime data from type S is out of range.", displayedValue));
            }

            switch (displayedValue)
            {
                case DashesValue:
                    dataMessage.DisplayTypeSRow.DisplayedValue = DashesRepresentation;
                    break;
                case BlackValue:
                    dataMessage.DisplayTypeSRow.DisplayedValue = BlackRepresentation;
                    break;

                default:
                    if (realtimeData.RealtimeDataTypeS.Attributes == ShowZero)
                    {
                        Logger.Trace("Adding leading 0");
                        dataMessage.DisplayTypeSRow.DisplayedValue =
                            displayedValue.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                        return;
                    }

                    dataMessage.DisplayTypeSRow.DisplayedValue = displayedValue.ToString(CultureInfo.InvariantCulture);

                    break;
            }
        }

        private static char ConvertCharForTypeCToString(sbyte value)
        {
            Logger.Trace("Received char value {0}", value);

            switch ((RealtimeSpecialCharacters)value)
            {
                case RealtimeSpecialCharacters.Digit9:
                case RealtimeSpecialCharacters.Digit8:
                case RealtimeSpecialCharacters.Digit7:
                case RealtimeSpecialCharacters.Digit6:
                case RealtimeSpecialCharacters.Digit5:
                case RealtimeSpecialCharacters.Digit4:
                case RealtimeSpecialCharacters.Digit3:
                case RealtimeSpecialCharacters.Digit2:
                case RealtimeSpecialCharacters.Digit1:
                case RealtimeSpecialCharacters.Digit0:
                    var c = value.ToString(CultureInfo.InvariantCulture).First();
                    Logger.Trace("Received digit '{0}' (ToString)", c);

                    return c;
                case RealtimeSpecialCharacters.DisplayedCharBlack:
                    return ' ';
                case RealtimeSpecialCharacters.DisplayedDashes:
                    return '-';
                case RealtimeSpecialCharacters.DisplayedCharA:
                    return 'A';
                case RealtimeSpecialCharacters.DisplayedCharC:
                    return 'C';
                case RealtimeSpecialCharacters.DisplayedCharE:
                    return 'E';
                case RealtimeSpecialCharacters.DisplayedCharF:
                    return 'F';
                case RealtimeSpecialCharacters.DisplayedCharH:
                    return 'H';
                case RealtimeSpecialCharacters.DisplayedCharL:
                    return 'L';
                case RealtimeSpecialCharacters.DisplayedCharP:
                    return 'P';
                case RealtimeSpecialCharacters.DisplayedCharU:
                    return 'U';
                default:
                    throw new ArgumentOutOfRangeException(
                        "value", value, "The specified realtime special character code is unknown");
            }
        }

        /// <summary>
        /// Fill the vdv message header according to the given vdv subtype
        /// </summary>
        private void FillHeader()
        {
            this.qnetMessage.Hdr.Type = (byte)MSGtyp.MsgTypeRealtimeMonitor;
            this.qnetMessage.Hdr.SubTyp = 0;
            this.qnetMessage.Hdr.TimeStruct = DosDateTime.DatetimeToDosDateTime(DateTime.Now);
        }
    }
}