
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Gorba.Common.Utility.Compatibility;

namespace Gorba.Common.Protocols.Alarming
{
    public static class SystemAlarmFactory
    {
        public static Alarm CreateRestart(SystemRestartAttribute attr)
        {
            return CreateRestart(attr, null);
        }

        public static Alarm CreateRestart(SystemRestartAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.System;
            alarm.Type = AlarmType.SystemRestart;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case SystemRestartAttribute.SoftwareUpdate:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case SystemRestartAttribute.User:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case SystemRestartAttribute.PowerLoss:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case SystemRestartAttribute.BatteryLow:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case SystemRestartAttribute.Unknown:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case SystemRestartAttribute.Timer:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case SystemRestartAttribute.MemoryLow:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case SystemRestartAttribute.TimeModified:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateFileSystem(SystemFileSystemAttribute attr)
        {
            return CreateFileSystem(attr, null);
        }

        public static Alarm CreateFileSystem(SystemFileSystemAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.System;
            alarm.Type = AlarmType.SystemFileSystem;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case SystemFileSystemAttribute.EepromError:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case SystemFileSystemAttribute.FlashError:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateTest(SystemTestAttribute attr)
        {
            return CreateTest(attr, null);
        }

        public static Alarm CreateTest(SystemTestAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.System;
            alarm.Type = AlarmType.SystemTest;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case SystemTestAttribute.Test:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateLedPanel(SystemLedPanelAttribute attr)
        {
            return CreateLedPanel(attr, null);
        }

        public static Alarm CreateLedPanel(SystemLedPanelAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.System;
            alarm.Type = AlarmType.SystemLedPanel;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case SystemLedPanelAttribute.Error:
                    alarm.Severity = AlarmSeverity.Severe;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateConfig(SystemConfigAttribute attr)
        {
            return CreateConfig(attr, null);
        }

        public static Alarm CreateConfig(SystemConfigAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.System;
            alarm.Type = AlarmType.SystemConfig;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case SystemConfigAttribute.ConfigChanged:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case SystemConfigAttribute.SetupChanged:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateDcmIO(SystemDcmIOAttribute attr)
        {
            return CreateDcmIO(attr, null);
        }

        public static Alarm CreateDcmIO(SystemDcmIOAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.System;
            alarm.Type = AlarmType.SystemDcmIO;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case SystemDcmIOAttribute.Error:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case SystemDcmIOAttribute.OK:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }
    }

    public static class ApplicationAlarmFactory
    {
        public static Alarm CreateRelaunch(ApplicationRelaunchAttribute attr)
        {
            return CreateRelaunch(attr, null);
        }

        public static Alarm CreateRelaunch(ApplicationRelaunchAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Application;
            alarm.Type = AlarmType.ApplicationRelaunch;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case ApplicationRelaunchAttribute.Unknown:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case ApplicationRelaunchAttribute.SystemBoot:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case ApplicationRelaunchAttribute.SoftwareUpdate:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case ApplicationRelaunchAttribute.User:
                    alarm.Severity = AlarmSeverity.Critical;
                    break;
                case ApplicationRelaunchAttribute.MemoryLow:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case ApplicationRelaunchAttribute.CpuExcess:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case ApplicationRelaunchAttribute.DiskFull:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case ApplicationRelaunchAttribute.Watchdog:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }
    }

    public static class FuncAlarmFactory
    {
        public static Alarm CreateBatteryTemperature(FuncBatteryTemperatureAttribute attr)
        {
            return CreateBatteryTemperature(attr, null);
        }

        public static Alarm CreateBatteryTemperature(FuncBatteryTemperatureAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncBatteryTemperature;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncBatteryTemperatureAttribute.Level1:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case FuncBatteryTemperatureAttribute.Level2:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case FuncBatteryTemperatureAttribute.Level3:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateBatteryEndOfLife(FuncBatteryEndOfLifeAttribute attr)
        {
            return CreateBatteryEndOfLife(attr, null);
        }

        public static Alarm CreateBatteryEndOfLife(FuncBatteryEndOfLifeAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncBatteryEndOfLife;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncBatteryEndOfLifeAttribute.EndOfLife:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateMainsPowerFaillure(FuncMainsPowerFaillureAttribute attr)
        {
            return CreateMainsPowerFaillure(attr, null);
        }

        public static Alarm CreateMainsPowerFaillure(FuncMainsPowerFaillureAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncMainsPowerFaillure;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncMainsPowerFaillureAttribute.Error:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case FuncMainsPowerFaillureAttribute.OK:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateDoorStatus(FuncDoorStatusAttribute attr)
        {
            return CreateDoorStatus(attr, null);
        }

        public static Alarm CreateDoorStatus(FuncDoorStatusAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncDoorStatus;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncDoorStatusAttribute.Open:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case FuncDoorStatusAttribute.Closed:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreatePowerMode(FuncPowerModeAttribute attr)
        {
            return CreatePowerMode(attr, null);
        }

        public static Alarm CreatePowerMode(FuncPowerModeAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncPowerMode;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncPowerModeAttribute.Initializing:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case FuncPowerModeAttribute.Full:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case FuncPowerModeAttribute.Reduced:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case FuncPowerModeAttribute.Minimal:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case FuncPowerModeAttribute.NoPower:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case FuncPowerModeAttribute.Off:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case FuncPowerModeAttribute.Manual:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                case FuncPowerModeAttribute.Shutdown:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateTractionFailure(FuncTractionFailureAttribute attr)
        {
            return CreateTractionFailure(attr, null);
        }

        public static Alarm CreateTractionFailure(FuncTractionFailureAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncTractionFailure;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncTractionFailureAttribute.Error:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case FuncTractionFailureAttribute.OK:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateInverterFailure(FuncInverterFailureAttribute attr)
        {
            return CreateInverterFailure(attr, null);
        }

        public static Alarm CreateInverterFailure(FuncInverterFailureAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncInverterFailure;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncInverterFailureAttribute.Error:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case FuncInverterFailureAttribute.OK:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateLowBattery(FuncLowBatteryAttribute attr)
        {
            return CreateLowBattery(attr, null);
        }

        public static Alarm CreateLowBattery(FuncLowBatteryAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncLowBattery;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncLowBatteryAttribute.Error:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case FuncLowBatteryAttribute.OK:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateVandalism(FuncVandalismAttribute attr)
        {
            return CreateVandalism(attr, null);
        }

        public static Alarm CreateVandalism(FuncVandalismAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncVandalism;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncVandalismAttribute.Vandalism:
                    alarm.Severity = AlarmSeverity.Severe;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateButtons(FuncButtonsAttribute attr)
        {
            return CreateButtons(attr, null);
        }

        public static Alarm CreateButtons(FuncButtonsAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncButtons;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncButtonsAttribute.TtsKeyPressed:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateSurgeProtection(FuncSurgeProtectionAttribute attr)
        {
            return CreateSurgeProtection(attr, null);
        }

        public static Alarm CreateSurgeProtection(FuncSurgeProtectionAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncSurgeProtection;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncSurgeProtectionAttribute.Active:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case FuncSurgeProtectionAttribute.Inactive:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateTimeSync(FuncTimeSyncAttribute attr)
        {
            return CreateTimeSync(attr, null);
        }

        public static Alarm CreateTimeSync(FuncTimeSyncAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Func;
            alarm.Type = AlarmType.FuncTimeSync;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case FuncTimeSyncAttribute.LowThreshold:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }
    }

    public static class OperationAlarmFactory
    {
        public static Alarm CreateMasterData(OperationMasterDataAttribute attr)
        {
            return CreateMasterData(attr, null);
        }

        public static Alarm CreateMasterData(OperationMasterDataAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Operation;
            alarm.Type = AlarmType.OperationMasterData;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case OperationMasterDataAttribute.Route:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case OperationMasterDataAttribute.Trip:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case OperationMasterDataAttribute.Calendar:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case OperationMasterDataAttribute.Points:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case OperationMasterDataAttribute.Led:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case OperationMasterDataAttribute.Tts:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                case OperationMasterDataAttribute.Line:
                    alarm.Severity = AlarmSeverity.Error;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateTripStateUnknown(OperationTripStateUnknownAttribute attr)
        {
            return CreateTripStateUnknown(attr, null);
        }

        public static Alarm CreateTripStateUnknown(OperationTripStateUnknownAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Operation;
            alarm.Type = AlarmType.OperationTripStateUnknown;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case OperationTripStateUnknownAttribute.StateUnknown:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateTripStateLeft(OperationTripStateLeftAttribute attr)
        {
            return CreateTripStateLeft(attr, null);
        }

        public static Alarm CreateTripStateLeft(OperationTripStateLeftAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Operation;
            alarm.Type = AlarmType.OperationTripStateLeft;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case OperationTripStateLeftAttribute.Early:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case OperationTripStateLeftAttribute.Late:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case OperationTripStateLeftAttribute.Skipped:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case OperationTripStateLeftAttribute.Aborted:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }

        public static Alarm CreateVdvDataMissing(OperationVdvDataMissingAttribute attr)
        {
            return CreateVdvDataMissing(attr, null);
        }

        public static Alarm CreateVdvDataMissing(OperationVdvDataMissingAttribute attr, string message)
        {
            var alarm = new Alarm();
            alarm.Unit = ApplicationHelper.MachineName;
            alarm.Category = AlarmCategory.Operation;
            alarm.Type = AlarmType.OperationVdvDataMissing;
            alarm.Attribute = (int)attr;
            alarm.Message = message;

            switch (attr)
            {
                case OperationVdvDataMissingAttribute.Ref:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                case OperationVdvDataMissingAttribute.Prc:
                    alarm.Severity = AlarmSeverity.Warning;
                    break;
                default:
                    alarm.Severity = AlarmSeverity.Info;
                    break;
            }

            return alarm;
        }
    }
}