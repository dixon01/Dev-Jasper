// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TftpOperationCode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public enum TftpOperationCode
    {
        FTP_READ_RQS = 1,

        TFTP_WRITE_RQS = 2,

        TFTP_DATA_MSG = 3,

        /// <summary>
        /// used to acknowledge the qmail frame (legacy code TFTP_ACK_MSG)
        /// </summary>
        Acknowledge = 4,

        TFTP_ERROR_MSG = 5,
   
        /// <summary>
        /// Mail request (legacy code TFTP_MAIL_RQS)
        /// </summary>
        MailRequest = 6,

        TFTP_FSTA_RQS = 7,

        TFTP_FSTA_RSP = 8,

        TFTP_DELETE_RQS = 9,

        TFTP_RENAME_RQS = 10,

        TFTP_CLIENT_RQS = 12
    }
}
