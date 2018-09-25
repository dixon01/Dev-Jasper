// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimulationRemoteComputer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimulationRemoteComputer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Protocols.Isi;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Motion.Protran.AbuDhabi.Config;
    using Gorba.Motion.Protran.AbuDhabi.Isi;

    using NLog;

    /// <summary>
    /// Remote computer subclass that reads from a simulation file
    /// instead of a real socket.
    /// </summary>
    public class SimulationRemoteComputer : RemoteComputer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IsiSimulationConfig simulationConfig;

        private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);

        private readonly IsiSerializer serializer = new IsiSerializer();

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationRemoteComputer"/> class.
        /// </summary>
        /// <param name="simulationConfig">
        /// The simulation config.
        /// </param>
        public SimulationRemoteComputer(IsiSimulationConfig simulationConfig)
        {
            this.simulationConfig = simulationConfig;
        }

        /// <summary>
        /// Configures this remote computer. 
        /// </summary>
        /// <param name="config">The configuration</param>
        public override void Configure(IsiConfig config)
        {
            // do nothing
        }

        /// <summary>
        /// Connects to the remote ISI TCP/IP server.
        /// <exception cref="FileNotFoundException">If the file to simulate
        /// doesn't exist, an exception will be thrown.</exception>
        /// </summary>
        public override void Connect()
        {
            if (this.running)
            {
                return;
            }

            if (!File.Exists(this.simulationConfig.SimulationFile))
            {
                // the file doesn't exist.
                // I can't simulate nothing.
                throw new FileNotFoundException("File missing", this.simulationConfig.SimulationFile);
            }

            this.running = true;
            var thread = new Thread(this.ReadFile) { IsBackground = true };
            thread.Start();
        }

        /// <summary>
        /// Disconnects from the remote ISI TCP/IP server.
        /// </summary>
        public override void Disconnect()
        {
            this.running = false;
            this.waitEvent.Set();
        }

        /// <summary>
        /// Sends an ISI put message to the remote ISI computer.
        /// </summary>
        /// <param name="isiMessage">The ISI message to be sent.</param>
        public override void SendIsiMessage(IsiMessageBase isiMessage)
        {
            // ignore messages
        }

        private void ReadFile()
        {
            this.waitEvent.WaitOne(500, false);
            this.RaiseConnected(EventArgs.Empty);
            Logger.Info("Starting to read from {0}", this.simulationConfig.SimulationFile);
            using (var reader = new StreamReader(this.simulationConfig.SimulationFile, Encoding.UTF8))
            {
                var lastTimestamp = DateTime.MaxValue;
                string line;
                while (this.running && (line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(new[] { ' ' }, 4);
                    if (parts.Length < 4)
                    {
                        continue;
                    }

                    if (parts[0] != "<")
                    {
                        // we only care about incoming telegrams
                        continue;
                    }

                    try
                    {
                        lastTimestamp = this.HandleLine(parts, lastTimestamp);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Could not parse line: " + line, ex);
                    }
                }

                if (this.running)
                {
                    Logger.Info("Finished reading log file");
                    this.running = false;
                }
            }
        }

        private DateTime HandleLine(string[] parts, DateTime lastTimestamp)
        {
            var date = DateTime.ParseExact(parts[1], "yyyy/MM/dd", CultureInfo.InvariantCulture);
            var time = DateTime.ParseExact(parts[2], "HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var timestamp = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
            var waitTime = timestamp - lastTimestamp;
            if (waitTime.TotalMilliseconds > 0)
            {
                this.waitEvent.WaitOne(waitTime, false);
                if (!this.running)
                {
                    return timestamp;
                }
            }

            var message = this.ParseMessage(parts[3]);
            this.RaiseIsiMessageReceived(new IsiMessageEventArgs { IsiMessage = message });
            return timestamp;
        }

        private IsiMessageBase ParseMessage(string input)
        {
            Logger.Debug("Parsing {0}", input);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            this.serializer.Input = stream;
            return this.serializer.Deserialize();
        }
    }
}