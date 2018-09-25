// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Triplet.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Datagram
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Ctu.Notifications;
    using Gorba.Common.Protocols.Ctu.Requests;
    using Gorba.Common.Protocols.Ctu.Responses;

    /// <summary>
    /// Representation of one CTU's payload triplet
    /// using an object oriented style.
    /// </summary>
    public abstract class Triplet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Triplet"/> class
        /// with an empty buffer.
        /// </summary>
        /// <param name="tag">
        /// The tag of this triplet.
        /// </param>
        protected Triplet(TagName tag)
        {
            this.Tag = tag;
        }

        /// <summary>
        /// Gets the triplet's tag.
        /// </summary>
        [XmlIgnore]
        public TagName Tag { get; private set; }

        /// <summary>
        /// Gets the triplet's payload length.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Gets the tag's type, depending on its value:
        /// from    1 to  254 notification
        /// from 1025 to 1278 request
        /// from 1281 to 2046 response
        /// </summary>
        public TagType TagType
        {
            get
            {
                var tag = (int)this.Tag;
                if (tag >= 1 && tag <= 254)
                {
                    return TagType.Notification;
                }

                if (tag >= 1025 && tag <= 1278)
                {
                    return TagType.Request;
                }

                if (tag >= 1281 && tag <= 2050)
                {
                    return TagType.Response;
                }

                return TagType.Invalid;
            }
        }

        /// <summary>
        /// Read a triplet from the given reader.
        /// </summary>
        /// <param name="reader">
        /// The reader to read the triplet from.
        /// </param>
        /// <returns>
        /// a subclass of <see cref="Triplet"/>.
        /// </returns>
        public static Triplet CreateFrom(BinaryReader reader)
        {
            var tag = (TagName)reader.ReadUInt16();
            int length = reader.ReadUInt16();
            switch (tag)
            {
                case TagName.Status:
                    return new Status(length, reader);
                case TagName.LogMessage:
                    return new LogMessage(length, reader);
                case TagName.DisplayStatus:
                    return new DisplayStatus(length, reader);
                case TagName.TripInfo:
                    return new TripInfo(length, reader);
                case TagName.DownloadStart:
                    return new DownloadStart(length, reader);
                case TagName.DownloadAbort:
                    return new DownloadAbort(length, reader);
                case TagName.LineInfo:
                    return new LineInfo(length, reader);
                case TagName.ExtendedLineInfo:
                    return new ExtendedLineInfo(length, reader);
                case TagName.ExteriorSignTexts:
                    return new ExteriorSignTexts(length, reader);
                case TagName.CountdownNumber:
                    return new CountdownNumber(length, reader);
                case TagName.SpecialInputInfo:
                    return new SpecialInputInfo(length, reader);
                case TagName.DownloadProgressRequest:
                    return new DownloadProgressRequest(length, reader);
                case TagName.DownloadProgressResponse:
                    return new DownloadProgressResponse(length, reader);
                case TagName.DeviceInfoRequest:
                    return new DeviceInfoRequest(length, reader);
                case TagName.DeviceInfoResponse:
                    return new DeviceInfoResponse(length, reader);
                default:
                    return new Unknown(tag, length, reader);
            }
        }

        /// <summary>
        /// Writes this packet's tag, length and value to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((ushort)this.Tag);
            writer.Write((ushort)this.Length);
            this.WritePayload(writer);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Tag.ToString();
        }

        /// <summary>
        /// Reads a Unicode string of a given length from the <see cref="reader"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="length">
        /// The length of the string in bytes (2 * character count).
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected static string ReadString(BinaryReader reader, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "Length has to be zero or positive");
            }

            if (length == 0)
            {
                return string.Empty;
            }

            var bytes = reader.ReadBytes(length);
            return Encoding.Unicode.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Subclasses have to implement this method to write their
        /// respective payload to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected abstract void WritePayload(BinaryWriter writer);

        private class Unknown : Triplet
        {
            private readonly byte[] data;

            public Unknown(TagName tag, int length, BinaryReader reader)
                : base(tag)
            {
                this.data = reader.ReadBytes(length);
            }

            public override int Length
            {
                get
                {
                    return this.data.Length;
                }
            }

            public override string ToString()
            {
                return "Unknown";
            }

            protected override void WritePayload(BinaryWriter writer)
            {
                writer.Write(this.data);
            }
        }
    }
}
