// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Definition of event logging types(legacy code = tEVENTid)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Definition of event logging types(legacy code = tEVENTid)
    /// </summary>
    [DataContract]
    public enum EventCode
    {
        /// <summary>
        /// Undefined event number
        /// </summary>
        [EnumMember]
        EventNone = -1,

        // *** System Events ****************************************************************************

        /// <summary>
        /// System Events min value
        /// </summary>
        EventSystemMin = 0x0FFF,           

        /// <summary>
        /// 0x1000: Iqube Restarted (EVENT_SYS_RESTART)
        /// </summary>
        [EnumMember]
        EventSysRestart = 0x1000,

        /// <summary>
        /// 0x1004: Error Filesystem (EVENT_SYS_FILESYSTEM)
        /// </summary>
        [EnumMember]
        EventSysFileSystem = 0x1004,

        /// <summary>
        /// 0x1005: Test Event (EVENT_SYS_TEST)
        /// </summary>
        [EnumMember]
        EventSysTest = 0x1005,

        /// <summary>
        /// 0x1006: Error LED-Panel (EVENT_SYS_LEDPANEL)
        /// </summary>
        [EnumMember]
        EventSysLedPanel = 0x1006,

        /// <summary>
        /// 0x1007: Config changed (EVENT_SYS_CONFIG_CHANGED)
        /// </summary>
        [EnumMember]
        EventSysConfigChanged = 0x1007,

        /// <summary>
        /// 0x1008: Setup changed
        /// </summary>
        [EnumMember]
        EventSysSetupChanged = 0x1008,

        /// <summary>
        /// 0x1009: Error DCM-I/O (EVENT_SYS_DCM_IO)
        /// </summary>
        [EnumMember]
        EventSysDmcIO = 0x1009,

        /// <summary>
        /// System Events min value
        /// </summary>
        EventSystemMax = 0x1FFE,

        // *** Functional Events ****************************************************************************
       
        /// <summary>
        /// Functional Events min value
        /// </summary>
        EventFunctionalMin = 0x1FFF,
        
        /// <summary>
        /// 0x2000: Battery Temperature High/Ok (EVENT_FNC_BATT_TEMP)
        /// </summary>
        [EnumMember]
        EventFncBatteryTemperature = 0x2000,

        /// <summary>
        /// 0x2001: Battery End of Life (EVENT_FNC_BATT_EOL)
        /// </summary>
        [EnumMember]
        EventFncBatteryEndOfLife = 0x2001,

        /// <summary>
        /// 0x2004: Mains Power Failure alarm (EVENT_FNC_MAINS_POWER_FAIL)
        /// </summary>
        [EnumMember]
        EventFncMainsPowerFaillure = 0x2004,

        /// <summary>
        /// 0x2005: Door Status alarm (EVENT_FNC_DOOR_STATUS)
        /// </summary>
        [EnumMember]
        EventFncDoorStatus = 0x2005,

        /// <summary>
        /// 0x2006: Power Mode changed alarm (EVENT_FNC_POWER_MODE)
        /// </summary>
        [EnumMember]
        EventFncPowerMode = 0x2006,

        /// <summary>
        /// 0x2007: Traction Failure alarm (EVENT_FNC_TRACTION_FAIL)
        /// </summary>
        [EnumMember]
        EventFncTractionFailure = 0x2007,

        /// <summary>
        /// 0x2008: Inverter Failure alarm (EVENT_FNC_INVERTER_FAIL)
        /// </summary>
        [EnumMember]
        EventFncInverterFailure = 0x2008,

        /// <summary>
        /// 0x2009: Low Battery alarm (EVENT_FNC_LOW_BATTERY)
        /// </summary>
        [EnumMember]
        EventFncLowBattery = 0x2009,

        /// <summary>
        /// 0x200A: vandalism sensor alarm (EVENT_FNC_VANDALISM)
        /// </summary>
        [EnumMember]
        EventFncVandalism = 0x200A,

        /// <summary>
        /// 0x200B: TTS key pressed (EVENT_FNC_TTS_KEY_PRESSED)
        /// </summary>
        [EnumMember]
        EventFncTTSKeyPressed = 0x200B,

        /// <summary>
        /// 0x200C: Over Voltage Protection alarm (EVENT_FNC_OVP)
        /// </summary>
        [EnumMember]
        EventFncOverVoltageProtection = 0x200C,

        /// <summary>
        /// Time sync alarm (time diff > 5 sec.) (EVENT_FNC_TIMESYNC)
        /// </summary>
        [EnumMember]
        EventFncTimeSync = 0x200D,

        /// <summary>
        /// Functional Events max value
        /// </summary>
        EventFunctionalMax = 0x3FFE,

        // *** Operational Events ****************************************************************************

        /// <summary>
        /// Minimun value for operational events
        /// </summary>
        EventOperationalMin  = 0x3FFF,

        /// <summary>
        /// 0x4000: master data alarm (EVENT_OP_MASTERDATA)
        /// </summary>
        [EnumMember]
        EventOpMasterData = 0x4000,

        /// <summary>
        /// 0x4002: tripstate unknown alarm (EVENT_OP_TRIPSTATE_UNKNOWN)
        /// </summary>
        [EnumMember]
        EventOpTripStateUnknown = 0x4002,

        /// <summary>
        /// 0x4003: tripstate left (EVENT_OP_TRIPSTATE_LEFT)
        /// </summary>
        [EnumMember]
        EventOpTripStateLeft = 0x4003,

        /// <summary>
        /// 0x4006: Several buses for the same service (EVENT_OP_MISSING_VDV_REF_DATA)
        /// Reference Data Request sent to RBL-Client
        /// </summary>
        [EnumMember]
        EventOpMissingVdvRefData = 0x4006,

        /// <summary>
        /// 0x4007: master data archive activation (EVENT_OP_MISSING_VDV_PRC_DATA)
        /// sent after timeout (if timeout > 0)
        /// </summary>
        [EnumMember]
        EventOpMissingVdvPrcData = 0x4007,
        
        /// <summary>
        /// Even operational max value
        /// </summary>
        EventOperationalMax = 0x4FFE,

        // *** Disposition Events *************************************************************************

        /// <summary>
        /// Disposition Events min value
        /// </summary>
        EventDispositionMin = 0x4FFF,

        /// <summary>
        /// internal use for dispo handling (EVENT_DISPO_SUCCESS)
        /// </summary>
        [EnumMember]
        EventDispoSuccess = 0x5000,

        /// <summary>
        /// internal use for dispo handling (EVENT_DISPO_FAILURE)
        /// </summary>
        [EnumMember]
        EventDispoFailure = 0x5001,

        /// <summary>
        /// internal use for dispo handling (EVENT_DISPO_STATUS)
        /// </summary>
        [EnumMember]
        EventDispoStatus = 0x5002,

        /// <summary>
        /// Disposition Events min value
        /// </summary>
        EventDispositionMax = 0x5FFE
        /*
         * 
  EVENT_NONE     = -1,

  EVENT_FILE_CREATED ,   //  0: New Eventlog file created
  EVENT_FILE_STARTED ,   //  1: Eventlogging started
  EVENT_FILE_STOPPED ,   //  2: Eventlogging stopped

  EVENT_DOOR_1_STATUS,   //  3: Status Bustuere 1
  EVENT_DOOR_2_STATUS,   //  4: Status Bustuere 2
  EVENT_DOOR_3_STATUS,   //  5: Status Bustuere 3

  EVENT_IGLOCK_STATUS,   //  6: Status Bus-Zuendschloss

  EVENT_BUS_DRIVES   ,   //  7: Fahrstatus Bus faehrt (param=distance from StartHS)
  EVENT_BUS_HALTED   ,   //  8: Fahrstatus Bus steht (param=distance from StartHS)
  EVENT_ZONE_SWITCHED,   //  9: Zone umgeschaltet (param=neue Zone)

  EVENT_NEW_L_TG     ,   // 10: IBIS Telegramm lZZZ
  EVENT_NEW_E_TG     ,   // 11: IBIS Telegramm eZZZZZZ
  EVENT_NEW_EA_TG    ,   // 12: IBIS Telegramm eAZZZZ

  EVENT_STATION_ENTRY,   // 13: Einfahrt in Haltestelle (param=distance from StartHS)
  EVENT_STATION_EXIT ,   // 14: Ausfahrt aus Haltestelle (param=distance from StartHS)

  EVENT_ODOMETER_RESET,  // 15: Reset Odometer counter

  EVENT_RADIOSTATISTIC,  // 16: radio statistic message received

  EVENT_ORTMSG_REQUEST,  // 17: Ort Message request from VCU received
  EVENT_NEW_D_TG     ,   // 18: IBIS Telegramm dZZZZZ
  EVENT_NEW_U_TG     ,   // 19: IBIS Telegramm uZZZZ
  EVENT_NEW_Z_TG     ,   // 20: IBIS Telegramm zZZZ

  EVENT_ORTMSG_RESPONSE, // 21: Ort Message response from Iqube received
  EVENT_BUS_DEPARTURE,   // 22: Departure Message from other Iqube received

  EVENT_TT_UPDATE    ,   // 23: Periodic TimeTable Update done

  EVENT_BUS_DELAY    ,   // 24: Delay Message from other Iqube received

  EVENT_TRIP_REQUEST ,   // 25: Trip Request from VCU received
  EVENT_TRIP_RESPONSE,   // 26: Trip Response from Iqube received

  EVENT_VCUVERIF_REQUEST , // 27: VCU Verification Request from Iqube received
  EVENT_VCUVERIF_RESPONSE, // 28: VCU Verification Response from VCU received

  EVENT_ORTMSG_NOTIFICATION, // 29: Ort Message Notification from VCU received

  EVENT_COACH_REQUEST,       // 30: Coach Message Request from Iqube received
  EVENT_COACH_RESPONSE,      // 31: Coach Message Response from VCU received

  EVENT_TRIP_STATE,          // 32: Trip State

  EVENT_BUS_DETECTED,        // 33: Bus Detected Message received!
  EVENT_BUS_MISSING,         // 34: Bus Missing Message received!
  EVENT_BUS_PASSED,          // 35: Bus Passed Message received!
  EVENT_LSA_MESSAGE,         // 36: LSA Message received!
  EVENT_SMS_RECEIVED,        // 37: SMS Message received!
  EVENT_DISPTAB_DATA,        // 38: Disptab Data Message received!
  EVENT_LSA_R_09_1X,         // 39: LSA R09.1X Message received!
  EVENT_LSA_RAW_DATA,        // 40: LSA or RBL Message received! (raw data)
  EVENT_BUS_MELDEPUNKT,      // 41: VCU Meldepunkt Message received!

  EVENT_GATE_STATUS,         // 42: Status Gate Open/Closed (dig. Input)

  EVENT_RBL_R_06_1,          // 43: RBL R06.1 Message received!
  EVENT_NEW_K_TG,            // 44: IBIS Telegramm kZZZ
  EVENT_ZANASY_RAW,          // 45: Zanasy Message received! (raw data)

  EVENT_STAUSENSOR_ON,       // 46: Stausensor EIN
  EVENT_STAUSENSOR_OFF,      // 47: Stausensor AUS

  EVENT_IBIS_MONITOR,        // 48: IBIS Monitor Message

  EVENT_VDV_FAHRPLANLAGE,        // 49: VDV Fahrplanlage Message received
  EVENT_VDV_FAHRTLOESCHEN,       // 50: VDV Fahrtloeschen Message received
  EVENT_VDV_SPEZIALTEXT,         // 51: VDV Spezialtext Message received
  EVENT_VDV_SPEZIALTEXTLOESCHEN, // 52: VDV Spezialtextloeschen Message received

  EVENT_POIS,                    // 53: POIS Message received

  EVENT_BUS_ENTRY,               // 54: bus entry message (iqube easy)
  EVENT_BUS_EXIT,                // 55: bus exit message (iqube easy)
         
  EVENT_NEW_LF_TG,               // 56: IBIS Telegramm lFzzzzz
          
  EVENT_BUS_RBL,                 // 57: RBL position message (TELABO/RVBW)

  EVENT_BUS_WAIT,                // 58: Bus Wait Message received!

  EVENT_TEST_MAX,

  // Technical events
  EVENT_TECH_MIN = 0x400,               // 0x400:
  EVENT_AC_ON    = EVENT_TECH_MIN,      // 0x400: AC Power Supply on
  EVENT_AC_OFF,                         // 0x401: AC Power Supply off
  EVENT_FAN_ON,                         // 0x402: Main Fan on
  EVENT_FAN_OFF,                        // 0x403: Main Fan off
  EVENT_LIGHT_ON,                       // 0x404: Light on
  EVENT_LIGHT_OFF,                      // 0x405: Light off
  EVENT_RADIO1_ON,                      // 0x406: Radio Modem #1 on
  EVENT_RADIO1_OFF,                     // 0x407: Radio Modem #1 off
  EVENT_RADIO2_ON,                      // 0x408: Radio Modem #2 on
  EVENT_RADIO2_OFF,                     // 0x409: Radio Modem #2 off
  EVENT_FAN_1_ON,                       // 0x40A: Add. Fan #1 on
  EVENT_FAN_1_OFF,                      // 0x40B: Add. Fan #1 off
  EVENT_FAN_2_ON,                       // 0x40C: Add. Fan #2 on
  EVENT_FAN_2_OFF,                      // 0x40D: Add. Fan #2 off
  EVENT_LED_CTRL_1_ON,                  // 0x40E: LED-Controller #1 on
  EVENT_LED_CTRL_1_OFF,                 // 0x40F: LED-Controller #1 off
  EVENT_LED_CTRL_2_ON,                  // 0x410: LED-Controller #2 on
  EVENT_LED_CTRL_2_OFF,                 // 0x411: LED-Controller #2 off
  EVENT_LED_MATRIX_1_ON,                // 0x412: LED-Matrix #1 on
  EVENT_LED_MATRIX_1_OFF,               // 0x413: LED-Matrix #1 off
  EVENT_LED_MATRIX_2_ON,                // 0x414: LED-Matrix #2 on
  EVENT_LED_MATRIX_2_OFF,               // 0x415: LED-Matrix #2 off
  EVENT_TTS_POWER_ON,                   // 0x416: Text-To-Speech power on
  EVENT_TTS_POWER_OFF,                  // 0x417: Text-To-Speech power off
  EVENT_TTS_SPEAKER_ON,                 // 0x418: Text-To-Speech speaker on
  EVENT_TTS_SPEAKER_OFF,                // 0x419: Text-To-Speech speaker off
  EVENT_DCM_LED_1_ON,                   // 0x41A: DCM-LED #1 on
  EVENT_DCM_LED_1_OFF,                  // 0x41B: DCM-LED #1 off
  EVENT_DCM_LED_2_ON,                   // 0x41C: DCM-LED #2 on
  EVENT_DCM_LED_2_OFF,                  // 0x41D: DCM-LED #2 off
  EVENT_DCM_QWATCH_1_ON,                // 0x41E: DCM-QWATCH #1 on
  EVENT_DCM_QWATCH_1_OFF,               // 0x41F: DCM-QWATCH #1 off
  EVENT_DCM_QWATCH_2_ON,                // 0x420: DCM-QWATCH #2 on
  EVENT_DCM_QWATCH_2_OFF,               // 0x421: DCM-QWATCH #2 off
  EVENT_DCMIO_POWER_ON,                 // 0x422: DCM-IO power on
  EVENT_DCMIO_POWER_OFF,                // 0x423: DCM-IO power off
  EVENT_IMMA_POWER_ON,                  // 0x424: IMMA power on
  EVENT_IMMA_POWER_OFF,                 // 0x425: IMMA power off
  EVENT_SERVICE_MODE_ON,                // 0x426: Service Mode on
  EVENT_SERVICE_MODE_OFF,               // 0x427: Service Mode off
  EVENT_MANUAL_MODE_ON,                 // 0x428: Manual Mode on
  EVENT_MANUAL_MODE_OFF,                // 0x429: Manual Mode off
  EVENT_LED_CTRL_KEYS_ON,               // 0x42A: LED Control keys on
  EVENT_LED_CTRL_KEYS_OFF,              // 0x42B: LED Control keys off

  EVENT_TECH_MAX,                       // 0x4xx: 

  // System Events
  EVENT_SYS_MIN     = 0x1000,           // 0x1000:
  EVENT_SYS_RESTART = EVENT_SYS_MIN,    // 0x1000: Iqube Restarted
  EVENT_SYS_RTC,                        // 0x1001: Error Real Time Clock
  EVENT_SYS_APPL,                       // 0x1002: not used
  EVENT_SYS_COMM,                       // 0x1003: not used
  EVENT_SYS_FILESYSTEM,                 // 0x1004: Error Filesystem
  EVENT_SYS_TEST,                       // 0x1005: Test Event
  EVENT_SYS_LEDPANEL,                   // 0x1006: Error LED-Panel
  EVENT_SYS_CONFIG_CHANGED,             // 0x1007: Config changed
  EVENT_SYS_SETUP_CHANGED,              // 0x1008: Setup changed
  EVENT_SYS_DCM_IO,                     // 0x1009: Error DCM-I/O
  EVENT_SYS_MAX,                        // 0x100A:

  // Functional Events
  EVENT_FNC_MIN       = 0x2000,         // 0x2000:
  EVENT_FNC_BATT_TEMP = EVENT_FNC_MIN,  // 0x2000: Battery Temperature High/Ok
  EVENT_FNC_BATT_EOL,                   // 0x2001: Battery End of Life
  EVENT_FNC_EVTLOG,                     // 0x2002: not used
  EVENT_FNC_DAYLIGHT_SAVING,            // 0x2003: daylight savings event
  EVENT_FNC_MAINS_POWER_FAIL,           // 0x2004: Mains Power Failure alarm
  EVENT_FNC_DOOR_STATUS,                // 0x2005: Door Status alarm
  EVENT_FNC_POWER_MODE,                 // 0x2006: Power Mode changed alarm
  EVENT_FNC_TRACTION_FAIL,              // 0x2007: Traction Failure alarm
  EVENT_FNC_INVERTER_FAIL,              // 0x2008: Inverter Failure alarm
  EVENT_FNC_LOW_BATTERY,                // 0x2009: Low Battery alarm
  EVENT_FNC_VANDALISM,                  // 0x200A: vandalism sensor alarm
  EVENT_FNC_TTS_KEY_PRESSED,            // 0x200B: TTS key pressed
  EVENT_FNC_OVP,                        // 0x200C: Over Voltage Protection alarm
  EVENT_FNC_MAX,                        // 0x200D:

  // Operational Events
  EVENT_OP_MIN        = 0x4000,         // 0x4000: 
  EVENT_OP_MASTERDATA = EVENT_OP_MIN,   // 0x4000: master data error
  EVENT_OP_DATA,                        // 0x4001: not used
  EVENT_OP_TRIPSTATE_UNKNOWN,           // 0x4002: tripstate unknown
  EVENT_OP_TRIPSTATE_LEFT,              // 0x4003: tripstate left
  EVENT_OP_WAGON_UMLAUF,                // 0x4004: mehrere wagen mit gleichem Umlauf
  EVENT_OP_MASTERDATA_ARCHIVE,          // 0x4005: master data archive activation
  EVENT_OP_MAX,

  // Disposition Events
  EVENT_DISPO_MIN     = 0x5000,
  EVENT_DISPO_SUCCESS = EVENT_DISPO_MIN,
  EVENT_DISPO_FAILURE,
  EVENT_DISPO_STATUS,
  EVENT_DISPO_MAX,

  EVENT_TOTAL_MAX
         * */
    }
}
