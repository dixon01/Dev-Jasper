// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlideShowMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SlideShowMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    using System;

    using NLog;

    /// <summary>
    /// Container of information of slide show message
    /// </summary>
    public class SlideShowMessage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        #region VARIABLES

        #endregion VARIABLES

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideShowMessage"/> class.
        /// </summary>
        public SlideShowMessage()
        {
            this.ClearAllFields();
        }

        #region PROPERTIES

        /// <summary>
        /// Gets Id.
        /// </summary>
        public short Id { get; private set; }

        /// <summary>
        /// Gets IconId.
        /// </summary>
        public short IconId { get; private set; }

        /// <summary>
        /// Gets Message TemplateId.
        /// </summary>
        public short MsgTemplateId { get; private set; }

        /// <summary>
        /// Gets StartOfValidity.
        /// </summary>
        public byte[] StartOfValidity { get; private set; }

        /// <summary>
        /// Gets EndOfValidity.
        /// </summary>
        public byte[] EndOfValidity { get; private set; }

        /// <summary>
        /// Gets HeaderTextLength.
        /// </summary>
        public short HeaderTextLength { get; private set; }

        /// <summary>
        /// Gets the header text.
        /// </summary>
        public byte[] HeaderText { get; private set; }

        /// <summary>
        /// Gets the message text length.
        /// </summary>
        public short MsgTextLength { get; private set; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public byte[] MsgText { get; private set; }

        #endregion PROPERTIES

        #region PUBLIC_FUNCTIONS
        /// <summary>
        /// Fills all the properties of this object, parsing the content
        /// of the incoming buffer.
        /// The incoming buffer represents a whole "EvtGorbaSlideShowMessage".
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
                Logger.Info("Invalid slide show buffer");
                return false;
            }

            if (buffer.Length < 56)
            {
                // invalid buffer's length.
                Logger.Info("Invalid slide show buffer length");
                return false;
            }

            // ok, let's start the parse.
            try
            {
                this.Id = (short)((buffer[32] << 8) | buffer[33]);
                this.IconId = (short)((buffer[34] << 8) | buffer[35]);
                this.MsgTemplateId = (short)((buffer[36] << 8) | buffer[37]);
                Array.Copy(buffer, 38, this.StartOfValidity, 0, 8);
                Array.Copy(buffer, 46, this.EndOfValidity, 0, 8);

                this.HeaderTextLength = (short)((buffer[54] << 8) | buffer[55]);
                this.HeaderText = new byte[2 * this.HeaderTextLength];
                Array.Copy(buffer, 56, this.HeaderText, 0, this.HeaderText.Length);

                this.MsgTextLength =
                    (short)((buffer[56 + this.HeaderText.Length] << 8) | buffer[56 + this.HeaderText.Length + 1]);
                this.MsgText = new byte[2 * this.MsgTextLength];
                Array.Copy(buffer, 58 + this.HeaderText.Length, this.MsgText, 0, this.MsgText.Length);
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
            this.Id = 0x0000;                       // 2 byte
            this.IconId = 0x0000;                   // 2 byte
            this.MsgTemplateId = 0x0000;            // 2 byte
            this.StartOfValidity = new byte[8];     // 8 byte
            this.EndOfValidity = new byte[8];       // 8 byte
            this.HeaderTextLength = 0x0000;         // 2 byte
            this.MsgTextLength = 0x0000;            // 2 byte

            // the fields "headerText" and "msgText"
            // have a not fixed size,
            // so, for the moment I set them to null.
            this.HeaderText = null;                 // (2 * headerTextLength) byte
            this.MsgText = null;                    // (2 * msgTextLength) byte
        }
        #endregion PRIVATE_FUNCTIONS
    }
}
