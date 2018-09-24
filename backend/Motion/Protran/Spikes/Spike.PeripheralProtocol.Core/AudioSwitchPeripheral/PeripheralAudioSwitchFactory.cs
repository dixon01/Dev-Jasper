// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSwitchFactory.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    /// <summary>The audio switch factory.</summary>
    public static class PeripheralAudioSwitchFactory
    {
        #region Static Fields

        private static AudioSwitchHandler audioSwitchHandler;

        #endregion

        #region Public Properties

        /// <summary>Gets the instance.</summary>
        public static AudioSwitchHandler Instance
        {
            get
            {
                lock (typeof(PeripheralAudioSwitchFactory))
                {
                    return audioSwitchHandler ?? (audioSwitchHandler = new AudioSwitchHandler());
                }
            }
        }

        #endregion
    }
}