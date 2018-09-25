// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;

    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// Message converter between the <see cref="Motion.SystemManager.ServiceModel"/>
    /// and <see cref="Gorba.Common.SystemManagement.ServiceModel"/> namespaces.
    /// </summary>
    internal static class MessageConverter
    {
        /// <summary>
        /// Converts the given state.
        /// </summary>
        /// <param name="state">
        /// The message state.
        /// </param>
        /// <returns>
        /// The converted state.
        /// </returns>
        public static ApplicationState Convert(Motion.SystemManager.ServiceModel.ApplicationState state)
        {
            switch (state)
            {
                case Motion.SystemManager.ServiceModel.ApplicationState.Unknown:
                    return ApplicationState.Unknown;
                case Motion.SystemManager.ServiceModel.ApplicationState.AwaitingLaunch:
                    return ApplicationState.AwaitingLaunch;
                case Motion.SystemManager.ServiceModel.ApplicationState.Launching:
                    return ApplicationState.Launching;
                case Motion.SystemManager.ServiceModel.ApplicationState.Starting:
                    return ApplicationState.Starting;
                case Motion.SystemManager.ServiceModel.ApplicationState.Running:
                    return ApplicationState.Running;
                case Motion.SystemManager.ServiceModel.ApplicationState.Exiting:
                    return ApplicationState.Exiting;
                case Motion.SystemManager.ServiceModel.ApplicationState.Exited:
                    return ApplicationState.Exited;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Converts the given state.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The converted message state.
        /// </returns>
        public static Motion.SystemManager.ServiceModel.ApplicationState Convert(ApplicationState state)
        {
            switch (state)
            {
                case ApplicationState.Unknown:
                    return Motion.SystemManager.ServiceModel.ApplicationState.Unknown;
                case ApplicationState.AwaitingLaunch:
                    return Motion.SystemManager.ServiceModel.ApplicationState.AwaitingLaunch;
                case ApplicationState.Launching:
                    return Motion.SystemManager.ServiceModel.ApplicationState.Launching;
                case ApplicationState.Starting:
                    return Motion.SystemManager.ServiceModel.ApplicationState.Starting;
                case ApplicationState.Running:
                    return Motion.SystemManager.ServiceModel.ApplicationState.Running;
                case ApplicationState.Exiting:
                    return Motion.SystemManager.ServiceModel.ApplicationState.Exiting;
                case ApplicationState.Exited:
                    return Motion.SystemManager.ServiceModel.ApplicationState.Exited;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Converts the given application reason.
        /// </summary>
        /// <param name="reason">
        /// The application reason.
        /// </param>
        /// <returns>
        /// The converted message application reason.
        /// </returns>
        public static Motion.SystemManager.ServiceModel.ApplicationReason Convert(ApplicationReason reason)
        {
            switch (reason)
            {
                case ApplicationReason.Unknown:
                    return Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.Unknown;
                case ApplicationReason.Requested:
                    return Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.Requested;
                case ApplicationReason.SystemBoot:
                    return Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.SystemBoot;
                case ApplicationReason.SystemShutdown:
                    return Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.SystemShutdown;
                case ApplicationReason.SystemCrash:
                    return Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.SystemCrash;
                case ApplicationReason.ApplicationRelaunch:
                    return Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.ApplicationRelaunch;
                case ApplicationReason.ApplicationExit:
                    return Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.ApplicationExit;
                default:
                    throw new ArgumentOutOfRangeException("reason");
            }
        }

        /// <summary>
        /// Converts the given message application reason.
        /// </summary>
        /// <param name="reason">
        /// The message application reason.
        /// </param>
        /// <returns>
        /// The converted application reason.
        /// </returns>
        public static ApplicationReason Convert(Motion.SystemManager.ServiceModel.ApplicationReason reason)
        {
            switch (reason)
            {
                case Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.Unknown:
                    return ApplicationReason.Unknown;
                case Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.Requested:
                    return ApplicationReason.Requested;
                case Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.SystemBoot:
                    return ApplicationReason.SystemBoot;
                case Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.SystemShutdown:
                    return ApplicationReason.SystemShutdown;
                case Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.SystemCrash:
                    return ApplicationReason.SystemCrash;
                case Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.ApplicationRelaunch:
                    return ApplicationReason.ApplicationRelaunch;
                case Gorba.Motion.SystemManager.ServiceModel.ApplicationReason.ApplicationExit:
                    return ApplicationReason.ApplicationExit;
                default:
                    throw new ArgumentOutOfRangeException("reason");
            }
        }
    }
}
