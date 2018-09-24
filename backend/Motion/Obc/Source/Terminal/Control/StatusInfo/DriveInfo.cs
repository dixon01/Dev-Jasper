// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    ///   Drive Information
    /// </summary>
    internal class DriveInfo : IDriveInfo
    {
        private const int BLength = 6;

        private const int LineLength = 4;

        private const int RuLength = 2;

        private const int RpLength = 4;

        private const int ZLength = 4;

        private const int DLength = 4;

        private const int DrLength = 6;

        private static readonly Logger Logger = LogHelper.GetLogger<DriveInfo>();

        private readonly IContext context;

        private bool detourActive;

        private DriveType driveType = DriveType.None;

        private int driverNumber; // Driver number. will be sent out with evDriver

        private bool isAdditionalDrive;

        private bool isAtBusStop = true;

        private bool isDrivingSchool;

        private bool razziaEnabled;

        private string specialDestinationName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveInfo"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public DriveInfo(IContext context)
        {
            this.context = context;
            this.StopLeftTime = TimeProvider.Current.Now;
            this.BlockDrive = new BlockDrive(this);
            MessageDispatcher.Instance.Subscribe<evBUSStopReached>(this.EvBusStopReachedEvent);
            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.EvBusStopLeftEvent);
            MessageDispatcher.Instance.Subscribe<evZoneChanged>(this.EvZoneChangedEvent);
            MessageDispatcher.Instance.Subscribe<evTripEnded>(this.EvTripEndedEvent);

            MessageDispatcher.Instance.Broadcast(new evSetService(0));
            MessageDispatcher.Instance.Broadcast(new evServiceEnded());
            MessageDispatcher.Instance.Broadcast(new ExtraService(0, 0));
        }

        /// <summary>
        /// Event that is risen every time the <see cref="IsAdditionalDrive"/> changes.
        /// </summary>
        public event EventHandler IsAdditionalDriveChanged;

        /// <summary>
        /// Event that is risen every time the <see cref="IsDrivingSchool"/> changes.
        /// </summary>
        public event EventHandler IsDrivingSchoolChanged;

        /// <summary>
        /// Event that is risen every time the <see cref="DriveType"/> changes.
        /// </summary>
        public event EventHandler DriveTypeChanged;

        /// <summary>
        /// Gets the zone number.
        /// </summary>
        public int ZoneNumber { get; private set; }

        /// <summary>
        /// Gets or sets the driver block.
        /// </summary>
        public string DriverBlock { get; set; }

        /// <summary>
        /// Gets the stop left time.
        /// </summary>
        public DateTime StopLeftTime { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether the bus is at a bus stop/station.
        ///   Default value is true
        ///   true: Bus is currently at a bus stop. Countdown window will be shown
        ///   false: Bus is driving
        /// </summary>
        public bool IsAtBusStop
        {
            get
            {
                if (this.BlockDrive.IsDriving == false)
                {
                    return false;
                }

                return this.isAtBusStop;
            }

            private set
            {
                this.isAtBusStop = value;
            }
        }

        /// <summary>
        /// Gets the stop id left.
        /// </summary>
        public int StopIdLeft { get; private set; }

        /// <summary>
        /// Gets the stop id current.
        /// </summary>
        public int StopIdCurrent { get; private set; }

        /// <summary>
        ///   Gets or sets the current drive type. This is managed by the current state
        /// </summary>
        public DriveType DriveType
        {
            get
            {
                return this.driveType;
            }

            set
            {
                this.driveType = value;
                this.RaiseDriveTypeChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is additional drive.
        /// </summary>
        public bool IsAdditionalDrive
        {
            get
            {
                return this.isAdditionalDrive;
            }

            set
            {
                this.isAdditionalDrive = value;
                this.RaiseIsAdditionalDriveChanged();
            }
        }

        /// <summary>
        /// Gets or sets the driver pin.
        /// </summary>
        public int DriverPin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is driving school.
        /// </summary>
        public bool IsDrivingSchool
        {
            get
            {
                return this.isDrivingSchool;
            }

            set
            {
                this.isDrivingSchool = value;
                this.RaiseIsDrivingSchoolChanged();
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether we are in Razzia mode.
        ///   By setting the razzia an event will be produced
        /// </summary>
        public bool RazziaEnabled
        {
            get
            {
                return this.razziaEnabled;
            }

            set
            {
                this.razziaEnabled = value;
                if (value)
                {
                    MessageDispatcher.Instance.Broadcast(new evRazziaStart());
                }
                else
                {
                    MessageDispatcher.Instance.Broadcast(new evRazziaEnded());
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether detour active.
        /// </summary>
        public bool DetourActive
        {
            get
            {
                return this.detourActive;
            }

            set
            {
                this.detourActive = value;
                if (value)
                {
                    MessageDispatcher.Instance.Broadcast(new evDeviationStarted());
                }
                else
                {
                    MessageDispatcher.Instance.Broadcast(new evDeviationEnded());
                }
            }
        }

        /// <summary>
        /// Gets the destination code.
        /// </summary>
        public int DestinationCode
        {
            get
            {
                if (RemoteEventHandler.CurrentExtraService != null)
                {
                    return RemoteEventHandler.CurrentExtraService.DestinationCode;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the block number.
        /// </summary>
        public int BlockNumber
        {
            get
            {
                if (RemoteEventHandler.CurrentExtraService != null)
                {
                    return RemoteEventHandler.CurrentExtraService.Service;
                }

                return this.BlockDrive.BlockNumber;
            }
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return this.BlockDrive.LineNumber;
            }
        }

        /// <summary>
        /// Gets the run number.
        /// </summary>
        public int RunNumber
        {
            get
            {
                if (RemoteEventHandler.CurrentExtraService != null)
                {
                    return RemoteEventHandler.CurrentExtraService.Service;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the route path number.
        /// </summary>
        public int RoutePathNumber
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the destination number.
        /// </summary>
        [XmlIgnore]
        public int DestinationNumber
        {
            get
            {
                try
                {
                    if (this.DriveType == DriveType.SpecialDestination)
                    {
                        if (RemoteEventHandler.CurrentExtraService != null)
                        {
                            return RemoteEventHandler.CurrentExtraService.DestinationCode;
                        }
                    }
                    else if (this.DriveType == DriveType.Block)
                    {
                        if (RemoteEventHandler.CurrentTrip != null)
                        {
                            return RemoteEventHandler.CurrentTrip.Stop.Get(this.StopIdCurrent).SignCode;
                            ////return RemoteEventHandler.CurrentTrip.Stop[StopIDCurrent].Ziel;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't retrieve the destination number", ex);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the driver number.
        /// </summary>
        public int DriverNumber
        {
            get
            {
                return this.driverNumber;
            }

            internal set
            {
                CheckLength(value, DrLength);
                this.driverNumber = value;
                MessageDispatcher.Instance.Broadcast(new evDriver(this.driverNumber));
            }
        }

        /// <summary>
        /// Gets the zone number.
        /// </summary>
        public string SZoneNumber
        {
            get
            {
                return ValueToString(this.ZoneNumber, ZLength);
            }
        }

        /// <summary>
        /// Gets the route path number.
        /// </summary>
        public string SRoutePathNumber
        {
            get
            {
                return ValueToString(this.RoutePathNumber, RpLength);
            }
        }

        /// <summary>
        /// Gets the run number.
        /// </summary>
        public string SRunNumber
        {
            get
            {
                return ValueToString(this.RunNumber, RuLength);
            }
        }

        /// <summary>
        /// Gets the block number.
        /// </summary>
        public string SBlockNumber
        {
            get
            {
                // PR : modifié pour afficher le code agent lorsque applicable
                if (this.driveType == DriveType.Block && this.BlockNumber != 0
                    && !string.IsNullOrEmpty(this.DriverBlock))
                {
                    return this.DriverBlock.PadLeft(BLength);
                }

                return ValueToString(this.BlockNumber, BLength);
            }
        }

        /// <summary>
        /// Gets the line name.
        /// </summary>
        public string SLineName
        {
            get
            {
                // PR retournait le code iqube
                // changé en code customer pour l'affichage dans la statusbar
                // return ValueToString(this.LineNumber, LineLength);
                if (RemoteEventHandler.CurrentTrip != null)
                {
                    return RemoteEventHandler.CurrentTrip.LineName.PadLeft(LineLength);
                }

                return ValueToString(0, LineLength);
            }
        }

        /// <summary>
        /// Gets the destination number.
        /// </summary>
        public string SDestinationNumber
        {
            get
            {
                return ValueToString(this.DestinationNumber, DLength);
            }
        }

        /// <summary>
        /// Gets the driver number.
        /// </summary>
        public string SDriverNumber
        {
            get
            {
                return ValueToString(this.driverNumber, DrLength);
            }
        }

        /// <summary>
        /// Gets the block drive.
        /// </summary>
        public BlockDrive BlockDrive { get; private set; }

        /// <summary>
        /// Gets the duty event options.
        /// </summary>
        internal evDuty.Options EvDutyOptions
        {
            get
            {
                var options = evDuty.Options.None;
                if (this.IsDrivingSchool)
                {
                    options = evDuty.Options.School;
                }

                if (this.IsAdditionalDrive)
                {
                    options |= evDuty.Options.Extension;
                }

                return options;
            }
        }

        /// <summary>
        /// Gets the destination text.
        /// </summary>
        internal string DestinationText
        {
            get
            {
                switch (this.driveType)
                {
                    case DriveType.Block:
                        return this.BlockDrive.LastName;
                    case DriveType.SpecialDestination:
                        return this.specialDestinationName;
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        ///   Clears the drive info. This will not affect the driver ID
        /// </summary>
        public void ClearDrive()
        {
            this.DriveType = DriveType.None;
            this.specialDestinationName = string.Empty;
            this.StopIdLeft = 0;
            this.StopIdCurrent = 0;
            this.IsDrivingSchool = false;
            this.IsAdditionalDrive = false;
            this.IsAtBusStop = true;
            this.ZoneNumber = 0;
            this.BlockDrive.Reset();

            MessageDispatcher.Instance.Broadcast(new evSetService(0));
            MessageDispatcher.Instance.Broadcast(new evServiceEnded());
            MessageDispatcher.Instance.Broadcast(new ExtraService(0, 0));
            MessageDispatcher.Instance.Broadcast(
                new evDuty(
                    this.DriverNumber.ToString(CultureInfo.InvariantCulture),
                    "0",
                    "-1",
                    evDuty.Types.DutyOff,
                    evDuty.Options.None));

            this.context.MessageHandler.SetDestinationText(this.DestinationText);
        }

        /// <summary>
        ///   Sets the destination text for special destination drive
        /// </summary>
        /// <param name = "destName">
        /// The special destination name
        /// </param>
        public void SetSpecialDestText(string destName)
        {
            this.specialDestinationName = destName;
        }

        /// <summary>
        /// Clears all data.
        /// </summary>
        public void ClearAll()
        {
            this.DriverNumber = 0;
            this.ClearDrive();
        }

        /// <summary>
        /// Raises the <see cref="IsAdditionalDriveChanged"/> event.
        /// </summary>
        protected virtual void RaiseIsAdditionalDriveChanged()
        {
            var handler = this.IsAdditionalDriveChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="IsDrivingSchoolChanged"/> event.
        /// </summary>
        protected virtual void RaiseIsDrivingSchoolChanged()
        {
            var handler = this.IsDrivingSchoolChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="DriveTypeChanged"/> event.
        /// </summary>
        protected virtual void RaiseDriveTypeChanged()
        {
            var handler = this.DriveTypeChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///   If the number digits is more than maxDigits -> throws an ArgumentException
        /// </summary>
        /// <param name = "number">Number to check</param>
        /// <param name = "maxDigits">The maximum digits which are allowed</param>
        private static void CheckLength(int number, int maxDigits)
        {
            if (number.ToString(CultureInfo.InvariantCulture).Length > maxDigits)
            {
                throw new ArgumentException(
                    string.Format("Number too big. Value is: {0} max length is {1}", number, maxDigits));
            }
        }

        private static string ValueToString(int value, int length)
        {
            switch (length)
            {
                case 1:
                    return value.ToString("0");
                case 2:
                    return value.ToString("00");
                case 3:
                    return value.ToString("000");
                case 4:
                    return value.ToString("0000");
                case 5:
                    return value.ToString("00000");
                case 6:
                    return value.ToString("000000");
                default:
                    return value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void EvTripEndedEvent(object sender, MessageEventArgs<evTripEnded> e)
        {
            this.StopIdLeft = 0;
            this.StopIdCurrent = 0;
            this.BlockDrive.IsDriving = false;
        }

        private void EvZoneChangedEvent(object sender, MessageEventArgs<evZoneChanged> e)
        {
            Logger.Info("Zone changed event received: {0}", e.Message.ZoneId);
            this.ZoneNumber = e.Message.ZoneId;
        }

        private void EvBusStopReachedEvent(object sender, MessageEventArgs<evBUSStopReached> e)
        {
            this.StopIdCurrent = e.Message.StopId;
            this.IsAtBusStop = true;
        }

        private void EvBusStopLeftEvent(object sender, MessageEventArgs<evBUSStopLeft> e)
        {
            this.StopIdLeft = e.Message.StopId;
            this.StopIdCurrent = e.Message.StopId + 1;
            this.BlockDrive.IsDriving = true;
            this.IsAtBusStop = false;
            this.StopLeftTime = TimeProvider.Current.Now;
        }
    }
}