// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineInfoMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LineInfoMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;

    using NLog;

    /// <summary>
    /// Class containing information about the line info messages
    /// </summary>
    public class LineInfoMessage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfoMessage"/> class.
        /// </summary>
        public LineInfoMessage()
        {
            this.ClearAllFields();
        }

        /// <summary>
        /// Gets Region.
        /// </summary>
        public short Region { get; private set; }

        /// <summary>
        /// Gets LineNumberPlanning.
        /// </summary>
        public short LineNumberPlanning { get; private set; }

        /// <summary>
        /// Fills all the properties of this object, parsing the content
        /// of the incoming buffer.
        /// The incoming buffer represents a whole "Event Gorba LineInfo".
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
                Logger.Info("Invalid line info buffer");
                return false;
            }

            if (buffer.Length < 36)
            {
                // invalid buffer's length.
                Logger.Info("Invalid line info buffer length");
                return false;
            }

            // ok, let's start the parse.
            try
            {
                this.Region = (short)((buffer[32] << 8) | buffer[33]);
                this.LineNumberPlanning = (short)((buffer[34] << 8) | buffer[35]);
            }
            catch (Exception ex)
            {
                // an error was occured parsing the buffer.
                // before returning, I clear all.
                Logger.Warn(ex, "Could not parse LineInfoMessage");
                this.ClearAllFields();
                return false;
            }

            // ok, it seems everything ok.
            return true;
        }

        private void ClearAllFields()
        {
            this.Region = 0x0000;               // 2 byte
            this.LineNumberPlanning = 0x0000;   // 2 byte
        }
    }
}
