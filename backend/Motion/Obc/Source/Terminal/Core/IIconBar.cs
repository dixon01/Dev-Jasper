// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIconBar.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The interface for the icon bar
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    ///   The interface for the icon bar
    /// </summary>
    public interface IIconBar
    {
        /// <summary>
        ///   The event when the status field is touched from the user. Is used to sign to menu screen!
        /// </summary>
        event EventHandler ContextIconClick;

        /// <summary>
        /// Set the voice icon
        /// </summary>
        /// <param name="state">state of the icon; which icon is displayed</param>
        void SetVoiceIcon(VoiceIconState state);

        /// <summary>
        /// Set the driver alarm icon
        /// </summary>
        /// <param name="state">state of the icon; which icon is displayed</param>
        void SetDriverAlarmIcon(DriverAlarmIconState state);

        /// <summary>
        /// Set the information message icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        void SetInformationMessageIcon(bool visible);

        /// <summary>
        /// Set the alarm message icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        void SetAlarmMessageIcon(bool visible);

        /// <summary>
        /// Set the razzia icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        void SetRazziaIcon(bool visible);

        /// <summary>
        /// Set the detour icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        void SetDetourIcon(bool visible);

        /// <summary>
        /// Set the driving school icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        void SetDrivingSchoolIcon(bool visible);

        /// <summary>
        /// Set the additional trip icon
        /// </summary>
        /// <param name="visible">if true, the icon will be visible</param>
        void SetAdditionalTripIcon(bool visible);

        /// <summary>
        /// Set the traffic light icon
        /// </summary>
        /// <param name="state">state of the icon; which icon is displayed</param>
        void SetTrafficLightIcon(TrafficLightIconState state);

        /// <summary>
        /// Sets the stop requested icon.
        /// </summary>
        /// <param name="visible">
        /// if true, the icon will be visible
        /// </param>
        void SetStopRequestedIcon(bool visible);
    }
}