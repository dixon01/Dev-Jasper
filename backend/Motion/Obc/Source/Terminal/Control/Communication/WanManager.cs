// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WanManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WanManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Communication
{
    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.Handlers;

    /// <summary>
    /// The WAN manager.
    /// </summary>
    internal class WanManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WanManager"/> class.
        /// </summary>
        /// <param name="stateVisualizationHandler">
        /// The state visualization handler.
        /// </param>
        public WanManager(StateVisualizationHandler stateVisualizationHandler)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Enables emergency mode.
        /// </summary>
        public void EmergencyModeEnable()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Disables emergency mode.
        /// </summary>
        public void EmergencyModeDisable()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Starts this manager.
        /// </summary>
        /// <param name="gsmConfig">
        /// The GSM config.
        /// </param>
        public void Start(GsmConfig gsmConfig)
        {
            throw new System.NotImplementedException();

            if (gsmConfig.SpeakerEnableOutput >= 0)
            {
                this.EnableSpeakerControlVTi(gsmConfig.SpeakerEnableOutput, gsmConfig.SpeakerIoInverted);
            }

            this.SetMicVolume(gsmConfig.MicVolume);
            this.SetSpeakerVolume(gsmConfig.SpeakerVolume);
        }

        /// <summary>
        /// Gets a flag indicating if speech is connected.
        /// </summary>
        /// <returns>
        /// True if connected.
        /// </returns>
        public bool IsSpeechConnected()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Hangs up the call.
        /// </summary>
        public void HangUp()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a flag indicating if a speech is being created.
        /// </summary>
        /// <returns>
        /// True if a call is being created.
        /// </returns>
        public bool IsBuildingCall()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Answers the call.
        /// </summary>
        public void AnswerCall()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Dials the given number.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// True if successful.
        /// </returns>
        public bool Dial(string number)
        {
            throw new System.NotImplementedException();
        }

        private void SetSpeakerVolume(int speakerVolume)
        {
            throw new System.NotImplementedException();
        }

        private void EnableSpeakerControlVTi(int speakerEnableOutput, bool speakerIoInverted)
        {
            throw new System.NotImplementedException();
        }

        private void SetMicVolume(int micVolume)
        {
            throw new System.NotImplementedException();
        }
    }
}
