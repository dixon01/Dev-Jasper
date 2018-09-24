using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorba.Common.Protocols.Qnet
{
    public class QnetMessageDecoder
    {
        private Byte[] m_Data;

        public QnetMessageBase ConvertDataToQnetMessage(Byte[] data, QnetType qnetType)
        {
            QnetMessageBase qnetMessage = null;
            m_Data = new Byte[data.Length];
            Array.Copy(m_Data, 0, data, 0, data.Length);

            switch (qnetType) {
                case QnetType.QNET_TYPE_IPP:
                    qnetMessage = ProcessIppMessage();
                    break;
                default:
                    qnetMessage = null;
                    break;
            }
            return qnetMessage;
        }

        private QnetMessageBase ProcessIppMessage()
        {
            return null;
            /* bool bRetVal = true;

             // Zuerst den HeaderBuffer unwandeln in einen IPP Header, mittels RawDeserialize
                 IPPHeader oTempIPPHeader = (IPPHeader)QNetCommonMethods.RawDeserialize(p_HeaderBuffer, typeof(IPPHeader));
                 // Message Typ bestimmen
                 switch (oTempIPPHeader.MessageType)
                 {
                     case QnetType.QNET_TYPE_IPP:
                         HandleIppMessage();
                        
                         RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Debug, this.ToString() +
                             " - ProcessReceivedIppMessage - IPP request message received -> send IPP response message.");
                         // IPP request message received -> send IPP response message
                         bRetVal = SendResponse(QNetConstants.IPP_MSGTYP_RESPONSE, oTempIPPHeader.SourceAddress);
                         break;
                     case QNetConstants.IPP_MSGTYP_DATA:
                         // IPP data message received
                         QNPHeader oQNPHeader = (QNPHeader)QNetCommonMethods.RawDeserialize(p_DataBuffer, typeof(QNPHeader));

                         Byte[] tmpBuffer = new Byte[QNetConstants.QNET_QDP_HEADER_LENGTH];

                         Array.Copy(p_DataBuffer, QNetConstants.QNET_QNP_HEADER_LENGTH,
                                    tmpBuffer, 0, QNetConstants.QNET_QDP_HEADER_LENGTH);

                         tmpBuffer = new Byte[QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH];

                         Array.Copy(p_DataBuffer, QNetConstants.QNET_QNP_HEADER_LENGTH + QNetConstants.QNET_QDP_HEADER_LENGTH,
                                    tmpBuffer, 0, QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH);

                         IqubeMessage oIqubeMessage = new IqubeMessage(VDVMessageType.mtEmpty);

                         oIqubeMessage.MessageHeader = (MsgHeader)QNetCommonMethods.RawDeserialize(tmpBuffer, typeof(MsgHeader));

                         switch ((QNetConstants.MsgType)oIqubeMessage.MessageHeader.Type)
                         {
                             // VDV Message
                             case QNetConstants.MsgType.MSG_TYP_VDV:
                                 tmpBuffer = new Byte[p_nDataLength];

                                 if (p_nDataLength > QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH)
                                 {
                                     Array.Copy(p_DataBuffer,
                                                QNetConstants.QNET_QNP_HEADER_LENGTH +
                                                QNetConstants.QNET_QDP_HEADER_LENGTH +
                                                QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH,
                                                tmpBuffer, 0,
                                                p_nDataLength - QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH);
                                 }

                                 switch (oIqubeMessage.MessageHeader.Subtype)
                                 {
                                     case QNetConstants.VDV_TYP_FAHRPLAN_ANFRAGE:
                                         RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Debug, this.ToString() +
                                             " - ProcessReceivedIppMessage - IPP data message received." +
                                             " - Type: VDVFahrplanAnfrage - DataLen: " + p_nDataLength +
                                             " - Address: " + oQNPHeader.SourceAddress.ToString());

                                         ProcessIqubeRefDataRequest(oQNPHeader.SourceAddress);
                                         break;
                                     case QNetConstants.VDV_TYP_REF_TEXT_ANFRAGE:
                                         RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Debug, this.ToString() +
                                             " - ProcessReceivedIppMessage - IPP data message received." +
                                             " - Type: VDVRefTextAnfrage - DataLen: " + p_nDataLength +
                                             " - Address: " + oQNPHeader.SourceAddress.ToString());

                                         ProcessIqubeRefTextRequest(oQNPHeader.SourceAddress);
                                         break;
                                     case QNetConstants.VDV_TYP_ALARM:
                                         RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Debug, this.ToString() +
                                             " - ProcessReceivedIppMessage - IPP data message received." +
                                             " - Type: VDVAlarm - DataLen: " + p_nDataLength +
                                             " - Address: " + oQNPHeader.SourceAddress.ToString());

                                         VdvAlarm oVdvAlarmMsgData = (VdvAlarm)QNetCommonMethods.RawDeserialize(tmpBuffer, typeof(VdvAlarm));

                                         ProcessIqubeAlarmMessage(oQNPHeader.SourceAddress, oVdvAlarmMsgData);
                                         break;
                                     default:
                                         RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Info, this.ToString() +
                                             " - ProcessReceivedIppMessage - Unhandled VDV data message received." +
                                             " - Subtype: " + oIqubeMessage.MessageHeader.Subtype +
                                             " - Address: " + oQNPHeader.SourceAddress.ToString());
                                         break;
                                 }
                                 break;
                             // Connection Status Message
                             case QNetConstants.MsgType.MSG_TYP_CON_STA:
                                 tmpBuffer = new Byte[p_nDataLength];

                                 if (p_nDataLength > QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH)
                                 {
                                     Array.Copy(p_DataBuffer,
                                                QNetConstants.QNET_QNP_HEADER_LENGTH +
                                                QNetConstants.QNET_QDP_HEADER_LENGTH +
                                                QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH,
                                                tmpBuffer, 0,
                                                p_nDataLength - QNetConstants.IQUBE_MESSAGE_HEADER_LENGTH);

                                     RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Debug, this.ToString() +
                                         " - ProcessReceivedIppMessage - IPP data message received." +
                                         " - Type: ConnectionStatus");

                                     ConnectionStatus oConnectionStatusMsg = (ConnectionStatus)QNetCommonMethods.RawDeserialize(tmpBuffer, typeof(ConnectionStatus));

                                     ProcessConnectionStatusMessage(oConnectionStatusMsg);
                                 }
                                 break;
                             // Unhandled Message
                             default:
                                 RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Info, this.ToString() +
                                     " - ProcessReceivedIppMessage - Unhandled IPP data message received." +
                                     " - Type: " + oIqubeMessage.MessageHeader.Type.ToString() +
                                     " - Subtype: " + oIqubeMessage.MessageHeader.Subtype.ToString() +
                                     " - DataLen: " + p_nDataLength +
                                     " - Address: " + oQNPHeader.SourceAddress.ToString());
                                 break;
                         }
                         break;
                     default:
                         RblClient.RBLClientLogger.GetLogger().LogEntry(NLog.LogLevel.Error, this.ToString() +
                             " - ProcessReceivedIppMessage - Invalid IPP Message received!" +
                             " - Type: " + oTempIPPHeader.MessageType.ToString() +
                             " - Address: " + oTempIPPHeader.SourceAddress.ToString());
                         bRetVal = false;
                         break;
                 }
             */
        }

        /*
         * private CommsMessage EncodeQnetSNMPMessage(QnetSNMPMessage qnetSNMPMessage)
        {
            CommsMessage msg = null;

            if (qnetSNMPMessage.QSNMP.msg.timeSync.qntp.Mode == MessageConstantes.QNTP_MODE_REQUEST) {                
                DateTime ReceiveTime = new DateTime(1,1,1, 
                    qnetSNMPMessage.QSNMP.msg.timeSync.qntp.ReceiveTime.ti_hour,
                    qnetSNMPMessage.QSNMP.msg.timeSync.qntp.ReceiveTime.Minute,
                    qnetSNMPMessage.QSNMP.msg.timeSync.qntp.ReceiveTime.Second);
                msg = new TimeSyncMessage(0, receiveTime);
            } // if

            return msg;
        }
         * */
    }
}
