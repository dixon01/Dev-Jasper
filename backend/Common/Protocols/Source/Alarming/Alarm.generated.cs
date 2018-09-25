
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Gorba.Common.Utility.Compatibility;

namespace Gorba.Common.Protocols.Alarming
{
    public partial class Alarm
    {
        public string GetAttributeText()
        {
            if (this.Type == AlarmType.SystemRestart)
            {
                return ((SystemRestartAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.SystemFileSystem)
            {
                return ((SystemFileSystemAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.SystemTest)
            {
                return ((SystemTestAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.SystemLedPanel)
            {
                return ((SystemLedPanelAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.SystemConfig)
            {
                return ((SystemConfigAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.SystemDcmIO)
            {
                return ((SystemDcmIOAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.ApplicationRelaunch)
            {
                return ((ApplicationRelaunchAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncBatteryTemperature)
            {
                return ((FuncBatteryTemperatureAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncBatteryEndOfLife)
            {
                return ((FuncBatteryEndOfLifeAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncMainsPowerFaillure)
            {
                return ((FuncMainsPowerFaillureAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncDoorStatus)
            {
                return ((FuncDoorStatusAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncPowerMode)
            {
                return ((FuncPowerModeAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncTractionFailure)
            {
                return ((FuncTractionFailureAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncInverterFailure)
            {
                return ((FuncInverterFailureAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncLowBattery)
            {
                return ((FuncLowBatteryAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncVandalism)
            {
                return ((FuncVandalismAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncButtons)
            {
                return ((FuncButtonsAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncSurgeProtection)
            {
                return ((FuncSurgeProtectionAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.FuncTimeSync)
            {
                return ((FuncTimeSyncAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.OperationMasterData)
            {
                return ((OperationMasterDataAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.OperationTripStateUnknown)
            {
                return ((OperationTripStateUnknownAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.OperationTripStateLeft)
            {
                return ((OperationTripStateLeftAttribute)this.Attribute).ToString();
            }

            if (this.Type == AlarmType.OperationVdvDataMissing)
            {
                return ((OperationVdvDataMissingAttribute)this.Attribute).ToString();
            }

            return this.Attribute.ToString();
        }
    }
}