// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IqubeTaskCode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates the list of the available iqube task codes.
//   An iqube task is part of a iqube command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Enumerates the list of the available iqube task codes.
    /// An iqube task is part of a iqube command.
    /// </summary>
    public enum IqubeTaskCode : sbyte
    {
        /// <summary>
        /// None task
        /// </summary>
        None = -1,

        // infoline disposals:

        /// <summary>
        /// Display infoline data
        /// </summary>
        TaskInfoData,

        /// <summary>
        /// stop display infoline
        /// </summary>
        TASK_DISPO_INFO_STOP,

        // general disposals:

        /// <summary>
        /// Revoke an activity
        /// </summary>
        ActivityRevoke,

        // display controller disposals:

        /// <summary>
        /// Turns the display off, used also for trip on/off if trip id is defined (legacy code = TASK_DISPO_DISP_OFF)
        /// </summary>
        ActivityDisplayOff,

        /// <summary>
        /// Turns the display on, used also for trip on/off if trip id is defined (legacy code = TASK_DISPO_DISP_ON)
        /// </summary>
        ActivityDisplayOn,

        // display table disposals:

        /// <summary>
        /// trip delay
        /// </summary>
        TASK_DISPO_TRIP_DELAY,

        /// <summary>
        /// extra trip
        /// </summary>
        TASK_DISPO_EXTRA_TRIP,

        /// <summary>
        /// Fahrt Löschen (legacy code = TASK_DISPO_DELETE_TRIP)
        /// </summary>
        ActivityDeleteTrip,

        /// <summary>
        /// Fahrzeig ersetzen
        /// </summary>
        TASK_DISPO_REPLACE_VEHICLE,

        /// <summary>
        /// Fahrzeig anmelden
        /// </summary>
        TASK_DISPO_REGISTER_VEHICLE,

        /// <summary>
        /// Fahrzeig abmelden
        /// </summary>
        TASK_DISPO_SIGN_OFF_VEHICLE,

        /// <summary>
        /// Umweg fahren
        /// </summary>
        TASK_DISPO_INDIRECTION,

        /// <summary>
        /// Abkürzung fahren
        /// </summary>
        TASK_DISPO_SHORTCUT,

        /// <summary>
        /// Display Algorithm
        /// </summary>
        TASK_DISPO_DISPLAY_ALGO,

        // technical disposals:

        /// <summary>
        /// extract an archive        
        /// </summary>
        TASK_DISPO_QARC_EXTRACT,

        /// <summary>
        /// Dispo text
        /// </summary>
        TASK_DISPO_TEST,

        /// <summary>
        /// activate eventlog
        /// </summary>
        TASK_DISPO_EVENTLOG,

        /// <summary>
        /// upgrade software
        /// </summary>
        TASK_DISPO_UPGRADE,

        /// <summary>
        /// execute batch job (file based)
        /// </summary>
        TASK_DISPO_BATCH_JOB,

        /// <summary>
        /// display special text (infoline) based on trip data
        /// </summary>
        TASK_DISPO_SPECIAL_TEXT,

        /// <summary>
        /// stop display special text
        /// </summary>
        TASK_DISPO_SPECIAL_STOP,

        /// <summary>
        /// voice text for announcement on TTS system (legacy code = TASK_DISPO_DISP_OFF)
        /// </summary>
        ActivityVoiceText,

        /// <summary>
        /// Max task enum
        /// </summary>
        TASK_MAX
    }
}