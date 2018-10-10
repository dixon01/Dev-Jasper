// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evERGError.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Used to submit ERG protocol error codes from ERGControl -&gt; VT3
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// Used to submit ERG protocol error codes from ERGControl -> VT3
    /// </summary>
    public class evERGError
    {
        ////public static int NO_ERROR = 0;

        /// <summary>
        /// The general error.
        /// </summary>
        public const int E1_GENERAL_ERROR = 1;

        /// <summary>
        /// The invalid data error.
        /// </summary>
        public const int E2_INVALID_DATA = 2;

        /// <summary>
        /// The ticketing error.
        /// </summary>
        public const int E3_TICKETING_ERROR = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="evERGError"/> class.
        /// </summary>
        public evERGError()
        {
            this.ErrorCode = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evERGError"/> class.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        public evERGError(int errorCode)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets or sets the errorcode
        /// 1:  E1  Fehler Kasse - Ticketing System general error
        /// 2:  E2  Ungültige Daten Kasse - Ticketing System invalid data
        /// 3:  E3  Entwerter blockiert - Ticket canceller blocked
        /// 4:  E4  Fehler Entwerter - Ticket canceller error
        /// </summary>
        public int ErrorCode { get; set; }
    }
}