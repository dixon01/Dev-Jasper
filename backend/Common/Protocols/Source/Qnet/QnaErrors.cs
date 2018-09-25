// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnaErrors.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Definition of QNA error codes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Definition of QNA error codes
    /// </summary>    
    public class QnaErrors
    {
        /// <summary>
        /// ErrorQnaInvalidHeaderType : (Int16)0x8100
        /// </summary>
        public const short ErrorQnaInvalidHeaderType = -32512;

        /// <summary>
        /// ErrorQnaFrameLength : (Int16)0x8101
        /// </summary>
        public const short ErrorQnaFrameLength = -32511;

        /// <summary>
        /// ErrorQnaRxMinLength : (Int16)0x8102
        /// </summary>
        public const short ErrorQnaRxMinLength = -32510;

        /// <summary>
        /// ErrorQnaRxVersion : (Int16)0x8103
        /// </summary>
        public const short ErrorQnaRxVersion = -32509;

        /// <summary>
        /// ErrorQnaRxType : (Int16)0x8104
        /// </summary>
        public const short ErrorQnaRxType = -32508;

        /// <summary>
        /// ErrorQnaRxMaxDataLength : (Int16)0x8105
        /// </summary>
        public const short ErrorQnaRxMaxDataLength = -32507;

        /// <summary>
        /// ErrorQnaRxCrc : (Int16)0x8106
        /// </summary>
        public const short ErrorQnaRxCrc = -32506;

        /// <summary>
        /// ErrorQnaRxState : (Int16)0x8107
        /// </summary>
        public const short ErrorQnaRxState = -32505;

        /// <summary>
        /// ErrorQnaRxSend : (Int16)0x8108
        /// </summary>
        public const short ErrorQnaRxSend = -32504;

        /// <summary>
        /// Convert the specified code into the corresponding string.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <returns>
        /// The string containing a meaning error code.
        /// </returns>
        public static string ConvertErrorCode(short errorCode)
        {
            switch (errorCode)
            {
                case ErrorQnaInvalidHeaderType:
                    return "ERROR_QNA_INVALID_HEADER_TYPE";

                case ErrorQnaFrameLength:
                    return "ERROR_QNA_FRAME_LENGTH";

                case ErrorQnaRxMinLength:
                    return "ERROR_QNA_RX_MIN_LENGTH";

                case ErrorQnaRxVersion:
                    return "ERROR_QNA_RX_VERSION";

                case ErrorQnaRxType:
                    return "ERROR_QNA_RX_TYPE";

                case ErrorQnaRxMaxDataLength:
                    return "ERROR_QNA_RX_MAX_DATA_LENGTH";

                case ErrorQnaRxCrc:
                    return "ERROR_QNA_RX_CRC";

                case ErrorQnaRxState:
                    return "ERROR_QNA_RX_STATE";

                case ErrorQnaRxSend:
                    return "ERROR_QNA_RX_SEND";
                default:
                    return "Unkown qna error code";
            }
        }
    }
}
