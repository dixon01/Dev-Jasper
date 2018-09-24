// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NormalPlayTime.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;

    /// <summary>
    /// The normal play time.
    /// </summary>
    public class NormalPlayTime
    {
        #region Constants and Fields

        private readonly bool isNow;

        private int fraction;

        private int seconds;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalPlayTime"/> class.
        /// </summary>
        /// <param name="npt">
        /// The play time string.
        /// </param>
        public NormalPlayTime(string npt)
        {
            if (npt.ToLower() == "now")
            {
                this.isNow = true;
                this.seconds = -1;
                this.fraction = -1;
            }
            else
            {
                string[] parts = npt.Split(':');
                if (parts.Length == 3)
                {
                    // npt-hhmmss format
                    this.ParseNptSec(parts[2]);
                    this.seconds += int.Parse(parts[1]) * 60;
                    this.seconds += int.Parse(parts[0]) * 3600;
                }
                else if (parts.Length == 1)
                {
                    // npt-sec format
                    this.ParseNptSec(parts[0]);
                }
                else
                {
                    throw new ArgumentException(npt + " is not a valid NPT.");
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Fraction.
        /// </summary>
        public int Fraction
        {
            get
            {
                return this.fraction;
            }
        }

        /// <summary>
        /// Gets Seconds.
        /// </summary>
        public int Seconds
        {
            get
            {
                return this.seconds;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The as npt hhmmss.
        /// </summary>
        /// <returns>
        /// The as npt hhmmss.
        /// </returns>
        public string AsNptHHMMSS()
        {
            return string.Format(
                "{0}:{1:2}:{2:2}.{3}", this.seconds / 3600, (this.seconds / 60) % 60, this.seconds % 60, this.fraction);
        }

        /// <summary>
        /// The as npt sec.
        /// </summary>
        /// <returns>
        /// The as npt sec.
        /// </returns>
        public string AsNptSec()
        {
            return string.Format("{0}.{1}", this.seconds, this.fraction);
        }

        /// <summary>
        /// The get 100 nano seconds.
        /// </summary>
        /// <returns>
        /// The get 100 nano seconds.
        /// </returns>
        public long Get100NanoSeconds()
        {
            double time = double.Parse(this.seconds + "." + this.fraction);
            return (long)(time * 10000000L);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            if (this.isNow)
            {
                return "now";
            }
            else
            {
                return this.seconds > 60 ? this.AsNptHHMMSS() : this.AsNptSec();
            }
        }

        #endregion

        #region Methods

        private void ParseNptSec(string nptSec)
        {
            string[] parts = nptSec.Split('.');
            this.seconds = int.Parse(parts[0]);
            if (parts.Length == 2)
            {
                this.fraction = int.Parse(parts[1]);
            }
            else if (parts.Length > 2)
            {
                throw new ArgumentException(nptSec + " is not a valid npt-sec.");
            }
        }

        #endregion
    }

    /// <summary>
    /// The normal play time range.
    /// </summary>
    public class NormalPlayTimeRange
    {
        #region Constants and Fields

        /// <summary>
        /// The all.
        /// </summary>
        public static readonly NormalPlayTimeRange ALL = new NormalPlayTimeRange("-");

        private readonly NormalPlayTime from;

        private readonly NormalPlayTime to;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalPlayTimeRange"/> class.
        /// </summary>
        /// <param name="nptRange">
        /// The npt range.
        /// </param>
        public NormalPlayTimeRange(string nptRange)
        {
            string[] parts = nptRange.Split('-');
            this.@from = this.ParseNpt(parts[0]);
            if (parts.Length > 1)
            {
                this.to = this.ParseNpt(parts[1]);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets From.
        /// </summary>
        public NormalPlayTime From
        {
            get
            {
                return this.@from;
            }
        }

        /// <summary>
        /// Gets To.
        /// </summary>
        public NormalPlayTime To
        {
            get
            {
                return this.to;
            }
        }

        #endregion

        #region Methods

        private NormalPlayTime ParseNpt(string npt)
        {
            npt = npt.Trim();
            if (npt.Length == 0)
            {
                return null;
            }
            else
            {
                return new NormalPlayTime(npt);
            }
        }

        #endregion
    }
}