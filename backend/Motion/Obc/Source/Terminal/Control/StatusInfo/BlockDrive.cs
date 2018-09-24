// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockDrive.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlockDrive type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The block drive information.
    /// </summary>
    internal class BlockDrive
    {
        private readonly DelayCalculator delayCalculator;

        private readonly DriveInfo driveInfo;

        private string errorDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDrive"/> class.
        /// </summary>
        /// <param name="driveInfo">
        /// The drive info.
        /// </param>
        public BlockDrive(DriveInfo driveInfo)
        {
            this.driveInfo = driveInfo;
            this.delayCalculator = new DelayCalculator(driveInfo, this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bus is driving.
        ///   false: -> show waiting screen
        ///   true: -> show driving screen
        /// </summary>
        public bool IsDriving { get; internal set; }

        /// <summary>
        /// Gets the error description.
        ///   If the return value is null, no error -> Trip and Block are loaded correctly.
        /// </summary>
        public string ErrorDescription
        {
            get
            {
                if (this.IsDataValid)
                {
                    return null;
                }

                return this.errorDescription;
            }
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber
        {
            get
            {
                if (RemoteEventHandler.CurrentService != null)
                {
                    // TODO: check if we can use the right property here
                    return RemoteEventHandler.CurrentService.Line;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the block number (<c>Umlaufnummer</c>).
        /// </summary>
        public int BlockNumber
        {
            get
            {
                if (RemoteEventHandler.CurrentService != null)
                {
                    return RemoteEventHandler.CurrentService.Umlauf;
                }

                return 0;
            }
        }

        /// <summary>
        ///   Gets the departure time from the start! It's not from the current stop. Only use it if isDriving is false.
        ///   By driving is or at a bus stop by driving, check the delay to know the departure time.
        /// </summary>
        public DateTime DepartureTime
        {
            get
            {
                if (RemoteEventHandler.CurrentTrip != null)
                {
                    return RemoteEventHandler.CurrentTrip.DateTimeStart;
                }

                return TimeProvider.Current.Now;
            }
        }

        /// <summary>
        ///   Gets the delay in seconds.
        ///   Only valid if isDriving is true
        ///   Positive: Bus has a delay -> too late
        ///   Negative: Bus is in advanced
        /// </summary>
        public int Delay
        {
            get
            {
                return this.delayCalculator.GetDelay();
            }
        }

        /// <summary>
        ///   Gets the name of the first stop. It's the station name of the start from this block
        /// </summary>
        public string StartName
        {
            get
            {
                return this.GetStopName(0);
            }
        }

        /// <summary>
        /// Gets the last stop name.
        /// </summary>
        public string LastName
        {
            get
            {
                if (this.IsTripValid)
                {
                    return this.GetStopName(RemoteEventHandler.CurrentTrip.Stop.Count - 1);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current trip is valid.
        /// </summary>
        internal bool IsTripValid
        {
            get
            {
                if (RemoteEventHandler.CurrentTrip != null)
                {
                    if (RemoteEventHandler.CurrentTrip.Stop != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private bool IsDataValid
        {
            get
            {
                if (RemoteEventHandler.CurrentService != null)
                {
                    if (RemoteEventHandler.CurrentTrip != null)
                    {
                        if (RemoteEventHandler.CurrentTrip.Stop != null)
                        {
                            this.errorDescription = null;
                            return true;
                        }

                        this.errorDescription = ml.ml_string(2, "No stop list available");
                    }
                    else
                    {
                        this.errorDescription = ml.ml_string(3, "No trip loaded");
                    }
                }
                else
                {
                    this.errorDescription = ml.ml_string(4, "No block loaded");
                }

                return false;
            }
        }

        /// <summary>
        /// Will reset this object.
        /// Hint: Trip and Service information are loaded from the EventHandler Object. This data will not be cleared
        /// </summary>
        public void Reset()
        {
            this.errorDescription = null;
            this.IsDriving = false;
        }

        /// <summary>
        /// Gets the next stop name by offset.
        /// </summary>
        /// <param name="offset">
        /// The offset starting from 0; offset = 0 means the next stop
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetNextStop(int offset)
        {
            return this.GetStopName(this.driveInfo.StopIdCurrent + offset);
        }

        private string GetStopName(int stopIdx)
        {
            if (this.IsTripValid)
            {
                if (stopIdx >= RemoteEventHandler.CurrentTrip.Stop.Count)
                {
                    // BlockDrivingScreen (au moins) appelle cette fonction avec +1;+2 codé en dur
                    // donc il faut retourner null ...
                    return null;
                }

                // WES: Really???
                if (LanguageManager.Instance.CurrentLanguage.Name == "fr"
                    && RemoteEventHandler.CurrentTrip.Stop[stopIdx].Name2.Length > 0)
                {
                    return RemoteEventHandler.CurrentTrip.Stop[stopIdx].Name2;
                }

                return RemoteEventHandler.CurrentTrip.Stop[stopIdx].Name1;
            }

            return null;
        }
    }
}