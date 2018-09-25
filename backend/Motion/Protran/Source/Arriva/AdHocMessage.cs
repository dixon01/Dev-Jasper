// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdHocMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdHocMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;

    using NLog;

    /// <summary>
    /// Container of information about Ad Hoc message
    /// </summary>
    public class AdHocMessage
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="AdHocMessage"/> class.
        /// </summary>
        public AdHocMessage()
        {
            this.ClearAllFields();
        }

        /// <summary>
        /// Gets IconId.
        /// </summary>
        public short IconId { get; private set; }

        /// <summary>
        /// Gets HeaderTextLength.
        /// </summary>
        public short HeaderTextLength { get; private set; }

        /// <summary>
        /// Gets HeaderText.
        /// </summary>
        public byte[] HeaderText { get; private set; }

        /// <summary>
        /// Gets Message TextLength.
        /// </summary>
        public short MsgTextLength { get; private set; }

        /// <summary>
        /// Gets Message text.
        /// </summary>
        public byte[] MsgText { get; private set; }

        /// <summary>
        /// Fills all the properties of this object, parsing the content
        /// of the incoming buffer.
        /// The incoming buffer represents a whole "Event Gorba AdHocMessage".
        /// </summary>
        /// <param name="buffer">The buffer containing the information to fill
        /// all the properties of this object.</param>
        /// <returns>True if the parsing operation has succeeded, else false.</returns>
        public bool Parse(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                Logger.Info("Invalid Ad Hoc buffer");
                return false;
            }

            if (buffer.Length < 38)
            {
                Logger.Info("Invalid Ad Hoc buffer length");
                return false;
            }

            // ok, let's start the parse.
            try
            {
                this.IconId = (short)((buffer[32] << 8) | buffer[33]);
                this.HeaderTextLength = (short)((buffer[34] << 8) | buffer[35]);
                this.HeaderText = new byte[2 * this.HeaderTextLength];
                Array.Copy(buffer, 36, this.HeaderText, 0, this.HeaderText.Length);

                this.MsgTextLength =
                    (short)((buffer[36 + this.HeaderText.Length] << 8) | buffer[36 + this.HeaderText.Length + 1]);
                this.MsgText = new byte[2 * this.MsgTextLength];
                Array.Copy(buffer, 38 + this.HeaderText.Length, this.MsgText, 0, this.MsgText.Length);
            }
            catch (Exception ex)
            {
                // before returning, I clear all.
                this.ClearAllFields();
                Logger.Warn(ex, "Error in parsing Ad Hoc message");
                return false;
            }

            // ok, it seems everything ok.
            return true;
        }

        private void ClearAllFields()
        {
            this.IconId = 0x0000;                   // 2 byte
            this.HeaderTextLength = 0x0000;         // 2 byte
            this.MsgTextLength = 0x0000;            // 2 byte

            // the fields "headerText" and "msgText"
            // have a not fixed size,
            // so, for the moment I set them to null.
            this.HeaderText = null;                 // (2 * headerTextLength) byte
            this.MsgText = null;                    // (2 * msgTextLength) byte
        }
    }
}
