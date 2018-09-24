// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsPilotClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsPilotClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Gps
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Configuration.HardwareManager.Gps;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Entities.Gps;

    using NLog;

    /// <summary>
    /// Implementation of a client to the GPS Pilot TCP server application.
    /// This can also be used with ADT to simulate GPS positions.
    /// </summary>
    public class GpsPilotClient : GpsClientBase
    {
        private const byte GpsStateAntCc = 0x04; /* -> antenne en court circuit */
        private const byte GpsStateAntUc = 0x08; /* -> antenne branchée mais consommation anormale */
        private const byte GpsStateAntNv = 0x0C; /* -> pas d'antenne */

        private const byte GpsStateSearchSat = 0x00;
        private const byte GpsStateBadGeo = 0x40;
        private const byte GpsStateMode2D = 0x80;
        ////private const byte GpsStateMode3D = 0xC0;

        private static readonly Logger Logger = LogHelper.GetLogger<GpsPilotClient>();

        private readonly GpsPilotConfig config;

        private readonly ITimer reconnectTimer;

        private readonly byte[] readBuffer = new byte[44];

        private Socket socket;

        private NetworkStream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsPilotClient"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public GpsPilotClient(GpsPilotConfig config)
        {
            this.config = config;

            this.reconnectTimer = TimerFactory.Current.CreateTimer("GpsPilotReconnect");
            this.reconnectTimer.AutoReset = false;
            this.reconnectTimer.Interval = TimeSpan.FromSeconds(5);
            this.reconnectTimer.Elapsed += (s, e) => this.BeginConnect();
        }

        /// <summary>
        /// Implementation of the start method.
        /// </summary>
        protected override void DoStart()
        {
            this.BeginConnect();
        }

        /// <summary>
        /// Implementation of the stop method.
        /// </summary>
        protected override void DoStop()
        {
            this.CloseConnection();
        }

        private void BeginConnect()
        {
            Logger.Debug("Connecting to {0}:{1}", this.config.IpAddress, this.config.Port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(IPAddress.Parse(this.config.IpAddress), this.config.Port);
            this.socket.BeginConnect(endPoint, this.Connected, null);
        }

        private void Connected(IAsyncResult ar)
        {
            try
            {
                this.socket.EndConnect(ar);
                this.stream = new NetworkStream(this.socket);
                Logger.Info("Connected to {0}:{1}", this.config.IpAddress, this.config.Port);
                this.stream.BeginRead(this.readBuffer, 0, this.readBuffer.Length, this.Read, null);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't connect to " + this.config.IpAddress + ":" + this.config.Port);
                this.StartReconnectTimer();
            }
        }

        private void Read(IAsyncResult ar)
        {
            try
            {
                var count = this.stream.EndRead(ar);

                if (count == this.readBuffer.Length)
                {
                    this.HandleData();
                    count = 0;
                }

                this.stream.BeginRead(this.readBuffer, count, this.readBuffer.Length - count, this.Read, null);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't read");
                this.StartReconnectTimer();
            }
        }

        private void HandleData()
        {
            var reader = new BinaryReader(new MemoryStream(this.readBuffer, false));
            var gpsData = new GpsData();
            gpsData.Longitude = reader.ReadSingle();
            gpsData.Latitude = reader.ReadSingle();
            gpsData.Altitude = reader.ReadInt16();
            reader.ReadInt16(); // byte alignment
            gpsData.Speed = reader.ReadSingle();
            gpsData.IsStopped = reader.ReadInt32() != 0;
            gpsData.PrecisionDilution = reader.ReadSingle();
            gpsData.Direction = reader.ReadSingle();
            var hour = reader.ReadByte();
            var minute = reader.ReadByte();
            var second = reader.ReadByte();
            var day = reader.ReadByte();
            var month = reader.ReadByte();
            reader.ReadByte(); // byte alignment
            var year = reader.ReadInt16();
            gpsData.UtcOffset = TimeSpan.FromMinutes(reader.ReadByte() * 30);
            gpsData.SatelliteCount = reader.ReadByte();
            var receiverState = reader.ReadByte();
            var antennaState = reader.ReadByte();

            if (receiverState == GpsStateSearchSat)
            {
                gpsData.State |= GpsState.Searching;
            }

            if ((receiverState | GpsStateBadGeo) != GpsStateBadGeo)
            {
                gpsData.State |= GpsState.BadLocation;
            }

            if ((receiverState | GpsStateMode2D) != GpsStateMode2D)
            {
                gpsData.State |= GpsState.Missing3RdDimenion;
            }

            if ((antennaState | GpsStateAntCc) != GpsStateAntCc)
            {
                gpsData.State |= GpsState.AntennaShortCircuit;
            }

            if ((antennaState | GpsStateAntUc) != GpsStateAntUc)
            {
                gpsData.State |= GpsState.AntennaPowerSurge;
            }

            if ((antennaState | GpsStateAntNv) != GpsStateAntNv)
            {
                gpsData.State |= GpsState.AntennaMissing;
            }

            gpsData.SatelliteTimeUtc = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            this.SendGpsData(gpsData);
        }

        private void StartReconnectTimer()
        {
            this.CloseConnection();

            this.reconnectTimer.Enabled = true;
        }

        private void CloseConnection()
        {
            if (this.stream != null)
            {
                this.stream.Close();
                this.stream = null;
            }

            if (this.socket != null)
            {
                if (this.socket.Connected)
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                }

                this.socket.Close();
                this.socket = null;
            }
        }
    }
}