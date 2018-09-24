// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusResponseFrame.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusResponseFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    using System;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    /// The status response frame (0x00).
    /// This frame is sent from the slave to the master.
    /// </summary>
    public class StatusResponseFrame : LongFrameBase
    {
        private byte[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusResponseFrame"/> class.
        /// </summary>
        /// <param name="data">
        /// The payload data of the status response.
        /// </param>
        public StatusResponseFrame(byte[] data)
            : this()
        {
            this.SetData(data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusResponseFrame"/> class.
        /// </summary>
        internal StatusResponseFrame()
            : base(FunctionCode.StatusResponse)
        {
        }

        /// <summary>
        /// The possible sign types.
        /// </summary>
        public enum SignTypes
        {
            /// <summary>
            /// The sign type could not be detected.
            /// Additional information is available in the <see cref="Version"/> property.
            /// </summary>
            Unknown,

            /// <summary>
            /// LED sign.
            /// </summary>
            Led,

            /// <summary>
            /// Color LED sign.
            /// </summary>
            ColorLed
        }

        /// <summary>
        /// Gets the sign type.
        /// </summary>
        [Category("General")]
        public SignTypes SignType { get; private set; }

        /// <summary>
        /// Gets the version string.
        /// </summary>
        [Category("General")]
        public string Version { get; private set; }

        /// <summary>
        /// Gets the width of the sign, if available.
        /// </summary>
        [Category("Dimensions")]
        public int? Width { get; private set; }

        /// <summary>
        /// Gets the height of the sign if available.
        /// </summary>
        [Category("Dimensions")]
        public int? Height { get; private set; }

        /// <summary>
        /// Gets the temperature measured in the sign, if available.
        /// </summary>
        [Category("Status")]
        public int? Temperature { get; private set; }

        /// <summary>
        /// Gets the DIP switch setting of the sign.
        /// </summary>
        [Category("Status")]
        public string DipSwitch { get; private set; }

        /// <summary>
        /// Reads the payload of this frame (without the command byte) from the given reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal override void ReadPayload(FrameReader reader)
        {
            this.SetData(reader.ReadAll());
        }

        /// <summary>
        /// Writes the payload of this frame (without the command byte) to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal override void WritePayload(FrameWriter writer)
        {
            writer.WriteBytes(this.data);
        }

        private void SetData(byte[] bytes)
        {
            this.data = bytes;

            if (bytes.Length >= 6)
            {
                // LEDV
                if (bytes[0] == 'L' && bytes[1] == 'E' && bytes[2] == 'D' && bytes[3] == 'V')
                {
                    if (bytes[4] > 0x7F)
                    {
                        this.Version = BitConverter.ToString(bytes, 4, 2);
                    }
                    else
                    {
                        this.Version = Encoding.ASCII.GetString(bytes, 4, 2);
                    }

                    this.SignType = SignTypes.Led;

                    if (bytes.Length >= 10)
                    {
                        this.Height = LittleEndianConverter.ToUInt16(bytes, 6);
                        this.Width = LittleEndianConverter.ToUInt16(bytes, 8);
                    }

                    return;
                }

                // HLED
                if (bytes.Length >= 20 && bytes[0] == 'H' && bytes[1] == 'L' && bytes[2] == 'E' && bytes[3] == 'D')
                {
                    this.Version = Encoding.ASCII.GetString(bytes, 4, 10);
                    this.SignType = SignTypes.Led;

                    this.Width = LittleEndianConverter.ToUInt16(bytes, 14);
                    this.Height = LittleEndianConverter.ToUInt16(bytes, 16);

                    unchecked
                    {
                        int temperature = (sbyte)bytes[18];
                        if (temperature != 0x80)
                        {
                            this.Temperature = temperature;
                        }
                    }

                    this.DipSwitch = bytes[19].ToString("X2");

                    return;
                }

                // LEDv
                if (bytes[0] == 'L' && bytes[1] == 'E' && bytes[2] == 'D' && bytes[3] == 'v')
                {
                    this.Version = bytes[4].ToString("X2");
                    this.SignType = SignTypes.Led;
                    this.Temperature = bytes[5];

                    if (bytes.Length >= 10)
                    {
                        this.Height = LittleEndianConverter.ToUInt16(bytes, 6);
                        this.Width = LittleEndianConverter.ToUInt16(bytes, 8);
                    }

                    return;
                }

                // GLED
                if (bytes[0] == 'G' && bytes[1] == 'L' && bytes[2] == 'E' && bytes[3] == 'D')
                {
                    this.Version = Encoding.ASCII.GetString(bytes, 4, 2);
                    this.SignType = SignTypes.ColorLed;
                    return;
                }
            }

            this.Version = BitConverter.ToString(bytes);
            this.SignType = SignTypes.Unknown;
        }
    }
}