// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PcapUdpFileReader.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PcapUdpFileReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Simulation
{
    using System;
    using System.IO;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Motion.Protran.Ibis.Parsers;

    /// <summary>
    /// File reader for PCAP files containing UDP port 47555 telegrams (7 bit).
    /// </summary>
    public class PcapUdpFileReader : IbisFileReader
    {
        private readonly Parser parser = new Parser7Bit(true, new TelegramConfig[0]);

        private BinaryReader fileReader;

        private Packet nextPacket;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcapUdpFileReader"/> class.
        /// </summary>
        /// <param name="config">
        /// The simulation config.
        /// </param>
        public PcapUdpFileReader(SimulationConfig config)
            : base(config)
        {
        }

        /// <summary>
        /// Opens the underlying file.
        /// </summary>
        public override void Open()
        {
            this.fileReader = new BinaryReader(File.OpenRead(this.Config.SimulationFile));
            this.ReadFileHeader();
        }

        /// <summary>
        /// Closes the underlying file.
        /// </summary>
        public override void Close()
        {
            if (this.fileReader == null)
            {
                return;
            }

            this.fileReader.Close();
        }

        /// <summary>
        /// Reads the next telegram from the file.
        /// </summary>
        /// <returns>
        /// true if a new telegram was found.
        /// </returns>
        public override bool ReadNext()
        {
            Packet packet;
            while ((packet = this.ReadNextPacket()) != null)
            {
                var raw = packet.Data;
                var ethType = BitConverter.ToUInt16(raw, 12);
                if (ethType != 0x0008)
                {
                    // not an IP packet
                    continue;
                }

                var versionIp = raw[14] >> 4;
                if (versionIp != 4)
                {
                    // not an IPv4 packet
                    continue;
                }

                var headerLenIp = (raw[14] & 0x0F) * 4;
                var totalLenIp = raw[16] << 8 | raw[17];
                var protocolIp = raw[23];

                if (protocolIp != 17)
                {
                    // not an UDP packet
                    continue;
                }

                if (raw[29] != 1)
                {
                    // packet doesn't come from x.x.x.1, so we assume it's not from the OBU
                    continue;
                }

                var destPort = raw[16 + headerLenIp] << 8 | raw[17 + headerLenIp];
                if (destPort != 47555)
                {
                    // packet isn't from port 47555, so we assume it's not an IBIS telegram
                    continue;
                }

                var lengthUdp = raw[18 + headerLenIp] << 8 | raw[19 + headerLenIp];

                if (lengthUdp + headerLenIp < totalLenIp)
                {
                    // invalid length, ignore packet
                    continue;
                }

                if (this.nextPacket == null)
                {
                    this.nextPacket = packet;
                    continue;
                }

                // now, let's look at the previous packet again.
                raw = this.nextPacket.Data;
                headerLenIp = (raw[14] & 0x0F) * 4;
                lengthUdp = raw[18 + headerLenIp] << 8 | raw[19 + headerLenIp];

                var data = new byte[lengthUdp - 8 + 1]; // add one byte for Checksum
                Array.Copy(raw, 42, data, 0, lengthUdp - 8);

                this.parser.UpdateChecksum(data);
                this.CurrentTelegram = data;
                this.CurrentWaitTime = new TimeSpan((packet.Timestamp - this.nextPacket.Timestamp) * 10);

                this.nextPacket = packet;
                return true;
            }

            return false;
        }

        private void ReadFileHeader()
        {
            // guint32 magic_number;   /* magic number */
            // guint16 version_major;  /* major version number */
            // guint16 version_minor;  /* minor version number */
            // gint32 thiszone;       /* GMT to local correction */
            // guint32 sigfigs;        /* accuracy of timestamps */
            // guint32 snaplen;        /* max length of captured packets, in octets */
            // guint32 network;        /* data link type */
            var magicNumber = this.fileReader.ReadUInt32();
            if (magicNumber == 0xd4c3b2a1)
            {
                throw new NotSupportedException("Reader doesn't support inverted byte order");
            }

            if (magicNumber != 0xa1b2c3d4)
            {
                throw new InvalidDataException("Invalid magic number 0x" + magicNumber.ToString("X08"));
            }

            var versionMajor = this.fileReader.ReadUInt16();
            var versionMinor = this.fileReader.ReadUInt16();
            var version = new Version(versionMajor, versionMinor);
            if (version > new Version(2, 4))
            {
                throw new NotSupportedException("Reader doesn't support version " + version);
            }

            this.fileReader.ReadInt32(); // skip thiszone
            this.fileReader.ReadUInt32(); // skip sigfigs
            this.fileReader.ReadUInt32(); // skip snaplen

            var network = this.fileReader.ReadUInt32();
            if (network != 1)
            {
                throw new NotSupportedException("Reader doesn't support network protocol #" + network);
            }
        }

        private Packet ReadNextPacket()
        {
            try
            {
                return new Packet(this.fileReader);
            }
            catch (EndOfStreamException)
            {
                return null;
            }
        }

        private class Packet
        {
            public Packet(BinaryReader reader)
            {
                // guint32 ts_sec;         /* timestamp seconds */
                // guint32 ts_usec;        /* timestamp microseconds */
                // guint32 incl_len;       /* number of octets of packet saved in file */
                // guint32 orig_len;       /* actual length of packet */
                long sec = reader.ReadUInt32();
                long usec = reader.ReadUInt32();
                this.Timestamp = (sec * 1000 * 1000) + usec;

                var inclLen = reader.ReadUInt32();
                reader.ReadUInt32(); // skip orig_len

                this.Data = reader.ReadBytes((int)inclLen);
            }

            /// <summary>
            /// Gets the timestamp in microseconds
            /// </summary>
            public long Timestamp { get; private set; }

            public byte[] Data { get; private set; }
        }
    }
}
