// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    using System;
    using System.IO;
    using System.Text;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The status handler.
    /// </summary>
    internal class StatusHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<StatusHandler>();

        private readonly IContext context;

        private readonly string savedStatusPath;

        private readonly ITimer updateTimer;

        private DriveInfo driveInfo;
        private GpsInfo gpsInfo;
        private StatusFieldConfig statusFieldConfig;

        private bool updateGps; // only used for speed optimization

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusHandler"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal StatusHandler(IContext context)
        {
            this.context = context;
            this.savedStatusPath = PathManager.Instance.CreatePath(FileType.Data, ConfigPaths.SavedTmpStatus);

            this.updateTimer = TimerFactory.Current.CreateTimer("StatusHandler");
            this.updateTimer.Interval = TimeSpan.FromSeconds(1);
            this.updateTimer.AutoReset = true;
            this.updateTimer.Elapsed += (s, e) => this.Update();

            this.Init();
            this.DriveInfo = new DriveInfo(context);
            this.GpsInfo = new GpsInfo("0", "0", "N"); // MLHIDE
        }

        /// <summary>
        /// Gets the drive info.
        /// </summary>
        public DriveInfo DriveInfo
        {
            get
            {
                return this.driveInfo;
            }

            private set
            {
                this.driveInfo = value;
                this.context.UiRoot.MainStatus.Update(value);
                ////Update();
            }
        }

        /// <summary>
        /// Gets the GPS info.
        /// </summary>
        public GpsInfo GpsInfo
        {
            get
            {
                return this.gpsInfo;
            }

            private set
            {
                this.gpsInfo = value;
                this.context.UiRoot.MainStatus.Update(value);
                if (this.updateGps)
                {
                    this.Update();
                }
            }
        }

        /// <summary>
        /// Loads the saved status.
        /// </summary>
        public void LoadSavedStatus()
        {
            try
            {
                if (File.Exists(this.savedStatusPath) == false)
                {
                    // Nothing to load
                    return;
                }

                var ser = new Configurator(this.savedStatusPath);
                var savedStatus = ser.Deserialize<SavedStatus>();
                if (savedStatus == null)
                {
                    this.DeleteSavedStatus();
                    return;
                }

                if (savedStatus.IsValid == false)
                {
                    this.DeleteSavedStatus();
                    return;
                }

                LanguageManager.Instance.CurrentLanguage = LanguageManager.Instance.GetLanguage(savedStatus.Language);
                DriveInfo di = this.context.StatusHandler.DriveInfo;
                di.IsAdditionalDrive = savedStatus.IsAdditionalDrive;
                di.IsDrivingSchool = savedStatus.IsDrivingSchool;
                di.DriverNumber = savedStatus.DriverNumber;

                if (savedStatus.BlockNumber != 0)
                {
                    // TODO: make sure this is not required anymore
                    ////Thread.Sleep(5000);
                    // on attend que les autres process soient lancés avant de restaurer l'état précédent
                    new BlockDriveLoad(this.context).LoadBlockDrive(
                        savedStatus.BlockNumber,
                        ml.ml_string(138, "Load saved status"));
                }
                else if (savedStatus.SpecDestServiceNbr != 0)
                {
                    // TODO: make sure this is not required anymore
                    ////Thread.Sleep(5000);
                    // on attend que les autres process soient lancés avant de restaurer l'état précédent
                    new SpecialDestinationLoad(this.context).LoadSpecialDestDrive(
                        savedStatus.SpecDestServiceNbr,
                        savedStatus.SpecDestCode,
                        savedStatus.SpecDestName,
                        ml.ml_string(138, "Load saved status"));
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't load saved status", ex);
            }
        }

        /// <summary>
        /// Saves the current status.
        /// </summary>
        public void SaveStatus()
        {
            try
            {
                this.DeleteSavedStatus();

                // TODO: replace with persistence
                var status = new SavedStatus();
                if (this.driveInfo.DriveType == DriveType.Block)
                {
                    status.BlockNumber = this.driveInfo.BlockNumber;
                    status.SpecDestCode = 0;
                    status.SpecDestServiceNbr = 0;
                    status.SpecDestName = string.Empty;
                }
                else if (this.driveInfo.DriveType == DriveType.SpecialDestination)
                {
                    status.BlockNumber = 0;
                    status.SpecDestCode = this.driveInfo.DestinationCode;
                    status.SpecDestServiceNbr = this.driveInfo.RunNumber;
                    status.SpecDestName = this.driveInfo.DestinationText;
                }
                else
                {
                    return; // Not logged in, no current drive active -> nothing to save
                }

                status.SavedTimeUtc = TimeProvider.Current.UtcNow;
                status.IsDrivingSchool = this.driveInfo.IsDrivingSchool;
                status.IsAdditionalDrive = this.driveInfo.IsAdditionalDrive;
                status.DriverNumber = this.driveInfo.DriverNumber;

                var configurator =
                    new Configurator(this.savedStatusPath);
                configurator.Serialize(status);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't save status", ex);
            }
        }

        /// <summary>
        /// Deletes the saved status.
        /// </summary>
        public void DeleteSavedStatus()
        {
            File.Delete(this.savedStatusPath);
        }

        private void Init()
        {
            var manager = new ConfigManager<StatusFieldConfig>();
            manager.FileName = PathManager.Instance.GetPath(FileType.Config, ConfigPaths.StatusConfig);
            manager.EnableCaching = true;
            this.statusFieldConfig = manager.Config;

            if (Array.IndexOf(this.statusFieldConfig.Fields, StatusFieldConfig.StatusFieldType.GpsStatus) != -1)
            {
                this.updateGps = true;
            }

            this.updateTimer.Enabled = true;
        }

        private void Update()
        {
            var sb = new StringBuilder();
            if (this.statusFieldConfig.Fields[3] != StatusFieldConfig.StatusFieldType.None)
            {
                sb.Append(
                          this.statusFieldConfig.GetShortName(this.statusFieldConfig.Fields[3]) +
                          ":" + this.GetValueString(this.statusFieldConfig.Fields[3])); // MLHIDE
            }

            if (this.statusFieldConfig.Fields[2] != StatusFieldConfig.StatusFieldType.None)
            {
                sb.Append(
                          " " + this.statusFieldConfig.GetShortName(this.statusFieldConfig.Fields[2]) + // MLHIDE
                          ":" + this.GetValueString(this.statusFieldConfig.Fields[2])); // MLHIDE
            }

            if (this.statusFieldConfig.Fields[1] != StatusFieldConfig.StatusFieldType.None)
            {
                sb.Append(
                          " " + this.statusFieldConfig.GetShortName(this.statusFieldConfig.Fields[1]) + // MLHIDE
                          ":" + this.GetValueString(this.statusFieldConfig.Fields[1])); // MLHIDE
            }

            if (this.statusFieldConfig.Fields[0] != StatusFieldConfig.StatusFieldType.None)
            {
                sb.Append(
                          " " + this.statusFieldConfig.GetShortName(this.statusFieldConfig.Fields[0]) + // MLHIDE
                          ":" + this.GetValueString(this.statusFieldConfig.Fields[0])); // MLHIDE
            }

            this.context.UiRoot.StatusField.SetMessage(sb.ToString());
        }

        private string GetValueString(StatusFieldConfig.StatusFieldType type)
        {
            switch (type)
            {
                case StatusFieldConfig.StatusFieldType.BlockNumber:
                    return this.driveInfo.SBlockNumber;
                case StatusFieldConfig.StatusFieldType.DestinationNumber:
                    return this.driveInfo.SDestinationNumber;
                case StatusFieldConfig.StatusFieldType.DriverNumber:
                    return this.driveInfo.SDriverNumber;
                case StatusFieldConfig.StatusFieldType.GpsStatus:
                    return this.gpsInfo.SIsGpsValid;
                case StatusFieldConfig.StatusFieldType.None:
                    return string.Empty;
                case StatusFieldConfig.StatusFieldType.RouteNumber:
                    return this.driveInfo.SLineName;
                case StatusFieldConfig.StatusFieldType.RoutePathNumber:
                    return this.driveInfo.SRoutePathNumber;
                case StatusFieldConfig.StatusFieldType.RunNumber:
                    return this.driveInfo.SRunNumber;
                case StatusFieldConfig.StatusFieldType.ZoneNumber:
                    return this.driveInfo.SZoneNumber;
                default:
                    return "Unknown type: " + type; // MLHIDE
            }
        }
    }
}