// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioSpeakersViewModel.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Natraj</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.AudioSwitch.WpfSerialPortTester.ViewModels
{
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Types;
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;

    /// <summary>The audio speakers view model.</summary>
    public class AudioSpeakersViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether audio output enable.</summary>
        public bool AudioOutputEnable { get; set; }

        /// <summary>Gets the audio status.</summary>
        public ActiveSpeakerZone AudioStatus
        {
            get
            {
                if (this.InteriorSpeakersEnabled && this.ExteriorSpeakersEnabled)
                {
                    return ActiveSpeakerZone.Both;
                }

                if (!this.InteriorSpeakersEnabled && !this.ExteriorSpeakersEnabled)
                {
                    return ActiveSpeakerZone.None;
                }

                if (!this.InteriorSpeakersEnabled && this.ExteriorSpeakersEnabled)
                {
                    return ActiveSpeakerZone.Exterior;
                }

                return ActiveSpeakerZone.Interior;
            }
        }

        /// <summary>Gets or sets a value indicating whether exterior speakers enabled.</summary>
        public bool ExteriorSpeakersEnabled { get; set; }

        /// <summary>Gets or sets a value indicating whether interior speakers enabled.</summary>
        public bool InteriorSpeakersEnabled { get; set; }

        #endregion
    }
}