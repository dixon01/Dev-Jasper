
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Gorba.Common.Protocols.Alarming
{
    public enum AlarmCategory
    {
        System,
        Application,
        Func,
        Operation,
    }
    
    public enum AlarmType
    {
        SystemRestart,
        SystemFileSystem,
        SystemTest,
        SystemLedPanel,
        SystemConfig,
        SystemDcmIO,
        ApplicationRelaunch,
        FuncBatteryTemperature,
        FuncBatteryEndOfLife,
        FuncMainsPowerFaillure,
        FuncDoorStatus,
        FuncPowerMode,
        FuncTractionFailure,
        FuncInverterFailure,
        FuncLowBattery,
        FuncVandalism,
        FuncButtons,
        FuncSurgeProtection,
        FuncTimeSync,
        OperationMasterData,
        OperationTripStateUnknown,
        OperationTripStateLeft,
        OperationVdvDataMissing,
    }

    public enum SystemRestartAttribute
    {
        SoftwareUpdate = 1,
        User = 2,
        PowerLoss = 3,
        BatteryLow = 4,
        Unknown = 5,
        Timer = 6,
        MemoryLow = 7,
        TimeModified = 8,
    }

    public enum SystemFileSystemAttribute
    {
        EepromError = 0,
        FlashError = 1,
    }

    public enum SystemTestAttribute
    {
        Test,
    }

    public enum SystemLedPanelAttribute
    {
        Error,
    }

    public enum SystemConfigAttribute
    {
        ConfigChanged,
        SetupChanged,
    }

    public enum SystemDcmIOAttribute
    {
        Error = 1,
        OK = 2,
    }

    public enum ApplicationRelaunchAttribute
    {
        Unknown,
        SystemBoot,
        SoftwareUpdate,
        User,
        MemoryLow,
        CpuExcess,
        DiskFull,
        Watchdog,
    }

    public enum FuncBatteryTemperatureAttribute
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
    }

    public enum FuncBatteryEndOfLifeAttribute
    {
        EndOfLife,
    }

    public enum FuncMainsPowerFaillureAttribute
    {
        Error = 1,
        OK = 2,
    }

    public enum FuncDoorStatusAttribute
    {
        Open = 1,
        Closed = 2,
    }

    public enum FuncPowerModeAttribute
    {
        Initializing = -1,
        Full = 0,
        Reduced = 1,
        Minimal = 2,
        NoPower = 3,
        Off = 4,
        Manual = 5,
        Shutdown = 6,
    }

    public enum FuncTractionFailureAttribute
    {
        Error = 1,
        OK = 2,
    }

    public enum FuncInverterFailureAttribute
    {
        Error = 1,
        OK = 2,
    }

    public enum FuncLowBatteryAttribute
    {
        Error = 1,
        OK = 2,
    }

    public enum FuncVandalismAttribute
    {
        Vandalism,
    }

    public enum FuncButtonsAttribute
    {
        TtsKeyPressed,
    }

    public enum FuncSurgeProtectionAttribute
    {
        Active = 1,
        Inactive = 2,
    }

    public enum FuncTimeSyncAttribute
    {
        LowThreshold,
    }

    public enum OperationMasterDataAttribute
    {
        Route = 1,
        Trip = 2,
        Calendar = 3,
        Points = 4,
        Led = 5,
        Tts = 6,
        Line = 7,
    }

    public enum OperationTripStateUnknownAttribute
    {
        StateUnknown,
    }

    public enum OperationTripStateLeftAttribute
    {
        Early = 1,
        Late = 2,
        Skipped = 3,
        Aborted = 4,
    }

    public enum OperationVdvDataMissingAttribute
    {
        Ref,
        Prc,
    }
}