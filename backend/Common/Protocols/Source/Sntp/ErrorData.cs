// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorData.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Class that holds data relating to any errors that occurred.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    using System;

    /// <summary>
    /// Class that holds data relating to any errors that occurred.
    /// </summary>
    public class ErrorData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorData"/> class.
        /// </summary>
        /// <param name="errorText">
        /// A textual representation of the error that occurred.
        /// </param>
        internal ErrorData(string errorText)
        {
            this.ErrorText = errorText;
            this.Error = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorData"/> class.
        /// </summary>
        /// <param name="exception">
        /// The exception (if any) that was caught.
        /// </param>
        internal ErrorData(Exception exception)
        {
            this.ErrorText = exception.Message;
            this.Exception = exception;
            this.Error = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorData"/> class.
        /// </summary>
        internal ErrorData()
        {
        }

        /// <summary>
        /// Gets a value indicating whether an error occurred.
        /// </summary>
        public bool Error
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a textual representation of the error that occurred.
        /// </summary>
        public string ErrorText
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exception (if any) that was caught.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }
    }
}
