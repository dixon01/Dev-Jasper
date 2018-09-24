// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsSerialClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.HardwareManager.Core.Gps
{
    using System;
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Common.Configuration.HardwareManager.Gps;
    using Gorba.Common.Configuration.Obc.Common;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Entities.Gps;

    using NLog;

    using NMEA;

    using Math = System.Math;

    /// <summary>
    /// Implementation of a client to the GPS receiver connected via a serial port.
    /// </summary>
    public class GpsSerialClient : GpsClientBase
    {
        private const byte GpsStateAntCc = 0x04; /* -> antenne en court circuit */
        private const byte GpsStateAntUc = 0x08; /* -> antenne branchée mais consommation anormale */
        private const byte GpsStateAntNv = 0x0C; /* -> pas d'antenne */

        private const byte GpsStateSearchSat = 0x00;
        private const byte GpsStateBadGeo = 0x40;
        private const byte GpsStateMode2D = 0x80;

        private static readonly Logger Logger = LogHelper.GetLogger<GpsSerialClient>();

        private readonly SerialPortConfig config;

        private SerialPort serialPort;

        private Thread readThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsSerialClient"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public GpsSerialClient(GpsSerialPortConfig config)
        {
            this.config = config.GpsSerialPort;

            /*
            this.reconnectTimer = TimerFactory.Current.CreateTimer("GpsReceiverReconnect");
            this.reconnectTimer.AutoReset = false;
            this.reconnectTimer.Interval = TimeSpan.FromSeconds(5);
            this.reconnectTimer.Elapsed += (s, e) => this.OpenConnection();
             */
        }

        /// <summary>
        /// The do start.
        /// </summary>
        protected override void DoStart()
        {
            this.OpenConnection();

            this.readThread = new Thread(this.ReadThread) { IsBackground = true };
            this.readThread.Start();
        }

        /// <summary>
        /// The do stop.
        /// </summary>
        protected override void DoStop()
        {
            this.CloseConnection();
        }

        private void OpenConnection()
        {
            Logger.Debug("Closing connection to the serial port {0}", this.config.ComPort);
            this.CloseConnection();

            Logger.Debug("Connecting to the serial port {0}", this.config.ComPort);

            this.serialPort = new SerialPort(
                this.config.ComPort,
                this.config.BaudRate,
                this.config.Parity,
                this.config.DataBits,
                this.config.StopBits);
            this.serialPort.Open();
        }

        private void CloseConnection()
        {
            if (this.serialPort == null)
            {
                return;
            }

            this.readThread = null;

            this.serialPort.Close();
            this.serialPort.Dispose();
            this.serialPort = null;
        }

        private void ReadThread()
        {
            try
            {
                var gpsData = new GpsData();
                string sentence;
                var rmcOk = false;
                var ggaOk = false;

                while (this.readThread != null)
                {
                    char b;
                    while (true)
                    {
                        b = (char)this.serialPort.ReadByte();
                        if (b == '$')
                        {
                            break;
                        }
                    }

                    sentence = b.ToString();
                    while (true)
                    {
                        sentence += (char)this.serialPort.ReadByte();
                        if (sentence.EndsWith("\n"))
                        {
                            break;
                        }
                    }

                    // TODO: Need to find out GpsState
                    NMEASentence nmeaParser = NMEAParser.Parse(sentence);

                    if (sentence.Contains("$GPRMC"))
                    {
                        gpsData.Latitude = (float)nmeaParser.parameters[2];
                        gpsData.Longitude = (float)nmeaParser.parameters[4];
                        gpsData.Speed = 1.852F * (float)nmeaParser.parameters[6];
                        gpsData.Direction = (float)nmeaParser.parameters[7];
                        var time = nmeaParser.parameters[0] as DateTime? ?? new DateTime();
                        var date = nmeaParser.parameters[8] as DateTime? ?? new DateTime();
                        gpsData.SatelliteTimeUtc = new DateTime(
                            date.Year,
                            date.Month,
                            date.Day,
                            time.Hour,
                            time.Minute,
                            time.Second);
                        rmcOk = true;
                    }
                    else if (sentence.Contains("$GPGGA"))
                    {
                        gpsData.SatelliteCount = (int)nmeaParser.parameters[6];
                        gpsData.PrecisionDilution = (float)nmeaParser.parameters[7];
                        gpsData.Altitude = (int)Math.Floor((float)nmeaParser.parameters[8]);
                        gpsData.State = (string)nmeaParser.parameters[5] == "GPS fix" ?
                            GpsState.Ok : GpsState.Searching;
                        ggaOk = true;
                    }

                    if (ggaOk && rmcOk)
                    {
                        this.SendGpsData(gpsData);
                        ggaOk = false;
                        rmcOk = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't read from serial port");
                this.ReOpen();
            }
        }

        private void ReOpen()
        {
            this.CloseConnection();
            this.DoStart();
        }
    }
}
