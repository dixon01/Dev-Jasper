// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WifiStatusMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WifiStatusMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;

    using NLog;

    /// <summary>
    /// Class containing information about the wifi messages
    /// </summary>
    public class WifiStatusMessage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="WifiStatusMessage"/> class.
        /// </summary>
        public WifiStatusMessage()
        {
            this.ClearAllFields();
        }

        #region PROPERTIES

        /// <summary>
        /// Gets Version.
        /// </summary>
        public byte Version { get; private set; }

        /// <summary>
        /// Gets WifiStatus.
        /// </summary>
        public byte WifiStatus { get; private set; }

        #endregion PROPERTIES

        #region PUBLIC_FUNCTIONS
        /// <summary>
        /// Fills all the properties of this object, parsing the content
        /// of the incoming buffer.
        /// The incoming buffer represents a whole "EvtWifiStatusMessage".
        /// </summary>
        /// <param name="buffer">The buffer containing the information to fill
        /// all the properties of this object.</param>
        /// <returns>True if the parsing operation has succeeded, else false.</returns>
        public bool Parse(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                // invalid buffer.
                // I cannot parse nothing.
                Logger.Info("Invalid wifi buffer");
                return false;
            }

            if (buffer.Length < 34)
            {
                // invalid buffer's length.
                Logger.Info("Invalid wifi buffer length");
                return false;
            }

            // ok, let's start the parse.
            try
            {
                this.Version = buffer[32];
                this.WifiStatus = buffer[33];
            }
            catch (Exception)
            {
                // an error was occured parsing the buffer.
                // before returning, I clear all.
                this.ClearAllFields();
                return false;
            }

            // ok, it seems everything ok.
            return true;
        }
        #endregion PUBLIC_FUNCTIONS

        #region PRIVATE_FUNCTIONS
        private void ClearAllFields()
        {
            this.Version = 0x00;            // 1 byte
            this.WifiStatus = 0x00;         // 1 byte
        }
        #endregion PRIVATE_FUNCTIONS
    }
}
