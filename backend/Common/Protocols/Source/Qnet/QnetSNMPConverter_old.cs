using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorba.Library.Protocols.Qnet
{
    public class QnetSNMPConverter_old
    {
        private tQSNMP m_tQSNMP;

        /// <summary>
        /// Convert a datagram (array of bytes) into a QnetSNMPMessage object.
        /// </summary>
        /// <param name="data">Data that represents a QSNMP datagram</param>
        /// <returns>Returns a QnetSNMPMessage object from the datagram. Returns null in case of a convertion error.</returns>
        public static QnetMessageBase ConvertDataToQnetMessage(ushort sourceAddrress, byte[] data)
        {
            QnetMessageBase qnetMessage = null;
            try {
                tQSNMP sntp = ProtocolPacket.ByteArrayToStruct<tQSNMP>(data);
                QSNMPCmd cmd = (QSNMPCmd)sntp.hdr.cmd;
                switch (cmd) {
                    case QSNMPCmd.QSNMP_CMD_TIME_SYNC:
                        if (sntp.msg.timeSync.qntp.version == MessageConstantes.QNTP_VERSION && sntp.msg.timeSync.qntp.mode == MessageConstantes.QNTP_MODE_REQUEST) {
                            DateTime receiveTime = new DateTime(0, 0, 0, sntp.msg.timeSync.qntp.receiveTime.ti_hour, sntp.msg.timeSync.qntp.receiveTime.ti_min, sntp.msg.timeSync.qntp.receiveTime.ti_sec);
                            qnetMessage = new QnetTimeSyncRequestMessage(sourceAddrress, receiveTime);
                        }
                        break;
                } // switch
            } catch {
                qnetMessage = null;
            }
            return qnetMessage;
        }
        public tQSNMP tQSNMP
        {
            get { return m_tQSNMP; }
        }

        /// <summary>
        /// Fill the structure tQSNMP for restart command using the QSNMP protocol
        /// </summary>
        public void QSNMPRestart()
        {
            SetQSNMPCommand(QSNMPCmd.QSNMP_CMD_RESTART);
        }

        public void QSNMPSetDisplay(bool dispOn)
        {
            if (dispOn) {
                SetQSNMPCommand(QSNMPCmd.QSNMP_CMD_DISP_ON);
            } else {
                SetQSNMPCommand(QSNMPCmd.QSNMP_CMD_DISP_OFF);
            } // else
        }

        /*
         * public void TimeSync()
        {           
            m_tQSNMP.msg.timeSync.qntp.mode = MessageConstantes.QNTP_MODE_BROADCAST;
            m_tQSNMP.msg.timeSync.qntp.referenceLevel = MessageConstantes.QNTP_LEVEL_SECONDARY;
            m_tQSNMP.msg.timeSync.qntp.referenceIdent = MessageConstantes.QNTP_IDENT_THIS;
            m_tQSNMP.msg.timeSync.qntp.timeServer = QnetConstantes.QNET_ADDR_NONE;

            SetQSNMPCommand(QSNMPCmd.QSNMP_CMD_TIME_SYNC);
        }
         * */
            
        /*
         * public void NotifyTimeSync()
        {
            m_tQSNMP.msg.timeSync.qntp.mode = MessageConstantes.QNTP_MODE_BROADCAST;
            m_tQSNMP.msg.timeSync.qntp.referenceLevel = MessageConstantes.QNTP_LEVEL_SECONDARY;
            m_tQSNMP.msg.timeSync.qntp.referenceIdent = MessageConstantes.QNTP_IDENT_OTHER;
            m_tQSNMP.msg.timeSync.qntp.timeServer = QnetConstantes.QNET_ADDR_NONE;

            SetQSNMPCommand(QSNMPCmd.QSNMP_CMD_TIME_SYNC);
        }
         * */

       
        public void RespondTimeSync(DateTime receiveTime)
        {            
            DateTime syncTime = DateTime.Now;
            m_tQSNMP.msg.timeSync.qntp.mode = MessageConstantes.QNTP_MODE_RESPONSE;
            m_tQSNMP.msg.timeSync.qntp.referenceLevel       = MessageConstantes.QNTP_LEVEL_NONE;
            m_tQSNMP.msg.timeSync.qntp.referenceIdent       = MessageConstantes.QNTP_IDENT_NONE;

            m_tQSNMP.msg.timeSync.qntp.receiveTime.ti_hour = (short)syncTime.Hour;
            m_tQSNMP.msg.timeSync.qntp.receiveTime.ti_min = (short)syncTime.Minute;
            m_tQSNMP.msg.timeSync.qntp.receiveTime.ti_sec = (short)syncTime.Second;

            m_tQSNMP.msg.timeSync.qntp.transmitTime.ti_hour = (short)receiveTime.Hour;
            m_tQSNMP.msg.timeSync.qntp.transmitTime.ti_min = (short)receiveTime.Minute;
            m_tQSNMP.msg.timeSync.qntp.transmitTime.ti_sec = (short)receiveTime.Second;           

            m_tQSNMP.msg.timeSync.qntp.transmitDate.da_day   = (short)syncTime.Day;
            m_tQSNMP.msg.timeSync.qntp.transmitDate.da_mon = (short)syncTime.Month;
            m_tQSNMP.msg.timeSync.qntp.transmitDate.da_year = (short)syncTime.Year;

            SetQSNMPCommand(QSNMPCmd.QSNMP_CMD_TIME_SYNC);    
        }


        /// <summary>
        /// Fill qsnmp header for the given command
        /// </summary>
        /// <param name="cmd"></param>
        private void SetQSNMPCommand(QSNMPCmd cmd)
        {
            m_tQSNMP.hdr.cmd = (Int16)cmd;
            m_tQSNMP.hdr.typ = 0;
            m_tQSNMP.hdr.par = 0;

            switch (cmd) {
                case QSNMPCmd.QSNMP_CMD_RESTART:
                case QSNMPCmd.QSNMP_CMD_DISP_ON:
                case QSNMPCmd.QSNMP_CMD_DISP_OFF:
                case QSNMPCmd.QSNMP_CMD_SYSTEM_DATA:
                    m_tQSNMP.hdr.msglen = MessageConstantes.QSNMP_HEADER_LEN;
                    break;
                case QSNMPCmd.QSNMP_CMD_TIME_SYNC:
                    DateTime dateTime = DateTime.Now;
                    m_tQSNMP.hdr.msglen = (UInt16)(MessageConstantes.QSNMP_HEADER_LEN + MessageConstantes.QSNMP_TIMESYNC_LEN);

                    m_tQSNMP.msg.timeSync.qntp.version = MessageConstantes.QNTP_VERSION;                   
                    break;               
            } // switch
        }
    }
}
