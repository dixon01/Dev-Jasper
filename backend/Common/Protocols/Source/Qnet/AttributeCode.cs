// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeCode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumeration of all available attribute code. Used for events and alarms.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Enumeration of all available attribute code. Used for events and alarms.
    /// </summary>
    public static class AttributeCode
    {
        /// <summary>
        /// Defines constantes for event flag attributes
        /// </summary>
        public static class EventFlagAttributes
        {
            /// <summary>
            /// None flag
            /// </summary>
            public const byte EventFlagNone = 0x0;

            /// <summary>
            /// additional data
            /// </summary>
            public const byte EventFlagAdditionalParameter = 0x1;

            /// <summary>
            /// external event
            /// </summary>
            public const byte EventFlagExternalEvent = 0x2;
        }

        /// <summary>
        /// Defines constantes for event flag attributes
        /// </summary>
        public static class BatteryTemperatureAttributes
        {
            /// <summary>
            /// 0x1 Temprature high (Legacy code = BATT_TEMP_HIGH)
            /// second alarm after 2 hours
            /// </summary>
            public const byte High = 0x1;

            /// <summary>
            ///  0x2 Temprature Low (Legacy code = BATT_TEMP_LOW)
            /// </summary>
            public const byte Low = 0x2;

            /// <summary>
            /// Temperature Ok (Legacy code = BATT_TEMP_OK)
            /// </summary>
            public const byte Ok = 0x3;
        }

        /// <summary>
        /// Defines attribute constantes concerning Mains Power Failure
        /// </summary>
        public static class MainsPowerAttributes
        {
            /// <summary>
            /// Mains Power Failure (Legacy code = MAINS_POWER_FAIL)
            /// </summary>
            public const byte Fail = 0x1;

            /// <summary>
            /// Mains Power OK (after failure) (Legacy code = MAINS_POWER_OK)
            /// </summary>
            public const byte Ok = 0x2;
        }

        /// <summary>
        /// Defines attribute constantes concerning doors status
        /// </summary>
        public static class DoorStatusAttributes
        {
            /// <summary>
            /// Door open detected (Legacy code = DOOR_STATUS_OPEN)
            /// </summary>
            public const byte Open = 0x1;

            /// <summary>
            /// Door closed detected (Legacy code = DOOR_STATUS_CLOSE)
            /// </summary>
            public const byte Close = 0x2;
        }

        /// <summary>
        /// Defines attributes concerning the power Mode changed alarm
        /// </summary>
        public static class PowerModeAttributes
        {
            /// <summary>
            /// Initialization, check I/O's (Legacy code = POWER_MODE_INIT)
            /// </summary>
            public const sbyte Init = -0x1;

            /// <summary>
            /// Full Power, all peripherals on (Legacy code = POWER_MODE_FULL)
            /// </summary>
            public const byte Full = 0x0;

            /// <summary>
            /// Reduced Power (Legacy code = POWER_MODE_REDUCED)
            /// </summary>
            public const byte Reduced = 0x1;

            /// <summary>
            /// Min. Power (VE, GPRS-Modem, DCM-IO on) (Legacy code = POWER_MODE_MINIMUM)
            /// </summary>
            public const byte Medium = 0x2;

            /// <summary>
            /// No Power (Legacy code = POWER_MODE_NO)
            /// </summary>
            public const byte None = 0x3;

            /// <summary>
            /// Power Off (Legacy code = POWER_MODE_DEAD)
            /// </summary>
            public const byte Off = 0x4;

            /// <summary>
            /// Manual Mode, all peripherals on (Legacy code = POWER_MODE_MANUAL)
            /// </summary>
            public const byte Manual = 0x5;

            /// <summary>
            /// Power Down, all peripherals off (Legacy code = POWER_MODE_DOWN)
            /// </summary>
            public const byte Down = 0x6;
        }

        /// <summary>
        /// Defines attribute constantes for restart
        /// </summary>
        public static class RestartAttributes
        {
            /// <summary>
            /// Restart after upgrade (Legacy code = UPGRADE)
            /// </summary>
            public const byte Upgrade = 0x1;

            /// <summary>
            /// Restart after user request (Legacy code = USER_REQUEST)
            /// </summary>
            public const byte UserRequest = 0x2;

            /// <summary>
            /// VCU: Ignition off (Legacy code = POWER_OFF)
            /// </summary>
            public const byte PowerOff = 0x3;

            /// <summary>
            /// IQU: Battery low (Tiefentladeschutz) (Legacy code = POWER_LOW)
            /// </summary>
            public const byte PowerLow = 0x4;

            /// <summary>
            /// Unexpected restart (Legacy code = UNEXPECTED)
            /// </summary>
            public const byte Unexpected = 0x5;

            /// <summary>
            /// Restart by timer (Legacy code = TIMER)
            /// </summary>
            public const byte Timer = 0x6;

            /// <summary>
            /// Restart by the system (Legacy code = SYSTEM)
            /// </summary>
            public const byte System = 0x7;

            /// <summary>
            /// Restart for daylight saving (Legacy code = DAYLIGHT_SAV)
            /// </summary>
            public const byte DayLightSaving = 0x8;
        }

        /// <summary>
        /// Defines attribute constantes for file system error 
        /// </summary>
        public static class FileSystemErrorAttributes
        {
            /// <summary>
            /// 0x0 : Error filesystem check after restart (Legacy code = QFS_DEV_EEPROM)
            /// </summary>
            public const byte EEPROM = 0x00;

            /// <summary>
            /// 0x01 : Error filesystem check after restart for memory flash (Legacy code = QFS_DEV_FLASH)
            /// </summary>
            public const byte Flash = 0x01;
        }

        /// <summary>
        /// Defines attribute constantes concerning the DMI IO
        /// </summary>
        public static class DmcIOAttributes
        {
            /// <summary>
            /// 0x01 :  DCM-I/O Failure detected (Legacy code = DCM_IO_FAIL)
            /// </summary>
            public const byte Fail = 0x01;

            /// <summary>
            /// 0x02 : DCM-I/O OK (after failure) (Legacy code = DCM_IO_OK)
            /// </summary>
            public const byte Ok = 0x02;
        }

        /// <summary>
        /// Defines attribute constantes concerning traction failure alarm
        /// </summary>
        public static class TractionFailureAttributes
        {
            /// <summary>
            /// 0x01 :  Traction Failure (Legacy code = TRACTION_FAIL)
            /// </summary>
            public const byte Fail = 0x01;

            /// <summary>
            /// 0x02 : Traction OK  (after failure) (Legacy code = TRACTION_OK)
            /// </summary>
            public const byte Ok = 0x02;
        }

        /// <summary>
        /// Defines attribute constantes concerning invert failure alarm
        /// </summary>
        public static class InvertFailureAttributes
        {
            /// <summary>
            /// 0x01 :  Traction Failure (Legacy code = INVERTER_FAIL)
            /// </summary>
            public const byte Fail = 0x01;

            /// <summary>
            /// 0x02 : Traction OK  (after failure) (Legacy code = INVERTER_OK)
            /// </summary>
            public const byte Ok = 0x02;
        }

        /// <summary>
        /// Defines attribute constantes concerning the battery low alarm
        /// </summary>
        public static class LowBatteryAttributes
        {
            /// <summary>
            /// 0x01 :  Battery is low (Legacy code = BATTERY_LOW)
            /// </summary>
            public const byte Low = 0x01;

            /// <summary>
            /// 0x02 : Traction OK  (after failure) (Legacy code = BATTERY_OK)
            /// </summary>
            public const byte Ok = 0x02;
        }

        /// <summary>
        /// Defines attribute constantes concerning over-voltage protection alarm
        /// </summary>
        public static class OverVoltageProtectionAttributes
        {
            /// <summary>
            /// 0x01 : Over Voltage Protection activated(Legacy code = OVP_ACTIVE)
            /// </summary>
            public const byte Active = 0x01;

            /// <summary>
            /// 0x02 : Over Voltage Protection deactivated (Legacy code = OVP_INACTIVE)
            /// </summary>
            public const byte Inactive = 0x02;
        }

        /// <summary>
        /// Defines attribute constantes concerning master data problems
        /// </summary>
        public static class MasterDataAttributes
        {
            /// <summary>
            /// rou file (xxx_rou.csv) (Legacy code MASTERDATA_ROU)
            /// </summary>
            public const byte RouteFile = 0x1;

            /// <summary>
            /// frt file (xxx_frt.csv) (Legacy code MASTERDATA_FRT)
            /// </summary>
            public const byte FrtFile = 0x2;

            /// <summary>
            /// calendar file (calendar.ini) (Legacy code MASTERDATA_CAL)
            /// </summary>
            public const byte CalendarFile = 0x3;

            /// <summary>
            /// ort file (ort.csv) (Legacy code MASTERDATA_ORT)
            /// </summary>
            public const byte StopFile = 0x4;

            /// <summary>
            /// led text config file (led.ini) (Legacy code MASTERDATA_LED)
            /// </summary>
            public const byte LedTextConfigFile = 0x5;

            /// <summary>
            /// Text-to-speech config file (tts.ini) (Legacy code )
            /// </summary>
            public const byte TextToSpeechFile = 0x6;

            /// <summary>
            /// line config file (line.ini) (Legacy code MASTERDATA_LIN)
            /// </summary>
            public const byte LineFile = 0x7;
        }

        /// <summary>
        /// Defines attribute constantes concerning Trip state left
        /// </summary>
        public static class TripStateLeftAttributes
        {
            /// <summary>
            /// bus has left too early (Frühabfahrt) (Legacy code = TRIPSTATE_LEFT_EARLY)
            /// </summary>
            public const byte TooEarly = 0x1;

            /// <summary>
            /// bus has left too late (Legacy code = TRIPSTATE_LEFT_LATE)
            /// </summary>
            public const byte TooLate = 0x2;

            /// <summary>
            /// Delete the trip on the previous iqube by the next one (Legacy code = TRIPSTATE_LEFT_BACKWARDS)
            /// </summary>
            public const byte Backwards = 0x3;

            /// <summary>
            /// Trip cancelled (Legacy code = LEFT_ABORTED)
            /// </summary>
            public const byte Aborted = 0x4;

            /// <summary>
            /// (Legacy code = TRIPSTATE_LEFT_EARLY_CLR)
            /// </summary>
            public const byte EarlyClr = 0x81;
        }
    }
}
