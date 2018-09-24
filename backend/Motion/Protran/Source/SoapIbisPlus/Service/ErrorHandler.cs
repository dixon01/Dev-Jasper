// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;

    using NLog;

    /// <summary>
    /// Class responsible for handling error codes in <see cref="MfdServiceImpl"/>.
    /// </summary>
    internal class ErrorHandler
    {
        private const int DeviceErrorBits = -2147483648;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets a value indicating whether the device has an error.
        /// </summary>
        public bool DeviceError { get; private set; }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        public ErrorType ErrorType { get; private set; }

        /// <summary>
        /// Gets the error number.
        /// </summary>
        public ErrorNumber ErrorNumber { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the system needs a full data update.
        /// </summary>
        public bool NeedFullDataUpdate { get; set; }

        /// <summary>
        /// Sets the error for a certain type.
        /// </summary>
        /// <param name="type">
        /// The error type.
        /// </param>
        /// <param name="number">
        /// The error number.
        /// </param>
        public void SetError(ErrorType type, ErrorNumber number)
        {
            if (number == ErrorNumber.Ok)
            {
                this.ClearError(type);
                return;
            }

            this.ErrorType = type;
            this.ErrorNumber = number;
            this.DeviceError = true;
        }

        /// <summary>
        /// Clears the error for a certain type.
        /// </summary>
        /// <param name="type">
        /// The error type.
        /// </param>
        public void ClearError(ErrorType type)
        {
            if (this.ErrorType == type)
            {
                this.ErrorType = ErrorType.None;
                this.ErrorNumber = ErrorNumber.Ok;
                this.DeviceError = false;
            }
        }

        /// <summary>
        /// Gets the current error bitmap including the individual code.
        /// </summary>
        /// <param name="individualCode">
        /// The individual code (bits 0  to 3).
        /// </param>
        /// <returns>
        /// The error bitmap.
        /// </returns>
        public int GetErrorBitmap(int individualCode)
        {
            try
            {
                var error = individualCode & 0xF;
                if (this.DeviceError)
                {
                    error |= DeviceErrorBits;
                }

                error |= ((int)this.ErrorType & 0x0F) << 27;
                error |= ((int)this.ErrorNumber & 0x0F) << 23;

                if (this.NeedFullDataUpdate)
                {
                    error |= 0x00400000;
                }

                return error;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't calculate error bitmap");
                return DeviceErrorBits;
            }
        }

        /// <summary>
        /// Gets the details about a certain error type and number.
        /// </summary>
        /// <param name="type">
        /// The error type.
        /// </param>
        /// <param name="number">
        /// The error number.
        /// </param>
        /// <returns>
        /// The error details or an empty string if the type is not given.
        /// </returns>
        public string GetErrorDetails(ErrorType type, ErrorNumber number)
        {
            switch (type)
            {
                case ErrorType.None:
                    return string.Empty;
                case ErrorType.Trip:
                    switch (number)
                    {
                        case ErrorNumber.NotFound:
                            return "Trip ID not found";
                        case ErrorNumber.BadData:
                            return "Trip data invalid";
                        default:
                            return "Unknown trip error";
                    }

                case ErrorType.Stop:
                    switch (number)
                    {
                        case ErrorNumber.NotFound:
                            return "Stop ID not found";
                        default:
                            return "Unknown stop error";
                    }
            }

            return "Unknown error";
        }
    }
}