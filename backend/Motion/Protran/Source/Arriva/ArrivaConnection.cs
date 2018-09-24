// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrivaConnection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Container of the information about a "connection"
//   coming from Arriva.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva
{
    /// <summary>
    /// Container of the information about a "connection"
    /// coming from Arriva.
    /// </summary>
    public class ArrivaConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaConnection"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        public ArrivaConnection(int type, string time, string destination, string line, string platform)
        {
            this.Type = type;
            this.Time = time;
            this.Destination = destination;
            this.Line = line;
            this.Platform = platform;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrivaConnection"/> class.
        /// </summary>
        /// <param name="refStop">
        /// The ref stop.
        /// </param>
        /// <param name="refStopArrivalTime">
        /// The ref stop arrival time.
        /// </param>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="delay">
        /// The delay.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="pto">
        /// The pto.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="stopCode">
        /// The stop code.
        /// </param>
        public ArrivaConnection(
            string refStop,
            string refStopArrivalTime,
            string time,
            string delay,
            string platform,
            string destination,
            string pto,
            string line,
            string stopCode)
        {
            this.RefStop = refStop;
            this.RefStopArrivalTime = refStopArrivalTime;
            this.Time = time;
            this.Delay = delay;
            this.Platform = platform;
            this.Destination = destination;
            this.Pto = pto;
            this.Line = line;
            this.StopCode = stopCode;
        }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets Time.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Gets or sets Destination.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets Line.
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// Gets or sets Platform.
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets RefStop.
        /// </summary>
        public string RefStop { get; set; }

        /// <summary>
        /// Gets or sets refStopArrivalTime.
        /// </summary>
        public string RefStopArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets Delay.
        /// </summary>
        public string Delay { get; set; }

        /// <summary>
        /// Gets or sets Pto.
        /// </summary>
        public string Pto { get; set; }

        /// <summary>
        /// Gets or sets StopCode.
        /// </summary>
        public string StopCode { get; set; }
    }
}