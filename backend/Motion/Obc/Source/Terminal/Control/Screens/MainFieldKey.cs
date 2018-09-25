// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainFieldKey.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainFieldKey type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    /// <summary>
    /// The possible main field keys.
    /// </summary>
    public enum MainFieldKey
    {
        /// <summary>
        /// No main field.
        /// </summary>
        None,

        /// <summary>
        /// The menu.
        /// </summary>
        Menu,

        /// <summary>
        /// The status.
        /// </summary>
        Status,

        /// <summary>
        /// The drive selection.
        /// </summary>
        DriveSelect,

        /// <summary>
        /// The block number input.
        /// </summary>
        BlockNumberInput,

        /// <summary>
        /// The special destination selection.
        /// </summary>
        SpecialDestinationSelect,

        /// <summary>
        /// The driver login.
        /// </summary>
        DriverLogin,

        /// <summary>
        /// The block drive wait screen.
        /// </summary>
        BlockDriveWait,

        /// <summary>
        /// The block driving.
        /// </summary>
        BlockDriving,

        /// <summary>
        /// The special destination drive.
        /// </summary>
        SpecialDestinationDrive,

        /// <summary>
        /// The incoming messages.
        /// </summary>
        InMessages,

        /// <summary>
        /// The brightness setting.
        /// </summary>
        Brightness,

        /// <summary>
        /// The speech GSM.
        /// </summary>
        SpeechGsm,

        /// <summary>
        /// The alarm.
        /// </summary>
        Alarm,

        /// <summary>
        /// The announcement.
        /// </summary>
        Announcement,

        /// <summary>
        /// The language selection.
        /// </summary>
        Language,

        /// <summary>
        /// The TTS volume.
        /// </summary>
        TtsVolume,

        /// <summary>
        /// The iqube radio.
        /// </summary>
        IqubeRadio,

        /// <summary>
        /// The passenger counting.
        /// </summary>
        PassengerCount,

        /// <summary>
        /// The system code.
        /// </summary>
        SystemCode,

        /// <summary>
        /// The trip number.
        /// </summary>
        TripNumber,

        /// <summary>
        /// The block auto completion.
        /// </summary>
        BlockAutoCompletion
    }
}