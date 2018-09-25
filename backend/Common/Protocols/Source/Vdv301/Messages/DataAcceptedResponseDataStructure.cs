// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAcceptedResponseDataStructure.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataAcceptedResponseDataStructure type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Messages
{
    using System;

    /// <summary>
    /// The IBIS-IP <c>DataAcceptedResponseDataStructure</c> used for "empty" return values of operations.
    /// </summary>
    public partial class DataAcceptedResponseDataStructure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcceptedResponseDataStructure"/> class.
        /// </summary>
        public DataAcceptedResponseDataStructure()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcceptedResponseDataStructure"/> class.
        /// </summary>
        /// <param name="timeStamp">
        /// The <see cref="TimeStamp"/> value.
        /// </param>
        /// <param name="dataAccepted">
        /// The <see cref="DataAccepted"/> value.
        /// </param>
        public DataAcceptedResponseDataStructure(DateTime timeStamp, bool dataAccepted)
        {
            this.TimeStamp = new IBISIPdateTime(timeStamp);
            this.DataAccepted = new IBISIPboolean(dataAccepted);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAcceptedResponseDataStructure"/> class.
        /// This constructor will also set the <see cref="ErrorCodeSpecified"/> property.
        /// </summary>
        /// <param name="timeStamp">
        /// The <see cref="TimeStamp"/> value.
        /// </param>
        /// <param name="dataAccepted">
        /// The <see cref="DataAccepted"/> value.
        /// </param>
        /// <param name="errorCode">
        /// The <see cref="ErrorCode"/> value.
        /// </param>
        /// <param name="errorInfo">
        /// The <see cref="ErrorInformation"/> value.
        /// </param>
        public DataAcceptedResponseDataStructure(
            DateTime timeStamp, bool dataAccepted, ErrorCodeEnumeration errorCode, string errorInfo)
            : this(timeStamp, dataAccepted)
        {
            this.ErrorCode = errorCode;
            this.ErrorCodeSpecified = true;
            if (errorInfo != null)
            {
                this.ErrorInformation = new IBISIPstring(errorInfo);
            }
        }
    }
}
